namespace elephant.core.model.p2p.message
{
    public abstract class Message
    {
        public abstract MessageType MessageType { get; set; }
        public abstract bool IsMessageValid();
        public abstract string SerializeAsJson();
    }
}
