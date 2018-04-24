using System;

namespace elephant.core.model.p2p.message
{
    public class AddPeerMessage : Message
    {
        public String peerAddress;

        public override MessageType MessageType
        {
            get
            {
                return MessageType.ADD_PEER;
            }
            set { }
        }

        public override bool IsMessageValid()
        {
            if (String.IsNullOrEmpty(peerAddress))
            {
                return false;
            }
            return true;
        }

        public override string SerializeAsJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
