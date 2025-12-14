using MindwavE.Services;

namespace MindwavE;

public partial class SubscriptionPage : ContentPage
{
    private readonly SubscriptionService _subscriptionService;

    public SubscriptionPage(SubscriptionService subscriptionService)
    {
        InitializeComponent();
        _subscriptionService = subscriptionService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CheckStatus();
    }

    private async Task CheckStatus()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        try
        {
            var sub = await _subscriptionService.GetSubscriptionAsync();
            if (sub != null && sub.Status == "active")
            {
                StatusLabel.Text = "Current Status: Active";
                StatusLabel.TextColor = Colors.Green;
                SubscribeButton.Text = "Manage Subscription";
                SubscribeButton.IsEnabled = false; // Or redirect to management portal
            }
            else
            {
                StatusLabel.Text = "Current Status: Free Plan";
                StatusLabel.TextColor = Colors.Gray;
                SubscribeButton.Text = "Subscribe ($9.99/mo)";
                SubscribeButton.IsEnabled = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to check status: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnSubscribeClicked(object sender, EventArgs e)
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        try
        {
            // Simulate payment flow
            await Task.Delay(1000); 
            
            await _subscriptionService.SubscribeAsync("pro_monthly");
            await DisplayAlert("Success", "You are now subscribed to Pro Plan!", "OK");
            await CheckStatus();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Subscription failed: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }
}
