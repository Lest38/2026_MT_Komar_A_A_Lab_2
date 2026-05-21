namespace Entities;

public interface IEntity<TKey>
    where TKey : struct
{
    TKey Id { get; }

    string ToLogString(string val = "");
}