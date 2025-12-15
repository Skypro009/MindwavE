using MindwavE.Services;

namespace MindwavE;

public partial class SubscriptionPage : ContentPage
{
    private readonly SubscriptionService _subscriptionService;
    private readonly PayPalService _payPalService;

    public SubscriptionPage(SubscriptionService subscriptionService, PayPalService payPalService)
    {
        InitializeComponent();
        _subscriptionService = subscriptionService;
        _payPalService = payPalService;
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
                
                if (sub.CurrentPeriodEnd.HasValue)
                {
                    ExpiryLabel.Text = $"Expires: {sub.CurrentPeriodEnd.Value.ToLocalTime():d}";
                    ExpiryLabel.IsVisible = true;
                }
                else
                {
                    ExpiryLabel.IsVisible = false;
                }

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

    private async void GoBackToMainPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomePage");
    }

    private async void OnSubscribeClicked(object sender, EventArgs e)
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        try
        {
            // 1. Create PayPal Order
            var (orderId, approveUrl) = await _payPalService.CreateOrderAsync(9.99m);

            // 2. Open PayPal WebView
            var payPalPage = new PayPalPage(approveUrl);
            
            payPalPage.PaymentCompleted += async (s, success) =>
            {
                if (success)
                {
                    // 3. Capture Payment
                    LoadingIndicator.IsVisible = true;
                    LoadingIndicator.IsRunning = true;
                    
                    var captured = await _payPalService.CaptureOrderAsync(orderId);
                    
                    if (captured)
                    {
                        // 4. Activate Subscription
                        await _subscriptionService.SubscribeAsync("pro_monthly");
                        await DisplayAlert("Success", "Payment successful! You are now subscribed.", "OK");
                        await CheckStatus();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Payment capture failed.", "OK");
                    }
                    
                    LoadingIndicator.IsVisible = false;
                    LoadingIndicator.IsRunning = false;
                }
                else
                {
                    await DisplayAlert("Cancelled", "Payment was cancelled.", "OK");
                    await Shell.Current.GoToAsync("//HomePage");
                }
            };

            await Navigation.PushModalAsync(payPalPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Subscription failed: {ex.Message}", "OK");
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }
}
