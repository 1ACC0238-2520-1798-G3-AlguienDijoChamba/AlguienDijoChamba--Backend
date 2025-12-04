namespace AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

public class CustomerCompleteRegistrationRequest
{
    public int PreferredPaymentMethod { get; set; } 
    public bool AcceptsBookingUpdates { get; set; }
    public bool AcceptsPromotionsAndOffers { get; set; }
    public bool AcceptsNewsletter { get; set; }
}