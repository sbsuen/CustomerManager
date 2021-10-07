using CustomerManager.DBContexts;
using CustomerManager.DTO;
using CustomerManager.Models;
using CustomerManager.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManager.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerRepository _customerRepository;
        private readonly CardRepository _cardRepository;

        public CustomersController(CustomerManagerContext context)
        {
            _customerRepository = new CustomerRepository(context);
            _cardRepository = new CardRepository(context);
        }

        // GET: api/Customers
        /// <summary>
        /// Lists all customers
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            var result = await _customerRepository.GetAll();
            return new ActionResult<IEnumerable<Customer>>(result);
        }

        // GET: api/Customers/5
        /// <summary>
        /// Gets a specific customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(long id)
        {
            var customer = await _customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        /// <summary>
        /// Edits an existing customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customerDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(long id, CustomerDTO customerDTO)
        {
            if (id != customerDTO.Id)
            {
                return BadRequest();
            }

            var customer = await _customerRepository.GetById(id);

            customer.Name = customerDTO.Name;
            customer.Address = customerDTO.Address;
            customer.DateOfBirth = customerDTO.DateOfBirth;
            customer.LastUpdatedDate = DateTime.Now;

            try
            {
                await _customerRepository.Update(customer);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        /// <summary>
        /// Adds a new customer
        /// </summary>
        /// <param name="customerDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerDTO customerDTO)
        {
            Customer customer = CreateCustomerFromDTO(customerDTO);
            await _customerRepository.Add(customer);

            return CreatedAtAction("GetCustomerById", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        /// <summary>
        /// Deletes an existing customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(long id)
        {
            var customer = await _customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            await _customerRepository.Delete(customer);
            await _cardRepository.DeleteCustomerCards(id);

            return customer;
        }

        private Customer CreateCustomerFromDTO(CustomerDTO customerDTO)
        {
            Customer customer = new Customer
            {
                Id = customerDTO.Id,
                Name = customerDTO.Name,
                Address = customerDTO.Address,
                DateOfBirth = customerDTO.DateOfBirth,
                CreateDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now
            };
            return customer;
        }

        private bool CustomerExists(long id)
        {
            return _customerRepository.Exists(id);
        }
    }
}
