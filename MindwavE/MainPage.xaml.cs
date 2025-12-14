using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace MindwavE;

public partial class MainPage : ContentPage
{
    public ICommand StartCommand { get; }

    private readonly Supabase.Client _supabase;

    public MainPage(Supabase.Client supabase)
    {
        InitializeComponent();
        _supabase = supabase;

        // Provide the command implementation and set the BindingContext so XAML {Binding StartCommand} resolves.
        StartCommand = new Command(OnStart);
        BindingContext = this;
        
        // Verify initialization
        VerifySupabase();
    }

    private async void VerifySupabase()
    {
        if (_supabase != null)
        {
           // Just a quick check that it's not null
           System.Diagnostics.Debug.WriteLine($"Supabase Initialized!");
        }
    }

    private async void OnStart()
    {
        // Use Shell routing so the Shell menu can be updated centrally.
        if (Shell.Current != null)
        {
            // Navigate to the HomePage route registered in AppShell.
            await Shell.Current.GoToAsync(nameof(HomePage));

            // After successful navigation, update the shell menu:
            if (Shell.Current is AppShell appShell)
            {
                appShell.EnableMenuAndRemoveMain();
            }
        }
        else
        {
            // Fallback: show a simple alert if Shell navigation isn't available.
            await DisplayAlert("Info", "Start pressed — navigation not available.", "OK");
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        if (StartCommand?.CanExecute(null) == true)
        {
            StartCommand.Execute(null);
        }
        else
        {
            // Fallback: call existing method directly
            OnStart();
        }
    }
}
