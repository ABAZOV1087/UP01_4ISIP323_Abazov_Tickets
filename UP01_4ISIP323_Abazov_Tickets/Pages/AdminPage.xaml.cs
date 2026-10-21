using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void BtnTab1_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new UsersTabPage());
        private void BtnTab2_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new CategoryTabPage());
        private void BtnTab3_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new VenueTabPage());
        private void BtnTab4_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new OrganizerTabPage());
        private void BtnTab5_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new EventTabPage());
        private void BtnTab6_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new PromocodeTabPage());
        private void BtnTab7_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new TicketSaleTabPage());
        private void BtnDiagrams_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new DiagrammPage());
    }
}