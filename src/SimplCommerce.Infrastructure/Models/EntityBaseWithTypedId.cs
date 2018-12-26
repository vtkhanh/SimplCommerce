namespace SimplCommerce.Infrastructure.Models
{
    public abstract class EntityBaseWithTypedId<TId> : ValidatableObject, IEntityWithTypedId<TId>
    {
        protected EntityBaseWithTypedId() { }
        protected EntityBaseWithTypedId(TId id) => Id = id;
        public TId Id { get; protected set; }
    }
}
