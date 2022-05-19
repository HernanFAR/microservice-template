namespace SharedKernel.Domain.Interfaces
{
    public interface IEntity
    {
        object[] GetKeys();

        bool EntityEquals(IEntity? other);
    }

    public interface IEntity<TKey> : IEntity
    {
        TKey Id { get; }
    }
}
