using System;
using System.Collections.Generic;
using System.Text;
using SFSU_VeteranServices_Tracker.Services;

namespace SFSU_VeteranServices_Tracker.View
{
    public partial class ManageStaffPage : ContentPage
    {
        private readonly ApiService apiService = new ApiService();
        public ManageStaffPage()
        {
            InitializeComponent();
        }

        private async void OnCreateStaffClicked(object sender, EventArgs e)
        {
            string username = usernameEntry.Text?.Trim() ?? string.Empty;

            string password = passwordEntry.Text ?? string.Empty;

            string confirmPassword = confirmPasswordEntry.Text ?? string.Empty;

            // Make sure all fields were filled out.
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlertAsync(
                    "Missing Information",
                    "Please complete all fields.",
                    "OK");

                return;
            }

            // Make sure both passwords match.
            if (password != confirmPassword)
            {
                await DisplayAlertAsync(
                    "Password Error",
                    "The passwords do not match.",
                    "OK");

                return;
            }

            try
            {
                bool accountCreated = await apiService.CreateStaffAsync(username,password);

                if (!accountCreated)
                {
                    await DisplayAlertAsync(
                        "Account Creation Failed",
                        "The staff account could not be created.",
                        "OK");

                    return;
                }

                await DisplayAlertAsync(
                    "Account Created",
                    $"Staff account '{username}' was created successfully.",
                    "OK");

                // Clear the form.
                usernameEntry.Text = string.Empty;
                passwordEntry.Text = string.Empty;
                confirmPasswordEntry.Text = string.Empty;
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync(
                    "Connection Error",
                    $"Unable to create the staff account.\n\n{ex.Message}",
                    "OK");
            }
        }
    }
}
