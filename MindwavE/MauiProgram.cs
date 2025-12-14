using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

namespace MindwavE
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("NunitoSans.ttf", "NunitoSans");
                    fonts.AddFont("NunitoSansItalic.ttf", "NunitoSans_Italic");
                });

            // Supabase Configuration
            var options = new Supabase.SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
            };

            // Register Supabase Client as a Singleton
            builder.Services.AddSingleton(provider => new Supabase.Client(Constants.SupabaseUrl, Constants.SupabaseKey, options));
            
            // Services
            builder.Services.AddSingleton<Services.AuthService>();
            builder.Services.AddSingleton<Services.ChatService>();
            builder.Services.AddSingleton<Services.SubscriptionService>();
            
            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ChatPage>();
            builder.Services.AddTransient<SubscriptionPage>();
            builder.Services.AddTransient<HomePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
