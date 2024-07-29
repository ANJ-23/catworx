using System;
using System.IO;
using System.Collections.Generic;

// async library
using System.Net.Http;
using System.Threading.Tasks;

// dotnet add package SkiaSharp --version 2.88.0
using SkiaSharp;

/* 
Feature ideas:
- upload images to online service (ex. Google Drive, Imgur)
- save employee data to database (ex. MySQL, MongoDB)
*/

namespace CatWorx.BadgeMaker
{
  class Employee
  {
    // public = variable/instance is accessible ANYWHERE
    // To distinguish between public and private variables, use PascalCase for public variables and camelCase for private variables.
    public string FirstName;
    public string LastName;
    public int Id;
    public string PhotoUrl;
    public string CompanyName;

    // Constructor - instances of "Employee" class requires a string argument (firstName)
    public Employee(string firstName, string lastName, string companyName, int id, string photoUrl)
    {
      FirstName = firstName;
      LastName = lastName;
      CompanyName = companyName;
      Id = id;
      PhotoUrl = photoUrl;
    }
  // Simpler constructor:
  /* 
  class Employee(string firstName, string lastName, int id, string photoUrl) {
    public string FirstName = firstName;
    public string LastName = lastName;
    public int Id = id;
    public string PhotoUrl = photoUrl;
    // ...
  }
  */

    // method that returns string (a full name)
    public string GetFullName() {
      return FirstName + " " + LastName;
    }

    // returns company name
    public string GetCompanyName()
    {
      return CompanyName;
    }

    // returns ID number
    public int GetId() {
        return Id;
    }

    // returns photo URL
    public string GetPhotoUrl() {
        return PhotoUrl;
    }
  }


  class Program
  {
    async static Task Main(string[] args)
    {
      // List<string> employees = new List<string>();
      // Console.WriteLine("Please enter a name: ");

      // Get a name from the console and assign it to a variable
      // (?? "") is a default (an "else" statement if user doesn't input anything)
      /* string input = Console.ReadLine() ?? "";
      employees.Add(input);
      for (int i = 0; i < employees.Count; i++) 
      {
        Console.WriteLine(employees[i]);
      } */

      // This is our employee-getting code now ("employees" adds names until user enters nothing, then prints employees)
      // Variable type must match function's
      List<Employee> employees = await PeopleFetcher.GetEmployees();
      Util.PrintEmployees(employees);
      Util.MakeCSV(employees);
      await Util.MakeBadges(employees);
    }
  }
}