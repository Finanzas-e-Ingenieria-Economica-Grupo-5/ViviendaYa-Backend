using Microsoft.EntityFrameworkCore;
using VIviendaYa.API.Shared.Domain.Model;
using VIviendaYa.API.Shared.Domain.Repositories;
using VIviendaYa.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace VIviendaYa.API.Shared.Infrastructure.Persistence.EFC.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> FindByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> FindByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await _context.Users.FirstOrDefaultAsync(u => 
            u.Username == usernameOrEmail || u.Email == usernameOrEmail);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<User?> FindByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}