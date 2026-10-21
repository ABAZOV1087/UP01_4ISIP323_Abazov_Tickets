using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class AddTicketSalePage : Page
    {
        private TicketSale _currentSale = new TicketSale { Date = DateTime.Today };

        public AddTicketSalePage(TicketSale selectedSale)
        {
            InitializeComponent();
            CBCategory.ItemsSource = Core.Context.Category.ToList();
            CBUser.ItemsSource = Core.Context.User.ToList();

            if (selectedSale != null)
                _currentSale = selectedSale;

            DataContext = _currentSale;
        }

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != ',' && c != '.')
                    e.Handled = true;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentSale.Name))
                errors.AppendLine("Укажите название мероприятия!");
            if (_currentSale.Num < 1 || _currentSale.Num > 50)
                errors.AppendLine("Количество билетов должно быть от 1 до 50!");
            if (_currentSale.Price < 100 || _currentSale.Price > 500000)
                errors.AppendLine("Цена билета должна быть от 100 до 500 000 руб.!");
            if (_currentSale.UserID <= 0)
                errors.AppendLine("Выберите покупателя!");
            if (_currentSale.CategoryID <= 0)
                errors.AppendLine("Выберите категорию мероприятия!");

            if (errors.Length > 0)
            {
                Core.ShowError(errors.ToString());
                return;
            }

            if (_currentSale.ID == 0)
                Core.Context.TicketSale.Add(_currentSale);

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
            TBPaymentName.Text = "";
            TBAmount.Text = "";
            TBCount.Text = "";
            CBUser.SelectedIndex = -1;
            CBCategory.SelectedIndex = -1;
        }
    }
}