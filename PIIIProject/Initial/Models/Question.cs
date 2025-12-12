namespace PIIIProject.Initial.Models
{
    public class Question
    {
        /// <summary>
        /// Gets or sets the text of the question
        /// </summary>
        public string Text { get; set; } // The question text.

        /// <summary>
        /// Gets or sets the possible answers for the question
        /// </summary>
        public string[] Answers { get; set; } // Possible answers.

        /// <summary>
        /// Gets or sets the index of the correct answer within the Answers array
        /// </summary>
        public int CorrectAnswerIndex { get; set; } // Index of the correct answer.

        /// <summary>
        /// Gets or sets the point value awarded for answering the question correctly
        /// </summary>
        public int PointValue { get; set; } // Points awarded for the question.

        /// <summary>
        /// Gets or sets a value indicating whether the question has been answered
        /// Default is false
        /// </summary>
        public bool IsAnswered { get; set; } // Tracks if the question has been answered.

        /// <summary>
        /// Initializes a new instance of the Question class with the specified details
        /// </summary>
        /// <param name="text">Text of the question</param>
        /// <param name="answers">Array of possible answers</param>
        /// <param name="correctAnswerIndex">Index of the correct answer in the answers array</param>
        /// <param name="pointValue">Points awarded for the question</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the correctAnswerIndex is outside the bounds of the answers array
        /// </exception>
        public Question(string text, string[] answers, int correctAnswerIndex, int pointValue)
        {
            if (correctAnswerIndex < 0 || correctAnswerIndex >= answers.Length)
                throw new ArgumentOutOfRangeException(nameof(correctAnswerIndex), "Index is out of bounds for the answers array.");

            Text = text;
            Answers = answers;
            CorrectAnswerIndex = correctAnswerIndex;
            PointValue = pointValue;
            IsAnswered = false; // Default to not answered.
        }
    }
}
