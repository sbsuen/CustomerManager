using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerManager.DBContexts;
using CustomerManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManager.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(CustomerManagerContext context) : base(context) { }
    }
}
