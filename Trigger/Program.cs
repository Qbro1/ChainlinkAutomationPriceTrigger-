namespace Trigger
{
    using Nethereum.Web3;
    using Nethereum.Web3.Accounts;
    using System;
    using System.Security.Principal;
    using System.Threading.Tasks;

    class Program
    {
        
        private static readonly string rpcUrl = "https://rpc.ankr.com/eth_sepolia";
        private static readonly string privateKey = "Your_Key_MetaMask";
        private static readonly string contractAddress = "Address_contract_after_feploy";

       
        private static readonly string abiJson = @"[
        {""inputs"":[],""name"":""getLatestPrice"",""outputs"":[{""internalType"":""int256"",""name"":"""",""type"":""int256""}],""stateMutability"":""view"",""type"":""function""},
        {""inputs"":[],""name"":""targetLiquidationPrice"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},
        {""inputs"":[],""name"":""isLiquidated"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""stateMutability"":""view"",""type"":""function""},
        {""inputs"":[],""name"":""checkAndExecuteTrigger"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""}
    ]";

        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=================================================");
            Console.WriteLine("   CHAINLINK PRICE FEED TRIGGER - C# BOT SYSTEM   ");
            Console.WriteLine("=================================================");
            Console.ResetColor();

            var account = new Account(privateKey);
            var web3 = new Web3(account, rpcUrl);

            var contract = web3.Eth.GetContract(abiJson, contractAddress);

            var getPriceFunc = contract.GetFunction("getLatestPrice");
            var getTargetPriceFunc = contract.GetFunction("targetLiquidationPrice");
            var getIsLiquidatedFunc = contract.GetFunction("isLiquidated");
            var executeTriggerFunc = contract.GetFunction("checkAndExecuteTrigger");

            while (true)
            {
                try
                {
                    
                    long rawPrice = await getPriceFunc.CallAsync<long>();
                    long rawTargetPrice = await getTargetPriceFunc.CallAsync<long>();
                    bool isAlreadyLiquidated = await getIsLiquidatedFunc.CallAsync<bool>();

                    double currentEthPrice = rawPrice / 100000000.0;
                    double targetPrice = rawTargetPrice / 100000000.0;

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ETH/USD (Chainlink): ${currentEthPrice} | Target: ${targetPrice} | Executed: {isAlreadyLiquidated}");

                 
                    if (currentEthPrice < targetPrice && !isAlreadyLiquidated)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[!] TARGET PRICE DETECTED! Sending automated blockchain transaction...");
                        Console.ResetColor();

                       
                        var gas = await executeTriggerFunc.EstimateGasAsync(account.Address, null, null);
                        var txHash = await executeTriggerFunc.SendTransactionAsync(account.Address, gas, null);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[SUCCESS] Tx Sent! Hash: {txHash}");
                        Console.WriteLine("Waiting for contract state to update...\n");
                        Console.ResetColor();

                        await Task.Delay(15000); 
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error or RPC Timeout: {ex.Message}");
                    Console.ResetColor();
                }

                
                await Task.Delay(8000);
            }
        }
    }

}
