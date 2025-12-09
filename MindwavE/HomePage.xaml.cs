namespace MindwavE;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}

    private async void GoToChatPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ChatPage");
    }
}
