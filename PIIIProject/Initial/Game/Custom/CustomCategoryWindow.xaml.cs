using System.Collections.ObjectModel;
using System.Windows;
using PIIIProject.Initial.Models;

namespace PIIIProject.Initial.Game.Custom
{
    public partial class CustomCategoriesWindow : Window
    {
        private ObservableCollection<CustomCategory> _customCategories; // Holds the list of custom categories
        private User _currentUser; // Current user managing categories
        private readonly CustomCategoryService _customCategoryService; // Service to handle category operations

        /// <summary>
        /// Initializes the CustomCategoriesWindow with the current user and category service
        /// </summary>
        /// <param name="currentUser">User managing the categories</param>
        /// <param name="customCategoryService">Service for handling category data</param>
        public CustomCategoriesWindow(User currentUser, CustomCategoryService customCategoryService)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _customCategoryService = customCategoryService;
            _customCategories = new ObservableCollection<CustomCategory>(_customCategoryService.LoadCustomCategoriesForUser(_currentUser.Username));
            CustomCategoriesList.ItemsSource = _customCategories;
            CustomCategoriesList.DisplayMemberPath = "Name";
        }

        /// <summary>
        /// Adds a new category after validating its uniqueness
        /// </summary>
        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            string newCategoryName = PromptForCategory("Add New Category", "Enter a category name:");
            if (!string.IsNullOrWhiteSpace(newCategoryName))
            {
                if (_customCategories.Any(category => category.Name == newCategoryName))
                {
                    MessageBox.Show("This category already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    CustomCategory newCategory = new CustomCategory(newCategoryName);

                    // Temporarily add the category for editing
                    _customCategories.Add(newCategory);

                    if (!EditCategory(newCategory))
                    {
                        // Remove the category if editing is incomplete
                        _customCategories.Remove(newCategory);
                        MessageBox.Show($"Category '{newCategoryName}' was not completed and has been removed.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        /// <summary>
        /// Edits the selected category and ensures subcategories are created
        /// </summary>
        private void EditCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomCategoriesList.SelectedItem is CustomCategory selectedCategory)
            {
                if (!EditCategory(selectedCategory))
                {
                    MessageBox.Show($"Category '{selectedCategory.Name}' was not completed and will remain unchanged.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a category to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Allows adding and managing subcategories within a category
        /// </summary>
        private bool EditCategory(CustomCategory selectedCategory)
        {
            int initialSubcategoryCount = selectedCategory.Types.Count;

            while (selectedCategory.Types.Count < 4) // Ensure 4 subcategories exist
            {
                string newSubcategoryName = PromptForCategory(
                    $"Add Subcategory {selectedCategory.Types.Count + 1}",
                    $"Enter the name for subcategory {selectedCategory.Types.Count + 1}:"
                );

                if (string.IsNullOrWhiteSpace(newSubcategoryName))
                {
                    break;
                }

                if (selectedCategory.Types.ContainsKey(newSubcategoryName))
                {
                    MessageBox.Show($"The subcategory '{newSubcategoryName}' already exists. Please enter a unique name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    continue;
                }

                selectedCategory.Types[newSubcategoryName] = new List<Question>(); // Add new subcategory

                AddQuestionWindow addQuestionWindow = new AddQuestionWindow(selectedCategory, newSubcategoryName); // Open question editing dialog
                if (addQuestionWindow.ShowDialog() != true)
                {
                    selectedCategory.Types.Remove(newSubcategoryName); // Remove incomplete subcategory
                    MessageBox.Show("You must complete all questions for this subcategory.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    continue;
                }
            }

            if (selectedCategory.Types.Count < 4)
            {
                // Restore the subcategory count if the category is incomplete
                while (selectedCategory.Types.Count > initialSubcategoryCount)
                {
                    var lastSubcategory = selectedCategory.Types.Last();
                    selectedCategory.Types.Remove(lastSubcategory.Key);
                }
                return false;
            }

            MessageBox.Show("Category and subcategories successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }

        /// <summary>
        /// Deletes the selected category after confirmation
        /// </summary>
        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomCategoriesList.SelectedItem is CustomCategory selectedCategory)
            {
                if (MessageBox.Show($"Are you sure you want to delete the category \"{selectedCategory.Name}\"?",
                                    "Confirm Delete",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _customCategories.Remove(selectedCategory); // Remove category from the collection
                    MessageBox.Show($"Category '{selectedCategory.Name}' deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a category to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Saves all categories after validating their structure
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var category in _customCategories)
            {
                foreach (var subcategory in category.Types)
                {
                    if (subcategory.Value.Count != 4)
                    {
                        MessageBox.Show($"Subcategory '{subcategory.Key}' in category '{category.Name}' must have exactly 4 questions with unique point values (200, 400, 600, 800).",
                                        "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var pointValues = subcategory.Value.Select(question => question.PointValue).Distinct().OrderBy(val => val).ToList();
                    if (!pointValues.SequenceEqual(new[] { 200, 400, 600, 800 }))
                    {
                        MessageBox.Show($"Subcategory '{subcategory.Key}' in category '{category.Name}' must have questions with unique point values of 200, 400, 600, and 800.",
                                        "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            if (_customCategories.Any())
            {
                MessageBox.Show("All categories and subcategories validated successfully. Saving your data!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _customCategoryService.SaveCustomCategoriesForUser(_currentUser.Username, _customCategories.ToList());
            }
            else
            {
                MessageBox.Show("No categories to save.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Prompts the user for category or subcategory names
        /// </summary>
        private string PromptForCategory(string title, string message, string defaultValue = "")
        {
            InputPromptWindow inputWindow = new InputPromptWindow(title, message, defaultValue);
            if (inputWindow.ShowDialog() == true)
            {
                return inputWindow.ResponseText.Trim();
            }
            return string.Empty;
        }
    }
}
