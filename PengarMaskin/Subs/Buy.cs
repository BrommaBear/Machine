using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;
using OpenQA.Selenium;
using System.Configuration;

namespace PengarMaskin
{
    class Buy
    {
        public static void Buyer(IWebDriver _driver, Aktie _Aktie)
        {

            Message.Log(MessageType.Info, "Buyer");

            var sText = string.Format("Köper = {0} Pris = {1} Procent = {2}", _Aktie.Namn, _Aktie.Pris, _Aktie.Procent);
            Message.Log(MessageType.Info, sText);

            int kopsumma = int.Parse(ConfigurationManager.AppSettings["KopSumma"]);
            int Volume = Convert.ToInt16(kopsumma / _Aktie.Pris);

            var _url = string.Format("https://classic.nordnet.se/mux/web/handla/kopAktier.html?identifier={0}&marketplace=11", _Aktie.Aktie_ID);
            _driver.Navigate().GoToUrl(_url);
            System.Threading.Thread.Sleep(2 * 1000);

            try
            {
                var stockVolume = _driver.FindElement(By.Id("stockVolumeField"));
                stockVolume.SendKeys(Volume.ToString());

                var priceask = _driver.FindElement(By.Id("price-ask"));
                var stockPriceField = _driver.FindElement(By.Id("stockPriceField"));
                stockPriceField.Clear();
                stockPriceField.SendKeys(priceask.Text);

            }
            catch (WebDriverException ex)
            {
                Message.Log(MessageType.Info, "Error i Buyer ");
                Message.Log(MessageType.Info, ex.Message);
                System.Threading.Thread.Sleep(2 * 1000);
                Init.Initialize(ref _driver);
                _url = string.Format("https://classic.nordnet.se/mux/web/handla/kopAktier.html?identifier={0}&marketplace=11", _Aktie.Aktie_ID);
                _driver.Navigate().GoToUrl(_url);
                System.Threading.Thread.Sleep(2 * 1000);
                var stockVolume = _driver.FindElement(By.Id("stockVolumeField"));
                stockVolume.SendKeys(Volume.ToString());
                var priceask = _driver.FindElement(By.Id("price-ask"));
                var stockPriceField = _driver.FindElement(By.Id("stockPriceField"));
                stockPriceField.Clear();
                stockPriceField.SendKeys(priceask.Text);
            }
            Message.Log(MessageType.Info, string.Format("Köp Antal = {0}", Volume.ToString()));

            var pricelast = _driver.FindElement(By.Id("price-last"));
            var bid = _driver.FindElement(By.XPath("//*[@id='price-bid']"));
            var ask = _driver.FindElement(By.XPath("//*[@id='price-ask']"));
            Message.Log(MessageType.Info, string.Format("Bid = {0} Ask = {1} stockPrice = {2}", bid.Text, ask.Text, pricelast.Text));

            var BuyButton = _driver.FindElement(By.Id("stockBuyButton"));
            BuyButton.Click();

        }
    }
}
