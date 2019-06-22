using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permuto.CxC.Exchange.Entities
{
    public class Base
    {
        public string id { get; set; }
    }
    public class Commodity : Base
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
    }

    public enum TradeType
    {
        Buy,
        Sell
    }

    public class AskBase : Base
    {
        public Commodity TradeCommodity { get; set; }
        public TradeType Type { get; set; }
        public bool ApplyCommissionToBuy { get; set; }
        public long AvailableQuantity { get; set; }
        public long StartingQuantity { get; set; }
        public long? MinTradeQuantity { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class AskLeg : AskBase
    {
        public long BuyRatio { get; set; }
        public long SellRatio { get; set; }

    }
    public class Ask : AskBase
    {
        public AskLeg[] Legs { get; set; }
        public bool AllowPartialFill { get; set; } = true;
        public int MaxLegDepth { get; set; } = 10;
        public DateTime AskDate { get; set; } = DateTime.UtcNow;
        public DateTime? ValidToDate { get; set; }
    }
    public class Order : Base
    {
        public Commodity BuyCommodity { get; set; }
        public Commodity SellCommodity { get; set; }
        public long BuyQuantity { get; set; }
        public long SellQuantity { get; set; }
        public Ask Ask { get; set; }
        public long Commission { get; set; }
        public Commodity CommissionCommodity { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
