using PIIIProject.Initial.Auth.Services;
using PIIIProject.Initial.Game;
using PIIIProject.Initial.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PIIIProject.Initial.Auth
{
    public partial class LoginWindow : Window
    {
        // Default placeholder text for the email and password fields
        private readonly string defaultEmailText = "youremail@gmail.com";
        private readonly string defaultPasswordText = "Password";

        private AuthenticationService _authenticationService; // Service for handling authentication
        private readonly LoginContext _loginContext; // Context for determining the login flow

        public User LoggedInUser { get; private set; } // User who successfully logs in
        public bool IsLoginSuccessful { get; private set; } = false; // Tracks if the login was successful

        /// <summary>
        /// Initializes the LoginWindow with authentication and context
        /// </summary>
        /// <param name="authenticationService">Service for validating login credentials</param>
        /// <param name="loginContext">Specifies the context of the login</param>
        public LoginWindow(AuthenticationService authenticationService, LoginContext loginContext)
        {
            InitializeComponent();
            _authenticationService = authenticationService;
            _loginContext = loginContext;

            // Adds focus handlers for the email and password fields
            AddFocusHandlers(LoginEmailTextBox, defaultEmailText);
            AddFocusHandlers(LoginPasswordTextBox, LoginPasswordPlaceholder);
        }

        /// <summary>
        /// Adds focus and blur behavior for a text box with a default placeholder
        /// </summary>
        private void AddFocusHandlers(TextBox textBox, string defaultText)
        {
            textBox.GotFocus += (sender, e) => TextBox_GotFocus(sender as TextBox, defaultText);
            textBox.LostFocus += (sender, e) => TextBox_LostFocus(sender as TextBox, defaultText);
        }

        /// <summary>
        /// Adds focus and blur behavior for a password box with a placeholder element
        /// </summary>
        private void AddFocusHandlers(PasswordBox passwordBox, UIElement placeholder)
        {
            passwordBox.GotFocus += (sender, e) => PasswordBox_GotFocus(placeholder);
            passwordBox.LostFocus += (sender, e) => PasswordBox_LostFocus(sender as PasswordBox, placeholder);
        }

        /// <summary>
        /// Clears the text and updates color when the text box gains focus
        /// </summary>
        private void TextBox_GotFocus(TextBox textBox, string defaultText)
        {
            if (textBox.Text == defaultText)
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        /// <summary>
        /// Hides the placeholder when the password box gains focus
        /// </summary>
        private void PasswordBox_GotFocus(UIElement placeholder)
        {
            placeholder.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Restores the placeholder text and color if the text box loses focus while empty
        /// </summary>
        private void TextBox_LostFocus(TextBox textBox, string defaultText)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = defaultText;
                textBox.Foreground = Brushes.Gray;
            }
        }

        /// <summary>
        /// Shows the placeholder if the password box loses focus while empty
        /// </summary>
        private void PasswordBox_LostFocus(PasswordBox passwordBox, UIElement placeholder)
        {
            SetPlaceholderVisibility(passwordBox, placeholder);
        }

        /// <summary>
        /// Sets the placeholder visibility based on the content of the password box
        /// </summary>
        private void SetPlaceholderVisibility(PasswordBox passwordBox, UIElement placeholder)
        {
            if (string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                placeholder.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Handles the login process when the continue button is clicked
        /// </summary>
        private void LoginContinueButton_Click(object sender, RoutedEventArgs e)
        {
            string email = LoginEmailTextBox.Text;
            string password = LoginPasswordTextBox.Password;

            if (string.IsNullOrEmpty(email) || email == defaultEmailText)
            {
                MessageBox.Show("Please enter an email.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(password) || password == defaultPasswordText)
            {
                MessageBox.Show("Please enter a password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                bool isAuthenticated = _authenticationService.VerifyLogin(email, password);
                if (isAuthenticated)
                {
                    LoggedInUser = _authenticationService.GetUserByEmail(email);
                    IsLoginSuccessful = true;

                    if (_loginContext == LoginContext.OriginalLogin)
                    {
                        // Handles original player login
                        CategorySelectionWindow categorySelectionWindow = new CategorySelectionWindow(LoggedInUser, _authenticationService);
                        categorySelectionWindow.Show();
                        this.Close();
                    }
                    else if (_loginContext == LoginContext.AddSecondPlayer)
                    {
                        // Handles the second player's login and closes the window
                        IsLoginSuccessful = true;
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid email or password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Opens the registration window when the register button is clicked
        /// </summary>
        private void RegisterButton_Click(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow(_loginContext, LoggedInUser);
            registerWindow.Show();
            this.Close();
        }
    }

    /// <summary>
    /// Enum for determining the login context
    /// </summary>
    public enum LoginContext
    {
        OriginalLogin, 
        AddSecondPlayer 
    }
}
