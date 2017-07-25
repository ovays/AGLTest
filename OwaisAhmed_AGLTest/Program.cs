using Newtonsoft.Json;
using System;
using System.Linq;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace OwaisAhmed_AGLTest
{
    class Program
    {
        static async void ApiCall()
        {
            //initializing httpClient
            using (var client = new HttpClient())
            {
                try
                {
                    //reading values from the AppSettings 
                    var url = ConfigurationSettings.AppSettings["ApiUrl"];
                    var key = ConfigurationSettings.AppSettings["SearchKey"];

                    //Checking the status code
                    HttpResponseMessage response = await client.GetAsync(url);                
                    response.EnsureSuccessStatusCode();

                    //reading response content
                    using (HttpContent content = response.Content)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                      
                        //deserializing json
                        Person[] personList = JsonConvert.DeserializeObject<Person[]>(responseBody);

                        //finding cats by gender type
                        var catsByGender = personList.Where(pet => pet.pets != null)
                                     .GroupBy(person => person.gender)
                                     .Select(g => new
                                     {
                                         personGender = g.Key,
                                         personCats = g.SelectMany(pet => pet.pets)
                                                    .Where(animal => animal.type == key)
                                                    .OrderBy(person => person.name)
                                                    .ToArray()
                                     });

                        //printing values
                        foreach (var gender in catsByGender)
                        {
                            Console.WriteLine(gender.personGender);
                            foreach (var person in gender.personCats)
                            {
                                Console.WriteLine("*" + person.name);
                            }
                            Console.WriteLine();
                        }
                    }

                }
                //handling exception
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        static void Main(string[] args)
        {
            Task T = new Task(ApiCall);
            T.Start();         
            Console.ReadLine();
        }
    }
}
