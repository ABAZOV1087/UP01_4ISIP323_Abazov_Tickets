using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class OrganizerTabPage : Page
    {
        public OrganizerTabPage()
        {
            InitializeComponent();
            this.IsVisibleChanged += (s, e) => { if (Visibility == Visibility.Visible) DataGridOrganizer.ItemsSource = Core.Context.Organizer.ToList(); };
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddOrganizerPage(null));
        private void ButtonEdit_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddOrganizerPage((sender as Button).DataContext as Organizer));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var items = DataGridOrganizer.SelectedItems.Cast<Organizer>().ToList();
            if (items.Count == 0) { Core.ShowError("Выберите записи для удаления!"); return; }
            if (MessageBox.Show($"Удалить организаторов ({items.Count} шт.)?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Organizer.RemoveRange(items);
                    Core.Context.SaveChanges();
                    Core.ShowInfo("Данные удалены!");
                    DataGridOrganizer.ItemsSource = Core.Context.Organizer.ToList();
                }
                catch (Exception ex) { Core.ShowError(ex.Message); }
            }
        }
    }
}