// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

interface AggregatorV3Interface {
    function latestRoundData()
        external
        view
        returns (
            uint80 roundId,
            int256 answer,
            uint256 startedAt,
            uint256 updatedAt,
            uint80 answeredInRound
        );
}

contract ChainlinkAutomationPriceTrigger {
    AggregatorV3Interface internal immutable priceFeed;
    address public immutable owner;
    
   
    uint256 public collateralAmount;
    uint256 public targetLiquidationPrice; 
    bool public isLiquidated;

    event CollateralDeposited(address indexed user, uint256 amount);
    event TargetPriceHit(int256 currentPrice, uint256 timestamp);
    event SystemReset(uint256 newTargetPrice);

    modifier onlyOwner() {
        require(msg.sender == owner, "Not an owner");
        _;
    }

    constructor() {
        owner = msg.sender;
        
        priceFeed = AggregatorV3Interface(0x694AA1769357215DE4FAC081bf1f309aDC325306);
        targetLiquidationPrice = 2500 * 10**8;
    }

   
    function depositCollateral() external payable {
        require(msg.value > 0, "Zero collateral");
        collateralAmount += msg.value;
        emit CollateralDeposited(msg.sender, msg.value);
    }

    
    function setTargetPrice(uint256 _newPriceInUsd) external onlyOwner {
        targetLiquidationPrice = _newPriceInUsd * 10**8;
        isLiquidated = false; 
        emit SystemReset(targetLiquidationPrice);
    }

    
    function getLatestPrice() public view returns (int256) {
        (, int256 price, , , ) = priceFeed.latestRoundData();
        return price; 
    }

    
    function checkAndExecuteTrigger() external {
        require(!isLiquidated, "Trigger already executed");
        
        int256 currentPrice = getLatestPrice();
        
        
        if (currentPrice < int256(targetLiquidationPrice)) {
            isLiquidated = true;
            collateralAmount = 0; 
            emit TargetPriceHit(currentPrice, block.timestamp);
        }
    }

   
    function withdrawAll() external onlyOwner {
        payable(owner).transfer(address(this).balance);
    }
}
