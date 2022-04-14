namespace MessengerAPI.Models
{
    public abstract class EntityBase
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Удален ли объект
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
