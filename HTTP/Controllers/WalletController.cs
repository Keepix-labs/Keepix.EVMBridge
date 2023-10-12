using Keepix.EVMBridge.EVM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keepix.EVMBridge.HTTP.Controllers
{
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        [HttpPut("{evmid}")]
        public IActionResult CreateWalletForInstance(int evmid)
        {
            var instance = EVMInstanceManager.EVMInstances.FirstOrDefault(x => x.EVMConnectionConfig.Id == evmid);

            var wallet = EVMWallet.CreateEthereumWallet();
            instance.AssignWallet(wallet);

            return new JsonResult(wallet);
        }

        [HttpGet("{evmid}/balance")]
        public async Task<IActionResult> GetWalletBalanceForInstance(int evmid)
        {
            var instance = EVMInstanceManager.EVMInstances.FirstOrDefault(x => x.EVMConnectionConfig.Id == evmid);
            var balance = await instance.GetEtherBalanceAsync();

            return new JsonResult(new
            {
                ether = balance
            });
        }

        [HttpGet("{evmid}/state")]
        public async Task<IActionResult> GetEVMState(int evmid)
        {
            var instance = EVMInstanceManager.EVMInstances.FirstOrDefault(x => x.EVMConnectionConfig.Id == evmid);
            var (web3State, wssState) = await instance.CheckConnectionsAsync();

            return new JsonResult(new
            {
                web3_state = web3State,
                wss_state = wssState
            });
        }

        [HttpGet("{evmid}/deposit")]
        public async Task<IActionResult> GetWalletDespositAddrForInstance(int evmid)
        {
            var instance = EVMInstanceManager.EVMInstances.FirstOrDefault(x => x.EVMConnectionConfig.Id == evmid);
            var addr = instance.Wallet.PublicKeyToEthereumAddress();

            return new JsonResult(new
            {
                address = addr
            });
        }

    }
}
