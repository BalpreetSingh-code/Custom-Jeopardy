using PIIIProject.Initial.Models;

namespace PIIIProject.Initial.Auth.Interface
{
    public interface IRegistrationService
    {
        bool RegisterUser(User user);
        bool IsDuplicateUser(string username, string email);
        string ValidateUser(User user); 
    }
}
