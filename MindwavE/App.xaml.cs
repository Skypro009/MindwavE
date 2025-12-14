using Microsoft.Extensions.DependencyInjection;

namespace MindwavE
{
    public partial class App : Application
    {
        private readonly Services.AuthService _authService;
        private readonly IServiceProvider _serviceProvider;

        public App(Services.AuthService authService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new LoadingPage(_authService, _serviceProvider));
        }
    }
}