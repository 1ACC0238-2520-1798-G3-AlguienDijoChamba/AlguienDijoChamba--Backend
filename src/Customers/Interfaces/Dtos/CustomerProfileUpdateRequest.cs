namespace AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

public class CustomerProfileUpdateRequest 
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Celular { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public int PreferredPaymentMethod { get; set; } 
    public bool AcceptsBookingUpdates { get; set; }
    public bool AcceptsPromotionsAndOffers { get; set; }
    public bool AcceptsNewsletter { get; set; }
}