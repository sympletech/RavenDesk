using System;
using System.Collections.Generic;
using System.Linq;
using IntroToRaven.Models;
using Raven.Client.Indexes;
using Raven.Imports.Newtonsoft.Json;

namespace IntroToRaven
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var dContext = new DataContext())
            {
                //Include
                var cookieOrderInclude = dContext.Session
                    .Include<Order>(x => x.CustomerId)
                    .Load<Order>("orders/65");

                //Does Not Make a DB Call
                cookieOrderInclude.Customer = dContext.Session.Load<Customer>(cookieOrderInclude.CustomerId);

                var inspect = 1;
            }
        }
    }

    class Customer
    {
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }

        [JsonIgnore]
        public List<Order> Orders { get; set; }
    }

    class Order
    {
        [JsonIgnore]
        public Customer Customer { get; set; }
        public string CustomerId { get; set; }

        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; }
        
        public double OrderTotal { get {
            return this.Items.Sum(x => x.Qty * x.ItemCost);
        } }
    }

    public class OrderItem
    {
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public double ItemCost { get; set; }
    }
}
