using System.Windows;
using System.Windows.Input;
using PIIIProject.Initial.Auth;
using PIIIProject.Initial.Auth.Services;

namespace PIIIProject.Initial
{
    public partial class MainWindow : Window
    {
        private AuthenticationService _authenticationService;
        public MainWindow()
        {
            InitializeComponent();
            _authenticationService = new AuthenticationService();
        }

        private void OpenWindow(Window window)
        {
            this.Close();
            window.Show();
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the RegisterWindow for new registrations
            RegisterWindow registerWindow = new RegisterWindow(LoginContext.OriginalLogin);
            OpenWindow(registerWindow);
        }

        private void RegisterButton_Click(object sender, MouseButtonEventArgs e)
        {
            // Open the RegisterWindow for new registrations
            RegisterWindow registerWindow = new RegisterWindow(LoginContext.OriginalLogin);
            OpenWindow(registerWindow);
        }

        private void SignInButton_Click(object sender, MouseButtonEventArgs e)
        {
            // Authentication service instance
            AuthenticationService authenticationService = new AuthenticationService();

            // Open LoginWindow with OriginalLogin context
            LoginWindow loginWindow = new LoginWindow(authenticationService, LoginContext.OriginalLogin);

            // Show the login window
            OpenWindow(loginWindow);
        }
    }
}
