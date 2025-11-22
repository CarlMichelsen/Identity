using Database.Entity;

namespace Database.Util;

public class TypedGuid<TEntity>
    where TEntity : IEntity
{
    private const int AllowedVersion = 7;
    
    protected TypedGuid(Guid value, bool allowWrongVersion = false)
    {
        if (!allowWrongVersion && value.Version != AllowedVersion)
        {
            throw new Exception(
                $"Attempted to instantiate {nameof(TypedGuid<TEntity>)} with a guid not of the allowed version {AllowedVersion}.");
        }

        this.Value = value;
    }

    protected TypedGuid()
    {
        this.Value = Guid.CreateVersion7();
    }

    public Guid Value { get; }

    // EF Core compatibility
    public static implicit operator Guid(TypedGuid<TEntity> id) => id.Value;
    
    public static explicit operator TypedGuid<TEntity>(Guid value) => new(value);

    // Equality operators
    public static bool operator ==(TypedGuid<TEntity>? a, TypedGuid<TEntity>? b) => a?.Value == b?.Value;

    public static bool operator !=(TypedGuid<TEntity>? a, TypedGuid<TEntity>? b) => a?.Value != b?.Value;

    public override string ToString() => this.Value.ToString();

    public override bool Equals(object? obj) =>
        obj is TypedGuid<TEntity> other && this.Value.Equals(other.Value);

    public override int GetHashCode() => this.Value.GetHashCode();
}