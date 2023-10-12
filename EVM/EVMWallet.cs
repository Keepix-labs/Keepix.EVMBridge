using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keepix.EVMBridge.EVM
{
    [Serializable]
    public class EVMWallet
    {
        public string Mnemonic { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }

        public static EVMWallet CreateEthereumWallet()
        {
            // Generate a new mnemonic (English word list)
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

            // Create a wallet from the mnemonic
            var wallet = new Wallet(mnemonic.ToString(), null);

            // Get the Ethereum account from the wallet
            var account = wallet.GetAccount(0);

            // Extract the private and public keys
            var privateKey = account.PrivateKey;
            var publicKey = new EthECKey(privateKey).GetPubKey().ToHex();

            return new EVMWallet()
            {
                Mnemonic = mnemonic.ToString(),
                PrivateKey = privateKey,
                PublicKey = publicKey
            };
        }

        public string PublicKeyToEthereumAddress()
        {
            var pubKey = new EthECKey(this.PublicKey);
            return pubKey.GetPublicAddress();
        }
    }
}
