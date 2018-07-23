using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Consul.Service
{
    public class ConsulRegistry
    {
        private readonly ConsulClient _consulClient;

        public ConsulRegistry()
        {
            _consulClient = new ConsulClient((options) =>
            {
                options.Address = new Uri("http://localhost:8500");
            }); 
        }

        public async Task Register()
        {
            await _consulClient.Agent.ServiceRegister(new AgentServiceRegistration
            {
                Address = "localhost",
                Tags = new [] { "urlprefix-/test-path" },
                Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    HTTP = "http://localhost:7777/health",
                    Interval = TimeSpan.FromSeconds(5),
                },
                Name = $"{Assembly.GetExecutingAssembly().GetName().Name}",
                ID = $"{Assembly.GetExecutingAssembly().GetName().Name}-localhost:7777",
                Port = 7777,
                EnableTagOverride = false
            });
        }
    }
}
