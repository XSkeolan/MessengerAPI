namespace MessengerAPI.Models
{
    public class File : EntityBase
    {
        /// <summary>
        /// Имя или IP адрес сервера, на котором храниться файл
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// Путь файла
        /// </summary>
        public string Path { get; set; }
    }
}
