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
    class Sell
    {
        public static void Seller(IWebDriver _driver, Aktie _Aktie)
        {
            //var sText = string.Format("Säljer = {0}  Procent = {1}", _Aktie.Namn, _Aktie.Procent);
            //Message.Log(MessageType.Info, sText);

            var _url = string.Format("https://classic.nordnet.se/mux/web/handla/kopAktier.html?identifier={0}&marketplace=11", _Aktie.Aktie_ID);
            _driver.Navigate().GoToUrl(_url);

            var stockOwnedVolumeField = _driver.FindElement(By.Id("stockOwnedVolumeField"));
            stockOwnedVolumeField.Click();

            var pricebid = _driver.FindElement(By.Id("price-bid"));
            var stockPriceField = _driver.FindElement(By.Id("stockPriceField"));
            stockPriceField.Clear();
            //stockPriceField.SendKeys(newprice.ToString("{0:0.0}"));
            stockPriceField.SendKeys(pricebid.Text);

            // var stockVolume = _driver.FindElement(By.Id("stockVolumeField"));
            //  stockVolume.SendKeys(stockOwnedVolume.Text);

            var flippdown = _driver.FindElement(By.XPath("//*[@id='order_fieldset']/div[3]/span/img[2]"));
            var flippup = _driver.FindElement(By.XPath("//*[@id='order_fieldset']/div[3]/span/img[1]"));
            //*[@id="order_fieldset"]/div[3]/span/img[2]


            //flippdown.Click();
            //flippup.Click();


            Message.Log(MessageType.Info, string.Format("Säljer = {0} Antal = {1} till {2}", _Aktie.Namn, stockOwnedVolumeField.Text, stockPriceField.Text));
            var BuyButton = _driver.FindElement(By.Id("stockSellButton"));
            BuyButton.Click();

        }
    }
}
