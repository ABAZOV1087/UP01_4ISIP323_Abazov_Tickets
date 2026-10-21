using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class VenueTabPage : Page
    {
        public VenueTabPage()
        {
            InitializeComponent();
            UpdateData();
            this.IsVisibleChanged += (s, e) => { if (Visibility == Visibility.Visible) UpdateData(); };
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e) => UpdateData();

        private void UpdateData()
        {
            var list = Core.Context.Venue.ToList();
            if (!string.IsNullOrWhiteSpace(TbSearch.Text))
                list = list.Where(x => x.Name.ToLower().Contains(TbSearch.Text.ToLower()) || x.Address.ToLower().Contains(TbSearch.Text.ToLower())).ToList();
            DataGridVenue.ItemsSource = list;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddVenuePage(null));
        private void ButtonEdit_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddVenuePage((sender as Button).DataContext as Venue));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var items = DataGridVenue.SelectedItems.Cast<Venue>().ToList();
            if (items.Count == 0) { Core.ShowError("Выберите записи для удаления!"); return; }
            if (MessageBox.Show($"Удалить выбранные площадки ({items.Count} шт.)?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Venue.RemoveRange(items);
                    Core.Context.SaveChanges();
                    Core.ShowInfo("Данные удалены!");
                    UpdateData();
                }
                catch (Exception ex) { Core.ShowError(ex.Message); }
            }
        }
    }
}