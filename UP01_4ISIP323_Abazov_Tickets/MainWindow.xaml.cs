using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;

namespace UP01_4ISIP323_Abazov_Tickets
{
    public partial class MainWindow : Window
    {
        private const string ThemeConfigFile = "theme.cfg";

        public MainWindow()
        {
            InitializeComponent();
            LoadSavedTheme();
            MainFrame.Navigate(new Pages.AuthPage());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) => { DateTimeNow.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"); };
            timer.Start();
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            BtnBack.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Hidden;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void CmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string dictPath = CmbTheme.SelectedIndex == 1 ? "CustomDictionary.xaml" : "Dictionary.xaml";
            var uri = new Uri(dictPath, UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            File.WriteAllText(ThemeConfigFile, CmbTheme.SelectedIndex.ToString());
        }

        private void LoadSavedTheme()
        {
            if (File.Exists(ThemeConfigFile) && int.TryParse(File.ReadAllText(ThemeConfigFile), out int themeIndex))
            {
                CmbTheme.SelectedIndex = (themeIndex == 1) ? 1 : 0;
            }
            else
            {
                CmbTheme.SelectedIndex = 0;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти?", "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;
            else
                e.Cancel = false;
        }
    }
}