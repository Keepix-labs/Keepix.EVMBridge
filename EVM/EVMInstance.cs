using log4net;
using Nethereum.HdWallet;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.JsonRpc.WebSocketClient;
using Nethereum.Web3.Accounts;

namespace Keepix.EVMBridge.EVM
{
    internal class EVMInstance
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EVMInstance));

        private BinaryObjectStore store;

        public EVMConnectionConfig EVMConnectionConfig { get; }
        public Web3 Web3 { get; set;  }
        public WebSocketClient WSS { get; }
        public EVMWallet Wallet { get; set; }
        public Account WalletAccount { get; set; }


        public EVMInstance(EVMConnectionConfig eVMConnectionConfig)
        {
            this.EVMConnectionConfig = eVMConnectionConfig;
            this.Web3 = new Web3(this.EVMConnectionConfig.Web3ConnectionString);
            this.WSS = new WebSocketClient(this.EVMConnectionConfig.WssConnectionString);
            this.store = new BinaryObjectStore("evm_" + this.EVMConnectionConfig.Id + ".bin");

            // Load assigned wallet if there is one
            try
            {
                this.Wallet = this.store.Retrieve<EVMWallet>("wallet");
                log.Info("Wallet " + this.Wallet.PublicKey + " assigned reloaded");
            }
            catch(Exception ex)
            {
                log.Error(ex);
                log.Info("No wallet assigned for this EVM instance");
            }
        }

        public void AssignWallet(EVMWallet wallet)
        {
            this.Wallet = wallet;
            this.WalletAccount = new Account(this.Wallet.PrivateKey);
            this.store.Store<EVMWallet>("wallet", this.Wallet);
            this.Web3 = new Web3(this.WalletAccount, this.WSS);
            log.Info("Wallet " + wallet.PublicKey + " assigned to evm instance id: " + this.EVMConnectionConfig.Id);
        }

        public async Task<decimal> GetEtherBalanceAsync()
        {
            if (this.Wallet == null)
            {
                throw new InvalidOperationException("No wallet assigned to this EVM instance.");
            }

            var balanceWei = await this.Web3.Eth.GetBalance.SendRequestAsync(this.Wallet.PublicKeyToEthereumAddress());
            var balanceEther = Web3.Convert.FromWei(balanceWei);

            return balanceEther;
        }

        public async Task<(bool Web3ConnectionState, bool WssConnectionState)> CheckConnectionsAsync()
        {
            bool web3State = false;
            bool wssState = false;

            // Check Web3 connection
            try
            {
                var blockNumber = await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
                web3State = blockNumber != null;
            }
            catch
            {
                web3State = false;
            }

            // Check WSS connection
            try
            {
                var wssWeb3 = new Web3(this.WSS);
                var blockNumberWss = await wssWeb3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
                wssState = blockNumberWss != null;
            }
            catch
            {
                wssState = false;
            }

            return (web3State, wssState);
        }

        public async Task<bool> ConvertEthToRplForCollateral()
        {
            const decimal MinEthRequired = 32m;
            const decimal ConversionPercentage = 0.11m;

            var balanceWei = await this.Web3.Eth.GetBalance.SendRequestAsync(this.Wallet.PublicKeyToEthereumAddress());
            var balanceEth = Web3.Convert.FromWei(balanceWei);

            if (balanceEth < MinEthRequired)
            {
                return false; // Not enough ETH
            }

            var ethToConvert = balanceEth * ConversionPercentage;
            var ethToConvertWei = Web3.Convert.ToWei(ethToConvert);

            var uniswapService = new UniswapService(this.Web3, this.Wallet.PrivateKey); 
            await uniswapService.SwapEthForTokens(ethToConvertWei, Program.Config.RplTokenAddress);

            return true; 
        }
    }
}
