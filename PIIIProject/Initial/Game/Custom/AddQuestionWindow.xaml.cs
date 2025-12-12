using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using PIIIProject.Initial.Models;

namespace PIIIProject.Initial.Game.Custom
{
    public partial class AddQuestionWindow : Window
    {
        private CustomCategory _category; // Parent category for the questions
        private string _currentType; // Current subcategory type
        private ObservableCollection<Question> _questions; // Collection of questions for binding
        public Question? NewQuestion { get; private set; } // Holds the most recently added question

        /// <summary>
        /// Initializes the AddQuestionWindow with a category and type
        /// </summary>
        /// <param name="category">Parent category</param>
        /// <param name="typeName">Type name of the subcategory</param>
        public AddQuestionWindow(CustomCategory category, string typeName)
        {
            InitializeComponent();
            _category = category;
            _currentType = typeName;

            // Ensure the type exists in the category
            if (!_category.Types.ContainsKey(typeName))
            {
                _category.Types[typeName] = new List<Question>();
            }

            // Bind questions from the subcategory to the UI
            _questions = new ObservableCollection<Question>(_category.Types[typeName]);
            QuestionsComboBox.ItemsSource = _questions;
            QuestionsComboBox.DisplayMemberPath = "Text";
        }

        /// <summary>
        /// Adds a new question to the current type after validation
        /// </summary>
        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetInput(out string questionText, out string[] options, out int correctAnswerIndex, out int pointValue)) return;

            // Ensure no duplicate point values
            if (_questions.Any(question => question.PointValue == pointValue))
            {
                MessageBox.Show($"A question with {pointValue} points already exists in this sub category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Add the new question to the collection
            Question newQuestion = new Question(questionText, options, correctAnswerIndex, pointValue);
            _questions.Add(newQuestion);
            QuestionsComboBox.SelectedItem = newQuestion; 

            ClearInputFields(); 
        }

        /// <summary>
        /// Edits the currently selected question after validation
        /// </summary>
        private void EditQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionsComboBox.SelectedItem is Question selectedQuestion)
            {
                if (!TryGetInput(out string questionText, out string[] options, out int correctAnswerIndex, out int pointValue)) return;

                // Prevent duplicate point values if editing the point value
                if (selectedQuestion.PointValue != pointValue && _questions.Any(question => question.PointValue == pointValue))
                {
                    MessageBox.Show($"A question with {pointValue} points already exists in this sub category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update the selected question with new data
                selectedQuestion.Text = questionText;
                selectedQuestion.Answers = options;
                selectedQuestion.CorrectAnswerIndex = correctAnswerIndex;
                selectedQuestion.PointValue = pointValue;

                QuestionsComboBox.Items.Refresh(); 
            }
            else
            {
                MessageBox.Show("Please select a question to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Saves the questions to the category after validating their structure
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure exactly 4 questions exist with unique and valid point values
            if (_questions.Count != 4)
            {
                MessageBox.Show("Each type must contain exactly 4 questions with unique point values (200, 400, 600, 800).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_questions.Select(question => question.PointValue).OrderBy(val => val).SequenceEqual(new[] { 200, 400, 600, 800 }))
            {
                MessageBox.Show("Questions must have point values of 200, 400, 600, and 800.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Save the questions to the category
            _category.Types[_currentType] = _questions.ToList();
            this.DialogResult = true; 
            this.Close();
        }

        /// <summary>
        /// Cancels the operation and closes the window without saving
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; 
            this.Close();
        }

        /// <summary>
        /// Validates and retrieves input data for a question
        /// </summary>
        /// <param name="questionText">Question text</param>
        /// <param name="options">Array of answer options</param>
        /// <param name="correctAnswerIndex">Index of the correct answer</param>
        /// <param name="pointValue">Point value for the question</param>
        /// <returns>True if the input is valid, otherwise false</returns>
        private bool TryGetInput(out string questionText, out string[] options, out int correctAnswerIndex, out int pointValue)
        {
            questionText = QuestionTextBox.Text.Trim();
            options = OptionsTextBox.Text.Trim().Split(',');
            correctAnswerIndex = 0;
            pointValue = 0;

            // Ensure all fields are filled
            if (string.IsNullOrEmpty(questionText) ||
                string.IsNullOrEmpty(CorrectAnswerIndexTextBox.Text.Trim()) ||
                string.IsNullOrEmpty(OptionsTextBox.Text.Trim()) ||
                PointValueComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please fill out all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate numeric inputs
            if (!int.TryParse(CorrectAnswerIndexTextBox.Text.Trim(), out correctAnswerIndex) ||
                !int.TryParse((PointValueComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(), out pointValue))
            {
                MessageBox.Show("Correct Answer Index and Point Value must be valid numbers.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Ensure the correct answer index is within range
            if (correctAnswerIndex < 0 || correctAnswerIndex >= options.Length)
            {
                MessageBox.Show("Correct Answer Index is out of range.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true; 
        }

        /// <summary>
        /// Clears all input fields for adding/editing a question
        /// </summary>
        private void ClearInputFields()
        {
            QuestionTextBox.Text = string.Empty;
            OptionsTextBox.Text = string.Empty;
            CorrectAnswerIndexTextBox.Text = string.Empty;
            PointValueComboBox.SelectedIndex = 0; 
        }
    }
}
