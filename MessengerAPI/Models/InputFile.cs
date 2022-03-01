namespace MessengerAPI.Models
{
    public class InputFile : EntityBase
    {
        public InputFile(string serverName, string path)
        {
            ServerName = serverName;
            Path = path;
        }
        public string ServerName { get; set; }
        public string Path { get; set; }
    }
}
