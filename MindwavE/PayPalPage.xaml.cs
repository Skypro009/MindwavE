namespace MindwavE;

public partial class PayPalPage : ContentPage
{
    private string _approveUrl;
    
    // Event to notify completion: (success, orderId)
    public event EventHandler<bool> PaymentCompleted;

    public PayPalPage(string approveUrl)
    {
        InitializeComponent();
        _approveUrl = approveUrl;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        PaymentWebView.Source = _approveUrl;
    }

    private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
    {
        // Check for return_url (success)
        if (e.Url.StartsWith("mindwave://paypal/success"))
        {
            e.Cancel = true;
            PaymentCompleted?.Invoke(this, true);
            Navigation.PopModalAsync();
        }
        // Check for cancel_url
        else if (e.Url.StartsWith("mindwave://paypal/cancel"))
        {
            e.Cancel = true;
            PaymentCompleted?.Invoke(this, false);
            Navigation.PopModalAsync();
            
        }
        else
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }
}
