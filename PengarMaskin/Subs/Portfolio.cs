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

        
            _driver.Navigate().GoToUrl("https://www.nordnet.se/ordrar-avslut");
            System.Threading.Thread.Sleep(1 * 1000);

            IWebElement Ordertab;
            try
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
                            var ordertyp = rd[1];
                            if (ordertyp == "Köp")
                            {
                                var deleteknapp = _driver.FindElement(By.XPath("//*[@id='main-content']/div/div[2]/div/div/div/div[1]/div/div/div/div/table/tbody/tr/td[8]/div/div/div[2]/button"));
                                deleteknapp.Click();
                                var delyes = _driver.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div/div/div[2]/div/div[4]/div/div/div[2]/div/button"));
                                delyes.Click();
                                AktierListBuy = new List<Aktie>();
                                Message.Log(MessageType.Info, string.Format("Bortag av order {0} {1} antal = {2} pris = {3}", ordertyp,rd[2], rd[3], rd[4]));
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Message.Log(MessageType.Info, "Finns inga ordrar");
            }

            //*[@id="main-content"]/div/div[2]/div/div/div/div[1]/div/div/div/div/table


            _driver.Navigate().GoToUrl("https://www.nordnet.se/oversikt/konto/2");
            System.Threading.Thread.Sleep(1 * 1000);
            

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
                
                ReadOnlyCollection<IWebElement> allRows1 = table.FindElements(By.CssSelector(".Flexbox__StyledFlexbox-sc-1ob4g1e-0 fDFCAG .Row__StyledRow-sc-1iamenj-0 xFbzK HeaderRow__StyledHeaderRow-sc-1vg8bsb-0 gvuBWh"));



                //_driver.Navigate().GoToUrl("https://classic.nordnet.se/mux/web/depa/mindepa/depaoversikt.html");
                //System.Threading.Thread.Sleep(2 * 1000) ;

                //var Tillgängligt = _driver.FindElement(By.XPath("//*[@id='tillgangligt']/table/tbody/tr[3]/td[2]"));
                //var Depåvärde = _driver.FindElement(By.XPath("//*[@id='portfolioToday']/table/tbody/tr[3]/td[2]/span"));



                //Find the Search text box UI Element
                //IWebElement table = _driver.FindElementc

                ReadOnlyCollection<IWebElement> allRows = table.FindElements(By.XPath("//*[@role='row']"));


                foreach (IWebElement row in allRows)
                {
                    //ReadOnlyCollection<IWebElement> row1 = row.FindElements(By.TagName("TR"));

                    var rowid = row.GetAttribute("id");
                    //ReadOnlyCollection<IWebElement> cells = row.FindElements(By.TagName("td"));

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
                            // string[] split2 = rd[3].Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

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

                            AktierListBuy.Add(_Aktie);
                            AntalAffarer++;

                            
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
