using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keepix.EVMBridge
{
    public class KeepixConfig
    {
        public string StorageKey { get; set; }
        public string RplTokenAddress { get; set; }
        public string UniswapRouterAddress { get; set; }
        public string WethAddress { get; set; }
        public Dictionary<string, EVM.EVMConnectionConfig> EvmInstances { get; set; }
        public HttpNode Http { get; set; }

        public class HttpNode
        {
            public int Port { get; set; }
        }
    }
}
