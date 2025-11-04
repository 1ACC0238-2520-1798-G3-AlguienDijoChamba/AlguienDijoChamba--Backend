using AlguienDijoChamba.Api.IAM.Domain;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;
using Microsoft.EntityFrameworkCore;

namespace AlguienDijoChamba.Api.IAM.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    // Métodos existentes
    public void Add(User user) => context.Users.Add(user);
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    
    // --- IMPLEMENTACIONES REQUERIDAS ---
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        
    // 2. Marcar entidad para eliminación
    public void Remove(User user) => context.Users.Remove(user);
}