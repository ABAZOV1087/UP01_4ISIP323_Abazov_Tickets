using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class CategoryTabPage : Page
    {
        public CategoryTabPage()
        {
            InitializeComponent();
            DataGridCategory.ItemsSource = Core.Context.Category.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridCategory.ItemsSource = Core.Context.Category.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddCategoryPage(null));

        private void ButtonEdit_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new AddCategoryPage((sender as Button).DataContext as Category));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var categoryForRemoving = DataGridCategory.SelectedItems.Cast<Category>().ToList();
            if (categoryForRemoving.Count == 0)
            {
                Core.ShowError("Выберите категории для удаления!");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {categoryForRemoving.Count} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Category.RemoveRange(categoryForRemoving);
                    Core.Context.SaveChanges();
                    Core.ShowInfo("Данные успешно удалены!");
                    DataGridCategory.ItemsSource = Core.Context.Category.ToList();
                }
                catch (Exception ex)
                {
                    Core.ShowError(ex.Message);
                }
            }
        }
    }
}