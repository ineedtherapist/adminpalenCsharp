using ConsoleApp1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace ConsoleApp1.Services;

public class UserFileManager
{
    private readonly string _filePath = "users.txt";
    private List<User> _users = new List<User>();
    
    public static UserFileManager Instance { get; } = new UserFileManager();
    
    public UserFileManager()
    {
        InitializeUsersFile();
    }

    private void InitializeUsersFile()
    {
        try
        {
            if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
            {
                // Створюємо файл з адміністратором за замовчуванням
                _users.Clear();
                _users.Add(new User("ADMIN", "admin", false, false));
                SaveUsers();
            }
            else
            {
                LoadUsers();
                
                // Перевіряємо, чи є ADMIN з правильним паролем
                var admin = GetUserByLogin("ADMIN");
                if (admin == null)
                {
                    _users.Add(new User("ADMIN", "admin", false, false));
                    SaveUsers();
                }
                else if (admin.Password != "admin")
                {
                    admin.Password = "admin";
                    SaveUsers();
                }
            }
        }
        catch (Exception ex)
        {
            // У WPF-застосунку повідомлення про помилку
            MessageBox.Show($"Помилка при ініціалізації файлу користувачів: {ex.Message}", 
                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void LoadUsers()
    {
        try
        {
            _users.Clear();
            
            if (File.Exists(_filePath))
            {
                var lines = File.ReadAllLines(_filePath);
                
                foreach (var line in lines)
                {
                    // Підтримка обох типів роздільників для сумісності
                    var parts = line.Contains('|') ? line.Split('|') : line.Split(',');
                    if (parts.Length == 4)
                    {
                        _users.Add(new User(
                            parts[0],
                            parts[1],
                            bool.Parse(parts[2]),
                            bool.Parse(parts[3])
                        ));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка при завантаженні користувачів: {ex.Message}", 
                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    public bool SaveUsers()
    {
        try
        {
            // Використовуємо кому як роздільник для консистентності
            var lines = _users.Select(u => $"{u.Login},{u.Password},{u.IsBlocked},{u.HasPasswordRestrictions}");
            File.WriteAllLines(_filePath, lines);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка при збереженні користувачів: {ex.Message}", 
                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public List<User> GetAllUsers()
    {
        return _users.ToList();
    }

    public User? GetUserByLogin(string login)
    {
        return _users.FirstOrDefault(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
    }
    
    // Метод для повного скидання бази користувачів
    // Метод для відображення поточного стану бази даних
    public void ShowCurrentUsersState()
    {
        // Тепер цей метод не показує повідомлення
        // використовується для внутрішньої діагностики
    }
    
    public bool ResetAllUsersData()
    {
        try
        {
            // Видаляємо файл, якщо він існує
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
            
            // Створюємо нову базу даних
            _users.Clear();
            _users.Add(new User("ADMIN", "admin", false, false));
            
            return SaveUsers();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка при скиданні даних користувачів: {ex.Message}", 
                           "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public bool IsLoginExists(string login)
    {
        return _users.Any(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
    }

    public bool AddUser(User user)
    {
        if (!IsLoginExists(user.Login))
        {
            _users.Add(user);
            SaveUsers();
            return true;
        }
        return false;
    }

    public bool UpdateUserPassword(string login, string newPassword)
    {
        var user = GetUserByLogin(login);
        if (user != null)
        {
            user.Password = newPassword;
            SaveUsers();
            return true;
        }
        return false;
    }

    public bool ToggleUserBlocked(string login, bool isBlocked)
    {
        var user = GetUserByLogin(login);
        if (user != null)
        {
            user.IsBlocked = isBlocked;
            SaveUsers();
            return true;
        }
        return false;
    }

    public bool TogglePasswordRestrictions(string login, bool hasRestrictions)
    {
        var user = GetUserByLogin(login);
        if (user != null)
        {
            user.HasPasswordRestrictions = hasRestrictions;
            SaveUsers();
            return true;
        }
        return false;
    }
    
    public bool IsUserBlocked(string login)
    {
        var user = GetUserByLogin(login);
        return user?.IsBlocked ?? false;
    }
    
    public bool HasUserPasswordRestrictions(string login)
    {
        var user = GetUserByLogin(login);
        return user?.HasPasswordRestrictions ?? false;
    }
    
    public bool DeleteUser(string login)
    {
        // Не дозволяємо видаляти адміністратора
        if (login.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        
        var user = GetUserByLogin(login);
        if (user != null)
        {
            _users.Remove(user);
            return SaveUsers();
        }
        return false;
    }
}
