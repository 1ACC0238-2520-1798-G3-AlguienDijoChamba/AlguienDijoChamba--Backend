
namespace AlguienDijoChamba.Api.Reputation.Domain;

public class ProfessionalTag
{
    // Clave foránea al Profesional (dueño de la reputación)
    public Guid ProfessionalId { get; private set; } 

    // Clave foránea al Tag (catálogo)
    public Guid TagId { get; private set; } 
    public Tag Tag { get; private set; } = default!; 

    private ProfessionalTag() { }

    public ProfessionalTag(Guid professionalId, Guid tagId)
    {
        ProfessionalId = professionalId;
        TagId = tagId;
    }
}