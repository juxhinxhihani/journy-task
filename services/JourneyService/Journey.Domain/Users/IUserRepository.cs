namespace Journey.Domain.Users;

public interface IUserRepository
{
    void Add(User user);
    void Update(User user);
    Task<User> Get(Guid id);
    
    Task<bool> IsEmailUniqueAsync(string email);
    Task<User?> GetUserByEmailAsync(string email);
    Task RemoveUser(User user);
}