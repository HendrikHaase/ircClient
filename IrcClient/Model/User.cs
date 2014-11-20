namespace IrcClient.Model
{
    public class User
    {
        public string Username { get; private set; }
        public string AltUsername { get; private set; }

        public User(string username, string altusername = "")
        {
            Username = username;
            AltUsername = altusername;
        }
    }
}
