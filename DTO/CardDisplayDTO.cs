using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerManager.Models;

namespace CustomerManager.DTO
{
    public class CardDisplayDTO : BaseDTO
    {
        public long CustomerId { get; set; }
        public CardType Type { get; set; }
        public string LastFourDigits { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
