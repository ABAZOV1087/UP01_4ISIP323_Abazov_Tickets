using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class AddEventPage : Page
    {
        private Event _current = new Event { EventDate = DateTime.Today, AgeLimit = "12+" };

        public AddEventPage(Event selected)
        {
            InitializeComponent();
            CmbCategory.ItemsSource = Core.Context.Category.ToList();
            CmbVenue.ItemsSource = Core.Context.Venue.ToList();
            CmbOrganizer.ItemsSource = Core.Context.Organizer.ToList();

            if (selected != null) _current = selected;
            DataContext = _current;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_current.Title)) errors.AppendLine("Укажите название мероприятия!");
            if (_current.CategoryID <= 0) errors.AppendLine("Выберите категорию!");
            if (_current.VenueID <= 0) errors.AppendLine("Выберите площадку!");
            if (_current.OrganizerID <= 0) errors.AppendLine("Выберите организатора!");
            if (string.IsNullOrWhiteSpace(CmbAge.Text)) errors.AppendLine("Укажите возрастной рейтинг!");
            else _current.AgeLimit = CmbAge.Text;

            if (errors.Length > 0) { Core.ShowError(errors.ToString()); return; }

            if (_current.ID == 0) Core.Context.Event.Add(_current);
            try
            {
                Core.Context.SaveChanges();
                Core.ShowInfo("Мероприятие сохранено!");
                NavigationService?.GoBack();
            }
            catch (Exception ex) { Core.ShowError(ex.Message); }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TbTitle.Text = "";
            CmbCategory.SelectedIndex = -1;
            CmbVenue.SelectedIndex = -1;
            CmbOrganizer.SelectedIndex = -1;
        }
    }
}