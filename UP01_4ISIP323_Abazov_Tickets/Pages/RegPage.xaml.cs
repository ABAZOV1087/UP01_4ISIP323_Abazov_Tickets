using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
            comboBxRole.SelectedIndex = 0;
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtbxLog.Text) || string.IsNullOrEmpty(txtbxFIO.Text) ||
                string.IsNullOrEmpty(passBxFrst.Password) || string.IsNullOrEmpty(passBxScnd.Password))
            {
                Core.ShowError("Заполните все поля!");
                return;
            }

            var existingUser = Core.Context.User.AsNoTracking().FirstOrDefault(u => u.Login == txtbxLog.Text);
            if (existingUser != null)
            {
                Core.ShowError("Пользователь с таким логином уже существует!");
                return;
            }

            if (!Core.ValidatePassword(passBxFrst.Password, out string errorMsg))
            {
                Core.ShowError(errorMsg);
                return;
            }

            if (passBxFrst.Password != passBxScnd.Password)
            {
                Core.ShowError("Пароли не совпадают!");
                return;
            }

            User userObject = new User
            {
                FIO = txtbxFIO.Text.Trim(),
                Login = txtbxLog.Text.Trim(),
                Password = Core.GetHash(passBxFrst.Password),
                Role = comboBxRole.Text
            };

            Core.Context.User.Add(userObject);
            Core.Context.SaveChanges();
            Core.ShowInfo("Пользователь успешно зарегистрирован!");

            txtbxLog.Clear();
            passBxFrst.Clear();
            passBxScnd.Clear();
            comboBxRole.SelectedIndex = 0;
            txtbxFIO.Clear();
        }

        private void txtbxFIO_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsLetter(c) && c != '-')
                    e.Handled = true;
            }
        }

        private void lblLogHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => txtbxLog.Focus();
        private void lblPassHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => passBxFrst.Focus();
        private void lblPassSecHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => passBxScnd.Focus();
        private void lblFioHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => txtbxFIO.Focus();

        private void txtbxLog_TextChanged(object sender, TextChangedEventArgs e) =>
            lblLogHitn.Visibility = txtbxLog.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;

        private void passBxFrst_PasswordChanged(object sender, RoutedEventArgs e) =>
            lblPassHitn.Visibility = passBxFrst.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;

        private void passBxScnd_PasswordChanged(object sender, RoutedEventArgs e) =>
            lblPassSecHitn.Visibility = passBxScnd.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;

        private void txtbxFIO_TextChanged(object sender, TextChangedEventArgs e) =>
            lblFioHitn.Visibility = txtbxFIO.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;
    }
}