using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Keepix.EVMBridge.HTTP
{
    public class HttpServer
    {
        private static HttpServer _instance;
        private IHost _host;

        public static HttpServer Instance
        {
            get
            {
                if (_instance == null) _instance = new HttpServer();
                return _instance;
            }
        }

        public HttpServer()
        {
            _host = new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddControllers();
                        services.AddRouting();
                    })
                    .UseUrls($"http://localhost:{Program.Config.Http.Port}")
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                })
                .Build();
        }

        public void Start()
        {
            _host.Start();
        }

        public void Stop()
        {
            _host.StopAsync().Wait();
        }
    }
}
