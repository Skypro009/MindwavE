namespace MindwavE
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(TrackingPage), typeof(TrackingPage));
            // Start with only the Main page enabled in the flyout.
            ShowOnlyMain();
        }

        // Disable other menu items so user cannot bypass the start button.
        void ShowOnlyMain()
        {
            if (HomeFlyoutItem != null)
                HomeFlyoutItem.IsEnabled = false;
            if (TrackingFlyoutItem != null)
                TrackingFlyoutItem.IsEnabled = false;
            if (MainFlyoutItem != null)
                MainFlyoutItem.IsEnabled = true;
        }

        // Enable the app menu and remove the initial Main entry from the flyout.
        public void EnableMenuAndRemoveMain()
        {
            if (HomeFlyoutItem != null)
                HomeFlyoutItem.IsEnabled = true;
            if (TrackingFlyoutItem != null)
                TrackingFlyoutItem.IsEnabled = true;

            // Remove the initial Main entry so it no longer appears in the flyout.
            if (MainFlyoutItem != null && this.Items.Contains(MainFlyoutItem))
            {
                this.Items.Remove(MainFlyoutItem);
            }
        }
    }
}
