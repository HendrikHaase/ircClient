using System;

namespace IrcClient.Model
{
    public class UserJoinedEventArgs : EventArgs
    {
        public string Channel { get; internal set; }
        public string User { get; internal set; }
        public UserJoinedEventArgs(string Channel, string User)
        {
            this.Channel = Channel;
            this.User = User;
        }
    }
}
