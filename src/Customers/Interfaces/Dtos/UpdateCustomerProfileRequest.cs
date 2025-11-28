namespace AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

public class UpdateCustomerProfileRequest
{
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? Celular { get; set; }
    public string? PhotoUrl { get; set; }
    public int PreferredPaymentMethod { get; set; }
    public bool AcceptsBookingUpdates { get; set; }
    public bool AcceptsPromotionsAndOffers { get; set; }
    public bool AcceptsNewsletter { get; set; }
}
