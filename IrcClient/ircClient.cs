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
        private int _randomnumber;
        private readonly Server _server;
        private readonly User _user;
        private TcpClient _client;
        private NetworkStream _stream;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;

        private string readLine;

        private AsyncOperation asyncOperation;
        #endregion
        #region Constructor
        public IrcClient(string server, string username) : this(server, 6667, username)
        {
        }

        public IrcClient(string server, int port, string username)
        {
            _randomnumber = new Random().Next(1234, 123456);
            _server = new Server(server, port, "");
            _user = new User(username + "#" + _randomnumber, username + "2#" + _randomnumber);
            asyncOperation = AsyncOperationManager.CreateOperation(null);
            readLine = string.Empty;
        }
        #endregion
        #region Events

        public event EventHandler<UserJoinedEventArgs> UserJoined = delegate { };
        public event EventHandler<ExceptionEventArgs> ExceptionThrown = delegate { };

        private void Fire_UserJoined(UserJoinedEventArgs o)
        {
            asyncOperation.Post(x => UserJoined(this, (UserJoinedEventArgs)x), o);
        }
        private void Fire_ExceptionThrown(Exception ex)
        {
            asyncOperation.Post(x => ExceptionThrown(this, (ExceptionEventArgs)x), new ExceptionEventArgs(ex));
        }
        #endregion
        #region Public Methods
        public void Dispose()
        {
            //todo: implement
        }
        public void Connect()
        {
            var t = new Thread(ConnectThread);
        }
        private void ConnectThread()
        {
            try
            {
                _client = new TcpClient(_server.ServerAddress, _server.Port);
                _stream = _client.GetStream();
                _streamReader = new StreamReader(_stream);
                _streamWriter = new StreamWriter(_stream);

                if (!string.IsNullOrEmpty(_server.Password))
                    Send(string.Format("PASS {0}", _server.Password));

                Send("NICK " + _user.Username);
                Send("USER " + _user.Username + " 0 * :" + _user.Username);

                Listen();
            }
            catch (Exception e)
            {
                Fire_ExceptionThrown(e);
            }
        }
        #endregion
        #region Private Methods
        private void Send(string msg)
        {
            _streamWriter.WriteLine(msg;
            _streamWriter.Flush();
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
