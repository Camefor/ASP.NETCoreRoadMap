using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HelloDotNet5 {
    public class Program {
        public static void Main (string[] args) {
            Person person = new ("Rhys", "Wang");
            Person person1 = new Person () {
                FirstName = "Andy",
                LastName = "Wang"
            };
            
            System.Console.WriteLine (person.FirstName);
            System.Console.WriteLine (person1.FirstName);
            System.Console.WriteLine (person.LastName);
            System.Console.WriteLine (person1.LastName);
            Console.ReadKey ();

            
            CreateHostBuilder (args).Build ().Run ();
        }

        public static IHostBuilder CreateHostBuilder (string[] args) =>
            Host.CreateDefaultBuilder (args)
            .ConfigureWebHostDefaults (webBuilder => {
                webBuilder.UseStartup<Startup> ();
            });
    }
}