using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using PetaPoco;

namespace PengarMaskin
{
    class Portfolio
    {
        public static void Kolla_portolio(IWebDriver _driver,ref List<Aktie> AktierListBuy,ref int AntalAffarer)
        {
            Message.Log(MessageType.Info, "Portfolio");
            var db = new Database("Bjorn");
            db.Execute("Delete from portfolio");
            

            Orders.Kolla_orders(_driver, "Köp");
                      
            _driver.Navigate().GoToUrl("https://www.nordnet.se/oversikt/konto/2");
            System.Threading.Thread.Sleep(2 * 1000);
            
            var Tillgängligt = _driver.FindElement(By.XPath("//*[@id='main-content']/div/div[2]/div/div/div/div[2]/div/div/div/div/div/span/span"));
            var Depåvärde = _driver.FindElement(By.XPath("//*[@id='main-content']/div/div[2]/div/div/div/div[1]/div/div/div/div[1]/div/span/span"));

            Message.Log(MessageType.Info, string.Format("Depåvärde = {0} Tillgängligt ={1}", Depåvärde.Text, Tillgängligt.Text));

            IWebElement tab;
            bool InnehavFinns = true; 
            try
            {
                tab = _driver.FindElement(By.XPath("//*[@id='main-content']/div/div[2]/div/div/div/div[4]/div/div/div"));
            }
            catch (Exception ex)
            {
                InnehavFinns = false;
                AntalAffarer = 0;                
            }

            if (Tillgängligt.Text != Depåvärde.Text & InnehavFinns)
            {
                IWebElement table = _driver.FindElement(By.XPath("//*[@id='main-content']/div/div[2]/div/div/div/div[4]/div/div/div"));
                ReadOnlyCollection<IWebElement> allRows = table.FindElements(By.XPath("//*[@role='row']"));

                foreach (IWebElement row in allRows)
                {
                    var rowid = row.GetAttribute("id");
                    
                    if (allRows.Count > 1)
                    {
                        if (string.IsNullOrWhiteSpace(row.Text) || row.Text.StartsWith("Totalt") || row.Text.StartsWith("Namn"))
                        { }
                        else
                        {
                            string[] rd = row.Text.Split(
                                 new[] { "\r\n", "\r", "\n" },
                                 StringSplitOptions.None
                             );

                            var name = rd[1];
                            string[] split1 = rd[4].Split(new char[0], StringSplitOptions.RemoveEmptyEntries);                            
                            var x = new DAL();
                            DAL.Buy aktiebuy = x.GetLastBuy(name);

                            var _Aktie = new Aktie
                            {
                                Aktie_ID = Convert.ToInt32(aktiebuy.Aktie_ID),
                                Namn = name,
                                Pris = Convert.ToDecimal(split1[0]),
                                Change = Convert.ToDecimal(0),
                                Omsatt = Convert.ToInt64(0),
                                Procent = Convert.ToDecimal(0),
                                DateTime = Convert.ToDateTime(aktiebuy.DateTime)
                            };

                            bool alreadyExist = AktierListBuy.Any(s => s.Aktie_ID == _Aktie.Aktie_ID);
                            if (alreadyExist == false) 
                            {
                                AktierListBuy.Add(_Aktie);
                                AntalAffarer++;
                            }                                                        
                            db.Insert("Portfolio", "Id", _Aktie);
                        }
                    }
                }
                try
                {
                    foreach (var BuyAkt in AktierListBuy)
                    {
                        Console.WriteLine(BuyAkt.Namn);
                        Message.Log(MessageType.Info, BuyAkt.Namn);
                    }
                    Message.Log(MessageType.Info, string.Format("AntalAffarer = {0}", AntalAffarer));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Message.Log(MessageType.Error, ex.Message);
                }
            }    
        }
    }
}
