namespace Entities;

public abstract class BaseEntity<TKey> : IEntity<TKey>
    where TKey : struct
{
    public abstract TKey Id { get; }

    public virtual string ToLogString(string val = "")
        => $"[{this.GetType().Name}] ID: {this.Id} | {val}";
}