using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManager.Models
{
    public enum CardType
    {
        Unknown = 0,
        Amex = 1,
        Visa = 2,
        MasterCard = 3
    }
    public class Card : BaseEntity
    {
        [Required]
        public long CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public CardType Type { get; set; }
        public string LastFourDigits { get; set; }
        public DateTime ExpiryDate { get; set; }
        public byte[] CardNumberHash { get; set; }
        public byte[] CVVHash { get; set; }
        public byte[] CCSalt { get; set; }
    }
}
