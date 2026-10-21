using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
        {
            InitializeComponent();
            DataGridUser.ItemsSource = Core.Context.User.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridUser.ItemsSource = Core.Context.User.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddUserPage(null));

        private void ButtonEdit_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new AddUserPage((sender as Button).DataContext as User));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var usersForRemoving = DataGridUser.SelectedItems.Cast<User>().ToList();
            if (usersForRemoving.Count == 0)
            {
                Core.ShowError("Выберите записи для удаления!");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {usersForRemoving.Count} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.User.RemoveRange(usersForRemoving);
                    Core.Context.SaveChanges();
                    Core.ShowInfo("Данные успешно удалены!");
                    DataGridUser.ItemsSource = Core.Context.User.ToList();
                }
                catch (Exception ex)
                {
                    Core.ShowError(ex.Message);
                }
            }
        }
    }
}