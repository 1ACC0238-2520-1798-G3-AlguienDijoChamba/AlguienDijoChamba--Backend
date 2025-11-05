public class Tag
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Tag() { }
    public Tag(string name) => Id = Guid.NewGuid();
}