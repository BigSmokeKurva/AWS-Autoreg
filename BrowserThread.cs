using Microsoft.Playwright;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AmazonAutoreg
{
    class BrowserThread
    {
        //static IPlaywright playwright;
        static IBrowser browser;
        static IBrowserContext context;

        static PageWaitForLoadStateOptions pageWaitOption = new() { Timeout = 60000 };
        static PageWaitForSelectorOptions waitSelectorOptions = new() { Timeout = (60*1000)*60 };
        static PageGotoOptions gotoOptions = new() { Timeout = 60000 };

        static string smshubApiKey = "82349Ue6b7c136067ec23d6b26923401b49a6e";

        static bool stop = false;
        static string proxy;
        static int year;
        static int mo;

        static WebClient webClient = new();

        static Random rnd = new();
        static ViewportSize vSize;

        static int num;
        static string email;
        static string password;
        static string login;
        static string name;
        static string phone;
        static string index;
        static string region;
        static string city;
        static string street;
        static string appart;
        static string card;
        static string cardName;
        static string userAgent;

        static void UserDate()
        {
            using var process = new Process();
            // Создание процесса
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "111.cmd",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            // Запуск парсера
            process.Start();
            process.WaitForExit();
            // Получение вывода
            var result = process.StandardOutput.ReadToEnd().Trim('\n').Trim('\r');
            // Разделение по строчкам
            var date = result.Split("\n");
            // Присвоение переменных
            email = date[2].Trim('\n').Trim('\r');
            password = date[3].Trim('\n').Trim('\r');
            login = date[4].Trim('\n').Trim('\r');
            name = date[5].Trim('\n').Trim('\r');
            cardName = date[6].Trim('\n').Trim('\r');
            userAgent = date[7].Trim('\n').Trim('\r');
        }
        async static Task Whoer()
        {
            // Создание страницы 
            var page = await context.NewPageAsync();
            // Переход на страницу
            await page.GotoAsync("https://whoer.net/ru", gotoOptions);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, pageWaitOption);

            await page.WaitForSelectorAsync("#main > section.section_main.section_user-ip.section > div > div > div > div.main-ip-info__anonimity > div > div.level__bg.completed_1.completed_2.completed_3.completed_4.completed_5.completed_6.completed");
            // Если с анонимностью все гуд =100
            if ((await (await page.QuerySelectorAsync("#hidden_rating_link")).TextContentAsync()).Contains("100"))
            {
                // Получение индекса для карт
                index = await (await page.QuerySelectorAsync("div.tab:nth-child(3) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > div:nth-child(4) > div:nth-child(2) > span:nth-child(1)")).TextContentAsync();
                return;
            }
            // <100
            else
            {
                // Перезапуск браузера
                throw new TimeoutException();
            }
        }
        async static Task Indexphone()
        {
            // Рандом
            var rnd = new Random();
            // Создание страницы
            var page = await context.NewPageAsync();
            // Переход на страницу
            try
            {
                await page.GotoAsync($"https://indexphone.ru/post-offices/{index}", gotoOptions);
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, pageWaitOption);
            }
            catch (PlaywrightException)
            {
                try
                {
                    await page.GotoAsync($"https://indexphone.ru/post-offices/{index}", gotoOptions);
                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, pageWaitOption);
                }
                catch (PlaywrightException)
                {
                    throw new TimeoutException();
                }
            }
            catch (TimeoutException) { }
            // Область и город
            var resp = await page.QuerySelectorAsync("div.post-offices-item-content:nth-child(8)");
            // Проверка ответа на null
            if (resp is null)
            {
                // Перезапуск браузера
                throw new TimeoutException();
            }
            var postalAddress = (await resp.TextContentAsync()).Split(",");
            var addressLocal = $"{postalAddress[0]}, {(postalAddress.Length == 3 ? (postalAddress[0] + "регион") : postalAddress[1])}";
            // Улица 
            resp = await page.QuerySelectorAsync("body > div > div > main > div > ul:nth-child(21)");
            // Проверка ответа на null 
            if(resp is null)
            {
                throw new TimeoutException();
            }
            var streets = (await resp.TextContentAsync()).Split(")");
            List<string> clearStreets = new() { };
            foreach(var i in streets)
            {
                var street = i.Split("(");
                if(street.Length < 2) { continue; }
                if (street[1].Contains("ул"))
                {
                    clearStreets.Add(street[0].Trim());
                }
            }
            if (clearStreets.Count < 1) { throw new TimeoutException(); }
            // Регион, город, улица
            addressLocal = $"{addressLocal}, {clearStreets[rnd.Next(0, clearStreets.Count - 1)]} улица".Trim();
            // Регион, город, улица, дом
            addressLocal = $"{addressLocal}, {rnd.Next(1, 20)}";
            // Регион, город, улица, дом, квартира
            addressLocal = $"{addressLocal}, {rnd.Next(1, 200)}";
            // Создание новой вкладки для перевода
            page = await context.NewPageAsync();
            // Переход на страницу
            await page.GotoAsync($"https://translate.google.com/?sl=auto&tl=en&text={addressLocal}&op=translate", gotoOptions);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, pageWaitOption);
            // Перевод .JLqJ4b > span:nth-child(1)
            // #yDmH0d > c-wiz > div > div.WFnNle > c-wiz > div.OlSOob > c-wiz > div.ccvoYb > div.AxqVh > div.OPPzxe > c-wiz.P6w8m.BDJ8fb.BLojaf > div.dePhmb > div > div.J0lOec > span.VIiyi > span > span
            // .JLqJ4b > span:nth-child(1)
            addressLocal = await (await page.WaitForSelectorAsync(".JLqJ4b > span:nth-child(1)")).TextContentAsync();
            Console.WriteLine(addressLocal);
            // Получение массива с переменными
            var addressList = addressLocal.Split(",");
            // Регион
            region = addressList[0].Trim();
            // Город
            city = addressList[1].Trim();
            // Улица
            street = $"{addressList[2].Trim()}, {addressList[3].Trim()}";
            // Квартира
            appart = addressList[4].Trim();
        }
        async static Task<bool> SetTimezone()
        {
            // Создание страницы 
            var page = await context.NewPageAsync();
            // Смена прокси
            while (true)
            {
                try
                {
                    dynamic json = JsonConvert.DeserializeObject(webClient.DownloadString(proxy.Split("@")[1]));
                    if (!(json.IP is null))
                    {
                        break;
                    }
                }
                catch { await Task.Delay(2000); }
            }
            // Переход на страницу
            await page.GotoAsync("https://whoer.net/ru", gotoOptions);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, pageWaitOption);
            // Ждем полной проверки
            await page.WaitForSelectorAsync("#main > section.section_main.section_user-ip.section > div > div > div > div.main-ip-info__anonimity > div > div.level__bg.completed_1.completed_2.completed_3.completed_4.completed_5.completed_6.completed");
            // Если с анонимностью все гуд =100
            if ((await (await page.QuerySelectorAsync("#hidden_rating_link")).TextContentAsync()).Contains("100"))
            {
                // Получение индекса для карт
                index = await (await page.QuerySelectorAsync("div.tab:nth-child(3) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > div:nth-child(4) > div:nth-child(2) > span:nth-child(1)")).TextContentAsync();
                return false;
            }
            // Если с анонимностью плохо, не поможет таймзона
            else if((await (await page.QuerySelectorAsync("#hidden_rating_link")).TextContentAsync()).Replace("%", "").Split(":")[1].Trim() != "90")
            {
                throw new TimeoutException();
            }
            // Получаем таймзону
            var timezone = await (await page.QuerySelectorAsync("div.tab:nth-child(3) > div:nth-child(1) > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(4) > div:nth-child(1) > div:nth-child(2)")).TextContentAsync();
            // Создаем настройки контекста
            var contextOptions = new BrowserNewContextOptions()
            {
                TimezoneId = timezone,
                UserAgent = userAgent,
                AcceptDownloads = true,
                ColorScheme = rnd.Next(0, 1) == 0 ? ColorScheme.Dark : ColorScheme.Light,
                ViewportSize = vSize
            };
            // Закрываем старый контекст
            await context.CloseAsync();
            // Создаем новый
            context = await browser.NewContextAsync(contextOptions);
            return true;
        }
        async static Task Register()
        {
            string[] searchRefers =
            {
                // Гугл
                "https://www.google.com/",
                // Yandex
                "https://yandex.ru/"
            };
            var refer = searchRefers[new Random().Next(0, searchRefers.Length - 1)];
            try
            {
                Console.WriteLine(1);
                // Получаем номер
                var response = webClient.DownloadString($"https://smshub.org/stubs/handler_api.php?api_key={smshubApiKey}&action=getNumber&service=am&country=russia").Split(":");
                // Получен ли номер
                if (response[0] != "ACCESS_NUMBER")
                {
                    Console.WriteLine($"Поток {num}| ошибка получения номера");
                    stop = true;
                    return;
                }
                // Записываем в переменную
                phone = response[2];
                // Номер операции
                string id = response[1];
                // Создаем новую вкладку
                var page = await context.NewPageAsync();
                // Переход на страницу
                await page.GotoAsync("https://aws.amazon.com/", new PageGotoOptions() { Referer = refer});
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, pageWaitOption);
                // Переход на страницу регистрации
                await page.ClickAsync("#m-nav > div.m-nav-header.lb-clearfix.m-nav-double-row > div.m-nav-secondary-links > div > div");
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, pageWaitOption);
                // Ждем загрузки
                for(var i = 0; i <= 3; i++)
                {
                    try
                    {
                        await page.WaitForSelectorAsync("#awsui-input-0");
                        break;
                    }
                    catch { await page.GotoAsync(page.Url); }
                    if (i == 3) { throw new TimeoutException(); }
                }
                
                // Ввод почты
                await page.TypeAsync("#awsui-input-0", email, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000)});
                // Ввод пароля
                await page.TypeAsync("#awsui-input-1", password, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Повтор пароля
                await page.TypeAsync("#awsui-input-2", password, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Ввод логина
                await page.TypeAsync("#awsui-input-3", login, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Переход к следующей ступени
                await page.ClickAsync(".awsui-button");
                // Ожидание возможной капчи
                await Task.Delay(6000);
                // Проверка капчи
                if (!((await page.QuerySelectorAsync("#CredentialCollection > fieldset > awsui-form-field:nth-child(7) > div > label")) is null))
                {
                    Console.WriteLine($"Поток {num}| капча 1");
                    //Console.ReadKey();
                }
                // Ожидание элементов
                await page.WaitForSelectorAsync("#awsui-radio-button-2", waitSelectorOptions);
                // Выставление типа аккаунта
                await page.ClickAsync("#awsui-radio-button-2");
                // Страна
                await page.ClickAsync(@"#address\.country");
                await page.ClickAsync("#awsui-select-1-dropdown-option-179");
                // ФИО
                await page.TypeAsync(@"#address\.fullName > div > input", name, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Телефон
                await page.TypeAsync(@"#address\.phoneNumber > div > input", $"+{phone}", new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Адрес
                await page.TypeAsync(@"#address\.addressLine1 > div > input", street, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Квартира
                await page.TypeAsync(@"#address\.addressLine2 > div > input", appart, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Город 
                await page.TypeAsync(@"#address\.city > div > input", city, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Регион 
                await page.TypeAsync(@"#address\.state > div > input", region, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Индекс 
                await page.TypeAsync(@"#address\.postalCode > div > input", index, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Условия 
                await page.ClickAsync("#awsui-checkbox-0");
                // Следующий этап
                await page.ClickAsync("#ContactInformation > fieldset > awsui-button > button");
                // Ожидание загрузки элементов
                await page.WaitForSelectorAsync("#accountHolderName > div > input");
                // Действие карты 
                await page.ClickAsync("#awsui-select-2-textbox");
                await page.ClickAsync($"#awsui-select-2-dropdown-option-{mo}");
                await page.ClickAsync("#awsui-select-3-textbox");
                await page.ClickAsync($"#awsui-select-3-dropdown-option-{year}");
                // Номер карты
                await page.TypeAsync("#cardNumber > div > input", card, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                await page.TypeAsync("#accountHolderName > div > input", cardName, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Следующий этап
                await page.ClickAsync(".awsui-button-variant-primary");
                // Ожидание элементов
                try { await page.WaitForSelectorAsync("#phoneNumber"); }
                catch
                {
                    stop = true;
                    return;
                }
                // Запись данных
                Program.WriteFile($"{proxy.Split("@")[0]}\n{email}\n{password}\n---");
                // Ext проверка
                while (!((await page.QuerySelectorAsync("#ext")) is null))
                {
                    await Task.Delay(2000);
                    await page.GotoAsync("https://portal.aws.amazon.com/billing/signup#/identityverification");
                    try { await page.WaitForSelectorAsync("#phoneNumber"); }
                    catch
                    {
                        stop = true;
                        return;
                    }
                }
                // Код страны
                for(var i = 0; i < 5; i++)
                {
                    try
                    {
                        await page.ClickAsync(@"#country");
                        await page.ClickAsync("#awsui-select-1-dropdown-option-179");
                    }
                    catch
                    {
                        await page.GotoAsync("https://portal.aws.amazon.com/billing/signup#/identityverification");
                        try { await page.WaitForSelectorAsync("#phoneNumber"); }
                        catch
                        {
                            stop = true;
                            return;
                        }
                    }
                }

                // Ввод номера
                await page.TypeAsync("#phoneNumber > div > input", phone.Remove(phone.IndexOf("7"), 1), new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                // Ожидание когда будет введена капча
                Console.WriteLine($"Поток {num}| капча 2");
                await page.WaitForSelectorAsync("#smsPin", waitSelectorOptions);
                var next = false;
                // Получение смс
                while (!next)
                {
                    // Нужно ли запросить еще один номер
                    if (id.Length == 1)
                    {
                        response = webClient.DownloadString($"https://smshub.org/stubs/handler_api.php?api_key={smshubApiKey}&action=getNumber&service=am&country=russia").Split(":");
                        // Получен ли номер
                        if (response[0] != "ACCESS_NUMBER")
                        {
                            Console.WriteLine($"Поток {num}| ошибка получения номера");
                            stop = true;
                            return;
                        }
                        // Записываем в переменную
                        phone = response[2];
                        // Номер операции
                        id = response[1];
                        // Переход на страницу ввода телефона TODO
                        await page.GotoAsync("https://portal.aws.amazon.com/billing/signup#/identityverification");
                        try { await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded); }
                        catch
                        {
                            stop = true;
                            return;
                        }
                        // Ожидание элементов
                        try { await page.WaitForSelectorAsync("#phoneNumber"); }
                        catch
                        {
                            stop = true;
                            return;
                        }
                        // Код страны
                        for (var i = 0; i < 5; i++)
                        {
                            try
                            {
                                await page.ClickAsync(@"#country");
                                await page.ClickAsync("#awsui-select-1-dropdown-option-179");
                            }
                            catch
                            {
                                await page.GotoAsync("https://portal.aws.amazon.com/billing/signup#/identityverification");
                                try { await page.WaitForSelectorAsync("#phoneNumber"); }
                                catch
                                {
                                    stop = true;
                                    return;
                                }
                            }
                        }
                        // Ввод номера TODO
                        await page.TypeAsync("#phoneNumber > div > input", phone.Remove(phone.IndexOf("7"), 1), new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                        // Ожидание когда будет введена капча
                        Console.WriteLine($"Поток {num}| капча 2");
                        await page.WaitForSelectorAsync("#smsPin", waitSelectorOptions);
                    }
                    // Получение смс
                    for (var attempt = 0; attempt <= 6; attempt++)
                    {
                        // Ждем 20 секунд
                        await Task.Delay(20000);
                        response = webClient.DownloadString($"https://smshub.org/stubs/handler_api.php?api_key={smshubApiKey}&action=getStatus&id={id}").Split(":");
                        foreach (var i in response) { Console.WriteLine(i); }
                        Console.WriteLine(response);
                        // Сообщение не полученно
                        if (response[0] != "STATUS_OK")
                        {
                            Console.WriteLine("bad");
                            // Если 6 попыток
                            if (attempt == 3) { id = "1"; }
                            continue;
                        }
                        // Ввод смс TODO
                        Console.WriteLine(response[1]);
                        await page.TypeAsync("#smsPin > div > input", response[1], new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });
                        // Закрыть цикл while
                        next = true;
                        // Закрыть цикл for
                        break;
                    }
                }
                // Переход к следующему этапу
                await page.ClickAsync("#IdentityVerificationSmsPin > fieldset > awsui-button > button");

                // Выбор плана, нажатие кнопки
                try { await page.WaitForSelectorAsync("#SupportPlan > fieldset > div > div"); }
                catch
                {
                    stop = true;
                    return;
                }
                try
                {
                    await page.ClickAsync("#SupportPlan > fieldset > div > awsui-button");
                }
                catch { }
                // Go to AWS management console
                try { await page.WaitForSelectorAsync("#aws-signup-app > div > div > div > div > h1"); }
                catch
                {
                    stop = true;
                    return;
                }
                
                for(var i = 0; i <= 3; i++)
                {
                    try 
                    {
                        await page.GotoAsync("https://console.aws.amazon.com/console/home");
                        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                        break;
                    }
                    catch { }
                    if (i == 0)
                    {
                        stop = true;
                        return;
                    }
                }
                // Ввод логина
                try { await page.WaitForSelectorAsync("#resolving_input_label"); }
                catch
                {
                    stop = true;
                    return;
                }
                await page.TypeAsync("#resolving_input", email, new PageTypeOptions() { Timeout=0, Delay=rnd.Next(100, 1000) });

                // Переход к паролю
                await page.ClickAsync("#next_button");

                // Ввод капчи и ожидание перехода на страницу пароля
                Console.WriteLine($"Поток {num}| капча 3");
                await page.WaitForSelectorAsync("#email_label", waitSelectorOptions);

                // Ввод пароля
                await page.TypeAsync("#password", password);

                // Авторизация
                await page.ClickAsync("#signin_button");

                Console.WriteLine($"Поток {num}| ожидание команды stop...");
                // Ждем команду stop
                while (!Program.stop)
                {
                    await Task.Delay(1000);
                    // Если команда введена
                    if (Program.stop)
                    {
                        stop = true;
                        return;
                    }
                }
            }
            catch
            {
                stop = true;
                return;
            }
            stop = true;
        }
        async static Task РeatingСookies()
        {
            // Хостинги
            string[] hostings = { "https://www.vultr.com/", "https://www.linode.com/", "https://www.digitalocean.com/", "https://azure.microsoft.com/ru-ru/", "https://mcs.mail.ru/cloud-servers/", "https://sbercloud.ru/ru", "https://russian.alibaba.com/", "https://www.win-vps.com/index.php", "https://www.smarterasp.net/index", "https://www.reg.ru/dedicated/", "https://firstvds.ru/", "https://www.ip-way.ru/uslugi/arenda-udalennogo-servera", "https://www.xelent.ru/services/dedicated/", "https://uguide.ru/rejting-luchshie-vydelennye-servera-dedicated", "http://gotw.ru/", "https://hostinghub.ru/top/dedicated-server", "https://selectel.ru/", "https://zomro.com/dedicated.html" };
            // Социальные сети
            string[] socNetworks = { "https://vk.com/", "https://ru-ru.facebook.com/", "https://www.reddit.com", "https://twitter.com/", "https://www.instagram.com/", "https://badoo.com/ru/", "https://www.snapchat.com/", "https://www.sberbank.com/ru/eco/sbercloud" };
            // Поисковые запросы
            string[] searchQueries = { "pornhub", "как сделать vps", "vds", "best vps hostings", "aws good hosting?", "hostings", "vk", "facebook", "хостинги", "aws", "amazon aws", "azure", "zomro", "digitalocean", "linoid", "amerecan express", ""};
            // Поисковые системы
            string[] searchRefers =
            {
                // Гугл
                "https://www.google.com/",
                "https://yandex.ru/"
            };
            var page = await context.NewPageAsync();

            // Поисковые запросы
            var count = rnd.Next(1, 3);
            List<string> last = new() { };
            for (var i = 0; i <= count; i++)
            {
                while (true)
                {
                    Console.WriteLine(1);
                    // Переход по ссылке
                    var str = searchQueries[rnd.Next(0, searchQueries.Length - 1)];
                    if(!last.Contains(str))
                    {
                        try
                        {
                            await page.GotoAsync((rnd.Next(0, 2) == 0 ? "https://yandex.ru/search/?text=" : "https://www.google.com/search?q=") + str);
                            // Ожидание загрузки
                            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                        }
                        catch { }
                        await Task.Delay(1000);
                        last.Add(str);
                        break;
                    }
                }
            }
            last.Clear();
            // Соц сети
            count = 1;
            for (var i = 0; i <= count; i++)
            {
                while (true)
                {
                    Console.WriteLine(1);
                    // Переход по ссылке
                    var str = socNetworks[rnd.Next(0, socNetworks.Length - 1)];
                    if (!last.Contains(str))
                    {
                        try
                        {
                            await page.GotoAsync(str,
                                new PageGotoOptions()
                                {
                                    // Редирект
                                    Referer = searchRefers[rnd.Next(0, searchRefers.Length - 1)]
                                });
                            // Ожидание загрузки
                            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                        }
                        catch { }
                        await Task.Delay(1000);
                        last.Add(str);
                        break;
                    }
                }
            }
            last.Clear();
            // Хостинги
            count = rnd.Next(1, 2);
            for(var i = 0; i <= count; i++)
            {
                while (true)
                {
                    Console.WriteLine(1);
                    // Переход по ссылке
                    var str = hostings[rnd.Next(0, hostings.Length - 1)];
                    if (!last.Contains(str))
                    {
                        try
                        {
                            await page.GotoAsync(str,
                                new PageGotoOptions()
                                {
                                    // Редирект
                                    Referer = searchRefers[rnd.Next(0, searchRefers.Length - 1)]
                                });
                            // Ожидание загрузки
                            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                        }
                        catch { }
                        await Task.Delay(1000);
                        last.Add(str);
                        break;
                    }
                }
            }
            Console.WriteLine((await context.CookiesAsync()).Count);
        }
        async public Task Start(int numT, int yearL, int moL, string cardLocal, string proxyL)
        {
            // Размер окна браузера
            switch (rnd.Next(0, 3))
            {
                case 0:
                    vSize = new() { Height = 760, Width = 1280 };
                    break;
                case 1:
                    vSize = new() { Height = 933, Width = 966 };
                    break;
                case 2:
                    vSize = new() { Height = 800, Width = 1670 };
                    break;
                case 3:
                    vSize = new() { Height = 900, Width = 1650 };
                    break;
            }
            // Присвоение номера классу
            num = numT;
            // Получение карты
            card = cardLocal;
            proxy = proxyL;
            year = yearL;
            mo = moL;
            // Playwright
            using var playwright = await Playwright.CreateAsync();
            while (!stop || !Program.stop)
            {
                try
                {
                    // Получение данных человека
                    UserDate();
                    // Настройки браузера
                    BrowserTypeLaunchOptions options = new()
                    {
                        Args = new string[] {
                            "--incognito"
                        },
                        Headless = false,
                        Proxy = new Proxy() { Server = proxy.Split("@")[0] },
                        ExecutablePath= @"win\chrome.exe",
                        DownloadsPath= @"C:\Users\vasya\Downloads"
                    };
                    var contextOptions = new BrowserNewContextOptions()
                    {
                        UserAgent = userAgent,
                        AcceptDownloads = true,
                        ColorScheme = rnd.Next(0, 1) == 0 ? ColorScheme.Dark : ColorScheme.Light,
                        ViewportSize = vSize
                    };
                    // Создание браузера
                    browser = await playwright.Chromium.LaunchAsync(options);
                    context = await browser.NewContextAsync(contextOptions);
                    // Timezone
                    if (await SetTimezone())
                    {
                        // Whoer
                        await Whoer();
                    }
                    // Indexphone
                    await Indexphone();
                    // Очистка куки
                    await context.ClearCookiesAsync();
                    // Прогрев куки
                    await РeatingСookies();
                    // Регистрация 
                    await Register();
                }
                catch (TimeoutException)
                {
                    await browser.CloseAsync();
                    //await Task.Delay(20000);
                }
                catch
                {
                    await browser.CloseAsync();
                    //return;
                }
                await browser.CloseAsync();
            }
            Console.WriteLine($"Поток {num}| остановлен.");
        }
    }
}
