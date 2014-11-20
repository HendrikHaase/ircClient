namespace IrcClient.Model
{
    public class Server
    {
        public string ServerAddress { get; private set; }
        public int Port { get; private set; }
        public string Password { get; private set; }

        public Server(string serverAddress, int port, string password = "")
        {
            ServerAddress = serverAddress;
            Port = port;
            Password = password;
        }
    }
}
