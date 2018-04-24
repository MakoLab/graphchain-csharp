using System;

namespace elephant.core.exception
{
    public class PeerToPeerConnectionException : Exception
    {
        public PeerToPeerConnectionException(string message) : base(message)
        {
        }

        public PeerToPeerConnectionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
