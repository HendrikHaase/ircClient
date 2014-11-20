using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using IrcClient.Model;

namespace IrcClient
{
    public class IrcClient : IDisposable
    {
        #region Variables
        private static int _randomnumber;
        private Server _server;
        private User _user;
        private TcpClient client;
        private NetworkStream stream;
        private StreamReader streamReader;
        private StreamWriter streamWriter;

        private string readLine;

        private AsyncOperation asyncOperation;
        #endregion
        #region Constructor
        public IrcClient(string server, string username)
        {
            _randomnumber = new Random().Next(1234, 123456);
            _user = new User(username + "#" + _randomnumber, username + "2#" + _randomnumber);
            _server = new Server(server, 6667, "");
            asyncOperation = AsyncOperationManager.CreateOperation(null);
        }

        public IrcClient(string server, int port, string username)
        {
            _randomnumber = new Random().Next(1234, 123456);
            _server = new Server(server, port, "");
            _user = new User(username + "#" + _randomnumber, username + "2#" + _randomnumber);
            asyncOperation = AsyncOperationManager.CreateOperation(null);
        }
        #endregion
        #region Events

        public event EventHandler<UserJoinedEventArgs> UserJoined = delegate { };

        private void Fire_UserJoined(UserJoinedEventArgs o)
        {
            asyncOperation.Post(x => UserJoined(this, (UserJoinedEventArgs)x), o);

        }
        #endregion
        #region Public Methods
        public void Dispose()
        {
            //todo: implement
        }

        public void Connect()
        {
            var t = new Thread();
        }

        private void ConnectThread()
        {
            try
            {
                client = new TcpClient(_server.ServerAddress, _server.Port);
                stream = client.GetStream();
                streamReader = new StreamReader(stream);
                streamWriter = new StreamWriter(stream);

                if (!string.IsNullOrEmpty(_server.Password))
                    Send(string.Format("PASS {0}", _server.Password));

                Send("NICK " + _user.Username);
                Send("USER " + _user.Username + " 0 * :" + _user.Username);

                Listen();
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        #endregion
        #region Private Methods
        private void Send(string msg)
        {
            streamWriter.WriteLine(msg;
            streamWriter.Flush();
        }
        private void Listen()
        {
            ParseData(readLine);
            Console.Write(readLine);
        }

        private void ParseData(string data)
        {
            
        }
        #endregion
    }
}
