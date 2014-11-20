using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using IrcClient.Model;

namespace IrcClient
{
    public class IrcClient : IDisposable
    {
        #region Variables / Properties
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
        /// <summary>
        /// parses data for IRC Messages, join notice nick etc and fires events accordingly
        /// </summary>
        /// <param name="data"></param>
        private void ParseData(string data)
        {
            string[] Data = data.Split(' ');

            if (data.Length > 4 && data.Substring(0, 3) == "PING")
            {
                Send("PONG " + data[1]);
                return;
            }

            switch (Data[1])
            {
                case "001": //server welcome msg
                    Send(string.Format("MODE {0} +B", _user.Username));
                    //todo: implement event for connected
                    break;
                case "353": //channel user list
                    //todo: implement event for updating userlist
                    break;
                case "433": //nick is taken!
                    //todo: implement nick-taken event
                    break;
                case "JOIN":    //someone joined the channel
                    Fire_UserJoined(new UserJoinedEventArgs(Data[2], Data[0].Substring(1, Data[0].IndexOf("!", System.StringComparison.Ordinal) - 1)));
                    break;
                case "NICK":    //someone changed username
                    //todo: implement someone-changed-nick-event
                    break;
                case "NOTICE":
                    //todo: implement notice-event
                    break;
                case "PRIVMSG": //someone sent a qry-msg
                    //todo: implement qry-msg-event
                    break;
                case "PART":    //someone closed their client
                case "QUIT":    //someone left the channel
                    //todo: implement user-left event
                    break;
                default:
                    //todo: implement debug-event
                    break;
            }
        }
        /// <summary>
        /// Strips the message of unnecessary characters, cp from IrcClient-CSharp - thanks!
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string StripMessage(string message)
        {
            // remove IRC Color Codes
            foreach (Match m in new Regex((char)3 + @"(?:\d{1,2}(?:,\d{1,2})?)?").Matches(message))
                message = message.Replace(m.Value, "");

            // if there is nothing to strip
            if (message == "")
                return "";
            else if (message.Substring(0, 1) == ":" && message.Length > 2)
                return message.Substring(1, message.Length - 1);
            else
                return message;
        }
        /// <summary>
        /// Joins the array into a string after a specific index, cp from IrcClient-CSharp - thanks!
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private string JoinArray(string[] strArray, int startIndex)
        {
            return StripMessage(String.Join(" ", strArray, startIndex, strArray.Length - startIndex));
        }
        #endregion
    }
}
