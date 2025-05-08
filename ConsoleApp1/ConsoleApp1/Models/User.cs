namespace ConsoleApp1.Models;

public class User
{
    public string Login { get; set; }
    public string Password { get; set; }
    public bool IsBlocked { get; set; }
    public bool HasPasswordRestrictions { get; set; }

    public User(string login, string password, bool isBlocked = false, bool hasPasswordRestrictions = false)
    {
        Login = login;
        Password = password;
        IsBlocked = isBlocked;
        HasPasswordRestrictions = hasPasswordRestrictions;
    }

    public override string ToString()
    {
        return $"Логін: {Login}, " +
               $"Пароль: {Password}, " +
               $"Заблоковано: {(IsBlocked ? "Так" : "Ні")}, " +
               $"Обмеження на пароль: {(HasPasswordRestrictions ? "Так" : "Ні")}";
    }
}
