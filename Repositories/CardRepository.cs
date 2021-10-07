using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerManager.DBContexts;
using CustomerManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManager.Repositories
{
    public class CardRepository : BaseRepository<Card>, ICardRepository
    {
        public CardRepository(CustomerManagerContext context) : base(context) { }

        public override async Task<IEnumerable<Card>> GetAll()
        {
            return await _context.Cards.ToListAsync();
        }

        public async Task<IEnumerable<Card>> GetCardsByCustomerId(long customerId)
        {
            return await _context.Cards.Where(card => card.CustomerId == customerId).ToListAsync();
        }

        public async Task DeleteCustomerCards(long customerId)
        {
            var customerCards = await GetCardsByCustomerId(customerId);
            _context.Cards.RemoveRange(customerCards);
            await _context.SaveChangesAsync();
        }
    }
}
