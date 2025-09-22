using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AlertEmail.ConsoleApp
{
    internal class Program
    {
        static async Task Main()
        {
            Console.Title = "Alert Email Management";
            var baseUrl = ConfigurationManager.AppSettings["AlertApiUrl"] ?? "http://127.0.0.1:8086/";

            using (var http = new HttpClient())
            {
                http.Timeout = TimeSpan.FromSeconds(10);

                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("==== AlertEmail Menu ====");
                    Console.WriteLine("1) List all");
                    Console.WriteLine("2) Add");
                    Console.WriteLine("3) Update");
                    Console.WriteLine("4) Delete");
                    Console.WriteLine("0) Exit");
                    Console.Write("Choose: ");

                    var choice = Console.ReadLine()?.Trim();
                    Console.WriteLine();

                    try
                    {
                        switch (choice)
                        {
                            case "1":
                                await ListAsync(http, baseUrl);
                                break;

                            case "2":
                                Console.Write("Email to add: ");
                                var add = Console.ReadLine();
                                await PostJson(http, baseUrl + "alertEmail/add", new { Email = add });
                                Console.WriteLine("OK");
                                break;

                            case "3":
                                Console.Write("Old email: ");
                                var oldE = Console.ReadLine();
                                Console.Write("New email: ");
                                var newE = Console.ReadLine();
                                await PostJson(http, baseUrl + "alertEmail/update", new { OldEmail = oldE, NewEmail = newE });
                                Console.WriteLine("OK");
                                break;

                            case "4":
                                Console.Write("Email to delete: ");
                                var del = Console.ReadLine();
                                await PostJson(http, baseUrl + "alertEmail/delete", new { Email = del });
                                Console.WriteLine("OK");
                                break;

                            case "0":
                                return;

                            default:
                                Console.WriteLine("Invalid choice.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }

        private static async Task ListAsync(HttpClient http, string baseUrl)
        {
            var resp = await http.GetAsync(baseUrl + "alertEmail/list");
            var txt = await resp.Content.ReadAsStringAsync();
            resp.EnsureSuccessStatusCode();

            var parsed = JsonConvert.DeserializeObject<ListResult>(txt) ?? new ListResult();
            Console.WriteLine("Emails:");
            if (parsed.Items == null || parsed.Items.Length == 0)
            {
                Console.WriteLine("  (none)");
                return;
            }

            foreach (var e in parsed.Items)
                Console.WriteLine("  - " + e);
        }

        private static async Task PostJson(HttpClient http, string url, object payload)
        {
            var json = JsonConvert.SerializeObject(payload);
            var resp = await http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            var txt = await resp.Content.ReadAsStringAsync();
            resp.EnsureSuccessStatusCode();
        }

        private sealed class ListResult { public string[] Items { get; set; } = Array.Empty<string>(); }
    }
}
