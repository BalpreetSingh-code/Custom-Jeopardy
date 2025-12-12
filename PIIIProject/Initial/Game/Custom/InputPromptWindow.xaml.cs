using System.Windows;

namespace PIIIProject.Initial.Game.Custom
{
    public partial class InputPromptWindow : Window
    {
        /// <summary>
        /// Holds user's input from the prompt window
        /// </summary>
        public string ResponseText { get; private set; }

        /// <summary>
        /// Initializes input prompt window with a title, message, and optional default value
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="message">Message displayed to the user</param>
        /// <param name="defaultValue">Optional default text prefilled in the input box</param>
        public InputPromptWindow(string title, string message, string defaultValue = "")
        {
            InitializeComponent();
            Title = title;
            PromptMessage.Text = message; 
            InputTextBox.Text = defaultValue;
        }

        /// <summary>
        /// Handles OK button click
        /// Sets response text and closes the dialog with true
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = InputTextBox.Text.Trim(); 
            DialogResult = true; 
        }

        /// <summary>
        /// Handles Cancel button click
        /// Closes the dialog with false
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
        }
    }
}
