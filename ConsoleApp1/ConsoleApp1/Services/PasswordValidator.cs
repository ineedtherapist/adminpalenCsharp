namespace ConsoleApp1.Services;

public class PasswordValidator
{
    public static bool ValidatePassword(string password)
    {
        // Перевірка на наявність великих літер
        bool hasUpperCase = password.Any(char.IsUpper);
        
        // Перевірка на наявність малих літер
        bool hasLowerCase = password.Any(char.IsLower);
        
        // Перевірка на наявність арифметичного символу (+, -, *, /, %, =)
        bool hasArithmeticSymbol = password.Any(c => "+-*/%=".Contains(c));

        return hasUpperCase && hasLowerCase && hasArithmeticSymbol;
    }
}
