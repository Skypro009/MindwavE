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
            System.Diagnostics.Debug.WriteLine("LoadingPage: Initializing Auth Service...");
            await _authService.InitializeAsync();
            System.Diagnostics.Debug.WriteLine("LoadingPage: Auth Service Initialized.");

            var currentSession = _authService.GetCurrentSession();
            System.Diagnostics.Debug.WriteLine($"LoadingPage: Final Session User = {currentSession?.User?.Email ?? "null"}");

            // Always set AppShell as the MainPage so Shell.Current is valid
            Application.Current.MainPage = new AppShell();

            if (currentSession?.User == null)
            {
                System.Diagnostics.Debug.WriteLine("LoadingPage: No user, going to Login.");
                // Navigate to Login Page using Shell routing
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("LoadingPage: User found, staying on AppShell.");
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
