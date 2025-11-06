// En: AlguienDijoChamba.Api.Reputation.Infrastructure.Repositories/ReputationRepository.cs

using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;
using Microsoft.EntityFrameworkCore;

// 1. ✅ Usamos SOLO el Primary Constructor (el parámetro 'context' se vuelve un campo de solo lectura)
namespace AlguienDijoChamba.Api.Reputation.Infrastructure.Repositories;

public class ReputationRepository(AppDbContext context) : IReputationRepository
{
    // ⚠️ ELIMINADA la declaración: private readonly AppDbContext _dbContext;
    // ⚠️ ELIMINADO el constructor: public ReputationRepository(AppDbContext dbContext) { ... }
    
    // Métodos existentes (usan 'context' que es el AppDbContext inyectado):
    
    public async Task<UserReputationTechnician?> GetByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken = default)
        => await context.Set<UserReputationTechnician>()
            .FirstOrDefaultAsync(r => r.ProfessionalId == professionalId, cancellationToken);

    public async Task<IEnumerable<UserReputationTechnician>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Set<UserReputationTechnician>().ToListAsync(cancellationToken);

    public void Add(UserReputationTechnician reputation) => context.Set<UserReputationTechnician>().Add(reputation);

    public void Update(UserReputationTechnician reputation) => context.Set<UserReputationTechnician>().Update(reputation);
    
    // 2. ✅ Implementación de Tags (usa 'context' directamente)
    public async Task<IEnumerable<Tag>> GetAllTagsAsync(CancellationToken cancellationToken)
    {
        // 🚀 CORRECCIÓN: Asumimos que el DbSet se llama 'Tags' en AppDbContext.
        // Si AppDbContext no tiene DbSet<Tag>, ¡ese es el error subyacente!
        return await context.Set<Tag>() 
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Tag?> GetTagByIdAsync(Guid tagId, CancellationToken cancellationToken)
    {
        return await context.Set<Tag>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tagId, cancellationToken);
    }

    public async Task<IEnumerable<Tag>> GetTagsByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken)
    {
        return await context.Set<ProfessionalTag>()
            .Where(pt => pt.ProfessionalId == professionalId)
            .Select(pt => pt.Tag) // ⬅️ Proyecta solo la entidad Tag a través de la relación
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    // 3. ✅ Implementación de funciones de la interfaz (ProfessionalTag)
    
    public void AddRange(IEnumerable<ProfessionalTag> professionalTags)
    {
        // Añade una colección de entidades de unión a la tabla ProfessionalTag
        context.Set<ProfessionalTag>().AddRange(professionalTags);
    }

    public void RemoveAllTagsByProfessionalId(Guid professionalId)
    {
        // Obtiene todos los ProfessionalTag de ese profesional
        var tagsToRemove = context.Set<ProfessionalTag>()
            .Where(pt => pt.ProfessionalId == professionalId);
            
        // Elimina las entidades encontradas
        context.Set<ProfessionalTag>().RemoveRange(tagsToRemove);
    }
    public async Task<Tag?> GetTagByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.Tags // Asumo que tienes un DbSet llamado 'Tags' en tu AppDbContext
            .FirstOrDefaultAsync(t => t.Name == name.ToUpper(), cancellationToken);
    }

    public void AddTag(Tag tag)
    {
        context.Tags.Add(tag);
    }
    
    public async Task<IEnumerable<Guid>> GetProfessionalsByTagIdAsync(
        Guid tagId, 
        CancellationToken cancellationToken)
    {
        // 🚀 CORRECCIÓN: Usamos 'context' en lugar de '_context'
        var professionalIds = await context.ProfessionalTags
            // 1. Filtra por el TagId que se busca
            .Where(pt => pt.TagId == tagId)
            // 2. Proyecta para seleccionar solo el ProfessionalId
            .Select(pt => pt.ProfessionalId)
            // 3. Ejecuta la consulta y materializa la lista
            .ToListAsync(cancellationToken);

        return professionalIds;
    }

}