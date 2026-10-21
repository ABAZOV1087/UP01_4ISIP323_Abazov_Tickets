using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        private void BtnSavePass_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentPasswordBox.Password) ||
                string.IsNullOrEmpty(NewPasswordBox.Password) ||
                string.IsNullOrEmpty(ConfirmPasswordBox.Password) ||
                string.IsNullOrEmpty(TbLogin.Text))
            {
                Core.ShowError("Все поля обязательны к заполнению!");
                return;
            }

            string hashedPass = Core.GetHash(CurrentPasswordBox.Password);
            var user = Core.Context.User.FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == hashedPass);

            if (user == null)
            {
                Core.ShowError("Текущий пароль/Логин неверный!");
                return;
            }

            if (!Core.ValidatePassword(NewPasswordBox.Password, out string errorMsg))
            {
                Core.ShowError(errorMsg);
                return;
            }

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                Core.ShowError("Новые пароли не совпадают!");
                return;
            }

            user.Password = Core.GetHash(NewPasswordBox.Password);
            Core.Context.SaveChanges();
            Core.ShowInfo("Пароль успешно изменен!");
            NavigationService?.Navigate(new AuthPage());
        }

        private void TbLogin_TextChanged(object sender, TextChangedEventArgs e) =>
            HintLogin.Visibility = TbLogin.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;

        private void CurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e) =>
            HintOldPass.Visibility = CurrentPasswordBox.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e) =>
            HintNewPass.Visibility = NewPasswordBox.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e) =>
            HintConfirmPass.Visibility = ConfirmPasswordBox.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;

        private void HintLogin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => TbLogin.Focus();
        private void HintOldPass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => CurrentPasswordBox.Focus();
        private void HintNewPass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => NewPasswordBox.Focus();
        private void HintConfirmPass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => ConfirmPasswordBox.Focus();
    }
}