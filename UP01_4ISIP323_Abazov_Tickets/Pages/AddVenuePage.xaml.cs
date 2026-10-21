using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class AddVenuePage : Page
    {
        private Venue _current = new Venue();

        public AddVenuePage(Venue selected)
        {
            InitializeComponent();
            if (selected != null) _current = selected;
            DataContext = _current;
        }

        private void TbCapacity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
                if (!char.IsDigit(c)) e.Handled = true;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_current.Name)) errors.AppendLine("Укажите название площадки!");
            if (string.IsNullOrWhiteSpace(_current.Address)) errors.AppendLine("Укажите адрес площадки!");
            if (_current.Capacity < 10 || _current.Capacity > 100000) errors.AppendLine("Вместимость должна быть от 10 до 100 000 человек!");

            if (errors.Length > 0) { Core.ShowError(errors.ToString()); return; }

            if (_current.ID == 0) Core.Context.Venue.Add(_current);
            try
            {
                Core.Context.SaveChanges();
                Core.ShowInfo("Данные сохранены!");
                NavigationService?.GoBack();
            }
            catch (Exception ex) { Core.ShowError(ex.Message); }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TbName.Text = "";
            TbAddress.Text = "";
            TbCapacity.Text = "";
        }
    }
}