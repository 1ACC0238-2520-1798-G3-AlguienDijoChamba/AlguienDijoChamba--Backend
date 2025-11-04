namespace AlguienDijoChamba.Api.Customers.Domain;

public class Customer
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Nombres { get; private set; }
    public string Apellidos { get; private set; }
    public string Celular { get; private set; }

    private Customer() { } 

    private Customer(Guid userId, string nombres, string apellidos, string celular)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Nombres = nombres;
        Apellidos = apellidos;
        Celular = celular;
    }

    public static Customer Create(Guid userId, string nombres, string apellidos, string celular)
    {
        return new Customer(userId, nombres, apellidos, celular);
    }
}