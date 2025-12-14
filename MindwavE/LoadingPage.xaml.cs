using MindwavE.Services;

namespace MindwavE;

public partial class LoadingPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly IServiceProvider _serviceProvider;

    public LoadingPage(AuthService authService, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _authService = authService;
        _serviceProvider = serviceProvider;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Give UI a moment to render
        await Task.Delay(500);

        try
        {
            await _authService.InitializeAsync();
            var session = _authService.GetCurrentSession();

            // Always set AppShell as the MainPage so Shell.Current is valid
            Application.Current.MainPage = new AppShell();

            if (session?.User == null)
            {
                // Navigate to Login Page using Shell routing
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
        }
        catch (Exception ex)
        {
            // Handle initialization error (maybe offline?)
            Console.WriteLine($"Auth Init Error: {ex.Message}");
            Application.Current.MainPage = new AppShell();
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}
