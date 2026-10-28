using Microsoft.Maui.Controls.Shapes;
using SFSU_VeteranServices_Tracker.Models;
using SFSU_VeteranServices_Tracker.Services;

namespace SFSU_VeteranServices_Tracker;

public partial class AiCheckInPage : ContentPage
{
    private string? previousResponseId;

    private Button? codeOfConductButton;

    private CheckInConversationState conversationState = new();

    public AiCheckInPage()
    {
        InitializeComponent();
    }

    private async void OnSendClicked(
        object sender,
        EventArgs e)
    {
        string message =
            MessageEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        // Display the student's message.
        AddStudentMessage(message);

        MessageEntry.Text = string.Empty;

        // Prevent multiple submissions while
        // waiting for the API.
        SendButton.IsEnabled = false;
        MessageEntry.IsEnabled = false;

        try
        {
            AiCheckInRequest request =
                new AiCheckInRequest
                {
                    Message = message,

                    PreviousResponseId =
                        previousResponseId,

                    State =
                        conversationState
                };

            AiCheckInResponse? response =
                await AiCheckInApiService
                    .SendMessageAsync(request);

            if (response == null)
            {
                AddAssistantMessage(
                    "I couldn't get a response. Please try again.");

                return;
            }

            // Keep the newest Foundry response ID.
            if (!string.IsNullOrWhiteSpace(
                    response.ResponseId))
            {
                previousResponseId =
                    response.ResponseId;
            }

            // Keep the newest C# conversation state.
            conversationState = response.State;

            AddAssistantMessage(response.Message);

            if (conversationState.CurrentStep == "AwaitingCodeOfConductReview")
            {
                ShowCodeOfConductButton();
            }

            // Once the database save succeeds,
            // don't allow additional check-in messages.
            if (conversationState.CurrentStep
                == "Completed")
            {
                MessageEntry.Placeholder =
                    "Check-in completed";

                return;
            }
        }
        catch (Exception)
        {
            AddAssistantMessage(
                "I couldn't connect to the check-in service. " +
                "Please try again.");
        }
        finally
        {
            if (conversationState.CurrentStep
                != "Completed")
            {
                SendButton.IsEnabled = true;
                MessageEntry.IsEnabled = true;
                MessageEntry.Focus();
            }
        }
    }


    private async void AddStudentMessage(string message)
    {
        Border bubble =
            CreateMessageBubble(
                "You",
                message,
                Color.FromArgb("#A58AEF"),
                Colors.White,
                LayoutOptions.End);

        ChatMessagesLayout.Children.Add(bubble);

        await ScrollToBottom();
    }


    private async void AddAssistantMessage(string message)
    {
        Border bubble =
            CreateMessageBubble(
                "AI Assistant",
                message,
                Color.FromArgb("#3A3A3A"),
                Colors.White,
                LayoutOptions.Start);

        ChatMessagesLayout.Children.Add(bubble);

        await ScrollToBottom();
    }


    private static Border CreateMessageBubble(
            string sender,
            string message,
            Color backgroundColor,
            Color textColor,
            LayoutOptions horizontalOptions)
    {
        Label senderLabel =
            new Label
            {
                Text = sender,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White
            };

        Label messageLabel =
            new Label
            {
                Text = message,
                FontSize = 16,
                TextColor = textColor,
                LineBreakMode = LineBreakMode.WordWrap
            };

        VerticalStackLayout content =
            new VerticalStackLayout
            {
                Spacing = 5
            };

        content.Children.Add(senderLabel);
        content.Children.Add(messageLabel);

        return new Border
        {
            BackgroundColor = backgroundColor,
            Padding = 12,
            MaximumWidthRequest = 600,
            HorizontalOptions = horizontalOptions,

            StrokeShape =
                new RoundRectangle
                {
                    CornerRadius = 14
                },

            Content = content
        };
    }
    private async Task ScrollToBottom()
    {
        await Task.Delay(50);

        await ChatScrollView.ScrollToAsync(
            ChatMessagesLayout,
            ScrollToPosition.End,
            true);
    }
    private void ShowCodeOfConductButton()
    {
        if (codeOfConductButton != null)
        {
            return;
        }

        codeOfConductButton = new Button
            {
                Text = "Review SFSU Code of Conduct",
                BackgroundColor =
                    Color.FromArgb("#A58AEF"),
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(20, 12),
                HorizontalOptions = LayoutOptions.Start
            };

        codeOfConductButton.Clicked +=
            OnCodeOfConductClicked;

        ChatMessagesLayout.Children.Add(
            codeOfConductButton);
    }
    private async void OnCodeOfConductClicked(object? sender, EventArgs e)
    {
        Uri url =
            new Uri(
                "https://conduct.sfsu.edu/standards");

        await Launcher.Default.OpenAsync(url);

        conversationState.CodeOfConductReviewed = true;

        conversationState.CurrentStep =
            "AwaitingCodeOfConduct";

        if (codeOfConductButton != null)
        {
            codeOfConductButton.IsEnabled = false;

            codeOfConductButton.Text =
                "Code of Conduct Opened ✓";
        }

        AddAssistantMessage(
            "After you have reviewed the SFSU Standards " +
            "for Student Conduct, type \"I agree\" " +
            "to continue.");
    }
}