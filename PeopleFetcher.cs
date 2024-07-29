using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// dotnet add package Newtonsoft.Json
// ^ makes JSONs readable in C#
using Newtonsoft.Json.Linq;

namespace CatWorx.BadgeMaker
{
    class PeopleFetcher
    {
        // Returns a List of type "Employee" instances
        async public static Task<List<Employee>> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();

            // FIRST, ask if employee wants to type the data in manually
            Console.WriteLine("Would you like to automatically retrieve sample employee data? Enter \"y\" to generate 10 sample badges; enter \"n\" to enter data manually.");
            string autoOrManual = Console.ReadLine() ?? "";
            while (!String.Equals(autoOrManual, "y", StringComparison.OrdinalIgnoreCase) && !String.Equals(autoOrManual, "n", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Please enter \"y\" or \"n\" to proceed.");
                autoOrManual = Console.ReadLine() ?? "";
            }
            // if "y", call GetFromApi() & end method early
            if (String.Equals(autoOrManual, "y", StringComparison.OrdinalIgnoreCase))
            {
                employees = await GetFromApi();
                return employees;
            }

            // while loop
            // This will keep adding user inputs until the user enters nothing
            while (true)
            {
                // User inputs data for the new "Employee"
                Console.WriteLine("Enter first name (leave empty to exit): ");
                string firstName = Console.ReadLine() ?? "";
                if (firstName == "")
                {
                    break;
                }
                Console.Write("Enter last name: ");
                string lastName = Console.ReadLine() ?? "";

                Console.Write("Enter Company Name: ");
                string companyName = Console.ReadLine() ?? "";

                Console.Write("Enter ID: ");
                // Int32.Parse() - turns string into an int
                int id = Int32.Parse(Console.ReadLine() ?? "");

                // PLACEHOLDER IMAGE: https://placehold.co/400x400/png
                Console.Write("Enter Photo URL: ");
                string photoUrl = Console.ReadLine() ?? "";

                // user data initializes a new "Employee"
                Employee currentEmployee = new Employee(firstName, lastName, companyName, id, photoUrl);
                employees.Add(currentEmployee);
            }
            // This is important!
            return employees;
        }

        // Gets Employee data from an API; returns employee list
        async public static Task<List<Employee>> GetFromApi()
        {
            List<Employee> employees = new List<Employee>();

            using (HttpClient client = new HttpClient())
            {
                // obtains JSON data
                string response = await client.GetStringAsync("https://randomuser.me/api/?results=10&nat=us&inc=name,id,picture");
                JObject json = JObject.Parse(response);

                foreach (JToken token in json.SelectToken("results")!)
                {
                    // Parse JSON data (first name, last name, and picture link become strings; id value becomes int)
                    Employee emp = new Employee
                    (
                      token.SelectToken("name.first")!.ToString(),
                      token.SelectToken("name.last")!.ToString(),
                      "Cat Worx",
                      Int32.Parse(token.SelectToken("id.value")!.ToString().Replace("-", "")),
                      token.SelectToken("picture.large")!.ToString()
                    );
                    employees.Add(emp);
                }

                // Console.WriteLine(json.SelectToken("results[0]"));
                // Console.WriteLine(json.SelectToken("results[0].name.first"));
            }
            return employees;
        }
    }
}