using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Models
{
    public class Customer
    {
        public string Id { get; set; }

        public List<Account> Accounts { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }
    }
}
