/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: AppShell
 * 
 * Description:
 * Defines the main navigation structure for the .NET MAUI application.
 * Registers application routes and controls navigation between pages such as
 * the student check-in page, Staff login, Staff Dashboard, and Manage Staff page.
 * 
 * ***************************************************************************/

using SFSU_VeteranServices_Tracker.View;

namespace SFSU_VeteranServices_Tracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(StaffPage), typeof(StaffPage));
            Routing.RegisterRoute(nameof(ManageStaffPage), typeof(ManageStaffPage));
        }
    }
}
