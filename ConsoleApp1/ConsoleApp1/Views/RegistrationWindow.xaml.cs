using ConsoleApp1.Models;
using ConsoleApp1.Services;
using System.Windows;
using System.Windows.Controls;

namespace ConsoleApp1.Views
{
    public partial class RegistrationWindow : Window
    {
        private readonly UserFileManager _userManager;
        private bool _hasPasswordRestrictions = false;
        
        public string NewUsername { get; private set; } = "";
        
        public RegistrationWindow()
        {
            InitializeComponent();
            _userManager = UserFileManager.Instance;
            
            // Встановлення повноекранного режиму при завантаженні вікна
            Loaded += (s, e) => App.SetFullscreenMode(this);
            
            UsernameTextBox.Focus();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username))
            {
                ShowError("Будь ласка, введіть логін");
                return;
            }
            
            if (username.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                ShowError("Логін 'ADMIN' зарезервований. Оберіть інший логін.");
                return;
            }

            if (_userManager.IsLoginExists(username))
            {
                ShowError($"Користувач з логіном '{username}' вже існує");
                return;
            }

            if (_hasPasswordRestrictions && !string.IsNullOrEmpty(password))
            {
                if (!PasswordValidator.ValidatePassword(password))
                {
                    ShowError("Пароль не відповідає вимогам безпеки");
                    return;
                }
            }

            // Створення нового користувача
            User newUser = new User(
                username,
                password,
                false,  // не заблокований
                _hasPasswordRestrictions
            );

            if (_userManager.AddUser(newUser))
            {
                NewUsername = username;
                DialogResult = true;
                Close();
            }
            else
            {
                ShowError("Не вдалося створити нового користувача");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        
        private void PasswordRestrictionsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _hasPasswordRestrictions = true;
            PasswordRequirementsText.Visibility = Visibility.Visible;
            // Переконуємося, що стиль чекбоксу відображає правильний стан
            UpdateCheckboxStyling();
        }
        
        private void PasswordRestrictionsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _hasPasswordRestrictions = false;
            PasswordRequirementsText.Visibility = Visibility.Collapsed;
            // Переконуємося, що стиль чекбоксу відображає правильний стан
            UpdateCheckboxStyling();
        }
        
        private void UpdateCheckboxStyling()
        {
            // Забезпечує правильне відображення стану чекбоксу
            if (PasswordRestrictionsCheckBox.IsChecked == true)
            {
                PasswordRestrictionsCheckBox.InvalidateVisual();
            }
        }
        
        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}
