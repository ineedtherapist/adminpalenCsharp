using ConsoleApp1.Models;
using ConsoleApp1.Services;
using System;
using System.Windows;

namespace ConsoleApp1.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UserFileManager _userManager;
        private int _loginAttempts = 0;
        private const int MaxLoginAttempts = 3;
        
        public LoginWindow()
        {
            InitializeComponent();
            _userManager = UserFileManager.Instance;
            Loaded += (s, e) => App.SetFullscreenMode(this);
            UsernameTextBox.Focus();
        }

        private void ToggleFullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            App.ToggleFullscreenMode(this);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Будь ласка, введіть логін та пароль");
                return;
            }

            User? user = _userManager.GetUserByLogin(username);
            
            if (user == null)
            {
                _loginAttempts++;
                ShowError($"Користувача з логіном \"{username}\" не знайдено. Залишилось спроб: {MaxLoginAttempts - _loginAttempts}");
                
                if (_loginAttempts >= MaxLoginAttempts)
                {
                    MessageBox.Show("Перевищено кількість спроб входу. Завершення програми.", 
                        "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
                
                return;
            }
            
            if (user.IsBlocked)
            {
                ShowError("Цей користувач заблокований. Доступ заборонено.");
                return;
            }
            
            if (user.Password != password)
            {
                _loginAttempts++;
                ShowError($"Невірний пароль. Залишилось спроб: {MaxLoginAttempts - _loginAttempts}");
                
                if (_loginAttempts >= MaxLoginAttempts)
                {
                    MessageBox.Show("Перевищено кількість спроб входу. Завершення програми.", 
                        "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
                
                return;
            }
            
            if (user.Login.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                AdminWindow adminWindow = new AdminWindow(user);
                App.SetFullscreenMode(adminWindow);
                adminWindow.Show();
                Close();
            }
            else
            {
                UserWindow userWindow = new UserWindow(user);
                App.SetFullscreenMode(userWindow);
                userWindow.Show();
                Close();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            App.SetFullscreenMode(registrationWindow);
            
            if (registrationWindow.ShowDialog() == true)
            {
                UsernameTextBox.Text = registrationWindow.NewUsername;
                PasswordBox.Password = "";
                PasswordBox.Focus();
                HideError();
            }
            
            App.SetFullscreenMode(this);
        }
        
        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
        
        private void HideError()
        {
            ErrorMessage.Visibility = Visibility.Collapsed;
        }
        
        private void ResetDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Ви впевнені, що хочете скинути всі дані користувачів? Ця дія відновить тільки адміністратора з паролем 'admin'.", 
                               "Підтвердження скидання", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                bool result = _userManager.ResetAllUsersData();
                if (result)
                {
                    MessageBox.Show("Базу даних користувачів успішно скинуто. Тепер ви можете увійти як ADMIN з паролем admin.", 
                                   "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    UsernameTextBox.Text = "ADMIN";
                    PasswordBox.Password = "admin";
                    HideError();
                }
                else
                {
                    ShowError("Не вдалося скинути базу даних користувачів.");
                }
            }
        }
    }
}
