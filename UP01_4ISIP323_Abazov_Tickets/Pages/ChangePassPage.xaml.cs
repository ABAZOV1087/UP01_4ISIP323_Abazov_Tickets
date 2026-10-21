using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
                Core.ShowError("Текущий пароль или Логин неверный!");
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
    }
}