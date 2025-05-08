using ConsoleApp1.Models;
using ConsoleApp1.Services;
using System.Windows;
using System.Windows.Controls;

namespace ConsoleApp1.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly UserFileManager _userManager;
        
        public RegisterWindow()
        {
            InitializeComponent();
            _userManager = UserFileManager.Instance;
            
            // Додавання обробника подій для чекбоксу обмежень паролю
            PasswordRestrictionsCheckBox.Checked += PasswordRestrictionsCheckBox_CheckedChanged;
            PasswordRestrictionsCheckBox.Unchecked += PasswordRestrictionsCheckBox_CheckedChanged;
        }
        
        private void PasswordRestrictionsCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            // Показуємо або приховуємо інформацію про вимоги до пароля
            PasswordRequirementsText.Visibility = PasswordRestrictionsCheckBox.IsChecked == true ? 
                Visibility.Visible : Visibility.Collapsed;
                
            // Переконуємося, що стиль чекбоксу відображає правильний стан
            PasswordRestrictionsCheckBox.InvalidateVisual();
        }
        
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            bool hasPasswordRestrictions = PasswordRestrictionsCheckBox.IsChecked ?? false;
            
            // Очищаємо попередні повідомлення про помилки
            ErrorMessage.Visibility = Visibility.Collapsed;
            
            // Валідація введених даних
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Логін не може бути порожнім");
                return;
            }
            
            if (username.Equals("ADMIN", System.StringComparison.OrdinalIgnoreCase))
            {
                ShowError("Неможливо зареєструвати користувача з логіном 'ADMIN'");
                return;
            }
            
            if (_userManager.IsLoginExists(username))
            {
                ShowError($"Користувач з логіном '{username}' вже існує");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Пароль не може бути порожнім");
                return;
            }
            
            if (password != confirmPassword)
            {
                ShowError("Паролі не співпадають");
                return;
            }
            
            // Перевірка відповідності вимогам до пароля
            if (hasPasswordRestrictions && !PasswordValidator.ValidatePassword(password))
            {
                ShowError("Пароль не відповідає встановленим вимогам безпеки");
                return;
            }
            
            // Створення нового користувача
            User newUser = new User(username, password, false, hasPasswordRestrictions);
            
            if (_userManager.AddUser(newUser))
            {
                MessageBox.Show("Реєстрація успішна! Тепер ви можете увійти в систему.", 
                    "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Повертаємося до вікна входу
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                Close();
            }
            else
            {
                ShowError("Не вдалося зареєструвати користувача. Спробуйте пізніше.");
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Повертаємося до вікна входу
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
        
        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}
