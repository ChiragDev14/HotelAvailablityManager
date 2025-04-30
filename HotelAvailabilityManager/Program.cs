using HotelAvailabilityManager.Models;
using HotelAvailabilityManager.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;


class Program
{
    static List<Hotel> hotels;
    static List<Booking> bookings;

    static string hotelsPath = string.Empty;
    static string bookingsPath = string.Empty;


    static void Main(string[] args)
    {
        hotelsPath = GetArgumentValue(args, "--hotels");
        bookingsPath = GetArgumentValue(args, "--bookings");

        while (true)
        {
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) break;

            if (input.StartsWith("Availability"))
            {
                HandleAvailability(input);
            }
            else if (input.StartsWith("Search"))
            {
                HandleSearch(input);
            }
        }
    }

    static string GetArgumentValue(string[] args, string key)
    {
        int index = Array.IndexOf(args, key);
        if (index >= 0 && index + 1 < args.Length)
        {
            return args[index + 1];
        }
        throw new ArgumentException($"Missing argument: {key}");
    }

    static void HandleAvailability(string input)
    {
        var inside = input.Substring(input.IndexOf('(') + 1).TrimEnd(')').Split(',');
        string hotelId = inside[0].Trim();
        string roomType = inside[2].Trim();

        DateTime start, end;
        if (inside[1].Contains('-'))
        {
            var dates = inside[1].Split('-');
            start = DateTime.ParseExact(dates[0].Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
            end = DateTime.ParseExact(dates[1].Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
        }
        else
        {
            start = DateTime.ParseExact(inside[1].Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
            end = start.AddDays(1);
        }

        var serviceObj = new AvailabilityService(hotelsPath,bookingsPath);
        int available = serviceObj.GetAvailability(hotelId, roomType, start, end);
        Console.WriteLine(available);
    }

    static void HandleSearch(string input)
    {
        var inside = input.Substring(input.IndexOf('(') + 1).TrimEnd(')').Split(',');
        string hotelId = inside[0].Trim();
        int daysAhead = int.Parse(inside[1].Trim());
        string roomType = inside[2].Trim();

        var serviceObj = new AvailabilityService(hotelsPath, bookingsPath);
        List<string> results = serviceObj.SearchAvailability(hotelId, daysAhead, roomType, DateTime.Today);

        Console.WriteLine(string.Join(", ", results));
    }



}
