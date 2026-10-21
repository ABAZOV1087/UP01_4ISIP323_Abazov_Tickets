using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class PromocodeTabPage : Page
    {
        public PromocodeTabPage()
        {
            InitializeComponent();
            this.IsVisibleChanged += (s, e) => { if (Visibility == Visibility.Visible) DataGridPromo.ItemsSource = Core.Context.Promocode.ToList(); };
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddPromocodePage(null));
        private void ButtonEdit_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddPromocodePage((sender as Button).DataContext as Promocode));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var items = DataGridPromo.SelectedItems.Cast<Promocode>().ToList();
            if (items.Count == 0) { Core.ShowError("Выберите записи для удаления!"); return; }
            if (MessageBox.Show($"Удалить промокоды ({items.Count} шт.)?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Promocode.RemoveRange(items);
                    Core.Context.SaveChanges();
                    Core.ShowInfo("Данные удалены!");
                    DataGridPromo.ItemsSource = Core.Context.Promocode.ToList();
                }
                catch (Exception ex) { Core.ShowError(ex.Message); }
            }
        }
    }
}