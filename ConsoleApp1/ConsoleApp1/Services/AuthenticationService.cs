using ConsoleApp1.Models;

namespace ConsoleApp1.Services;

public class AuthenticationService
{
    private readonly UserFileManager _userFileManager;
    private const int MaxLoginAttempts = 3;

    public AuthenticationService(UserFileManager userFileManager)
    {
        _userFileManager = userFileManager;
    }

    public User? AuthenticateUser()
    {
        int attempts = 0;
        
        while (attempts < MaxLoginAttempts)
        {
            Console.Write("Введіть логін: ");
            string login = Console.ReadLine() ?? "";
            
            Console.Write("Введіть пароль: ");
            string password = ReadPasswordMasked();
            
            var user = _userFileManager.GetUserByLogin(login);
            
            if (user == null)
            {
                attempts++;
                Console.WriteLine($"Користувача з логіном \"{login}\" не знайдено. Залишилось спроб: {MaxLoginAttempts - attempts}");
                
                if (attempts >= MaxLoginAttempts)
                {
                    Console.WriteLine("Перевищено кількість спроб входу. Завершення програми.");
                    return null;
                }
                
                if (!ContinueOrExit("Бажаєте повторити спробу? (т/н): "))
                {
                    return null;
                }
                
                continue;
            }
            
            if (user.IsBlocked)
            {
                Console.WriteLine("Цей користувач заблокований. Доступ заборонено.");
                return null;
            }
            
            if (user.Password == password)
            {
                Console.WriteLine($"Вітаємо, {login}!");
                return user;
            }
            
            attempts++;
            Console.WriteLine($"Невірний пароль. Залишилось спроб: {MaxLoginAttempts - attempts}");
            
            if (attempts >= MaxLoginAttempts)
            {
                Console.WriteLine("Перевищено кількість спроб входу. Завершення програми.");
                return null;
            }
            
            if (!ContinueOrExit("Бажаєте повторити спробу? (т/н): "))
            {
                return null;
            }
        }
        
        return null;
    }
    
    public static string ReadPasswordMasked()
    {
        string password = "";
        ConsoleKeyInfo key;
        
        do 
        {
            key = Console.ReadKey(true);
            
            if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);
        
        Console.WriteLine();
        return password;
    }
    
    public static bool ContinueOrExit(string message)
    {
        Console.Write(message);
        string response = Console.ReadLine()?.ToLower() ?? "";
        return response == "т" || response == "так";
    }
}
