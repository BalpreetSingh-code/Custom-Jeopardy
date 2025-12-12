using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using PIIIProject.Initial.Models;

namespace PIIIProject.Initial.Game
{

    public partial class QuestionWindow : Window
    {
        private Question currentQuestion; // The current question being displayed
        private int timeLeft = 30; // Time limit for answering
        private DispatcherTimer timer; // Timer for the countdown
        private bool isAnswered = false; // Tracks if the question has been answered
        private Action<bool, int> resultCallback; // Callback to notify the result

        /// <summary>
        /// Constructor for the QuestionWindow.
        /// </summary>
        /// <param name="question">The question to display.</param>
        /// <param name="onAnswerSelected">Callback for handling the result of the answer.</param>
        public QuestionWindow(Question question, Action<bool, int> onAnswerSelected)
        {
            InitializeComponent();
            currentQuestion = question; // Set the question
            resultCallback = onAnswerSelected; // Set the callback
            DisplayQuestion(); // Display the question and answers
            StartCountdown(); // Start the timer
        }

        /// <summary>
        /// Displays the question and shuffles the answers.
        /// </summary>
        private void DisplayQuestion()
        {
            QuestionText.Text = currentQuestion.Text; // Set the question text

            // Shuffle the answers
            List<string> shuffledAnswers = new List<string>(currentQuestion.Answers);
            shuffledAnswers = ShuffleList(shuffledAnswers);

            // Assign answers to buttons
            AnswerButton1.Content = shuffledAnswers[0];
            AnswerButton2.Content = shuffledAnswers[1];
            AnswerButton3.Content = shuffledAnswers[2];
            AnswerButton4.Content = shuffledAnswers[3];

            // Store whether each button is the correct answer
            AnswerButton1.Tag = shuffledAnswers[0] == currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];
            AnswerButton2.Tag = shuffledAnswers[1] == currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];
            AnswerButton3.Tag = shuffledAnswers[2] == currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];
            AnswerButton4.Tag = shuffledAnswers[3] == currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];
        }

        /// <summary>
        /// Starts the countdown timer.
        /// </summary>
        private void StartCountdown()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // 1-second interval
            };
            timer.Tick += TimerTick; // Attach the tick handler
            timer.Start(); // Start the timer
        }

        /// <summary>
        /// Handles the countdown timer tick.
        /// </summary>
        private void TimerTick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--; // Decrement time
                TimerText.Text = timeLeft.ToString(); // Update the timer display
            }
            else
            {
                timer.Stop(); // Stop the timer
                HandleTimeUp(); // Handle time expiration
            }
        }

        /// <summary>
        /// Handles what happens when time runs out.
        /// </summary>
        private void HandleTimeUp()
        {
            if (!isAnswered)
            {
                MessageBox.Show($"Time's up! The correct answer was: {currentQuestion.Answers[currentQuestion.CorrectAnswerIndex]}",
                    "Time's Up", MessageBoxButton.OK, MessageBoxImage.Information);
                ReturnToBoardButton.Visibility = Visibility.Visible; // Show the "Return to Board" button
            }

            DisableAnswerButtons(); // Disable all answer buttons
        }

        /// <summary>
        /// Handles the click event for an answer button.
        /// </summary>
        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (isAnswered) return; // Prevent multiple answers

            isAnswered = true; // Mark as answered
            timer.Stop(); // Stop the timer

            Button button = sender as Button; // Get the clicked button
            bool isCorrect = (bool)button.Tag; // Check if the answer is correct

            if (isCorrect)
            {
                MessageBox.Show("Correct Answer!", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Wrong Answer! The correct answer was: {currentQuestion.Answers[currentQuestion.CorrectAnswerIndex]}",
                    "Result", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            resultCallback(isCorrect, currentQuestion.PointValue); // Notify the result
            ReturnToBoardButton.Visibility = Visibility.Visible; // Show the "Return to Board" button
            DisableAnswerButtons(); // Disable all buttons
        }

        /// <summary>
        /// Disables all answer buttons.
        /// </summary>
        private void DisableAnswerButtons()
        {
            AnswerButton1.IsEnabled = false;
            AnswerButton2.IsEnabled = false;
            AnswerButton3.IsEnabled = false;
            AnswerButton4.IsEnabled = false;
        }

        /// <summary>
        /// Handles the "Return to Board" button click event.
        /// </summary>
        private void ReturnToBoardButton_Click(object sender, RoutedEventArgs e)
        {
            Close(); // Close the window
        }

        /// <summary>
        /// Utility method to shuffle a list of answers.
        /// </summary>
        private List<string> ShuffleList(List<string> list)
        {
            Random random = new Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]); // Swap
            }
            return list;
        }
    }
}