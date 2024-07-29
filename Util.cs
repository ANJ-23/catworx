using System;
using System.IO;
using System.Collections.Generic;

// async libraries
using System.Net.Http;
using System.Threading.Tasks;

// dotnet add package SkiaSharp --version 2.88.0
using SkiaSharp;

namespace CatWorx.BadgeMaker
{
    class Util
    {
        // Takes a List of "Employee" instances & prints out all the "employees"
        // "static" = no need to create a class object to call method (ex. Util.PrintEmployees();)
        public static void PrintEmployees(List<Employee> employees)
        {
            for (int i = 0; i < employees.Count; i++)
            {
                // below is a template literal
                // {0}  = left-aligned & padded to at least 10 characters
                // \t   = Tab spacing
                // {1}  = left-aligned & padded to 20 characters
                // {2}  = last argument, no formatting
                string template = "{0,-10}\t{1,-20}\t{2, -20}\t{3}";
                Console.WriteLine(String.Format(template, employees[i].GetId(), employees[i].GetFullName(), employees[i].GetCompanyName(), employees[i].GetPhotoUrl()));
            }
        }

        public static void MakeCSV(List<Employee> employees)
        {
            // Check to see if folder exists
            // If not, create it
            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }

            // "using () { ..." = whatever is in the parentheses is deleted after it's run; ensures it only runs when called & saves memory
            // Writes down employee info in a .CSV file
            using (StreamWriter file = new StreamWriter("data/employees.csv"))
            {
                file.WriteLine("ID,Name,CompanyName,PhotoUrl");

                // Loop over employees
                for (int i = 0; i < employees.Count; i++)
                {
                    // Write each employee to the file; calls methods in "Employee" class
                    string template = "{0},{1},{2},{3}";
                    file.WriteLine(String.Format(template, employees[i].GetId(), employees[i].GetFullName(), employees[i].GetCompanyName(), employees[i].GetPhotoUrl()));
                }
            }
            // (?) can also use:
            /* 
            using StreamWriter file = new StreamWriter("data/employees.csv");
            file.WriteLine("ID,Name,PhotoUrl");
            */
        }

        // Uses SKImage and HttpClient to make badges
        async public static Task MakeBadges(List<Employee> employees)
        {
            // Layout variables
            int BADGE_WIDTH = 669;
            int BADGE_HEIGHT = 1044;
            
            int PHOTO_LEFT_X = 184;
            int PHOTO_TOP_Y = 215;
            int PHOTO_RIGHT_X = 486;
            int PHOTO_BOTTOM_Y = 517;

            int COMPANY_NAME_Y = 150;

            int EMPLOYEE_NAME_Y = 600;
            int EMPLOYEE_ID_Y = 730;

            SKPaint paint = new SKPaint();
            paint.TextSize = 42.0f;
            paint.IsAntialias = true;
            paint.Color = SKColors.White;
            paint.IsStroke = false;
            paint.TextAlign = SKTextAlign.Center;
            paint.Typeface = SKTypeface.FromFamilyName("Arial");

            // instance of HttpClient is disposed after code in the block has run
            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < employees.Count; i++)
                {
                    // Reads image, then creates badge
                    SKImage photo = SKImage.FromEncodedData(await client.GetStreamAsync(employees[i].GetPhotoUrl()));
                    SKImage background = SKImage.FromEncodedData(File.OpenRead("badge.png"));

                    // sets Width & Height of badge. SKCanvas is required to modify the Bitmap.
                    SKBitmap badge = new SKBitmap(BADGE_WIDTH, BADGE_HEIGHT);
                    SKCanvas canvas = new SKCanvas(badge);

                    canvas.DrawImage(background, new SKRect(0, 0, BADGE_WIDTH, BADGE_HEIGHT));
                    canvas.DrawImage(photo, new SKRect(PHOTO_LEFT_X, PHOTO_TOP_Y, PHOTO_RIGHT_X, PHOTO_BOTTOM_Y));

                    // Company name
                    paint.Color = SKColors.White;
                    canvas.DrawText(employees[i].GetCompanyName(), BADGE_WIDTH / 2f, COMPANY_NAME_Y, paint);

                    // Employee name
                    paint.Color = SKColors.Black;
                    canvas.DrawText(employees[i].GetFullName(), BADGE_WIDTH / 2f, EMPLOYEE_NAME_Y, paint);
                    
                    // Employee ID
                    paint.Typeface = SKTypeface.FromFamilyName("Courier New");
                    canvas.DrawText(employees[i].GetId().ToString(), BADGE_WIDTH / 2f, EMPLOYEE_ID_Y, paint);

                    SKImage finalImage = SKImage.FromBitmap(badge);
                    SKData data = finalImage.Encode();
                    string template = "data/{0}_badge.png";
                    data.SaveTo(File.OpenWrite(string.Format(template, employees[i].GetId())));
                }
            };
        }
    }
}