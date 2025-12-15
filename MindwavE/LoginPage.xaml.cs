using MindwavE.Services;

namespace MindwavE;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please enter email and password", "OK");
            return;
        }

        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        

        
        try
        {
            var session = await _authService.SignIn(email, password);
            if (session?.User != null)
            {
                // Navigate to main app logic
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                await DisplayAlert("Error", "Login failed. Please check credentials.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Login error: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}
