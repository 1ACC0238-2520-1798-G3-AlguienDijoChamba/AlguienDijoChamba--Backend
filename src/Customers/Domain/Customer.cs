using AlguienDijoChamba.Api.Customers.Domain.Enum;

namespace AlguienDijoChamba.Api.Customers.Domain;

public class Customer
{
    // Propiedades existentes (Configuradas en Etapa 1)
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Nombres { get; private set; }
    public string Apellidos { get; private set; }
    public string Celular { get; private set; }
    
    // Propiedades nuevas (Configuradas en Etapa 2 o Update)
    public string PhotoUrl { get; private set; }
    public PreferredPaymentMethod PreferredPaymentMethod { get; private set; }
    public bool AcceptsBookingUpdates { get; private set; }
    public bool AcceptsPromotionsAndOffers { get; private set; }
    public bool AcceptsNewsletter { get; private set; }

    // Constructor privado para ORM/Deserialización
    private Customer() { } 

    // Constructor privado para la CREACIÓN (Registro Etapa 1: Datos esenciales)
    private Customer(Guid userId, string nombres, string apellidos, string celular)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Nombres = nombres;
        Apellidos = apellidos;
        Celular = celular;

        // Inicialización de los campos nuevos a valores por defecto
        PhotoUrl = string.Empty;
        PreferredPaymentMethod = PreferredPaymentMethod.None;
        AcceptsBookingUpdates = true; // Manteniendo tu valor inicial por defecto
        AcceptsPromotionsAndOffers = false;
        AcceptsNewsletter = false;
    }

    // Método de fábrica: Registro Etapa 1
    public static Customer Create(Guid userId, string nombres, string apellidos, string celular)
    {
        return new Customer(userId, nombres, apellidos, celular);
    }
    
    // --- MÉTODOS DE MODIFICACIÓN DEL DOMINIO ---

    // 1. COMPLETAR REGISTRO (Tu Etapa 2: Solo agrega los datos que faltaban)
    // 💡 NOTA: Este método se usa si la foto y las preferencias se envían juntas.
    public void CompleteRegistrationProfile(
        string photoUrl, 
        PreferredPaymentMethod preferredPaymentMethod,
        bool acceptsBookingUpdates, 
        bool acceptsPromotionsAndOffers, 
        bool acceptsNewsletter)
    {
        PhotoUrl = photoUrl;
        PreferredPaymentMethod = preferredPaymentMethod;
        
        AcceptsBookingUpdates = acceptsBookingUpdates;
        AcceptsPromotionsAndOffers = acceptsPromotionsAndOffers;
        AcceptsNewsletter = acceptsNewsletter;
    }
    
    // 2. ACTUALIZACIÓN GENERAL DEL PERFIL (Posterior al registro, incluye todos los campos)
    public void UpdateProfile(
        string nombres, 
        string apellidos, 
        string celular,
        string photoUrl, 
        PreferredPaymentMethod preferredPaymentMethod,
        bool acceptsBookingUpdates, 
        bool acceptsPromotionsAndOffers, 
        bool acceptsNewsletter)
    {
        Nombres = nombres;
        Apellidos = apellidos;
        Celular = celular;
        
        PhotoUrl = photoUrl;
        PreferredPaymentMethod = preferredPaymentMethod;
        
        AcceptsBookingUpdates = acceptsBookingUpdates;
        AcceptsPromotionsAndOffers = acceptsPromotionsAndOffers;
        AcceptsNewsletter = acceptsNewsletter;
    }

    // 3. ACTUALIZAR SOLO LA URL DE LA FOTO (Usado por el Handler de subida de archivos)
    public void UpdatePhotoUrl(string photoUrl)
    {
        if (string.IsNullOrEmpty(photoUrl))
        {
            throw new ArgumentException("La URL de la foto no puede estar vacía.");
        }
        PhotoUrl = photoUrl;
    }

    // 4. ✅ NUEVO: ACTUALIZAR SOLO LAS PREFERENCIAS (Usado por el Handler de preferencias)
    public void UpdatePreferences(
        PreferredPaymentMethod preferredPaymentMethod,
        bool acceptsBookingUpdates, 
        bool acceptsPromotionsAndOffers, 
        bool acceptsNewsletter)
    {
        PreferredPaymentMethod = preferredPaymentMethod;
        AcceptsBookingUpdates = acceptsBookingUpdates;
        AcceptsPromotionsAndOffers = acceptsPromotionsAndOffers;
        AcceptsNewsletter = acceptsNewsletter;
    }
}