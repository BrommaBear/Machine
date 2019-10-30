﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using PetaPoco;

namespace PengarMaskin
{
    public partial class PengarMaskin
    {
        public Boolean Online = false;
        public static IWebDriver _driver = null;
        public static List<Aktie> AktierListLow = new List<Aktie>();
        public static List<Aktie> AktierListHigh = new List<Aktie>();
        public static List<Aktie> AktierListBuy = new List<Aktie>();
        public static List<Aktie> AktierListSell = new List<Aktie>();
        public static List<Aktie> AktierListBlanka = new List<Aktie>();
        public static List<Aktie> AktierListBlankaReturn = new List<Aktie>();
        public static List<Aktie> AktierListPlus2 = new List<Aktie>();
        public static List<Aktie> AktierListPlus3 = new List<Aktie>();
        public static int AntalAffarer;
        public static int AntalBlanka;

        public PengarMaskin()
        {

            Message.Log(MessageType.Info, "Start");
            Init.Initialize(ref _driver);

            try
            {
                Portfolio.Kolla_portolio(_driver,
                                         ref AktierListBuy,
                                         ref AntalAffarer);
            }
            catch (WebDriverException ex)
            {
                Message.Log(MessageType.Info, "Error i Kolla_portfolio");
                Message.Log(MessageType.Info, ex.Message);
            }

        }



        public void Run()
        {

            try
            {
                if (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 35, 00)
                || (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                || (DateTime.Today.DayOfWeek == DayOfWeek.Sunday))
                {
                    _driver.Close();
                    _driver.Dispose();
                    Message.Log(MessageType.Info, "Exit");
                    Environment.Exit(0);
                }

                ListRefresh();
            }
            catch (Exception ex)
            {
                Message.Log(MessageType.Info, "Error i ListRefresh");
                Message.Log(MessageType.Info, ex.Message);
                Message.Log(MessageType.Info, "Sleep 3 sek");
                System.Threading.Thread.Sleep(3 * 1000);
                try
                {

                    Init.Initialize(ref _driver);

                }
                catch (WebDriverException ex2)
                {

                    Message.Log(MessageType.Info, ex2.Message);
                    Message.Log(MessageType.Info, "Andra försök InitChrome");
                    System.Threading.Thread.Sleep(10 * 1000);

                    try
                    {
                        Init.Initialize(ref _driver);

                    }
                    catch (WebDriverException ex3)
                    {
                        Console.WriteLine(ex3.Message);
                        Message.Log(MessageType.Info, ex3.Message);
                        Console.WriteLine(DateTime.Now);
                        Console.WriteLine("Tredje försök InitChrome");
                        System.Threading.Thread.Sleep(240 * 1000);
                        Console.WriteLine(DateTime.Now);
                        Console.WriteLine("Ny init 3");
                        Init.Initialize(ref _driver);
                    }

                }

            }
            try
            {
                GetPrices();
            }
            catch (Exception ex)
            {
                Message.Log(MessageType.Info, "Fel i GetPrices");
                Message.Log(MessageType.Info, ex.Message);
            }
        }



        public void GetPrices()
        {
            var dt = DateTime.Now;
            _driver.Navigate().GoToUrl("https://www.nordnet.se/mux/web/marknaden/kurslista/aktier.html?marknad=Sverige&lista=1_1&large=on&mid=on&sektor=0&subtyp=historic_return&sortera=dev_percent&sorteringsordning=fallande");
            IWebElement table = _driver.FindElement(By.XPath("//table[@id='kurstabell']"));
            ReadOnlyCollection<IWebElement> allRows = table.FindElements(By.TagName("tr"));

            var ind = 0;
            foreach (IWebElement row in allRows)
            {
                ReadOnlyCollection<IWebElement> row1 = row.FindElements(By.TagName("TR"));

                var rowid = row.GetAttribute("id");
                ReadOnlyCollection<IWebElement> cells = row.FindElements(By.TagName("td"));
                //var x = cells[0].Text;
                if (cells.Count > 5 && cells[1].Text != "")
                {
                    using (var _db = new Database("Bjorn"))
                    {
                        try
                        {
                            var _AktieUtv = new AktieUtveckling
                            {
                                Aktie_ID = Convert.ToInt32(rowid.Substring(5)),
                                Namn = cells[1].Text,
                                Idag = Convert.ToDecimal(cells[2].Text.Remove(cells[2].Text.Length - 1, 1)),
                                EnVecka = Convert.ToDecimal(cells[3].Text.Remove(cells[3].Text.Length - 1, 1)),
                                EnMan = Convert.ToDecimal(cells[4].Text.Remove(cells[4].Text.Length - 1, 1)),
                                TreMan = Convert.ToDecimal(cells[5].Text.Remove(cells[5].Text.Length - 1, 1)),
                                SexMan = Convert.ToDecimal(cells[6].Text.Remove(cells[6].Text.Length - 1, 1)),
                                EttAr = Convert.ToDecimal(cells[7].Text.Remove(cells[7].Text.Length - 1, 1)),
                                TvaAr = Convert.ToDecimal(cells[8].Text.Remove(cells[8].Text.Length - 1, 1)),
                                TreAr = Convert.ToDecimal(cells[9].Text.Remove(cells[9].Text.Length - 1, 1)),
                                FemAr = Convert.ToDecimal(cells[10].Text.Remove(cells[10].Text.Length - 1, 1)),

                                DateTime = dt
                            };
                            if ((_AktieUtv.Idag > 0)
                             & (_AktieUtv.EnVecka > 0)
                             & (_AktieUtv.EnMan > 0))
                            {
                                Console.WriteLine(_AktieUtv.Idag);
                                _db.Insert("AktieUtv", "Id", _AktieUtv);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Message.Log(MessageType.Error, ex.Message);
                        }
                    }
                    ind++;
                }

                if (ind > 29)
                {
                    break;
                }
            }

        }

        public void ListRefresh()
        {


            _driver.Navigate().GoToUrl("https://www.nordnet.se/mux/web/marknaden/marknadsoversikt/marknadsoversiktSE.html");
            //System.Threading.Thread.Sleep(1 * 1000) ;
            _driver.Navigate().GoToUrl("https://www.nordnet.se/mux/web/marknaden/marknadsoversikt/marknadsoversiktSE.html");
            IWebElement tab = _driver.FindElement(By.ClassName("changeindicator"));
            IWebElement Btrend = tab.FindElement(By.TagName("p"));
            decimal Borstrend = Convert.ToDecimal(Btrend.Text.Remove(Btrend.Text.Length - 1, 1));


            var db = new Database("Bjorn");

            var _borstrend = new DAL.BorsTrend();
            var dt = DateTime.Now;
            _borstrend.Procent = Borstrend;
            _borstrend.DateTime = dt;
            db.Insert("BorsTrend", "Id", _borstrend);

            _driver.Navigate().GoToUrl("https://www.nordnet.se/mux/web/marknaden/kurslista/aktier.html?marknad=Sverige&lista=26_1&large=on&mid=on&sektor=0&subtyp=price&sortera=dev_percent&sorteringsordning=fallande");
            // _driver.Navigate().GoToUrl("https://www.nordnet.se/mux/web/marknaden/kurslista/aktier.html?marknad=Sverige&lista=1_1&large=on&mid=on&sektor=0&subtyp=price&sortera=dev_percent&sorteringsordning=fallande");

            //Find the Search text box UI Element
            IWebElement table = _driver.FindElement(By.XPath("//table[@id='kurstabell']"));

            ReadOnlyCollection<IWebElement> allRows = table.FindElements(By.TagName("tr"));


            var ind = 0;
            foreach (IWebElement row in allRows)
            {
                ReadOnlyCollection<IWebElement> row1 = row.FindElements(By.TagName("TR"));

                var rowid = row.GetAttribute("id");
                ReadOnlyCollection<IWebElement> cells = row.FindElements(By.TagName("td"));
                //var x = cells[0].Text;
                if (cells.Count > 5 && cells[1].Text != "")
                {

                    if (ind == 0)
                    {
                        var delaytyp = cells[11].GetAttribute("innerHTML").ToString().Substring(36, 12);
                        Message.Log(MessageType.Info, delaytyp);
                        if (delaytyp == "15 min. förd")
                        {
                            throw new System.InvalidOperationException("15 min. fördröjning");
                        }

                    }
                    using (var _db = new Database("Bjorn"))
                    {
                        try
                        {

                            //var Histpost = new DAL.History
                            var _Aktie = new Aktie
                            {
                                Aktie_ID = Convert.ToInt32(rowid.Substring(5)),
                                Namn = cells[1].Text,
                                Pris = Convert.ToDecimal(cells[2].Text),
                                Change = Convert.ToDecimal(cells[3].Text),
                                Omsatt = Convert.ToInt64(cells[9].Text.Replace(" ", "")),
                                Procent = Convert.ToDecimal(cells[4].Text.Remove(cells[4].Text.Length - 1, 1)),
                                DateTime = dt
                            };



                            if (_Aktie.Pris > 0)
                            {
                                _db.Insert("History", "Id", _Aktie);

                                Check.CheckLow(_Aktie, AktierListLow);

                                Check.CheckHigh(_Aktie, AktierListHigh);

                                if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 25, 00)
                                 & DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 00, 00))
                                {
                                    var buy = Check.Buyer(_driver, _db, _Aktie, ind, ref AntalAffarer, AktierListHigh, ref AktierListBuy, AktierListLow);
                                    if (buy)
                                    {
                                        //om köp görs måste vi gå ur
                                        ind = 30;
                                    }
                                }


                                var sell = Check.Seller(_driver, _db, _Aktie, ref AntalAffarer, AktierListHigh, ref AktierListBuy, ref AktierListSell, AktierListLow);
                                if (sell)
                                {
                                    //om köp görs måste vi gå ur
                                    ind = 30;
                                }

                                //var blanka = CheckBlankning(_db, _Aktie);
                                //if (blanka)
                                //{
                                //    //om köp görs måste vi gå ur
                                //   // ind = 30;
                                //}

                                //var blankaReturn = CheckBlankningReturn(_db, _Aktie);
                                //if (blankaReturn)
                                //{
                                //    //om köp görs måste vi gå ur
                                //    // ind = 30;
                                //}

                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Message.Log(MessageType.Error, ex.Message);
                        }
                    }
                    ind++;
                }

                if (ind > 29)
                {
                    break;
                }
            }
        }
    }   
}

