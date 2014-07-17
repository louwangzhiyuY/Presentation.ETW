using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GC
{
    internal class Program
    {
        private const int Iterations = 25*1000*1000;
        private const int Clear = 1000*1000;

        private static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            CreateCustomers();
            
            stopwatch.Stop();
            Console.WriteLine("Execution time: " + stopwatch.Elapsed.ToString());
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CreateCustomers()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < Iterations; i++)
            {
                Customer customer = new Customer
                {
                    Name = "Customer " + i.ToString(),
                    Id = i
                };
                customers.Add(customer);
                if (0 == i % Clear)
                {
                    customers.Clear();
                }
            }
        }

        public class Customer
        {
            public String Name { get; set; }
            public int Id { get; set; }
        }
    }
}