using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerManager.Models;

namespace CustomerManager.Repositories
{
    interface ICardRepository : IBaseRepository<Card>
    {
        Task<IEnumerable<Card>> GetCardsByCustomerId(long customerId);
        Task DeleteCustomerCards(long customerId);
    }
}
