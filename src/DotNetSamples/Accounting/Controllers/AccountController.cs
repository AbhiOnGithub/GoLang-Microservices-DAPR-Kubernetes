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
    [Route("/api/account/")]
    public class AccountController : ControllerBase
    {
        public const string StoreName = "statestore";

        /// <summary>
        /// add a new customer account
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="daprClient">State client to interact with Dapr runtime.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        ///  "pubsub", the first parameter into the Topic attribute, is name of the default pub/sub configured by the Dapr CLI.
        [Topic("pubsub", "addaccount")]
        [HttpPost]
        public async Task<ActionResult<Customer>> AddCustomerAccount(Customer customer, [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<Customer>(StoreName, customer.Id);
            if (state == null || state.Value == null)
            {
                return this.BadRequest($"Customer with Id {customer.Id} doesn't exists");
            }
            
            if(state.Value.Accounts == null)
            {
                state.Value.Accounts = new List<Account>();
            }
            state.Value.Accounts.Add(new Account(){ Id = $"ac-{state.Value.Accounts.Count}-{customer.Id}",Balance = 0 });
            await state.SaveAsync();
            return state.Value;
        }
    }
}