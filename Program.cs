using log4net;
using Microsoft.AspNetCore.Builder;
using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Microsoft.AspNetCore.Hosting;
using Keepix.EVMBridge.HTTP;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace Keepix.EVMBridge 
{
    internal class Program
    {
        public static KeepixConfig Config { get; set; }
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            loadConfiguration();
            log.Info("Keepix Bridge configuration loaded");

            // Setup connection
            foreach (var evmConfig in Config.EvmInstances)
            {
                EVM.EVMInstanceManager.LoadInstance(evmConfig.Value);
                log.Info(evmConfig.Key.ToUpper() + "[" + evmConfig.Value.Id + "] configuration loaded");
            }

            HttpServer.Instance.Start();
            log.Info("Http API Server started on *:" + Config.Http.Port);

            while (true) Console.ReadLine();
        }

        private static void loadConfiguration()
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();
            Config = deserializer.Deserialize<KeepixConfig>(System.IO.File.ReadAllText("./config.yml"));
        }
    }
}