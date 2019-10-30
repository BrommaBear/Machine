using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;

namespace PengarMaskin
{
    class Portfolio

    {
        public static void Kolla_portolio(IWebDriver _driver,ref List<Aktie> AktierListBuy,ref int AntalAffarer)
        {
            Message.Log(MessageType.Info, "Portfolio");
            _driver.Navigate().GoToUrl("https://www.nordnet.se/mux/web/depa/mindepa/depaoversikt.html");
            System.Threading.Thread.Sleep(2 * 1000) ;
            
            var Tillgängligt = _driver.FindElement(By.XPath("//*[@id='tillgangligt']/table/tbody/tr[3]/td[2]"));
            var Depåvärde = _driver.FindElement(By.XPath("//*[@id='portfolioToday']/table/tbody/tr[3]/td[2]/span"));

            Message.Log(MessageType.Info, string.Format("Depåvärde = {0} Tillgängligt ={1}",  Depåvärde.Text, Tillgängligt.Text));

            //Find the Search text box UI Element
            IWebElement table = _driver.FindElement(By.XPath("//table[@id='aktier']"));

            ReadOnlyCollection<IWebElement> allRows = table.FindElements(By.TagName("tr"));


            foreach (IWebElement row in allRows)
            {
                ReadOnlyCollection<IWebElement> row1 = row.FindElements(By.TagName("TR"));

                var rowid = row.GetAttribute("id");
                ReadOnlyCollection<IWebElement> cells = row.FindElements(By.TagName("td"));

                if (cells.Count > 3)
                {
                    if (string.IsNullOrWhiteSpace(row.Text) || row.Text.StartsWith("TOTAL"))
                    { }
                    else
                    {
                        string[] rd = row.Text.Split(
                             new[] { "\r\n", "\r", "\n" },
                             StringSplitOptions.None
                         );
                        var name = rd[0];

                        string[] split1 = rd[1].Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                        string[] split2 = rd[2].Split(new char[0], StringSplitOptions.RemoveEmptyEntries);


                        var x = new DAL();
                        DAL.AktieID _AktieID = x.GetAktieID(name);

                        var _Aktie = new Aktie
                        {
                            Aktie_ID = Convert.ToInt32(_AktieID.Aktie_ID),
                            Namn = name,
                            Pris = Convert.ToDecimal(split1[2]),
                            Change = Convert.ToDecimal(0),
                            Omsatt = Convert.ToInt64(0),
                            Procent = Convert.ToDecimal(0),
                            DateTime = DateTime.Now
                        };

                        AktierListBuy.Add(_Aktie);
                        AntalAffarer++;

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
