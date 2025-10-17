namespace AlguienDijoChamba.Api.IAM.Domain;
public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    private User(Guid id, string email, string passwordHash) { Id = id; Email = email; PasswordHash = passwordHash; }
    public static User Create(string email, string passwordHash) => new(Guid.NewGuid(), email, passwordHash);
}