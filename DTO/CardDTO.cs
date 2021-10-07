using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManager.DTO
{
    public class CardDTO : BaseDTO
    {
        public long CustomerId { get; set; }

        public string CardType { get; set; }
        public int TypeId { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
        public string CardNumber { get; set; }
    }
}
