using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;using OpenQA.Selenium;
using System.Configuration;
using System.Diagnostics;

namespace PengarMaskin
{
    class Sell
    {
        public static void Seller(IWebDriver _driver, Aktie _Aktie,bool sell_direct)
        {   
            //var sText = string.Format("Säljer = {0}  Procent = {1}", _Aktie.Namn, _Aktie.Procent);
            //Message.Log(MessageType.Info, sText);

            var _url = string.Format("https://www.nordnet.se/marknaden/aktiekurser/16335289-samhallsbyggnadsbo-i-norden/order/salj?accid=2", _Aktie.Aktie_ID);
            _driver.Navigate().GoToUrl(_url);

            var stockOwnedVolumeField = _driver.FindElement(By.XPath("//*[@id='main-content']/div/div[2]/div/div[1]/div/div[1]/div[9]/div/div/div/div/div/form/div[1]/div/div[1]/span/div[2]/div/div[2]/button"));
            stockOwnedVolumeField.Click();

            var price = _Aktie.Pris * Convert.ToDecimal(1.008);

            //var pricebid = _driver.FindElement(By.Id("price-bid"));
            var priceask = _driver.FindElement(By.XPath("//*[@id='main-content']/div[1]/div[1]/div/div/div[1]/div[3]/div/div[1]/div[4]/div/div/span"));
            var pricebid = _driver.FindElement(By.XPath("//*[@id='main-content']/div[1]/div[1]/div/div/div[1]/div[3]/div/div[1]/div[5]/div/div/span"));

            if (Convert.ToDecimal(pricebid.Text) > price)
            {
                price = Convert.ToDecimal(pricebid.Text);
            }

            if (sell_direct)
            {
                price = Convert.ToDecimal(priceask.Text); 
            }

            //var stockPriceField = _driver.FindElement(By.Id("stockPriceField"));
            var stockPriceField = _driver.FindElement(By.Id("price"));
            while (!stockPriceField.GetAttribute("value").Equals(""))
            {
                stockPriceField.SendKeys(Keys.Backspace);
            }
            stockPriceField.Clear();

            var sprice = string.Empty;


            if (price < 10)
            {
                sprice = string.Format("{0:0.000}", price).Replace(",", ".");
                if (price >= 2 & price < 5)
                {
                    if (sprice.Substring(4, 1) == "1") { sprice = string.Format("{0}{1}", sprice.Substring(0, 4), "2"); }
                    if (sprice.Substring(4, 1) == "3") { sprice = string.Format("{0}{1}", sprice.Substring(0, 4), "4"); }
                    if (sprice.Substring(4, 1) == "5") { sprice = string.Format("{0}{1}", sprice.Substring(0, 4), "6"); }
                    if (sprice.Substring(4, 1) == "7") { sprice = string.Format("{0}{1}", sprice.Substring(0, 4), "8"); }
                    if (sprice.Substring(4, 1) == "9") { sprice = string.Format("{0}{1}", sprice.Substring(0, 4), "8"); }
                }
                else
                {
                    if (Convert.ToInt32(sprice.Substring(4, 1)) > 2 & (Convert.ToInt32(sprice.Substring(4, 1)) < 8))
                    {
                        sprice = string.Format("{0}{1}", sprice.Substring(0, 4), "5");
                    }
                    else
                    {
                        sprice = string.Format("{0}{1}", sprice.Substring(0, 4), "0");
                    }
                }
            }

            if (price >= 10 & price < 100)
            {
                sprice = string.Format("{0:0.00}", price).Replace(",", ".");
                if (price <= 50 & price < 100)
                {
                    if (Convert.ToInt32(sprice.Substring(4, 1)) > 2 & (Convert.ToInt32(sprice.Substring(4, 1)) < 8))
                    {
                        sprice = string.Format("{0}{1}", sprice.Substring(0, 4), "5");
                    }
                    else
                    {
                        sprice = string.Format("{0}{1}", sprice.Substring(0, 4), "0");
                    }
                }
            }

            if (price >= 100 & price < 1000)
            {
                sprice = string.Format("{0:0.0}", price).Replace(",", ".");
                if (price <= 500 & price < 100)
                { 
                    if (Convert.ToInt32(sprice.Substring(4, 1)) > 2 & (Convert.ToInt32(sprice.Substring(4, 1)) < 8))
                    {
                        sprice = string.Format("{0}{1}", sprice.Substring(0, 4), "5");
                    }
                    else
                    {
                        sprice = string.Format("{0}{1}", sprice.Substring(0, 4), "0");
                    }
                }
            }
        
     
            
            //if (price < 500) { sprice = price.ToString("{0:0}").Replace(",", "."); }



            stockPriceField.SendKeys(sprice);

            //stockPriceField.Clear();
            //stockPriceField.SendKeys(newprice.ToString("{0:0.0}"));
            //stockPriceField.SendKeys(price.ToString());

            // var stockVolume = _driver.FindElement(By.Id("stockVolumeField"));
            //  stockVolume.SendKeys(stockOwnedVolume.Text);

            //var flippdown = _driver.FindElement(By.XPath("//*[@id='order_fieldset']/div[3]/span/img[2]"));
            //var flippup = _driver.FindElement(By.XPath("//*[@id='order_fieldset']/div[3]/span/img[1]"));
            //*[@id="order_fieldset"]/div[3]/span/img[2]


            //flippdown.Click();
            //flippup.Click();
                    
           
            Message.Log(MessageType.Info, string.Format("Säljer = {0} Antal = {1} till {2}", _Aktie.Namn, stockOwnedVolumeField.Text, stockPriceField.Text));
            var BuyButton = _driver.FindElement(By.Id("stockSellButton"));
            BuyButton.Click();

        }
    }
}
