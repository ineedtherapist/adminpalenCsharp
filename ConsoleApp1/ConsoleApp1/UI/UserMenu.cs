using ConsoleApp1.Models;
using ConsoleApp1.Services;

namespace ConsoleApp1.UI;

public class UserMenu
{
    private readonly User _user;
    private readonly UserFileManager _userFileManager;
    
    public UserMenu(User user, UserFileManager userFileManager)
    {
        _user = user;
        _userFileManager = userFileManager;
    }
    
    public void ShowMenu()
    {
        bool exit = false;
        
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== Меню користувача ===");
            Console.WriteLine("1. Змінити власний пароль");
            Console.WriteLine("2. Вихід");
            Console.Write("Оберіть опцію: ");
            
            string choice = Console.ReadLine() ?? "";
            
            switch (choice)
            {
                case "1":
                    ChangePassword();
                    break;
                case "2":
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
        Console.WriteLine("=== Зміна пароля ===");
        
        Console.Write("Введіть поточний пароль: ");
        string currentPassword = AuthenticationService.ReadPasswordMasked();
        
        if (currentPassword != _user.Password)
        {
            Console.WriteLine("Невірний поточний пароль. Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
            return;
        }
        
        bool isPasswordValid = false;
        string newPassword = "";
        
        while (!isPasswordValid)
        {
            Console.Write("Введіть новий пароль: ");
            newPassword = AuthenticationService.ReadPasswordMasked();
            
            Console.Write("Підтвердіть новий пароль: ");
            string confirmPassword = AuthenticationService.ReadPasswordMasked();
            
            if (newPassword != confirmPassword)
            {
                Console.WriteLine("Паролі не співпадають.");
                
                if (!AuthenticationService.ContinueOrExit("Бажаєте повторити спробу? (т/н): "))
                {
                    return;
                }
                
                continue;
            }
            
            if (_user.HasPasswordRestrictions && !PasswordValidator.ValidatePassword(newPassword))
            {
                Console.WriteLine("Пароль не відповідає вимогам безпеки:");
                Console.WriteLine("- Повинен містити великі літери");
                Console.WriteLine("- Повинен містити малі літери");
                Console.WriteLine("- Повинен містити хоча б один арифметичний символ (+, -, *, /, %, =)");
                
                Console.WriteLine("\nОберіть опцію:");
                Console.WriteLine("1. Спробувати ще раз");
                Console.WriteLine("2. Відмовитись від зміни пароля");
                Console.WriteLine("3. Вийти з програми");
                Console.Write("Ваш вибір: ");
                
                string choice = Console.ReadLine() ?? "";
                
                switch (choice)
                {
                    case "1":
                        continue;
                    case "2":
                        return;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Невідома опція. Повторюємо спробу зміни пароля...");
                        continue;
                }
            }
            
            isPasswordValid = true;
        }
        
        _userFileManager.UpdateUserPassword(_user.Login, newPassword);
        _user.Password = newPassword;
        
        Console.WriteLine("Пароль успішно змінено. Натисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }
}
