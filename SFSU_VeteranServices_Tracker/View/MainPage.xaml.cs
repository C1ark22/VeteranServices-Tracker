/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: MainPage.xaml.cs
 * 
 * Description: This class represents the main page of the application. It 
 * provides a user interface for checking in students and viewing their 
 * check-in status.
 * 
 * ***************************************************************************/
using SFSU_VeteranServices_Tracker.Model;
using SFSU_VeteranServices_Tracker.View;
using SFSU_VeteranServices_Tracker.Services;

namespace SFSU_VeteranServices_Tracker
{
    public partial class MainPage : ContentPage
    {
        private readonly ApiService apiService = new ApiService();
        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnStaffClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        private async void OnCheckInClicked(object sender, EventArgs e)
        {
            // Check if full name is empty
            if (string.IsNullOrWhiteSpace(fullNameEntry.Text))
            {
                await DisplayAlertAsync(
                    "Missing Information",
                    "Please enter your Full Name",
                    "OK");

                return;
            }

            // Make sure the name only contains letters and spaces
            foreach (char character in fullNameEntry.Text)
            {
                if (!char.IsLetter(character) && character != ' ')
                {
                    await DisplayAlertAsync(
                        "Invalid Name",
                        "Student name can only contain letters and spaces.",
                        "OK");

                    return;
                }
            }

            // Make sure Student ID only contains numbers
            if (!int.TryParse(studentIdEntry.Text, out int studentID))
            {
                await DisplayAlertAsync(
                    "Invalid Student ID",
                    "Please enter numbers only.",
                    "OK");

                return;
            }

            // Make sure a status was selected
            if (studentStatusPicker.SelectedItem == null)
            {
                await DisplayAlertAsync(
                    "Missing Information",
                    "Please select your status.",
                    "OK");

                return;
            }

            // Make sure the Code of Conduct was acknowledged
            if (!codeOfConductCheckBox.IsChecked)
            {
                await DisplayAlertAsync(
                    "Code of Conduct",
                    "Please read and acknowledge the Code of Conduct",
                    "OK");

                return;
            }

            // Create the StudentCheckIn object
            StudentCheckIn student = new StudentCheckIn
            {
                FullName = fullNameEntry.Text,
                StudentId = studentID.ToString(),
                Status = studentStatusPicker.SelectedItem.ToString(),
                CheckInTime = DateTime.Now
            };

            // Send the check-in to the API
            bool checkInSaved =
                await apiService.CreateCheckInAsync(student);


            // Make sure the API successfully saved the check-in
            if (!checkInSaved)
            {
                await DisplayAlertAsync(
                    "Check-In Failed",
                    "The check-in could not be saved to the server.",
                    "OK");

                return;
            }

            await DisplayAlertAsync(
                "Check In Successful",
                $"Thank you for checking in, {student.FullName}!",
                "OK");
        }
        private async void OnCodeOfConductTapped(object sender, TappedEventArgs e)
        {
            string url = "https://conduct.sfsu.edu/standards";

            await Launcher.Default.OpenAsync(url);

            codeOfConductCheckBox.IsEnabled = true;
        } 
    }
}
