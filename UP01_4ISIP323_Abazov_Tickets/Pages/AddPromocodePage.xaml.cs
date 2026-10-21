using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class AddPromocodePage : Page
    {
        private Promocode _current = new Promocode { ValidUntil = DateTime.Today.AddMonths(1) };

        public AddPromocodePage(Promocode selected)
        {
            InitializeComponent();
            if (selected != null) _current = selected;
            DataContext = _current;
        }

        private void TbDiscount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
                if (!char.IsDigit(c)) e.Handled = true;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_current.Code)) errors.AppendLine("Укажите код промокода!");
            if (_current.DiscountPercent < 1 || _current.DiscountPercent > 90) errors.AppendLine("Скидка должна быть от 1 до 90%!");

            if (errors.Length > 0) { Core.ShowError(errors.ToString()); return; }

            if (_current.ID == 0) Core.Context.Promocode.Add(_current);
            try
            {
                Core.Context.SaveChanges();
                Core.ShowInfo("Промокод сохранен!");
                NavigationService?.GoBack();
            }
            catch (Exception ex) { Core.ShowError(ex.Message); }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TbCode.Text = "";
            TbDiscount.Text = "";
        }
    }
}