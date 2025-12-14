namespace MindwavE
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(ChatPage), typeof(ChatPage));
            Routing.RegisterRoute(nameof(TrackingPage), typeof(TrackingPage));
            Routing.RegisterRoute(nameof(AppointmentsPage), typeof(AppointmentsPage));
        }
    }
}
