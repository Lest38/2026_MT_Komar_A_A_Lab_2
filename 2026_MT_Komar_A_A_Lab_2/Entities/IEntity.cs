namespace Entities;

public interface IEntity<out TKey>
    where TKey : struct
{
    TKey Id { get; }

    string ToLogString(string val = "");
}