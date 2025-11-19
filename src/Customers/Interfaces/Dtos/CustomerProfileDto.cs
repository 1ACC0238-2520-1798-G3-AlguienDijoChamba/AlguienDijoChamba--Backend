namespace AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

// DTO para devolver los datos del cliente
// Se utiliza para transportar la respuesta de vuelta al API Controller
public class CustomerProfileDto
{
    // Campos básicos (para mostrar en el perfil)
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Celular { get; set; } = string.Empty;
    // Métodos de pago y preferencias (los campos nuevos)
    public string PreferredPaymentMethod { get; set; } // Lo mostramos como string (ej: "Credit/Debit Card")
    public bool AcceptsBookingUpdates { get; set; }
    public bool AcceptsPromotionsAndOffers { get; set; }
    public bool AcceptsNewsletter { get; set; }
}