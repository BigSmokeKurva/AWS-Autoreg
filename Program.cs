using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace AmazonAutoreg
{
    class Program
    {
        public static Dictionary<string, string> proxies = new() { };
        public static Dictionary<int, string> numProxies = new() { };
        public static Dictionary<int, string> cards = new() { };
        public static bool stop = false;
        static void ProxyParser()
        {
            // Открытие файла
            StreamReader txt = new StreamReader("proxies.txt");
            // Чтение строк
            var num = 0;
            foreach(var line in txt.ReadToEnd().Split("\n"))
            {
                // Добавление в словарь прокси:ссылка на смену ип
                var clearLine = line.Trim().Split("@");
                var proxy = $"--proxy-server=http://{clearLine[0]}";
                proxies.Add(proxy, clearLine[1]);
                numProxies.Add(num, proxy);
                num++;
            }
        }
        static void CardParser()
        {
            Console.WriteLine($"Всего {proxies.Keys.Count} прокси.");
            for(var i = 0; i < proxies.Keys.Count; i++)
            {
                Console.Write($"Поток {i}| введите карту: ");
                cards.Add(i, Console.ReadLine());
            }
        }
        static void Commands()
        {
            while (!stop)
            {
                switch (Console.ReadLine())
                {
                    case "stop":
                        Console.WriteLine();
                        stop = true;
                        break;
                }
            }
        }
        static public void WriteFile(string str)
        {
            StreamWriter sw = new StreamWriter(File.Open("accs.txt", FileMode.Append));
            sw.WriteLine(str);
            sw.Close();
        }
        async static Task Main(string[] args)
        {
            int num = int.Parse(args[0]);
            int year = int.Parse(args[1]);
            int mo = int.Parse(args[2]);
            string  card = args[3];
            string  proxy = args[4];
            await new BrowserThread().Start(num, year, mo, card, proxy);
            

        }
    }
}
