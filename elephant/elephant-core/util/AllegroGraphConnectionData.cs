using Microsoft.Extensions.Configuration;

namespace elephant.core.util
{
    public class AllegroGraphConnectionData
    {
        private IConfigurationRoot _configuration;

        public string Url { get; private set; }
        public string StoreId { get; private set; }
        public string User { get; private set; }
        public string Pass { get; private set; }

        public AllegroGraphConnectionData(IConfigurationRoot configuration)
        {
            _configuration = configuration;
            Url = _configuration["StoreConfiguration:AllegroGraphConnection:url"];
            StoreId = _configuration["StoreConfiguration:AllegroGraphConnection:storeId"];
            User = _configuration["StoreConfiguration:AllegroGraphConnection:user"];
            Pass = _configuration["StoreConfiguration:AllegroGraphConnection:pass"];
        }
    }
}
