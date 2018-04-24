using elephant.core.model.p2p.message;

namespace elephant.core.model.p2p
{
    public class PeerToPeerCommand
    {
        public CommandType CommandType;
        public Message Message;

        public PeerToPeerCommand(CommandType commandType)
        {
            CommandType = commandType;
        }

        public PeerToPeerCommand(CommandType commandType, Message message) : this(commandType)
        {
            Message = message;
        }

        public MessageType? GetMessageType()
        {
            if (Message != null)
                return Message.MessageType;
            else
                return null;
        }

        public override string ToString()
        {
            return "PeerToPeerCommand{" + "commandType=" + CommandType + ", message=" + Message + "}";
        }
    }
}
