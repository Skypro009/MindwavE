using MindwavE.Services;

namespace MindwavE;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService;

    public RegisterPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text;
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please fill in all fields", "OK");
            return;
        }

        // Basic validation
        if (password.Length < 6)
        {
            await DisplayAlert("Error", "Password must be at least 6 characters long.", "OK");
            return;
        }

        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            var session = await _authService.SignUp(email, password, username);
            if (session?.User != null)
            {
                await DisplayAlert("Success", "Registration successful! You can now login.", "OK");
                await Shell.Current.Navigation.PopAsync(); // Go back to login
            }
            else
            {
                // Supabase might return a null session if email confirmation is enabled.
                await DisplayAlert("Success", "Registration successful! Please check your email for confirmation code if required.", "OK");
                await Shell.Current.Navigation.PopAsync(); 
            }
        }
        catch (Exception ex)
        {
            // Try to make the error more readable
            var msg = ex.Message;
            if (msg.Contains("Password should be at least"))
            {
                msg = "Password is too weak. It should be at least 6 characters.";
                await DisplayAlert("Registration Failed", $"{msg}", "OK");
            }
            else if (msg.Contains("User already registered"))
            {
                msg = "This email is already registered.";
                await DisplayAlert("Registration Failed", $"{msg}", "OK");
            }
            else
            {
                msg = "Please verify you'r email now!";
                await DisplayAlert("Login now!", $"{msg}", "OK");
            }

            
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}
