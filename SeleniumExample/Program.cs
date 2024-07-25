using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumExample
{
    public class Program
    {
        static void Main(string[] args)
        {           
            List<string> items = new List<string>();

            int hour = 0;
            int minute = 10;
            int second = 0;

            var hubUri = Environment.GetEnvironmentVariable("selenium_url");

            var dateRange = int.Parse(Environment.GetEnvironmentVariable("DATE_RANGE"));

#if DEBUG
            IWebDriver webDriver = new ChromeDriver();
#else
            ChromeOptions options = new ChromeOptions();
            IWebDriver webDriver = new RemoteWebDriver(new Uri(hubUri), options.ToCapabilities(), new TimeSpan(hour, minute, second));
#endif

            try
            {
                webDriver.Navigate().GoToUrl("https://github.com/trending");

                //konuşma dilleri >> ingilizce
                webDriver.FindElement(By.XPath("//*[@id=\"select-menu-spoken-language\"]/summary")).Click();
                SpinWait.SpinUntil(() => false, 2000);
                webDriver.FindElement(By.XPath("//*[@id=\"select-menu-spoken-language\"]/details-menu/div[3]/div/a[41]/span")).Click();
                SpinWait.SpinUntil(() => false, 2000);

                //programlama dilleri >> c#
                webDriver.FindElement(By.XPath("//*[@id=\"select-menu-language\"]/summary/span")).Click();
                SpinWait.SpinUntil(() => false, 2000);
                webDriver.FindElement(By.XPath("//*[@id=\"languages-menuitems\"]/div/a[71]")).Click();
                SpinWait.SpinUntil(() => false, 2000);

                //trendler >> haftalık trendler               
                webDriver.FindElement(By.XPath("//*[@id=\"select-menu-date\"]/summary")).Click();
                webDriver.FindElements(By.ClassName("select-menu-list"))[2]
                    .FindElements(By.TagName("a"))[dateRange]
                    .Click();

                SpinWait.SpinUntil(() => false, 2000);

                var articles = webDriver.FindElements(By.TagName("article"));

                foreach (var article in articles)
                {
                    GithubModel githubModels = new GithubModel();

                    githubModels.Name = article.FindElement(By.TagName("h2")).Text;

                    githubModels.Description = article.FindElement(By.ClassName("col-9")).Text;

                    githubModels.ProgrammingLanguage = article
                        .FindElements(By.TagName("span"))
                        .First(e => e.GetAttribute("itemprop") == "programmingLanguage").Text;

                    githubModels.DailyStar = article.FindElement(By.ClassName("float-sm-right")).Text;

                    githubModels.Branch = article.FindElements(By.ClassName("Link--muted"))[1].Text;

                    githubModels.TotalStar = article.FindElement(By.ClassName("Link--muted")).Text;

                    items.Add(githubModels.Name);
                    items.Add(githubModels.Description);
                    items.Add(githubModels.ProgrammingLanguage);
                    items.Add(githubModels.DailyStar);
                    items.Add(githubModels.Branch);
                    items.Add(githubModels.TotalStar);

                    foreach (var item in items)
                        Console.WriteLine(item);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                webDriver?.Quit();
            }           
        }
    }
}
