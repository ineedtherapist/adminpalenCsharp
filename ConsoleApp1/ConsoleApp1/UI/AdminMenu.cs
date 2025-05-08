using ConsoleApp1.Models;
using ConsoleApp1.Services;

namespace ConsoleApp1.UI;

public class AdminMenu
{
    private readonly User _admin;
    private readonly UserFileManager _userFileManager;
    
    public AdminMenu(User admin, UserFileManager userFileManager)
    {
        _admin = admin;
        _userFileManager = userFileManager;
    }
    
    public void ShowMenu()
    {
        bool exit = false;
        
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== Меню адміністратора ===");
            Console.WriteLine("1. Змінити власний пароль");
            Console.WriteLine("2. Переглянути список користувачів");
            Console.WriteLine("3. Додати нового користувача");
            Console.WriteLine("4. Заблокувати/розблокувати користувача");
            Console.WriteLine("5. Увімкнути/вимкнути обмеження на пароль для користувача");
            Console.WriteLine("6. Вихід");
            Console.Write("Оберіть опцію: ");
            
            string choice = Console.ReadLine() ?? "";
            
            switch (choice)
            {
                case "1":
                    ChangePassword();
                    break;
                case "2":
                    ViewUsersList();
                    break;
                case "3":
                    AddNewUser();
                    break;
                case "4":
                    ToggleUserBlocked();
                    break;
                case "5":
                    TogglePasswordRestrictions();
                    break;
                case "6":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Невідома опція. Натисніть будь-яку клавішу для продовження...");
                    Console.ReadKey();
                    break;
            }
        }
    }
    
    private void ChangePassword()
    {
        Console.Clear();
        Console.WriteLine("=== Зміна пароля адміністратора ===");
        
        Console.Write("Введіть поточний пароль: ");
        string currentPassword = AuthenticationService.ReadPasswordMasked();
        
        if (currentPassword != _admin.Password)
        {
            Console.WriteLine("Невірний поточний пароль. Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
            return;
        }
        
        Console.Write("Введіть новий пароль: ");
        string newPassword = AuthenticationService.ReadPasswordMasked();
        
        Console.Write("Підтвердіть новий пароль: ");
        string confirmPassword = AuthenticationService.ReadPasswordMasked();
        
        if (newPassword != confirmPassword)
        {
            Console.WriteLine("Паролі не співпадають. Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
            return;
        }
        
        _userFileManager.UpdateUserPassword(_admin.Login, newPassword);
        _admin.Password = newPassword;
        
        Console.WriteLine("Пароль успішно змінено. Натисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }
    
    private void ViewUsersList()
    {
        var users = _userFileManager.GetAllUsers();
        
        if (users.Count == 0)
        {
            Console.WriteLine("Список користувачів порожній. Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
            return;
        }
        
        Console.Clear();
        Console.WriteLine("=== Список користувачів ===");
        Console.WriteLine("Оберіть опцію перегляду:");
        Console.WriteLine("1. Показати всіх користувачів");
        Console.WriteLine("2. Переглядати по одному");
        Console.Write("Ваш вибір: ");
        
        string viewOption = Console.ReadLine() ?? "";
        
        switch (viewOption)
        {
            case "1":
                ShowAllUsers(users);
                break;
            case "2":
                ShowUsersOneByOne(users);
                break;
            default:
                Console.WriteLine("Невідома опція. Натисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
                break;
        }
    }
    
    private void ShowAllUsers(List<User> users)
    {
        Console.Clear();
        Console.WriteLine("=== Усі користувачі ===");
        
        foreach (var user in users)
        {
            Console.WriteLine(user);
        }
        
        Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }
    
    private void ShowUsersOneByOne(List<User> users)
    {
        int currentIndex = 0;
        
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== Користувач {currentIndex + 1}/{users.Count} ===");
            Console.WriteLine(users[currentIndex]);
            Console.WriteLine("\nНатисніть -> для наступного, <- для попереднього, Esc для виходу");
            
            var key = Console.ReadKey();
            
            if (key.Key == ConsoleKey.RightArrow)
            {
                currentIndex = (currentIndex + 1) % users.Count;
            }
            else if (key.Key == ConsoleKey.LeftArrow)
            {
                currentIndex = currentIndex == 0 ? users.Count - 1 : currentIndex - 1;
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }
    
    private void AddNewUser()
    {
        Console.Clear();
        Console.WriteLine("=== Додавання нового користувача ===");
        
        Console.Write("Введіть логін нового користувача: ");
        string login = Console.ReadLine() ?? "";
        
        if (string.IsNullOrWhiteSpace(login))
        {
            Console.WriteLine("Логін не може бути порожнім. Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
            return;
        }
        
        if (_userFileManager.IsLoginExists(login))
        {
            Console.WriteLine($"Користувач з логіном '{login}' вже існує. Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
            return;
        }
        
        var newUser = new User(login, "", false, false);
        _userFileManager.AddUser(newUser);
        
        Console.WriteLine($"Користувача '{login}' успішно додано з порожнім паролем. Натисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }
    
    private void ToggleUserBlocked()
    {
        Console.Clear();
        Console.WriteLine("=== Блокування/розблокування користувача ===");
        
        Console.Write("Введіть логін користувача: ");
        string login = Console.ReadLine() ?? "";
        
        var user = _userFileManager.GetUserByLogin(login);
        
        if (user == null)
        {
            Console.WriteLine($"Користувача з логіном '{login}' не знайдено. Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
            return;
        }
        
        if (user.Login == "ADMIN")
        {
            Console.WriteLine("Неможливо заблокувати адміністратора. Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
            return;
        }
        
        string action = user.IsBlocked ? "розблокувати" : "заблокувати";
        Console.Write($"Ви впевнені, що хочете {action} користувача '{login}'? (т/н): ");
        string confirm = Console.ReadLine()?.ToLower() ?? "";
        
        if (confirm == "т" || confirm == "так")
        {
            _userFileManager.ToggleUserBlocked(login, !user.IsBlocked);
            string status = !user.IsBlocked ? "заблоковано" : "розблоковано";
            Console.WriteLine($"Користувача '{login}' успішно {status}. Натисніть будь-яку клавішу для продовження...");
        }
        else
        {
            Console.WriteLine("Операцію скасовано. Натисніть будь-яку клавішу для продовження...");
        }
        
        Console.ReadKey();
    }
    
    private void TogglePasswordRestrictions()
    {
        Console.Clear();
        Console.WriteLine("=== Керування обмеженнями на пароль ===");
        
        Console.Write("Введіть логін користувача: ");
        string login = Console.ReadLine() ?? "";
        
        var user = _userFileManager.GetUserByLogin(login);
        
        if (user == null)
        {
            Console.WriteLine($"Користувача з логіном '{login}' не знайдено. Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
            return;
        }
        
        string action = user.HasPasswordRestrictions ? "вимкнути" : "увімкнути";
        Console.Write($"Ви впевнені, що хочете {action} обмеження на пароль для користувача '{login}'? (т/н): ");
        string confirm = Console.ReadLine()?.ToLower() ?? "";
        
        if (confirm == "т" || confirm == "так")
        {
            _userFileManager.TogglePasswordRestrictions(login, !user.HasPasswordRestrictions);
            string status = !user.HasPasswordRestrictions ? "увімкнено" : "вимкнено";
            Console.WriteLine($"Обмеження на пароль для користувача '{login}' успішно {status}. Натисніть будь-яку клавішу для продовження...");
        }
        else
        {
            Console.WriteLine("Операцію скасовано. Натисніть будь-яку клавішу для продовження...");
        }
        
        Console.ReadKey();
    }
}
