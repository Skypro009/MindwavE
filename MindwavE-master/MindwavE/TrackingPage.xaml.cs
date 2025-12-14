namespace MindwavE;

public partial class TrackingPage : ContentPage
{
	public TrackingPage()
	{
		InitializeComponent();
	}

    private async void GoToChatPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ChatPage");
    }
}
