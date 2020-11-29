using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;
using OpenQA.Selenium;
using System.Configuration;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Interactions;



namespace PengarMaskin
{
    class Buy
    {
        public static void Buyer(IWebDriver _driver, Aktie _Aktie, AktieURL Aktieurl)
        {

            Message.Log(MessageType.Info, string.Format("Buyer {0}", _Aktie.Namn));


            //var sText = string.Format("Köper = {0} Pris = {1} Procent = {2}", _Aktie.Namn, _Aktie.Pris, _Aktie.Procent);
            //Message.Log(MessageType.Info, sText);

            int kopsumma = int.Parse(ConfigurationManager.AppSettings["KopSumma"]);

            var url = string.Format("{0}{1}", Aktieurl.URLBuy, "?accid=2");
            int Volume = Convert.ToInt16(kopsumma / _Aktie.Pris);

            try
            {
                try
                {
                    _driver.Navigate().GoToUrl(url);
                    System.Threading.Thread.Sleep(2 * 1000);
                    var sVolume = _driver.FindElement(By.Id("volume"));
                }
                catch (WebDriverException ex)
                {
                    Init.Logon(_driver);
                    _driver.Navigate().GoToUrl(url);
                }
                    System.Threading.Thread.Sleep(2 * 1000);


                    var stockVolume = _driver.FindElement(By.Id("volume"));
                    stockVolume.SendKeys(Volume.ToString());


                    var priceask = _driver.FindElement(By.XPath("//*[@id='main-content']/div[1]/div[1]/div/div/div[1]/div[3]/div/div[1]/div[4]/div/div/span"));
                    var pricebid = _driver.FindElement(By.XPath("//*[@id='main-content']/div[1]/div[1]/div/div/div[1]/div[3]/div/div[1]/div[5]/div/div/span"));
                    //*[@id="main-content"]/div[1]/div[1]/div/div/div[1]/div[3]/div/div[1]/div[4]/div/div/span
                    var stockPriceField = _driver.FindElement(By.Id("price"));

                    while (!stockPriceField.GetAttribute("value").Equals(""))
                    {
                        stockPriceField.SendKeys(Keys.Backspace);
                    }
                    stockPriceField.Clear();
                    stockPriceField.SendKeys(priceask.Text);

                }
                catch (WebDriverException ex)
                {
                    Message.Log(MessageType.Info, "Error i Buyer ");
                    Message.Log(MessageType.Info, ex.Message);
                    Init.Logon(_driver);
                    _driver.Navigate().GoToUrl(url);
                    System.Threading.Thread.Sleep(2 * 1000);
                    var stockVolume = _driver.FindElement(By.Id("volume"));
                    stockVolume.SendKeys(Volume.ToString());
                    var pricebid = _driver.FindElement(By.Id("price-bid"));
                    var stockPriceField = _driver.FindElement(By.Id("stockPriceField"));
                    while (!stockPriceField.GetAttribute("value").Equals(""))
                    {
                        stockPriceField.SendKeys(Keys.Backspace);
                    }
                    stockPriceField.Clear();
                    stockPriceField.SendKeys(pricebid.Text);
                }
            Message.Log(MessageType.Info, string.Format("Köp Antal = {0}", Volume.ToString()));

            var pricelast = _driver.FindElement(By.XPath("//*[@id='main-content']/div[1]/div[1]/div/div/div[1]/div[3]/div/div[1]/div[1]/div/div/span"));
            var bid = _driver.FindElement(By.XPath("//*[@id='main-content']/div[1]/div[1]/div/div/div[1]/div[3]/div/div[1]/div[5]/div/div/span"));
            var ask = _driver.FindElement(By.XPath("//*[@id='main-content']/div[1]/div[1]/div/div/div[1]/div[3]/div/div[1]/div[4]/div/div/span"));
            Message.Log(MessageType.Info, string.Format("Bid = {0} Ask = {1} stockPrice = {2}", bid.Text, ask.Text, pricelast.Text));

            var BuyButton = _driver.FindElement(By.XPath("//*[@id='main-content']/div/div[2]/div/div[1]/div/div[1]/div[9]/div/div/div[1]/div/div/form/div[2]/div[1]/div[2]/button"));

            Message.Log(MessageType.Info, "Dax att trycka på knappen");

            //BuyButton.Click();
                       

            try
            {
                //new WebDriverWait(_driver, TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='main-content']/div/div[2]/div/div[1]/div/div[1]/div[9]/div/div/div[1]/div/div/form/div[2]/div[1]/div[2]/button"))).Click();
                BuyButton.Submit();
                Message.Log(MessageType.Info, "Hurra det funkade");

            }
            catch (WebDriverException ex)
            {
                Message.Log(MessageType.Info, "Gick inte att trycka på knappen");
                Message.Log(MessageType.Info, ex.Message);
            }
        }
    }
}
