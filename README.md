# Chainlink Price Feed Off-Chain Automated Trigger (C# & Solidity)

A production-ready architecture demonstrating how to combine decentralized data validation via Chainlink Data Feeds with an automated, ultra-fast off-chain monitoring engine written in C# using Nethereum.

### How it works:
1. **Smart Contract (Solidity):** Integrates `AggregatorV3Interface` to securely read real-time ETH/USD oracle values in Sepolia Testnet without flash-loan or price-manipulation vulnerabilities.
2. **Off-Chain Keeper (C#):** Continuously tracks live blockchain events and states using direct JSON-RPC node connections. When the target price condition is hit, it automatically signs, estimates gas, and broadcasts a liquidation transaction to the EVM network.

### Tech Stack:
- Solidity ^0.8.20
- Chainlink Data Feeds (Oracles)
- C# / .NET 8.0
- Nethereum Web3 Integration Framework
