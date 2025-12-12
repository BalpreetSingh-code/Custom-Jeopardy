namespace PIIIProject.Initial.Models
{
    public class Subcategory
    {
        public string Name { get; set; }
        public List<Question> Questions { get; private set; } = new();

        public Subcategory(string name)
        {
            Name = name;
        }

        public bool AddQuestion(Question question)
        {
            if (Questions.Count >= 4) return false; // Max 4 questions
            if (Questions.Any(q => q.PointValue == question.PointValue)) return false; // No duplicate point values
            Questions.Add(question);
            return true;
        }
    }

}
