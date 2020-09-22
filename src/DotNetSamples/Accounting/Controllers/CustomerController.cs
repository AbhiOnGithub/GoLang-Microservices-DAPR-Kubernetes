using Accounting.Models;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Controllers
{
    /// <summary>
    /// Transaction controller.
    /// </summary>
    [ApiController]
    [Route("/api/customer/")]
    public class CustomerController : ControllerBase
    {
        public const string StoreName = "statestore";

        /// <summary>
        /// check account balance as specified by account id.
        /// </summary>
        /// <param name="account">Account information for the id from Dapr state store.</param>
        /// <returns>Account information.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> Get(string id, [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<Customer>(StoreName, id);
            if (state == null || state.Value == null)
            {
                return this.BadRequest($"Customer with Id {id} doesn't exists");
            }

            return state.Value;
        }

        /// <summary>
        /// add a new customer
        /// </summary>
        /// <param name="customer">Customer info.</param>
        /// <param name="daprClient">State client to interact with Dapr runtime.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        ///  "pubsub", the first parameter into the Topic attribute, is name of the default pub/sub configured by the Dapr CLI.
        [Topic("pubsub", "addcustomer")]
        [HttpPost]
        public async Task<ActionResult<Customer>> AddCustomer(Customer customer, [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<Customer>(StoreName, customer.Id);
            if (state !=null && state.Value != null)
            {
                return this.BadRequest($"Customer with Id {state.Value.Id} already exists");
            }
            state.Value = new Customer()
            {
                Id = customer.Id,
                Accounts = new List<Account>(), // Account will be assigned to this customer later
                Name = customer.Name,
                Address = customer.Address,
                Phone = customer.Phone
            };
            await state.SaveAsync();
            return state.Value;
        }

        /// <summary>
        /// add a new customer
        /// </summary>
        /// <param name="customer">Customer info.</param>
        /// <param name="daprClient">State client to interact with Dapr runtime.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        ///  "pubsub", the first parameter into the Topic attribute, is name of the default pub/sub configured by the Dapr CLI.
        [Topic("pubsub", "updatecustomer")]
        [HttpPut]
        public async Task<ActionResult<Customer>> UpdateCustomer(Customer customer, [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<Customer>(StoreName, customer.Id);
            if (state == null || state?.Value == null)
            {
                return this.BadRequest($"Customer with Id {customer.Id} doesn't exists");
            }
            else
            {
                //Ideally only these 3 details of a customer should be updated from here
                state.Value.Name = customer.Name;
                state.Value.Address = customer.Address;
                state.Value.Phone = customer.Phone;

                await state.SaveAsync();
                return state.Value;
            }
        }

        /// <summary>
        /// delete a existing customer
        /// </summary>
        /// <param name="customer">Customer info.</param>
        /// <param name="daprClient">State client to interact with Dapr runtime.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        ///  "pubsub", the first parameter into the Topic attribute, is name of the default pub/sub configured by the Dapr CLI.
        [Topic("pubsub", "deletecustomer")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteCustomer(string id, [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<Customer>(StoreName, id);
            if (state == null || state?.Value == null)
            {
                return this.BadRequest($"Customer with Id {id} doesn't exists");
            }
            else
            {
                if (state.Value.Accounts?.Count > 0)
                    return this.BadRequest($"First delete/close the {state.Value.Accounts.Count} accounts of Customer with Id {id}");
                else
                {    
                    await state.DeleteAsync();
                    return $"Deleted the Customer with Id {id}";
                }
            }
        }
    }
}