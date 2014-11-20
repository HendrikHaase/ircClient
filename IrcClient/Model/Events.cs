using System;

namespace IrcClient.Model
{
    public class UserJoinedEventArgs : EventArgs
    {
        public string Channel
        {
            get; 
            private set;
        }

        public string User
        {
            get; 
            private set;
        }
        public UserJoinedEventArgs(string channel, string user)
        {
            Channel = channel;
            User = user;
        }
    }
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception
        {
            get; 
            private set;
        }
        public ExceptionEventArgs(Exception x)
        {
            Exception = x;
        }
        public override string ToString()
        {
            return Exception.ToString();
        }
    }
}
