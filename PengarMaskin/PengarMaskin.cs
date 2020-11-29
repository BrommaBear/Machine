using System;
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
        public static List<AktieURL> AktierURL = new List<AktieURL>();
        public static int AntalAffarer;
        public static int AntalBlanka;
        public static int i;
        public static bool kontrollera_kop = false;
        public static DateTime kontrollera_nast_tid = DateTime.Now;

        public PengarMaskin()
        {

            Message.Log(MessageType.Info, "Start");
            Message.Log(MessageType.Info, string.Format("MaxAntalKop = {0}", ConfigurationManager.AppSettings["MaxAntalKop"]));
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

                MainLoop();
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
           
        }
        
        public void MainLoop()
        {            
            var db = new Database("Bjorn");
            var _db = new DAL();
            var dt = DateTime.Now;
            var dagensid = new Int64();
            int MaxAntalKop = int.Parse(ConfigurationManager.AppSettings["MaxAntalKop"]);
            AktierURL = _db.GetAktieURL();
            var antal_i_portfolio = new int();


            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 31, 00)
              & DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 08, 02, 00))
            {
                
                if (dagensid == 0)
                {
                    dagensid = db.Single<Int64>(string.Format("select isnull(min(id),0) from Prices with(nolock) where datetime > '{0}'", (DateTime.Today)));
                }
                              

                if (kontrollera_kop & kontrollera_nast_tid < DateTime.Now)
                {
                    try
                    {
                        Portfolio.Kolla_portolio(_driver,
                                                 ref AktierListBuy,
                                                 ref AntalAffarer);

                        foreach (var aktie in AktierListBuy)
                        {
                            bool finns = db.Fetch<Portfolio>("WHERE Aktie_ID=@0", aktie.Aktie_ID).Any();
                            if (finns)
                            {
                                //Sälj
                                AktierListSell.Add(aktie);
                                db.Insert("Sell", "Id", aktie);
                                Sell.Seller(_driver, aktie, false);
                                db.Execute("Delete from portfolio where aktie_id = @0", aktie.Aktie_ID);
                                AntalAffarer--;

                                antal_i_portfolio++;

                            }
                            else
                            {
                                kontrollera_nast_tid = DateTime.Now.AddMinutes(+5);
                            }
                            
                        }

                        if (antal_i_portfolio == AktierListBuy.Count)
                        {
                            kontrollera_kop = false;
                        }

                    }
                    catch (WebDriverException ex)
                    {
                        Message.Log(MessageType.Info, "Error i Kolla_portfolio");
                        Message.Log(MessageType.Info, ex.Message);
                    }

                }

                var result = db.Fetch<Aktie>(";EXEC Stockbuy @0,0,@1", dagensid, DateTime.Now); //"2020-01-14 09:02:20");

                if (result.Count > 0)
                {
                    var _Aktie = result.First();

                    var _AktieBuy = AktierListBuy.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
                    if (_AktieBuy == null)
                    {

                        if (AntalAffarer < MaxAntalKop)
                        {

                            //Köp
                            var ActieUrl = AktierURL.Find(item => item.Id == _Aktie.Aktie_ID);
                                
                            //Message.Log(MessageType.Info, string.Format("Uppgång {0} Pris = {1} ", _Aktie.Namn, _Aktie.Pris.ToString()));
                            Buy.Buyer(_driver, _Aktie, ActieUrl);
                            AktierListBuy.Add(_Aktie);
                            db.Insert("Buy", "Id", _Aktie);
                            kontrollera_kop = true;
                            kontrollera_nast_tid = DateTime.Now.AddMinutes(+3);
                            //db.Insert("Portfolio", "Id", _Aktie);
                            AntalAffarer++;
                        }
                        else
                        {
                            //logga
                            Message.Log(MessageType.Info, string.Format("Skulle köpt Uppgång {0} Pris = {1} ", _Aktie.Namn, _Aktie.Pris.ToString()));
                        }
                    }
                }

                

                //Sälj
                if (AntalAffarer > 0)
                {
                    if (dagensid == 0)
                    {
                        dagensid = db.Single<Int64>(string.Format("select isnull(min(id),0) from Prices with(nolock) where datetime > '{0}'", (DateTime.Today)));
                    }

                    //var result1 = db.Fetch<Aktie>(";EXEC Stocksell @0, @1", dagensid, DateTime.Now); //"2020-01-14 09:02:20");
                    //if (result1.Count > 0)

                    foreach (var _Aktie in AktierListBuy)
                    {
                        //var _Aktie = result1.First();
                        if (_Aktie.DateTime.AddMinutes(15) < DateTime.Now)
                        {
                            //Sälj
                            Message.Log(MessageType.Info, string.Format("Säljer {0} 15 min ", _Aktie.Namn));
                            AktierListSell.Add(_Aktie);
                            db.Insert("Sell", "Id", _Aktie);
                            Sell.Seller(_driver, _Aktie, true);
                            db.Execute("Delete from portfolio where aktie_id = @0", _Aktie.Aktie_ID);
                            AntalAffarer--;
                        }   
                    }
                   

                }
            }

            i++;
             
        }
    }   
}

