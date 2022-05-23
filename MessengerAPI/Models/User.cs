using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    [Table("users")]
    public class User : EntityBase
    {
        /// <summary>
        /// Ник пользователя
        /// </summary>
        [Column("nickname")]
        public string Nickname { get; set; }
        /// <summary>
        /// Пароль в виде хеша
        /// </summary>
        [Column("password")]
        public string Password { get; set; }
        /// <summary>
        /// Номер телефона пользователя
        /// </summary>
        [Column("phonenumber")]
        public string Phonenumber { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Column("name")]
        public string Name { get; set; }
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        [Column("surname")]
        public string Surname { get; set; }
        /// <summary>
        /// Электронная почта пользователя
        /// </summary>
        [Column("email")]
        public string Email { get; set; }
        /// <summary>
        /// Является ли почта подтвержденной
        /// </summary>
        [Column("isconfirmed")]
        public bool IsConfirmed { get; set; } = false;
        /// <summary>
        /// Статус пользователя
        /// </summary>
        [Column("status")]
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Причина удаления аккаунта
        /// </summary>
        [Column("reason")]
        public string Reason { get; set; } = string.Empty;
    }
}
