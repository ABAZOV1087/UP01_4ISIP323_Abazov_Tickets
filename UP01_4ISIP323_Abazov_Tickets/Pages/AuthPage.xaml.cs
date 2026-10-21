using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_4ISIP323_Abazov_Tickets.Pages
{
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;

        public AuthPage()
        {
            InitializeComponent();
        }

        private void ButtonEnter_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                Core.ShowError("Введите логин и пароль!");
                return;
            }

            string hashedPassword = Core.GetHash(PasswordBox.Password);
            var user = Core.Context.User.AsNoTracking()
                .FirstOrDefault(u => u.Login == TextBoxLogin.Text && u.Password == hashedPassword);

            if (user == null)
            {
                Core.ShowError("Пользователь с такими данными не найден!");
                failedAttempts++;
                if (failedAttempts >= 3)
                {
                    if (CaptchaPanel.Visibility != Visibility.Visible)
                        CaptchaSwitch();
                    CaptchaChange();
                }
                return;
            }

            Core.ShowInfo("Пользователь успешно найден!");
            switch (user.Role)
            {
                case "User":
                    NavigationService?.Navigate(new UserPage());
                    break;
                case "Admin":
                    NavigationService?.Navigate(new AdminPage());
                    break;
            }
        }

        public void CaptchaSwitch()
        {
            if (CaptchaPanel.Visibility == Visibility.Visible)
            {
                TextBoxLogin.Clear();
                PasswordBox.Clear();
                CaptchaPanel.Visibility = Visibility.Hidden;
                AuthPanel.Visibility = Visibility.Visible;
            }
            else
            {
                captchaInput.Clear();
                CaptchaPanel.Visibility = Visibility.Visible;
                AuthPanel.Visibility = Visibility.Hidden;
            }
        }

        public void CaptchaChange()
        {
            captcha.Text = Core.GenerateCaptcha();
        }

        private void submitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (captchaInput.Text != captcha.Text)
            {
                Core.ShowError("Неверно введена капча");
                CaptchaChange();
            }
            else
            {
                Core.ShowInfo("Капча введена успешно, можете продолжить авторизацию");
                CaptchaSwitch();
                failedAttempts = 0;
            }
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }

        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtHintLogin.Visibility = TextBoxLogin.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtHintPass.Visibility = PasswordBox.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private void txtHintLogin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBoxLogin.Focus();
        }

        private void txtHintPass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PasswordBox.Focus();
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegPage());
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ChangePassPage());
        }
    }
}