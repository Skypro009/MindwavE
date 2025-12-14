namespace MindwavE;

public partial class HomePage : ContentPage
{
    private readonly Services.AuthService _authService;

	public HomePage(Services.AuthService authService)
	{
		InitializeComponent();
        _authService = authService;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateWelcomeMessage();
    }

    private void UpdateWelcomeMessage()
    {
        var currentUser = _authService.GetCurrentUser();
        if (currentUser != null && currentUser.UserMetadata.TryGetValue("username", out var usernameObj))
        {
            string username = usernameObj?.ToString();
            if (!string.IsNullOrEmpty(username))
            {
                WelcomeLabel.Text = $"Dobrodošao/la nazad, {username}!";
            }
        }
    }

    private async void GoToChatPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ChatPage"); // Using absolute route for TabBar page
    }

    private async void GoToSubscriptionPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SubscriptionPage));
    }
}
