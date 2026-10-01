/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: StaffPage.xaml.cs
 * 
 * Description:
 * Handles the Staff Dashboard used by authenticated Staff and Manager users.
 * Loads student check-in records from the backend API, displays check-in
 * totals by status, filters records by selected date ranges, and sorts the
 * results so the most recent check-ins appear first. It also controls
 * Manager-only access to the Manage Staff page and handles staff sign-out.
 * 
 * ***************************************************************************/

using SFSU_VeteranServices_Tracker.Model;
using SFSU_VeteranServices_Tracker.Services;
using Microsoft.Maui.Storage;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFSU_VeteranServices_Tracker.View
{
    public partial class StaffPage : ContentPage
    {
        private readonly ApiService apiService = new ApiService();
        private List<StudentCheckIn> checkIns = new List<StudentCheckIn>();
        public StaffPage() {

            InitializeComponent();

            dateFilterPicker.SelectedIndex = 0;

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Get the role that was saved after login.
            string? role =
                await SecureStorage.Default.GetAsync("staff_role");

            // Only the Manager can see the Manage Staff button.
            manageStaffButton.IsVisible =
                role == "Manager";

            await LoadCheckInsAsync();
        }
        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            // Remove the saved login information.
            SecureStorage.Default.Remove("auth_token");
            SecureStorage.Default.Remove("staff_role");
            SecureStorage.Default.Remove("staff_username");

            await Shell.Current.GoToAsync("..");
        }

        private async Task LoadCheckInsAsync()
        {
            try
            {
                // Get all check-ins from the API
                checkIns =
                    await apiService.GetCheckInsAsync();

                // Update the dashboard with the data
                UpdateDashboard();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync(
                    "Connection Error",
                    $"Unable to load check-ins.\n\n{ex.Message}",
                    "OK");
            }
        }
        private async void OnManageStaffClicked(object sender,EventArgs e)
        {
            await Shell.Current.GoToAsync(
                nameof(ManageStaffPage));
        }

        private void UpdateDashboard()
        {
            string selectedFilter = dateFilterPicker.SelectedItem?.ToString() ?? "Today";

            DateTime today = DateTime.Today;

            List<StudentCheckIn> filteredCheckIns;

            if (selectedFilter == "Today")
            {
                filteredCheckIns = checkIns
                    .Where(student => student.CheckInTime.Date == today).ToList();
            }
            else if (selectedFilter == "Last 7 Days")
            {
                DateTime startDate = today.AddDays(-6);

                filteredCheckIns = checkIns
                    .Where(student => student.CheckInTime >= startDate).ToList();
            }
            else if (selectedFilter == "Last 30 Days")
            {
                DateTime startDate = today.AddDays(-29);

                filteredCheckIns = checkIns
                    .Where(student => student.CheckInTime >= startDate).ToList();
            }
            else if (selectedFilter == "This Month")
            {
                filteredCheckIns = checkIns
                    .Where(student =>
                        student.CheckInTime.Year == today.Year &&
                        student.CheckInTime.Month == today.Month).ToList();
            }
            else if (selectedFilter == "Previous Month")
            {
                DateTime firstDayThisMonth =
                    new DateTime(
                        today.Year,
                        today.Month,
                        1);

                DateTime firstDayPreviousMonth =
                    firstDayThisMonth.AddMonths(-1);

                filteredCheckIns =checkIns
                    .Where(student =>
                        student.CheckInTime >= firstDayPreviousMonth &&
                        student.CheckInTime < firstDayThisMonth).ToList();
            }
            else
            {
                // All Check-Ins
                filteredCheckIns = checkIns.ToList();
            }

            // Always put the newest check-in at the top.
            filteredCheckIns = filteredCheckIns
                    .OrderByDescending(student => student.CheckInTime).ToList();

            totalCheckInsLabel.Text = filteredCheckIns.Count.ToString();

            veteranCountLabel.Text = filteredCheckIns.Count(student =>
                    student.Status == "Veteran").ToString();

            activeDutyCountLabel.Text = filteredCheckIns.Count(student =>
                    student.Status == "Active Duty").ToString();

            reserveCountLabel.Text = filteredCheckIns.Count(student =>
                    student.Status == "Reserve").ToString();

            dependentCountLabel.Text = filteredCheckIns.Count(student =>
                    student.Status == "Dependent").ToString();

            civilianCountLabel.Text = filteredCheckIns.Count(student =>
                    student.Status == "Civilian").ToString();

            checkInCollectionView.ItemsSource = filteredCheckIns;

            UpdateDashboardTitles(selectedFilter);
        }
        private void OnDateFilterChanged(object sender, EventArgs e)
        {
            UpdateDashboard();
        }
        private void UpdateDashboardTitles(string selectedFilter)
        {
            totalCheckInsTitleLabel.Text = $"{selectedFilter} Total Check-Ins";

            studentListTitleLabel.Text = $"{selectedFilter} Student Check-Ins";
        }
    }
}
