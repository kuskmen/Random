using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consul.Service
{
    public class ConsulRegistry
    {
        private readonly HttpClient _httpClient;
        private readonly ConsulRegistryPayload _payload;

        public ConsulRegistry(ConsulRegistryPayload payload)
        {
            _payload = payload;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8500")
            };
        }

        public async Task Register()
        {
            var response = await _httpClient.PutAsync("/v1/agent/service/register", new StringContent(JsonConvert.SerializeObject(_payload)));
        }


    }

    public class ConsulRegistryPayload
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Tags { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public Dictionary<string, string> Meta { get; set; }
        public bool EnableTagOverride { get; set; }
        public ConsulRegistryCheck Check { get; set; }
        public ConsulRegistryCheck[] Checks { get; set; }
    }

    public class ConsulRegistryCheck
    {
        public string Name { get; set; }
        public string DeregisterCriticalServicesAfter { get; set; }
        public string[] Args { get; set; }
        public string Http { get; set; }
        public string Interval { get; set; }
        public string TTL { get; set; }
    }

    public class ConsulRegistryMetadata
    {

    }

}
