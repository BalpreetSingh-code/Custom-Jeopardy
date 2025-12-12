namespace PIIIProject.Initial.Auth.Interface
{
    public interface IUserValidator
    {
        bool IsValidEmail(string email);
        bool IsValidUsername(string username);
        bool IsValidPassword(string password);
    }
}
