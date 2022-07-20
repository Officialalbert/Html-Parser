using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var result = Parsing(url: "https://www.banki.ru/products/currency/usd/");
            if (result != null)
            {
                foreach (var item in result)
                {
                    Console.WriteLine("==========");
                    Console.WriteLine(item.Key);
                    Console.WriteLine("===========");
                }
            }
        }
        private static Dictionary<string, List<List<string>>> Parsing(string url)
        {
            try
            {
                Dictionary<string, List<List<string>>> result = new Dictionary<string, List<List<string>>>();
                using (HttpClientHandler hdl = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None })
                {
                    using (HttpClient client = new HttpClient(hdl)) // hdl чтобы передать значения
                    {
                        using (HttpResponseMessage response = client.GetAsync(url).Result) // получаем
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var html = response.Content.ReadAsStringAsync().Result;
                                if (!string.IsNullOrEmpty(html))
                                {
                                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                    doc.LoadHtml(html);
                                    var tables = doc.DocumentNode.SelectNodes(".//section[@class='widget']");
                                    if (tables != null && tables.Count > 0)
                                    {
                                        foreach (var table in tables)
                                        {
                                            var titleNode = table.SelectSingleNode(".//div[@class='currency-table']");
                                            if (titleNode != null)
                                            {
                                                var tbl = table.SelectSingleNode(".//table[@class='currency-table__table']");
                                                if (tbl != null)
                                                {
                                                    var aer = tbl.SelectSingleNode(".//thead");
                                                    if (aer != null)
                                                    {
                                                        var tb = aer.SelectSingleNode(".//tr");
                                                        if (tb != null)
                                                        {
                                                            var t = tb.SelectSingleNode(".//td[@class='currency-table__main-currency-col currency-table__title-first']");
                                                            if (t != null)
                                                            {
                                                                var Block = table.SelectSingleNode(".//tbody");
                                                                if (Block != null)
                                                                {
                                                                    var Bloc = Block.SelectSingleNode(".//tr[@class='currency-table__bordered-row']");
                                                                    if (Bloc != null)
                                                                    {
                                                                        var afez = Bloc.SelectSingleNode(".//td[@class='currency-table__large-text color-turquoise']");
                                                                        if (afez != null)
                                                                        {
                                                                            var Blo = Bloc.SelectNodes(".//td[@class='currency-table__rate currency-table__darken-bg']");
                                                                            if (Blo != null)
                                                                            {
                                                                                var res = new List<List<String>>();
                                                                                foreach (var Bl in Blo)
                                                                                {
                                                                                    var B = Bl.SelectNodes(".//div");
                                                                                    if (B != null && B.Count > 0)
                                                                                    {
                                                                                        res.Add(new List<string>(B.Select(x => x.InnerText)));
                                                                                    }
                                                                                    result[titleNode.InnerText] = res;
                                                                                }

                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }     
                                                    }
                                                }
                                            }

                                        }
                                        return result;
                                    }
                                    else
                                    {
                                        Console.WriteLine("tables == null ну накосячил в html");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("вот тут проблема");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }
}


