using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using PetaPoco;


namespace PengarMaskin
{
    class Orders

    {
        public static void Kolla_orders(IWebDriver _driver,string typ)
        {
            Message.Log(MessageType.Info, "Orders");
            
            _driver.Navigate().GoToUrl("https://www.nordnet.se/ordrar-avslut");
            System.Threading.Thread.Sleep(1 * 1000);
            bool OrdrarFinns = true;

            IWebElement Ordertab;
            try
            {
                
                try
                {
                    Ordertab = _driver.FindElement(By.XPath("//*[@id='main-content']/div/div[2]/div/div/div/div[1]/div/div/div/div/table"));
                }
                catch (Exception ex)
                {
                    OrdrarFinns = false;
                    Message.Log(MessageType.Info, "Finns inga ordrar");
                }
                if (OrdrarFinns)
                {
                    Ordertab = _driver.FindElement(By.XPath("//*[@id='main-content']/div/div[2]/div/div/div/div[1]/div/div/div/div/table"));
                    ReadOnlyCollection<IWebElement> allRows = Ordertab.FindElements(By.TagName("tr"));
                    var i = 0;
                    foreach (IWebElement row in allRows)
                    {
                        if (allRows.Count() > 1)
                        {
                            i++;
                            if (i > 1)
                            {
                                string[] rd = row.Text.Split(
                                     new[] { "\r\n", "\r", "\n" },
                                     StringSplitOptions.None
                                 );
                                var deleteknapp = _driver.FindElement(By.XPath("//*[@id='main-content']/div/div[2]/div/div/div/div[1]/div/div/div/div/table/tbody/tr/td[8]/div/div/div[2]/button"));
                                
                                var ordertyp = rd[1];
                                if (ordertyp == "Köp" & typ == "Köp")
                                {
                                    deleteknapp.Click();
                                    var delyes = _driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div/div/div[2]/div/div[4]/div/div/div[2]/div/button"));
                                    delyes.Click();
                                    // AktierListBuy = new List<Aktie>();       
                                    Message.Log(MessageType.Info, string.Format("Bortag av order {0} {1} antal = {2} pris = {3}", ordertyp, rd[2], rd[3], rd[4]));
                                }
                                if (ordertyp == "Sälj" & typ == "Sälj")
                                {
                                    deleteknapp.Click();
                                    var delyes = _driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div/div/div[2]/div/div[4]/div/div/div[2]/div/button"));
                                    delyes.Click();
                                    // AktierListBuy = new List<Aktie>();           
                                    Message.Log(MessageType.Info, string.Format("Bortag av order {0} {1} antal = {2} pris = {3}", ordertyp, rd[2], rd[3], rd[4]));
                                }
                                
                            }
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                Message.Log(MessageType.Info,string.Format( "Fel i kolla_Ordrar error = {0}",ex.Message));
            }           
               
        }
    }
}
