using PIIIProject.Initial.Models;

namespace PIIIProject.Initial.Auth.Interface
{
    public interface IAuthenticationService
    {
        User VerifyLogin(string email, string password); // Return User instead of bool
    }
}
