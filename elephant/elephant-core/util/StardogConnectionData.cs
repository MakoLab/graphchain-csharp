using Microsoft.Extensions.Configuration;

namespace elephant.core.util
{
    public class StardogConnectionData
    {
        private IConfigurationRoot _configuration;

        public string Url { get; private set; }
        public string StoreId { get; private set; }
        public string User { get; private set; }
        public string Pass { get; private set; }

        public StardogConnectionData(IConfigurationRoot configuration)
        {
            _configuration = configuration;
            Url = _configuration["StardogConnection:url"];
            StoreId = _configuration["StardogConnection:storeId"];
            User = _configuration["StardogConnection:user"];
            Pass = _configuration["StardogConnection:pass"];
        }
    }
}
