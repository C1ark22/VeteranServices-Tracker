/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: LoginPage.xaml.cs
 * 
 * Description: This class represents the login page of the application. 
 * It handles user input for username and password, validates the input, and 
 * navigates to the StaffPage upon successful login.
 * 
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Storage;
using SFSU_VeteranServices_Tracker.Model;
using SFSU_VeteranServices_Tracker.Services;

namespace SFSU_VeteranServices_Tracker.View;

public partial class LoginPage : ContentPage
{
    private readonly ApiService apiService = new ApiService();
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = usernameEntry.Text;
        string password = passwordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlertAsync(
                "Missing Information",
                "Please enter your username and password.",
                "OK");

            return;
        }

        // Attempt to log in using the API service.
        try
        {
            LoginResponse? loginResponse = await apiService.LoginAsync(username, password);

            // API returned 401 Unauthorized.
            if (loginResponse == null)
            {
                await DisplayAlertAsync(
                    "Login Failed",
                    "Invalid username or password.",
                    "OK");

                return;
            }

            // Store the JWT securely on the device.
            await SecureStorage.Default.SetAsync("auth_token", loginResponse.Token);

            // Save the role so we know whether
            // this user is Manager or Staff.
            await SecureStorage.Default.SetAsync("staff_role", loginResponse.Role);

            await SecureStorage.Default.SetAsync("staff_username", loginResponse.Username);

            await Shell.Current.GoToAsync(nameof(StaffPage));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Connection Error",
                $"Unable to log in.\n\n{ex.Message}",
                "OK");
        }
    }
}