namespace MessengerAPI.Models
{
    public class UserType : EntityBase
    {
        /// <summary>
        /// Название типа
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// Права данного типа
        /// </summary>
        public string Permissions { get; set; }
        /// <summary>
        /// Уровень приоритета типа над другими
        /// </summary>
        public short PriorityLevel { get; set; } = 1;
        /// <summary>
        /// Является ли этот тип типом по умолчанию
        /// </summary>
        public bool IsDefault { get; set; } = false;
    }
}
