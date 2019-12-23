using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Configuration;


namespace PengarMaskin
{
    class Init
    {
        public static void Initialize(ref IWebDriver _driver)
        {
            try
            {
                InitChrome(ref _driver);
                Logon(_driver);

            }
            catch (WebDriverException ex)
            {
                Message.Log(MessageType.Info, "Första logon");
                Message.Log(MessageType.Info, ex.Message);
                try
                {
                    Logon(_driver);
                }
                catch (WebDriverException ex2)
                {
                    Message.Log(MessageType.Info, "Andra logon");
                    Message.Log(MessageType.Info, ex2.Message);
                    try
                    {
                        Logon(_driver);
                    }
                    catch (WebDriverException ex3)
                    {
                        Message.Log(MessageType.Info, "Tredje logon");
                        Message.Log(MessageType.Info, ex3.Message);
                    }
                }
            }
        }
        public static void InitChrome(ref IWebDriver _driver)
        {

            Message.Log(MessageType.Info, "InitChrome");
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--silent");
            //options.AddArgument("--window-position=-32000,-32000");
            //IWebDriver driver = new ChromeDriver(options);
            _driver = new ChromeDriver(driverService, options);
        }
        public static void Logon(IWebDriver _driver)
        {
            Message.Log(MessageType.Info, "Logon");
            _driver.Navigate().GoToUrl("https://www.nordnet.se/mux/login/start.html?cmpi=start-loggain&state=signin");
            System.Threading.Thread.Sleep(1 * 1000);

            var loginSidaButton = _driver.FindElement(By.XPath("//*[@id='authentication-login']/section/section[2]/section/section/section/div[2]/div/button"));

            loginSidaButton.Click();

            System.Threading.Thread.Sleep(1 * 1000);

            var userNameField = _driver.FindElement(By.Id("username"));
            var userPasswordField = _driver.FindElement(By.Id("password"));
            userNameField.SendKeys("brommabjorn");
            userPasswordField.SendKeys("Lk(5SMZg");
            
            var loginButton = _driver.FindElement(By.XPath("//*[@id='authentication-login']/section/section[2]/section/section/section/section/section/section/form/section[2]/div[1]/button"));
            //*[@id="authentication-login"]/section/section[2]/section/section/section/section/section/section/form/section[2]/div[1]/button

            loginButton.Click();
            System.Threading.Thread.Sleep(2 * 1000);

        }
    }
}
