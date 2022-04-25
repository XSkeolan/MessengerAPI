using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    [Table("files")]
    public class File : EntityBase
    {
        /// <summary>
        /// Имя или IP адрес сервера, на котором храниться файл
        /// </summary>
        [Column("server")]
        public string Server { get; set; }
        /// <summary>
        /// Путь файла
        /// </summary>
        [Column("path")]
        public string Path { get; set; }
    }
}
