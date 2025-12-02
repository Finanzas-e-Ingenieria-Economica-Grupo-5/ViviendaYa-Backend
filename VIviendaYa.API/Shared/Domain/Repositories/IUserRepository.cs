using VIviendaYa.API.Shared.Domain.Model;

namespace VIviendaYa.API.Shared.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByUsernameAsync(string username);
    Task<User?> FindByUsernameOrEmailAsync(string usernameOrEmail);
    Task AddAsync(User user);
    Task<User?> FindByIdAsync(int id);
    Task SaveChangesAsync();
}