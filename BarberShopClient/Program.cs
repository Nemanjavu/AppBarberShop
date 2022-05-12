using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace BarberShopClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        public static async Task RunAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://appbarbershop20220511222004.azurewebsites.net/Booking/");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //test get free meetings rooms on 15/05/2022 from 09:00 to 10:00
                    Console.WriteLine("List of free Barbers on 15/05/2022 from 9am to 10am");

                    HttpResponseMessage response = await client.GetAsync("GetAvailableRooms?_date=20220515000000&_startTime=20220609090001&_endTime=20220609100000");
                    if (response.IsSuccessStatusCode)
                    {
                        var barbers = await response.Content.ReadAsAsync<IEnumerable<Barber>>();
                        if (barbers == null)
                        {
                            Console.WriteLine("Sorry, there is no barber free on those date and times.");
                        }
                        else
                        {
                            foreach (var barber in barbers)
                            {
                                Console.WriteLine(barber);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadKey();
        }
    }
}
