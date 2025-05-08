using ConsoleApp1.Models;
using ConsoleApp1.Services;
using System.Windows;
using System.Windows.Controls;

namespace ConsoleApp1.Views
{
    public partial class UserWindow : Window
    {
        private readonly User _user;
        private readonly UserFileManager _userManager;
        
        public UserWindow(User user)
        {
            InitializeComponent();
            _user = user;
            _userManager = UserFileManager.Instance;
            
            // Встановлення повноекранного режиму при завантаженні вікна
            Loaded += (s, e) => App.SetFullscreenMode(this);
            
            InitializeUserData();
        }
        
        // Обробник кнопки переключення повноекранного режиму
        private void ToggleFullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            App.ToggleFullscreenMode(this);
        }
        
        private void InitializeUserData()
        {
            // Встановлюємо дані користувача у вікні
            UserTitleTextBlock.Text = $"Особистий кабінет: {_user.Login}";
            UsernameTextBlock.Text = _user.Login;
            PasswordTextBlock.Text = _user.Password; // Відображаємо реальний пароль
            
            // Відображаємо інформацію про обмеження
            bool hasRestrictions = _user.HasPasswordRestrictions;
            RestrictionsTextBlock.Text = hasRestrictions ? 
                "Увімкнено (потрібен складний пароль)" : 
                "Вимкнено (будь-який пароль)";
            
            // Показуємо вимоги до пароля, якщо вони є
            if (hasRestrictions)
            {
                PasswordRequirementsText.Visibility = Visibility.Visible;
            }
        }
        
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            App.SetFullscreenMode(loginWindow); // Встановлюємо повноекранний режим
            loginWindow.Show();
            Close();
                    }
        
        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = CurrentPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            
            PasswordChangeMessage.Foreground = System.Windows.Media.Brushes.Red;
            
            if (currentPassword != _user.Password)
            {
                PasswordChangeMessage.Text = "Невірний поточний пароль";
                PasswordChangeMessage.Visibility = Visibility.Visible;
                return;
            }
            
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                PasswordChangeMessage.Text = "Новий пароль не може бути порожнім";
                PasswordChangeMessage.Visibility = Visibility.Visible;
                return;
            }
            
            if (newPassword != confirmPassword)
            {
                PasswordChangeMessage.Text = "Паролі не співпадають";
                PasswordChangeMessage.Visibility = Visibility.Visible;
                return;
            }
            
            // Перевіряємо обмеження на пароль
            if (_user.HasPasswordRestrictions && !PasswordValidator.ValidatePassword(newPassword))
            {
                PasswordChangeMessage.Text = "Пароль не відповідає вимогам безпеки";
                PasswordChangeMessage.Visibility = Visibility.Visible;
                return;
            }
            
            if (_userManager.UpdateUserPassword(_user.Login, newPassword))
            {
                _user.Password = newPassword;
                PasswordChangeMessage.Text = "Пароль успішно змінено";
                PasswordChangeMessage.Foreground = System.Windows.Media.Brushes.Green;
                PasswordChangeMessage.Visibility = Visibility.Visible;
                
                // Очищаємо поля введення
                CurrentPasswordBox.Password = "";
                NewPasswordBox.Password = "";
                ConfirmPasswordBox.Password = "";
            }
            else
            {
                PasswordChangeMessage.Text = "Не вдалося змінити пароль";
                PasswordChangeMessage.Visibility = Visibility.Visible;
            }
        }
    }
}
