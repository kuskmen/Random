using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Consul.Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .Configure(builder => builder.Map("/healthcheck", HealthCheckHandler))
                .Build();

            await new ConsulRegistry(new ConsulRegistryPayload
            {
                Name = $"{Assembly.GetExecutingAssembly().GetName().Name}",
                Tags = new string[0],
                Address = "localhost:7777",
                Port = 7777,
                Check = new ConsulRegistryCheck
                {
                    Http = "http://localhost:7777/healthcheck",
                    DeregisterCriticalServicesAfter = "90m",
                    Interval = "5s",
                },
                Checks = null,
                EnableTagOverride = false,
            }).Register();

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();


        private static void HealthCheckHandler(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                context.Response.ContentType = context.Request.ContentType;
                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(new
                    {
                        StatusCode = context.Response.StatusCode.ToString(),
                        PathBase = context.Request.PathBase.Value.Trim('/'),
                        Path = context.Request.Path.Value.Trim('/'),
                        Method = context.Request.Method,
                        Scheme = context.Request.Scheme,
                        ContentType = context.Request.ContentType,
                        ContentLength = (long?)context.Request.ContentLength,
                        Content = new StreamReader(context.Request.Body).ReadToEnd(),
                        QueryString = context.Request.QueryString.ToString(),
                        Query = context.Request.Query
                            .ToDictionary(
                                item => item.Key,
                                item => item.Value,
                                StringComparer.OrdinalIgnoreCase)
                    })
                );
            });
        }

    }
}
