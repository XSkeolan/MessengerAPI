using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    public class UserType : EntityBase
    {
        /// <summary>
        /// Название типа
        /// </summary>
        [Column("typename")]
        public string TypeName { get; set; }
        /// <summary>
        /// Права данного типа
        /// </summary>
        [Column("permissions")]
        public string Permissions { get; set; }
        /// <summary>
        /// Уровень приоритета типа над другими
        /// </summary>
        [Column("prioritylevel")]
        public short PriorityLevel { get; set; } = 1;
        /// <summary>
        /// Является ли этот тип типом по умолчанию
        /// </summary>
        [Column("isdefault")]
        public bool IsDefault { get; set; } = false;
    }
}
