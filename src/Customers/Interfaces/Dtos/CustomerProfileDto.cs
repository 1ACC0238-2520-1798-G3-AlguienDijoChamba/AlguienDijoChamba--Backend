namespace AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

public class CustomerProfileDto
{
    public Guid Id { get; set; }  // ✅ AGREGADO
    public Guid UserId { get; set; }  // ✅ AGREGADO
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public string Celular { get; set; }
    public string? PhotoUrl { get; set; }  // ✅ AGREGADO
    public string PreferredPaymentMethod { get; set; }
    public bool AcceptsBookingUpdates { get; set; }
    public bool AcceptsPromotionsAndOffers { get; set; }
    public bool AcceptsNewsletter { get; set; }

    // Constructor vacío para EF
    public CustomerProfileDto() { }

    // Constructor con parámetros (opcional)
    public CustomerProfileDto(
        Guid id,
        Guid userId,
        string nombres,
        string apellidos,
        string celular,
        string? photoUrl,
        string preferredPaymentMethod,
        bool acceptsBookingUpdates,
        bool acceptsPromotionsAndOffers,
        bool acceptsNewsletter)
    {
        Id = id;
        UserId = userId;
        Nombres = nombres;
        Apellidos = apellidos;
        Celular = celular;
        PhotoUrl = photoUrl;
        PreferredPaymentMethod = preferredPaymentMethod;
        AcceptsBookingUpdates = acceptsBookingUpdates;
        AcceptsPromotionsAndOffers = acceptsPromotionsAndOffers;
        AcceptsNewsletter = acceptsNewsletter;
    }
}
