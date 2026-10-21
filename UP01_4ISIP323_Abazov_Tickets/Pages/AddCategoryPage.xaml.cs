using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class AddCategoryPage : Page
    {
        private Category _currentCategory = new Category();

        public AddCategoryPage(Category selectedCategory)
        {
            InitializeComponent();
            if (selectedCategory != null)
                _currentCategory = selectedCategory;
            DataContext = _currentCategory;
        }

        private void ButtonSaveCategory_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentCategory.Name))
                errors.AppendLine("Укажите название категории!");

            if (errors.Length > 0)
            {
                Core.ShowError(errors.ToString());
                return;
            }

            if (_currentCategory.ID == 0)
                Core.Context.Category.Add(_currentCategory);

            try
            {
                Core.Context.SaveChanges();
                Core.ShowInfo("Данные успешно сохранены!");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                Core.ShowError(ex.Message);
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBCategoryName.Text = "";
        }
    }
}