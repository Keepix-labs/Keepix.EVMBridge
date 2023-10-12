using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3.Accounts;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Util;
using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.RPC.Eth.DTOs;

namespace Keepix.EVMBridge.EVM
{
    public class UniswapService
    {
        private readonly Web3 _web3;
        private readonly string _privateKey;

        public UniswapService(Web3 web3, string privateKey)
        {
            _web3 = web3;
            _privateKey = privateKey;
        }

        public async Task<TransactionReceipt> SwapEthForTokens(BigInteger ethAmount, string tokenAddress)
        {
            var senderAddress = (await _web3.Eth.Accounts.SendRequestAsync())[0];
            var routerContract = _web3.Eth.GetContract(File.ReadAllText("./ABI/SwapRouter.json"), Program.Config.UniswapRouterAddress);

            var swapFunction = routerContract.GetFunction("exactInputSingle");

            var deadline = new DateTimeOffset(DateTime.UtcNow.AddMinutes(20)).ToUnixTimeSeconds(); 

            var txInput = swapFunction.CreateTransactionInput(senderAddress);
            txInput.Value = ethAmount.ToHexBigInteger(); 
            txInput.Data = swapFunction.GetData(
                Program.Config.WethAddress,
                tokenAddress,
                3000, 
                senderAddress,
                deadline,
                ethAmount,
                new BigInteger(0), 
                new byte[0] 
            );

            var txHash = await _web3.Eth.Transactions.SendTransaction.SendRequestAsync(txInput);
            var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
            return receipt;
        }
    }
}
