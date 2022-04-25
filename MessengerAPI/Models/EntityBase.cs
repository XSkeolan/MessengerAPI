using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    public abstract class EntityBase
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Удален ли объект
        /// </summary>
        [Column("isdeleted")]
        public bool IsDeleted { get; set; } = false;
    }
}
