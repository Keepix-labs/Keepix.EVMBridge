using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keepix.EVMBridge.EVM
{
    internal class EVMInstanceManager
    {
        public static List<EVMInstance> EVMInstances = new List<EVMInstance>();

        public static EVMInstance LoadInstance(EVMConnectionConfig eVMConnectionConfig)
        {
            var instance = new EVMInstance(eVMConnectionConfig);
            EVMInstances.Add(instance);
            return instance;
        }
    }
}
