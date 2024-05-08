namespace Jwt.Core.Contexts.SharedContext.Entities;
public abstract class Entity : IEquatable<Guid> // Para realizar a comparação com outros objetos no futuro
{
    protected Entity() =>
        Id = Guid.NewGuid();

    public Guid Id { get; }

    public bool Equals(Guid id)
        => Id == id;

    public override int GetHashCode()
        => Id.GetHashCode();
}
