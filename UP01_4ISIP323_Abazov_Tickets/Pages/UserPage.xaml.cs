using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            UpdateUsers();
        }

        private void clearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            fioFilterTextBox.Text = "";
            sortComboBox.SelectedIndex = 0;
            onlyAdminCheckBox.IsChecked = false;
        }

        private void fioFilterTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateUsers();
        private void sortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateUsers();
        private void onlyAdminCheckBox_Checked(object sender, RoutedEventArgs e) => UpdateUsers();
        private void onlyAdminCheckBox_Unchecked(object sender, RoutedEventArgs e) => UpdateUsers();

        private void UpdateUsers()
        {
            if (!IsInitialized) return;
            try
            {
                List<User> currentUsers = Core.Context.User.ToList();

                if (!string.IsNullOrWhiteSpace(fioFilterTextBox.Text))
                {
                    currentUsers = currentUsers.Where(x => x.FIO.ToLower().Contains(fioFilterTextBox.Text.ToLower())).ToList();
                }

                if (onlyAdminCheckBox.IsChecked == true)
                {
                    currentUsers = currentUsers.Where(x => x.Role == "Admin").ToList();
                }

                ListUser.ItemsSource = (sortComboBox.SelectedIndex == 0)
                    ? currentUsers.OrderBy(x => x.FIO).ToList()
                    : currentUsers.OrderByDescending(x => x.FIO).ToList();
            }
            catch (Exception)
            {
            }
        }
    }
}