using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class EventTabPage : Page
    {
        public EventTabPage()
        {
            InitializeComponent();
            this.IsVisibleChanged += (s, e) => { if (Visibility == Visibility.Visible) DataGridEvent.ItemsSource = Core.Context.Event.ToList(); };
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddEventPage(null));
        private void ButtonEdit_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddEventPage((sender as Button).DataContext as Event));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var items = DataGridEvent.SelectedItems.Cast<Event>().ToList();
            if (items.Count == 0) { Core.ShowError("Выберите записи для удаления!"); return; }
            if (MessageBox.Show($"Удалить мероприятия ({items.Count} шт.)?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Event.RemoveRange(items);
                    Core.Context.SaveChanges();
                    Core.ShowInfo("Данные удалены!");
                    DataGridEvent.ItemsSource = Core.Context.Event.ToList();
                }
                catch (Exception ex) { Core.ShowError(ex.Message); }
            }
        }
    }
}