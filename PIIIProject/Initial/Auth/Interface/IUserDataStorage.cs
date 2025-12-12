using PIIIProject.Initial.Models;

namespace PIIIProject.Initial.Auth.Interface
{
    public interface IUserDataStorage
    {
        IEnumerable<User> LoadUsersAsync();
        User GetUserByEmail(string email);
        bool SaveUserAsync(User user);
    }
}