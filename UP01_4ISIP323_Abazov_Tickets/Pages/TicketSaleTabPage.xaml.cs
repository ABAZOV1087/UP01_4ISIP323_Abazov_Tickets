using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class TicketSaleTabPage : Page
    {
        public TicketSaleTabPage()
        {
            InitializeComponent();
            var categories = Core.Context.Category.ToList();
            categories.Insert(0, new Category { ID = 0, Name = "Все категории" });
            CmbFilterCategory.ItemsSource = categories;
            CmbFilterCategory.SelectedIndex = 0;

            UpdateSales();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                UpdateSales();
            }
        }

        private void Filter_Changed(object sender, EventArgs e) => UpdateSales();

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            TbSearchEvent.Text = "";
            CmbFilterCategory.SelectedIndex = 0;
            CmbSortPrice.SelectedIndex = 0;
            UpdateSales();
        }

        private void UpdateSales()
        {
            if (!IsInitialized) return;
            List<TicketSale> sales = Core.Context.TicketSale.ToList();

            if (!string.IsNullOrWhiteSpace(TbSearchEvent.Text))
                sales = sales.Where(x => x.Name.ToLower().Contains(TbSearchEvent.Text.ToLower())).ToList();

            if (CmbFilterCategory.SelectedItem is Category selectedCat && selectedCat.ID != 0)
                sales = sales.Where(x => x.CategoryID == selectedCat.ID).ToList();

            sales = (CmbSortPrice.SelectedIndex == 0)
                ? sales.OrderBy(x => x.Price).ToList()
                : sales.OrderByDescending(x => x.Price).ToList();

            DataGridTicketSale.ItemsSource = sales;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddTicketSalePage(null));

        private void ButtonEdit_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new AddTicketSalePage((sender as Button).DataContext as TicketSale));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var salesForRemoving = DataGridTicketSale.SelectedItems.Cast<TicketSale>().ToList();
            if (salesForRemoving.Count == 0)
            {
                Core.ShowError("Выберите записи для удаления!");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {salesForRemoving.Count} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.TicketSale.RemoveRange(salesForRemoving);
                    Core.Context.SaveChanges();
                    Core.ShowInfo("Данные успешно удалены!");
                    UpdateSales();
                }
                catch (Exception ex)
                {
                    Core.ShowError(ex.Message);
                }
            }
        }
    }
}