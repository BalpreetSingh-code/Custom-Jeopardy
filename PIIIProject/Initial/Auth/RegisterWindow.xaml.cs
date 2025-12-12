using PIIIProject.Initial.Auth.Services;
using PIIIProject.Initial.Game;
using PIIIProject.Initial.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PIIIProject.Initial.Auth
{
    public partial class RegisterWindow : Window
    {
        private readonly RegistrationService _registrationService; // Handles user registration
        private readonly LoginContext _loginContext; // Determines registration flow context
        private readonly User _originalUser; // Holds first player's instance for the second player context

        /// <summary>
        /// Initializes RegisterWindow with context and optional original user
        /// </summary>
        /// <param name="loginContext">Specifies the registration context</param>
        /// <param name="originalUser">Optional, the original user for adding a second player</param>
        public RegisterWindow(LoginContext loginContext, User originalUser = null)
        {
            InitializeComponent();
            _registrationService = new();
            _loginContext = loginContext;
            _originalUser = originalUser;

            // Add focus behavior for username, email, and password fields
            AddFocusHandlers(RegisterUsernameTextBox, "Username");
            AddFocusHandlers(RegisterEmailTextBox, "youremail@gmail.com");
            AddFocusHandlers(RegisterPasswordTextBox, RegisterPasswordPlaceholder);
            AddFocusHandlers(RegisterConfirmPasswordTextBox, RegisterConfirmPasswordPlaceholder);
        }

        /// <summary>
        /// Adds focus behavior for a text box with default placeholder text
        /// </summary>
        private void AddFocusHandlers(TextBox textBox, string defaultText)
        {
            textBox.GotFocus += (sender, e) => TextBox_GotFocus(sender as TextBox, defaultText);
            textBox.LostFocus += (sender, e) => TextBox_LostFocus(sender as TextBox, defaultText);
        }

        /// <summary>
        /// Adds focus behavior for a password box with a placeholder element
        /// </summary>
        private void AddFocusHandlers(PasswordBox passwordBox, UIElement placeholder)
        {
            passwordBox.GotFocus += (sender, e) => PasswordBox_GotFocus(placeholder);
            passwordBox.LostFocus += (sender, e) => PasswordBox_LostFocus(sender as PasswordBox, placeholder);
        }

        /// <summary>
        /// Clears the placeholder text and sets color when a text box gains focus
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
        /// Hides the placeholder when a password box gains focus
        /// </summary>
        private void PasswordBox_GotFocus(UIElement placeholder)
        {
            placeholder.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Restores placeholder text and sets color if the text box loses focus while empty
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
        /// Updates the visibility of the placeholder based on the content of the password box
        /// </summary>
        private void SetPlaceholderVisibility(PasswordBox passwordBox, UIElement placeholder)
        {
            if (string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                placeholder.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Handles the registration process when the continue button is clicked
        /// </summary>
        private void RegisterContinueButton_Click(object sender, RoutedEventArgs e)
        {
            string username = RegisterUsernameTextBox.Text;
            string email = RegisterEmailTextBox.Text;
            string password = RegisterPasswordTextBox.Password;
            string confirmPassword = RegisterConfirmPasswordTextBox.Password;

            if (!TermsCheckBox.IsChecked.HasValue || !TermsCheckBox.IsChecked.Value)
            {
                MessageBox.Show("You must agree to the terms and conditions to register.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(username) || username == "Username")
            {
                MessageBox.Show("Please enter a username.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || email == "youremail@gmail.com")
            {
                MessageBox.Show("Please enter an email.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password == "Password")
            {
                MessageBox.Show("Please enter a password.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User user = new(username, password, email, 0); // Create a new user with initial score
            string validationMessage = _registrationService.RegisterUser(user);

            if (validationMessage == "Success")
            {
                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                AuthenticationService authenticationService = new();

                if (_loginContext == LoginContext.OriginalLogin)
                {
                    // Opens the LoginWindow for the first player
                    LoginWindow loginWindow = new LoginWindow(authenticationService, LoginContext.OriginalLogin);
                    loginWindow.Show();
                }
                else if (_loginContext == LoginContext.AddSecondPlayer && _originalUser != null)
                {
                    // Navigates to GameSetupWindow with both players set
                    GameSetupWindow gameSetupWindow = new GameSetupWindow("CATEGORY", authenticationService, _originalUser, user);
                    gameSetupWindow.Show();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show(validationMessage, "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Navigates back to the login window when the login button is clicked
        /// </summary>
        private void LoginButton_Click(object sender, MouseButtonEventArgs e)
        {
            AuthenticationService authenticationService = new();

            if (_loginContext == LoginContext.OriginalLogin)
            {
                LoginWindow loginWindow = new LoginWindow(authenticationService, LoginContext.OriginalLogin);
                loginWindow.Show();
            }
            else if (_loginContext == LoginContext.AddSecondPlayer)
            {
                LoginWindow loginWindow = new LoginWindow(authenticationService, LoginContext.AddSecondPlayer);
                loginWindow.Show();
            }

            this.Close();
        }
    }
}
