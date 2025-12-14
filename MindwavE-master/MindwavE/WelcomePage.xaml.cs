namespace MindwavE;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        // Navigate to the main app (HomePage via TabBar)
        await Shell.Current.GoToAsync("//HomePage");
    }
}
