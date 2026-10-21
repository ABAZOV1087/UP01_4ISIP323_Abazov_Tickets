using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class AddOrganizerPage : Page
    {
        private Organizer _current = new Organizer();

        public AddOrganizerPage(Organizer selected)
        {
            InitializeComponent();
            if (selected != null) _current = selected;
            DataContext = _current;
        }

        private void TbPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
                if (!char.IsDigit(c)) e.Handled = true;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_current.Name)) errors.AppendLine("Укажите наименование организатора!");
            if (string.IsNullOrWhiteSpace(_current.Phone) || _current.Phone.Length < 10) errors.AppendLine("Укажите корректный номер телефона!");
            if (string.IsNullOrWhiteSpace(_current.Email) || !_current.Email.Contains("@")) errors.AppendLine("Укажите корректный Email!");

            if (errors.Length > 0) { Core.ShowError(errors.ToString()); return; }

            if (_current.ID == 0) Core.Context.Organizer.Add(_current);
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
            TbPhone.Text = "";
            TbEmail.Text = "";
        }
    }
}