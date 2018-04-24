using Microsoft.Extensions.Configuration;

namespace elephant.core.util
{
    public class StoreConfiguration
    {
        private IConfigurationRoot _configuration;

        public string ChainGraphIri { get; private set; }


        public StoreConfiguration(IConfigurationRoot configuration)
        {
            _configuration = configuration;
            ChainGraphIri = _configuration["StoreConfiguration:chainGraphIri"];
        }
    }
}
