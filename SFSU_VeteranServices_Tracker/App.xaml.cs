/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: App.xaml.cs
 * 
 * Description:
 * Represents the main application class for the .NET MAUI application.
 * Initializes the application and sets the starting page used when the
 * Veteran Services Tracker launches.
 * 
 * ***************************************************************************/

using Microsoft.Extensions.DependencyInjection;

namespace SFSU_VeteranServices_Tracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

    }
}