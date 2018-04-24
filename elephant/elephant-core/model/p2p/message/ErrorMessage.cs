namespace elephant.core.model.p2p.message
{
    public class ErrorMessage : Message
    {
        public override MessageType MessageType { get { return MessageType.ERROR; } set { } }

        private string _errorDescription;

        public ErrorMessage(string message)
        {
            _errorDescription = message;
        }

        public override bool IsMessageValid()
        {
            if (_errorDescription == null)
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
