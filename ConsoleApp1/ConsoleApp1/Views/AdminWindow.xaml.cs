                using ConsoleApp1.Models;
                using ConsoleApp1.Services;
                using System;
                using System.Collections.Generic;
                using System.Windows;
                using System.Windows.Controls;
                
                namespace ConsoleApp1.Views
                {
                    public partial class AdminWindow : Window
                    {
                        private readonly User _admin;
                        private readonly UserFileManager _userManager;
                        private User? _selectedUser;
                        
                        public AdminWindow(User admin)
                        {
                            InitializeComponent();
                            _admin = admin;
                            _userManager = UserFileManager.Instance;
                            
                            // Встановлення повноекранного режиму при завантаженні вікна
                            Loaded += (s, e) => App.SetFullscreenMode(this);
                        }
                        
                        // Обробник кнопки переключення повноекранного режиму
                        private void ToggleFullscreenButton_Click(object sender, RoutedEventArgs e)
                        {
                            App.ToggleFullscreenMode(this);
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
                            HideAllPanels();
                            ChangePasswordPanel.Visibility = Visibility.Visible;
                        }
                        
                        private void ViewUsersButton_Click(object sender, RoutedEventArgs e)
                        {
                            HideAllPanels();
                            ViewUsersPanel.Visibility = Visibility.Visible;
                            
                            var users = _userManager.GetAllUsers();
                            UsersDataGrid.ItemsSource = users;
                            UserCountTextBlock.Text = $"Загальна кількість користувачів: {users.Count}";
                        }
                        
                        private void AddUserButton_Click(object sender, RoutedEventArgs e)
                        {
                            HideAllPanels();
                            AddUserPanel.Visibility = Visibility.Visible;
                            
                            // Очищаємо поля форми
                            NewUserLoginTextBox.Text = "";
                            NewUserPasswordBox.Password = "";
                            NewUserBlockedCheckBox.IsChecked = false;
                            NewUserPasswordRestrictionsCheckBox.IsChecked = false;
                            AddUserMessage.Visibility = Visibility.Collapsed;
                            
                            // Примусове оновлення стилів чекбоксів
                            NewUserBlockedCheckBox.InvalidateVisual();
                            NewUserPasswordRestrictionsCheckBox.InvalidateVisual();
                            
                            // Переконуємося, що залишаємося в повноекранному режимі
                            App.SetFullscreenMode(this);
                        }
                        
                        private void ManageUsersButton_Click(object sender, RoutedEventArgs e)
                        {
                            HideAllPanels();
                            ManageUsersPanel.Visibility = Visibility.Visible;
                            
                            // Приховуємо деталі користувача
                            UserManagementBorder.Visibility = Visibility.Collapsed;
                            NoUserFoundTextBlock.Visibility = Visibility.Collapsed;
                            ManageUserSearchTextBox.Text = "";
                            UserManagementMessage.Visibility = Visibility.Collapsed;
                        }
                        
                        private void HideAllPanels()
                        {
                            ChangePasswordPanel.Visibility = Visibility.Collapsed;
                            ViewUsersPanel.Visibility = Visibility.Collapsed;
                            AddUserPanel.Visibility = Visibility.Collapsed;
                            ManageUsersPanel.Visibility = Visibility.Collapsed;
                            DeleteUserPanel.Visibility = Visibility.Collapsed;
                            WelcomePanel.Visibility = Visibility.Collapsed;
                            
                            // Приховуємо повідомлення
                            PasswordChangeMessage.Visibility = Visibility.Collapsed;
                            AddUserMessage.Visibility = Visibility.Collapsed;
                            UserManagementMessage.Visibility = Visibility.Collapsed;
                            DeleteUserMessage.Visibility = Visibility.Collapsed;
                        }
                        
                        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
                        {
                            HideAllPanels();
                            DeleteUserPanel.Visibility = Visibility.Visible;
                            
                            // Очищаємо поля
                            DeleteUserLoginTextBox.Text = "";
                            DeleteUserMessage.Visibility = Visibility.Collapsed;
                        }
                        
                        private void DeleteUserConfirmButton_Click(object sender, RoutedEventArgs e)
                        {
                            string login = DeleteUserLoginTextBox.Text.Trim();
                            DeleteUserMessage.Foreground = System.Windows.Media.Brushes.Red;
                            
                            if (string.IsNullOrWhiteSpace(login))
                            {
                                DeleteUserMessage.Text = "Введіть логін користувача для видалення";
                                DeleteUserMessage.Visibility = Visibility.Visible;
                                return;
                            }
                            
                            if (login.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
                            {
                                DeleteUserMessage.Text = "Неможливо видалити адміністратора";
                                DeleteUserMessage.Visibility = Visibility.Visible;
                                return;
                            }
                            
                            var user = _userManager.GetUserByLogin(login);
                            if (user == null)
                            {
                                DeleteUserMessage.Text = $"Користувача з логіном '{login}' не знайдено";
                                DeleteUserMessage.Visibility = Visibility.Visible;
                                return;
                            }
                            
                            if (_userManager.DeleteUser(login))
                            {
                                DeleteUserMessage.Text = $"Користувача '{login}' успішно видалено";
                                DeleteUserMessage.Foreground = System.Windows.Media.Brushes.Green;
                                DeleteUserMessage.Visibility = Visibility.Visible;
                                DeleteUserLoginTextBox.Text = "";
                            }
                            else
                            {
                                DeleteUserMessage.Text = $"Не вдалося видалити користувача '{login}'";
                                DeleteUserMessage.Visibility = Visibility.Visible;
                            }
                        }
                        
                        private void ChangeAdminPasswordButton_Click(object sender, RoutedEventArgs e)
                        {
                            string currentPassword = CurrentPasswordBox.Password;
                            string newPassword = NewPasswordBox.Password;
                            string confirmPassword = ConfirmPasswordBox.Password;
                            
                            // Переконуємося, що залишаємося в повноекранному режимі
                            App.SetFullscreenMode(this);
                            
                            PasswordChangeMessage.Foreground = System.Windows.Media.Brushes.Red;
                            
                            if (currentPassword != _admin.Password)
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
                            
                            if (_userManager.UpdateUserPassword(_admin.Login, newPassword))
                            {
                _admin.Password = newPassword;
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
                        
                        private void AddNewUserButton_Click(object sender, RoutedEventArgs e)
                        {
                            string login = NewUserLoginTextBox.Text.Trim();
                            string password = NewUserPasswordBox.Password;
                            bool isBlocked = NewUserBlockedCheckBox.IsChecked ?? false;
                            bool hasPasswordRestrictions = NewUserPasswordRestrictionsCheckBox.IsChecked ?? false;
                            
                            AddUserMessage.Foreground = System.Windows.Media.Brushes.Red;
                            
                            if (string.IsNullOrWhiteSpace(login))
                            {
                AddUserMessage.Text = "Логін не може бути порожнім";
                AddUserMessage.Visibility = Visibility.Visible;
                return;
                            }
                            
                            if (login.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
                            {
                AddUserMessage.Text = "Неможливо створити користувача з логіном 'ADMIN'";
                AddUserMessage.Visibility = Visibility.Visible;
                return;
                            }
                            
                            if (_userManager.IsLoginExists(login))
                            {
                AddUserMessage.Text = $"Користувач з логіном '{login}' вже існує";
                AddUserMessage.Visibility = Visibility.Visible;
                return;
                            }
                            
                            if (hasPasswordRestrictions && !string.IsNullOrEmpty(password) && !PasswordValidator.ValidatePassword(password))
                            {
                AddUserMessage.Text = "Пароль не відповідає вимогам безпеки";
                AddUserMessage.Visibility = Visibility.Visible;
                return;
                            }
                            
                            User newUser = new User(login, password, isBlocked, hasPasswordRestrictions);
                            
                            if (_userManager.AddUser(newUser))
                            {
                AddUserMessage.Text = $"Користувача '{login}' успішно додано";
                AddUserMessage.Foreground = System.Windows.Media.Brushes.Green;
                AddUserMessage.Visibility = Visibility.Visible;
                
                // Очищаємо форму
                NewUserLoginTextBox.Text = "";
                NewUserPasswordBox.Password = "";
                NewUserBlockedCheckBox.IsChecked = false;
                NewUserPasswordRestrictionsCheckBox.IsChecked = false;
                            }
                            else
                            {
                AddUserMessage.Text = "Не вдалося додати користувача";
                AddUserMessage.Visibility = Visibility.Visible;
                            }
                        }
                        
                        private void FindUserButton_Click(object sender, RoutedEventArgs e)
                        {
                            string login = ManageUserSearchTextBox.Text.Trim();
                            
                            UserManagementMessage.Visibility = Visibility.Collapsed;
                            
                            if (string.IsNullOrWhiteSpace(login))
                            {
                UserManagementBorder.Visibility = Visibility.Collapsed;
                NoUserFoundTextBlock.Visibility = Visibility.Collapsed;
                return;
                            }
                            
                            if (login.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
                            {
                MessageBox.Show("Не можна керувати обліковим записом адміністратора через цей інтерфейс.", 
                    "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
                            }
                            
                            _selectedUser = _userManager.GetUserByLogin(login);
                            
                            if (_selectedUser != null)
                            {
                // Показуємо панель керування користувачем
                UserManagementBorder.Visibility = Visibility.Visible;
                NoUserFoundTextBlock.Visibility = Visibility.Collapsed;
                
                // Заповнюємо інформацію про користувача
                SelectedUserNameTextBlock.Text = $"Користувач: {_selectedUser.Login}";
                
                // Стан блокування
                bool isBlocked = _selectedUser.IsBlocked;
                UserBlockedStatusTextBlock.Text = isBlocked ? "Так" : "Ні";
                ToggleBlockButton.Content = isBlocked ? "Розблокувати" : "Заблокувати";
                
                // Стан обмежень на пароль
                bool hasRestrictions = _selectedUser.HasPasswordRestrictions;
                PasswordRestrictionsStatusTextBlock.Text = hasRestrictions ? "Так" : "Ні";
                ToggleRestrictionsButton.Content = hasRestrictions ? "Вимкнути" : "Увімкнути";
                            }
                            else
                            {
                // Користувача не знайдено
                UserManagementBorder.Visibility = Visibility.Collapsed;
                NoUserFoundTextBlock.Visibility = Visibility.Visible;
                            }
                        }
                        
                        private void ToggleBlockButton_Click(object sender, RoutedEventArgs e)
                        {
                            if (_selectedUser == null) return;
                            
                            // Змінюємо стан блокування на протилежний
                            bool newBlockedState = !_selectedUser.IsBlocked;
                            
                            if (_userManager.ToggleUserBlocked(_selectedUser.Login, newBlockedState))
                            {
                _selectedUser.IsBlocked = newBlockedState;
                
                // Оновлюємо UI
                UserBlockedStatusTextBlock.Text = newBlockedState ? "Так" : "Ні";
                ToggleBlockButton.Content = newBlockedState ? "Розблокувати" : "Заблокувати";
                
                // Показуємо повідомлення про успіх
                UserManagementMessage.Text = $"Стан блокування користувача успішно змінено";
                UserManagementMessage.Foreground = System.Windows.Media.Brushes.Green;
                UserManagementMessage.Visibility = Visibility.Visible;
                            }
                            else
                            {
                UserManagementMessage.Text = "Не вдалося змінити стан блокування";
                UserManagementMessage.Foreground = System.Windows.Media.Brushes.Red;
                UserManagementMessage.Visibility = Visibility.Visible;
                            }
                        }
                        
                        private void ToggleRestrictionsButton_Click(object sender, RoutedEventArgs e)
                        {
                            if (_selectedUser == null) return;
                            
                            // Змінюємо стан обмежень на пароль на протилежний
                            bool newRestrictionsState = !_selectedUser.HasPasswordRestrictions;
                            
                            if (_userManager.TogglePasswordRestrictions(_selectedUser.Login, newRestrictionsState))
                            {
                _selectedUser.HasPasswordRestrictions = newRestrictionsState;
                
                // Оновлюємо UI
                PasswordRestrictionsStatusTextBlock.Text = newRestrictionsState ? "Так" : "Ні";
                ToggleRestrictionsButton.Content = newRestrictionsState ? "Вимкнути" : "Увімкнути";
                
                // Показуємо повідомлення про успіх
                UserManagementMessage.Text = $"Стан обмежень на пароль успішно змінено";
                UserManagementMessage.Foreground = System.Windows.Media.Brushes.Green;
                UserManagementMessage.Visibility = Visibility.Visible;
            }
            else
            {
                UserManagementMessage.Text = "Не вдалося змінити стан обмежень на пароль";
                UserManagementMessage.Foreground = System.Windows.Media.Brushes.Red;
                UserManagementMessage.Visibility = Visibility.Visible;
            }
        }
    }
}
