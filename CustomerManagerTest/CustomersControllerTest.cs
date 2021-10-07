using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using CustomerManager.Controllers;
using CustomerManager.DTO;
using CustomerManager.Models;
using CustomerManager.DBContexts;
using CustomerManager.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using Microsoft.EntityFrameworkCore.Storage;

namespace CustomerManagerTest
{
    public class CustomersControllerTest
    {

        [Fact]
        public async Task Test1_PostCustomer()
        {
            var options = getOptions();

            using (var context = new CustomerManagerContext(options))
            {
                CustomersController controller = new CustomersController(context);
                CustomerDTO custOne = new CustomerDTO
                {
                    Id = 1,
                    Name = "John Smith",
                    Address = "21 Jump Street",
                    DateOfBirth = DateTime.ParseExact("04/24/1992", "MM/dd/yyyy", CultureInfo.InvariantCulture)
                };
                var data = await controller.PostCustomer(custOne);
                var response = data.Result as CreatedAtActionResult;
                int? statusCode = response.StatusCode;
                Console.WriteLine(statusCode);
                Assert.True(statusCode == ((int)HttpStatusCode.Created));
            }
        }

        [Fact]
        public async Task Test2_PutCustomer_Success()
        {
            var options = getOptions();

            using (var context = new CustomerManagerContext(options))
            {
                CustomersController controller = new CustomersController(context);
                CustomerDTO custOne = new CustomerDTO
                {
                    Id = 1,
                    Name = "John Smith",
                    Address = "123 Fake Street",
                    DateOfBirth = DateTime.ParseExact("04/24/1992", "MM/dd/yyyy", CultureInfo.InvariantCulture)
                };
                await controller.PutCustomer(1, custOne);
                var newValue = await controller.GetCustomerById(1);
                Assert.Equal("123 Fake Street", newValue.Value.Address);
            }
        }
        
        [Fact]
        public async Task Test3_PutCustomer_Failure()
        {
            var options = getOptions();

            using (var context = new CustomerManagerContext(options))
            {
                CustomersController controller = new CustomersController(context);
                CustomerDTO custOne = new CustomerDTO
                {
                    Id = 1,
                    Name = "John Smith",
                    Address = "21 Jump Street",
                    DateOfBirth = DateTime.ParseExact("04/24/1992", "MM/dd/yyyy", CultureInfo.InvariantCulture)
                };
                custOne.Address = "123 Fake Street";
                var result = await controller.PutCustomer(2, custOne);
                var badRequestResult = result as BadRequestResult;
                Assert.NotNull(badRequestResult);
                Assert.True(badRequestResult.StatusCode == 400);
            }
        }

        [Fact]
        public async Task Test4_GetCustomerById_Success()
        {
            var options = getOptions();

            using (var context = new CustomerManagerContext(options))
            {
                CustomersController controller = new CustomersController(context);
                var result = await controller.GetCustomerById(1);
                Assert.Equal("John Smith", result.Value.Name);
                Assert.Equal("123 Fake Street", result.Value.Address);
            }
        }

        [Fact]
        public async Task Test5_GetCustomerById_NotFound()
        {
            var options = getOptions();

            using (var context = new CustomerManagerContext(options))
            {
                CustomersController controller = new CustomersController(context);
                var result = await controller.GetCustomerById(2);
                var notFoundResult = result.Result as NotFoundResult;
                Assert.NotNull(notFoundResult);
                Assert.True(notFoundResult.StatusCode == 404);
            }
        }

        [Fact]
        public async Task Test6_GetAllCustomers()
        {
            var options = getOptions();

            using (var context = new CustomerManagerContext(options))
            {
                CustomersController controller = new CustomersController(context);
                CustomerDTO custTwo = new CustomerDTO
                {
                    Id = 2,
                    Name = "Jane Doe",
                    Address = "123 Sesame Street"
                };

                await controller.PostCustomer(custTwo);

                var result = await controller.GetAllCustomers();
                var resultActual = (List<Customer>)result.Value;
                Assert.True(resultActual.Count == 2);
            }
        }

        private DbContextOptions<CustomerManagerContext> getOptions()
        {
            return new DbContextOptionsBuilder<CustomerManagerContext>()
                .UseInMemoryDatabase(databaseName: "CustomerManager")
                .Options;
        }
    }
}
