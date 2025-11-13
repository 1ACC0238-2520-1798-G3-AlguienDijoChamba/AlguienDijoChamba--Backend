
namespace AlguienDijoChamba.Api.Reputation.Domain;

public class Tag
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    // Propiedad de navegación para la relaciónn N:M
    public ICollection<ProfessionalTag> ProfessionalTags { get; set; } = new List<ProfessionalTag>();

    private Tag() { }
    public Tag(string name) 
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}