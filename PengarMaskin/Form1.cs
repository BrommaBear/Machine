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
using PengarMaskin.Models.Dal;


namespace PengarMaskin
{
    public partial class Form1 : Form
    {

        public Boolean Online = false;
        public static IWebDriver _driver = null;
        public Form1()
        {
            InitializeComponent();

            Message.Log(MessageType.Info, "Start");

            try
            {
                InitChrome();
                Logon();
            }
            catch (WebDriverException ex)
            {
                Message.Log(MessageType.Info, "Första logon");
                Message.Log(MessageType.Info, ex.Message);
                try
                {
                   Logon();
                }
                catch (WebDriverException ex2)
                {
                    Message.Log(MessageType.Info, "Andra logon");
                    Message.Log(MessageType.Info, ex2.Message);
                    try
                    {
                        Logon();
                    }
                    catch (WebDriverException ex3)
                    {
                        Message.Log(MessageType.Info, "Tredje logon");
                        Message.Log(MessageType.Info, ex3.Message);
                    }
                }
            }
            
            var x = 1;
            while (x==1)
            {
                try
                {
                Start();
                }
                catch (WebDriverException ex)
            {
                Message.Log(MessageType.Info, "Error i ListRefresh");
                Message.Log(MessageType.Info, ex.Message);
                    Message.Log(MessageType.Error, "Hit ska vi aldrig komma");
                }
            }

            //ListRefresh();

            //Timer timer = new Timer();
            //timer.Interval = (30 * 1000); // 10 secs
            //timer.Tick += new EventHandler(timer_Tick);
            //timer.Start();
        }
        
        public void InitChrome()
        {
            
            Message.Log(MessageType.Info, "InitChrome");
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--silent");
            //options.AddArgument("--window-position=-32000,-32000");
            //IWebDriver driver = new ChromeDriver(options);
            _driver = new ChromeDriver(driverService, options);
        }
        public void Logon()
        {
            Message.Log(MessageType.Info, "Logon");
           _driver.Navigate().GoToUrl("https://www.nordnet.se/mux/login/start.html?cmpi=start-loggain&state=signin");
            System.Threading.Thread.Sleep(1 * 1000);
            ///*[@id="authentication-login"]/section/section[2]/section/section/section[4]/div[2]/a

            var loginSidaButton = _driver.FindElement(By.XPath("//section/section[2]/section/section/section[4]/div[2]/div/a[contains(text(), 'Användarnamn och lösenord')]"));
            loginSidaButton.Click();

            System.Threading.Thread.Sleep(1 * 1000);

            var userNameField = _driver.FindElement(By.Id("username"));
            var userPasswordField = _driver.FindElement(By.Id("password"));
            userNameField.SendKeys("brommabjorn");
            userPasswordField.SendKeys("Lk(5SMZg");
            var loginButton = _driver.FindElement(By.XPath("//button[contains(@class, 'button-2524543156 primary-4057425872 size-m-4144456052 block-646414812')]"));
                      
           
            loginButton.Click();
            Online = true;
           
         
        }

        public void Buyer(Database db, Aktie _Aktie)
        {

            Message.Log(MessageType.Info, "Buyer");

            var sText = string.Format("Köper = {0} Pris = {1} Procent = {2}",_Aktie.Namn,_Aktie.Pris,_Aktie.Procent);
            Message.Log(MessageType.Info, sText);
            
            int kopsumma = int.Parse(ConfigurationManager.AppSettings["KopSumma"]);
            int Volume = Convert.ToInt16(kopsumma / _Aktie.Pris);

            var _url = string.Format("https://www.nordnet.se/mux/web/handla/kopAktier.html?identifier={0}&marketplace=11", _Aktie.Aktie_ID);
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
                InitChrome();
                Logon();
                _url = string.Format("https://www.nordnet.se/mux/web/handla/kopAktier.html?identifier={0}&marketplace=11", _Aktie.Aktie_ID);
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
            Message.Log(MessageType.Info, string.Format("Bid = {0} stockPrice = {1}", bid.Text, pricelast.Text));
            
            var BuyButton = _driver.FindElement(By.Id("stockBuyButton"));
            BuyButton.Click();

        }

        public void Seller(Database db, Aktie _Aktie)
        {
            var sText = string.Format("Säljer = {0} Pris = {1} Procent = {2}", _Aktie.Namn, _Aktie.Pris, _Aktie.Procent);
            Message.Log(MessageType.Info, sText);

            var _url = string.Format("https://www.nordnet.se/mux/web/handla/kopAktier.html?identifier={0}&marketplace=11", _Aktie.Aktie_ID);
            _driver.Navigate().GoToUrl(_url);

            var stockOwnedVolumeField = _driver.FindElement(By.Id("stockOwnedVolumeField"));
            stockOwnedVolumeField.Click();

            var pricebid = _driver.FindElement(By.Id("price-bid"));
            var stockPriceField = _driver.FindElement(By.Id("stockPriceField"));
            stockPriceField.Clear();
            stockPriceField.SendKeys(pricebid.Text);

            var stockVolume = _driver.FindElement(By.Id("stockVolumeField"));
           // stockVolume.SendKeys(stockOwnedVolume.Text);
            Console.WriteLine(" Sälj Antal = {0}", stockOwnedVolumeField.Text);
            var BuyButton = _driver.FindElement(By.Id("stockSellButton"));
            BuyButton.Click();

        }

        //private void timer_Tick(object sender, EventArgs e)
        private void Start()
        {
            try
            {
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
                    
                    InitChrome();
                    Message.Log(MessageType.Info, "Ny logon 1");
                    Logon();
                    Message.Log(MessageType.Info, "logon klar 1");
                    
                }
                catch (WebDriverException ex2)
                {
                    
                    Message.Log(MessageType.Info, ex2.Message);
                    Message.Log(MessageType.Info, "Andra försök InitChrome");
                    System.Threading.Thread.Sleep(10 * 1000);
                    
                    try
                    {
                        InitChrome();
                        Message.Log(MessageType.Info, "Ny logon 2");
                        
                        Logon();
                        Message.Log(MessageType.Info, "logon klar 2");
                        
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
                        InitChrome();
                        Console.WriteLine("Ny logon 3");
                        Logon();
                        Console.WriteLine("logon klar");
                    }
                    
                }
          
            }
           
        }
        public class Aktie: ICloneable
        {
            public Int32 Aktie_ID { get; set; }
            public String Namn { get; set; }
            public Decimal Pris { get; set; }
            public Decimal Change { get; set; }
            public Decimal Procent { get; set; }
            public DateTime DateTime { get; set; }
            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
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


        public void ListRefresh()
        {                       
            if (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 35, 00)
            || (DateTime.Today.DayOfWeek == DayOfWeek.Saturday) 
            || (DateTime.Today.DayOfWeek == DayOfWeek.Saturday))
            {
                _driver.Close();
                _driver.Dispose();
                Message.Log(MessageType.Info, "Exit");
                Environment.Exit(0);
            }

            //if (Online == false)
            //{
            //InitChrome();
            //Logon();
            //}
            
            _driver.Navigate().GoToUrl("https://www.nordnet.se/mux/web/marknaden/marknadsoversikt/marknadsoversiktSE.html");
            //System.Threading.Thread.Sleep(1 * 1000) ;
            _driver.Navigate().GoToUrl("https://www.nordnet.se/mux/web/marknaden/marknadsoversikt/marknadsoversiktSE.html");
            IWebElement tab = _driver.FindElement(By.ClassName("changeindicator"));
            IWebElement  Btrend = tab.FindElement(By.TagName("p"));
            decimal Borstrend =  Convert.ToDecimal(Btrend.Text.Remove(Btrend.Text.Length - 1, 1));
           

            var db = new Database("Bjorn");
           
            var _borstrend = new DAL.BorsTrend();
            var dt = DateTime.Now;
            _borstrend.Procent = Borstrend;
            _borstrend.DateTime = dt;
            db.Insert("BorsTrend", "Id", _borstrend);

            _driver.Navigate().GoToUrl("https://www.nordnet.se/mux/web/marknaden/kurslista/aktier.html?marknad=Sverige&lista=26_1&large=on&mid=on&sektor=0&subtyp=price&sortera=dev_percent&sorteringsordning=fallande");

            //Find the Search text box UI Element
            IWebElement table = _driver.FindElement(By.XPath("//table[@id='kurstabell']"));

            ReadOnlyCollection<IWebElement> allRows = table.FindElements(By.TagName("tr"));

            Grid1.Rows.Clear();
            Grid1.Refresh(); 

            var ind = 0;
            foreach (IWebElement row in allRows)
            {
                ReadOnlyCollection<IWebElement> row1 = row.FindElements(By.TagName("TR"));

                var rowid = row.GetAttribute("id");
                ReadOnlyCollection<IWebElement> cells = row.FindElements(By.TagName("td"));
                //var x = cells[0].Text;
                if (cells.Count > 5 && cells[1].Text != "")
                {
                    Grid1.Rows.Add(rowid.Substring(5), cells[1].Text, cells[2].Text, cells[3].Text, cells[4].Text);
                    //if (cells[11].GetAttribute("class") == "last")
                    //    Online = false;
                    //else
                    //    Online = true;
                    if (ind == 0) 
                    { 
                        var delaytyp = cells[11].GetAttribute("innerHTML").ToString().Substring(36,12);
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
                                Procent = Convert.ToDecimal(cells[4].Text.Remove(cells[4].Text.Length - 1,  1 )),
                                DateTime = dt
                            };

                            

                            if (_Aktie.Pris > 0)
                            {
                                _db.Insert("History", "Id", _Aktie);

                                CheckLow(_db, _Aktie);

                                CheckHigh(_db, _Aktie);

                                if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 05, 00))
                                {
                                    var buy = CheckBuy(_db, _Aktie);
                                    if (buy)
                                    {
                                        //om köp görs måste vi gå ur
                                        ind = 30;
                                    }
                                }

                                var sell  = CheckSell(_db, _Aktie);
                                if (sell)
                                {
                                    //om köp görs måste vi gå ur
                                    ind = 30;
                                }

                                var blanka = CheckBlankning(_db, _Aktie);
                                if (blanka)
                                {
                                    //om köp görs måste vi gå ur
                                   // ind = 30;
                                }

                                var blankaReturn = CheckBlankningReturn(_db, _Aktie);
                                if (blankaReturn)
                                {
                                    //om köp görs måste vi gå ur
                                    // ind = 30;
                                }

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
       
        public void CheckLow(Database db,Aktie _Aktie)
        {
            var _AktieLow = AktierListLow.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            if (_AktieLow == null)
            {
                _AktieLow = (Aktie)_Aktie.Clone();
                AktierListLow.Add(_AktieLow);
                
            }
            if (_AktieLow.Pris > _Aktie.Pris)
            {
                _AktieLow.Pris = _Aktie.Pris;
                
            }
        }
        public void CheckHigh(Database db, Aktie _Aktie)
        {
            var _AktieHigh = AktierListHigh.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            if (_AktieHigh == null)
            {
                _AktieHigh = (Aktie)_Aktie.Clone();
                AktierListHigh.Add(_AktieHigh);
            }
            if  (_AktieHigh.Pris < _Aktie.Pris)
            {
               _AktieHigh.Pris = _Aktie.Pris;
            }
        }

        public Boolean  CheckBuy(Database db,Aktie _Aktie)
        {
            var retu = false;
            var _AktieLow = AktierListLow.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
           
            //if ((_Aktie.Pris / _AktieLow.Pris) > Convert.ToDecimal(1.014)) // & 
            if ((_Aktie.Procent > Convert.ToDecimal(1.4)) 
             & (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 05, 00))
             & (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 30, 00)))
            {
                var _AKtieBuy = AktierListBuy.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
                if (_AKtieBuy == null)
                {
                    var x = new DAL();
                    DAL.Trend _trend =  x.GetTrend(db, _Aktie.Aktie_ID); 

                    //if (((_trend.Min15 < _trend.Min10)
                    // & (_trend.Min10 < _trend.Min05)
                    // & (_trend.Min05 < _Aktie.Pris)
                    // & (_trend.Trend30 < _trend.Trend20)
                    // & (_trend.Trend20 < _trend.Trend10)
                    // & (_trend.Trend10 < _trend.TrendNU))
 
                    // || ((_trend.Min20 < _trend.Min10)
                    // & (_trend.Min10 < _trend.Min05)
                    // & (_trend.Min05 < _Aktie.Pris)
                    // & (_trend.Trend25 < _trend.Trend15)
                    // & (_trend.Trend15 < _trend.Trend05)
                    // & (_trend.Trend05 < _trend.TrendNU))) 
                    //{

                    if (_trend.Min05 <= _Aktie.Pris
                        & _trend.Min02 < _Aktie.Pris)
                    { 
                        var _AKtieHigh = AktierListHigh.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
                        
                        int MaxAntalKop = int.Parse(ConfigurationManager.AppSettings["MaxAntalKop"]);
                        if (AntalAffarer < MaxAntalKop)
                        {
                            Buyer(db, _Aktie);
                            AktierListBuy.Add(_Aktie);
                            db.Insert("Buy", "Id", _Aktie);
                            AntalAffarer++;
                            
                            var sText = (string.Format("Köper = {0} pris_min = {2} pris_max = {12} Min20 = {3} Min15 = {4} Min05 = {5} Min02 = {6} pris = {1} trend20 = {7} trend15 = {8} trend10 = {9} trend05 = {10} trendNU = {11}"
                                                , _Aktie.Namn.ToString(), _Aktie.Pris.ToString(), _AktieLow.Pris.ToString()
                                                , _trend.Min20.ToString().ToString(), _trend.Min15.ToString(), _trend.Min05.ToString()
                                                , _trend.Min02.ToString(), _trend.Trend20.ToString(), _trend.Trend15.ToString()
                                                , _trend.Trend10.ToString(), _trend.Trend05.ToString(), _trend.TrendNU.ToString(), _AKtieHigh.Pris.ToString()));
                            Message.Log(MessageType.Info, sText);
                            _AKtieHigh.Pris = _Aktie.Pris;
                            _AktieLow.Pris = _Aktie.Pris;
                            retu = true;
                        }
                     }
                   
                }
            }
            return retu;
        }

        public Boolean CheckSell(Database db, Aktie _Aktie)
        {
            var retu = false;
            var _AKtieBuy = AktierListBuy.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            if (_AKtieBuy != null)
            {
                var _AKtieSell = AktierListSell.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
                if (_AKtieSell == null)
                {
                    var _AKtieHigh = AktierListHigh.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);

                    var x = new DAL();
                    DAL.Trend _trend = x.GetTrend(db, _Aktie.Aktie_ID);

                    decimal underMax;
                    if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 00, 00))
                    {
                        underMax = Convert.ToDecimal(1.01);
                    }
                    else
                    {
                        underMax = Convert.ToDecimal(1.005);

                        if (_Aktie.Procent > Convert.ToDecimal(3.0))
                        {
                            underMax = Convert.ToDecimal(1.0025);
                        }
                        if (_Aktie.Procent > Convert.ToDecimal(5.0))
                        {
                            underMax = Convert.ToDecimal(1.001);
                        }
                    }

                    if (((_AKtieHigh.Pris / _Aktie.Pris) > Convert.ToDecimal(underMax))
                    //|| (_Aktie.Procent < Convert.ToDecimal(1.0))
                    || (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 28, 00)))
                    
                    {
                        AktierListSell.Add(_Aktie);
                        db.Insert("Sell", "Id", _Aktie);

                        var _AKtieLow = AktierListLow.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
                        Seller(db, _Aktie);
                        AntalAffarer--;
                        //AktierListBuy.Remove(_Aktie);
                        var sText = (string.Format("Säljer = {0} aktie_procent = {13} pris_min = {12} pris_max = {2} Min20 = {3} Min15 = {4} Min05 = {5} Min02 = {6} pris = {1} trend30 = {14} trend25 = {13} trend20 = {7} trend15 = {8} trend10 = {9} trend05 = {10} trendNU = {11}"
                                            , _Aktie.Namn.ToString(), _Aktie.Pris.ToString(), _AKtieHigh.Pris.ToString()
                                            , _trend.Min20.ToString(), _trend.Min15.ToString(), _trend.Min05.ToString()
                                            , _trend.Min02.ToString(), _trend.Trend20.ToString(), _trend.Trend15.ToString()
                                            , _trend.Trend10.ToString(), _trend.Trend05.ToString(), _trend.TrendNU.ToString()
                                            , _AKtieLow.Pris.ToString(), _trend.Trend25.ToString(), _trend.Trend30.ToString(),_Aktie.Procent ));
                        Message.Log(MessageType.Info, sText);
                        
                        
                        retu = true;
                    }
                }
            }
            return retu;
        }
        public Boolean CheckBlankning(Database db, Aktie _Aktie)
        {
            var retu = false;
            var _AKtieHigh = AktierListHigh.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);

            if ((_AKtieHigh.Pris / _Aktie.Pris) > Convert.ToDecimal(1.014)) // & (_Aktie.Procent < Convert.ToDecimal(1.25)) & (DateTime.Now > new DateTime(2016, 11, 25, 09, 30, 00)))
            {
                var _AKtieBlanka = AktierListBlanka.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
                if (_AKtieBlanka == null)
                {
                    var x = new DAL();
                    DAL.Trend _trend = x.GetTrend(db, _Aktie.Aktie_ID);

                    if (((_trend.Min15 > _trend.Min05)
                     & (_trend.Min05 > _Aktie.Pris)
                     & (_trend.Trend20 < 0)
                     & (_trend.Trend20 > _trend.Trend10)
                     & (_trend.Trend10 > _trend.TrendNU))
                     || ((_trend.Min20 > _trend.Min10)
                     & (_trend.Min10 > _Aktie.Pris))
                     & (_trend.Trend15 < 0)
                     & (_trend.Trend15 > _trend.Trend05)
                     & (_trend.Trend05 > _trend.TrendNU))
                    {
                        if (AntalBlanka < 2)
                        {
                            var exists = db.Exists<DAL.Blanka>("Aktie_ID = @0", _Aktie.Aktie_ID);
                            if (exists == false)
                            {
                                db.Insert("Blanka", "Id", _Aktie);
                            }
                            var _AktieLow = AktierListHigh.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
                            _AKtieHigh = (Aktie)_Aktie.Clone();
                            _AktieLow = (Aktie)_Aktie.Clone();
                        
                        
                            //Seller(db, _Aktie);
                            AktierListBlanka.Add(_Aktie);
                            Console.WriteLine(string.Format("Blankar = {0} pris = {1} pris_min = {2} Min20 = {3} Min15 = {4} Min10 = {5} Min05 = {6} trend20 = {7} trend15 = {8} trend10 = {9} trend05 = {10} trendNU = {11}"
                                                , _Aktie.Namn.ToString(), _Aktie.Pris.ToString(), _AktieLow.Pris.ToString()
                                                , _trend.Min20.ToString().ToString(), _trend.Min15.ToString(), _trend.Min10.ToString()
                                                , _trend.Min05.ToString(), _trend.Trend20.ToString(), _trend.Trend15.ToString()
                                                , _trend.Trend10.ToString(), _trend.Trend05.ToString(), _trend.TrendNU.ToString()));

                            AntalBlanka++;
                            retu = true;
                        }
                    }

                }
            }
            return retu;
        }
        public Boolean CheckBlankningReturn(Database db, Aktie _Aktie)
        {
            var retu = false;
            var _AKtieBlanka = AktierListBlanka.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            if (_AKtieBlanka != null)
            {
                var _AKtieBlankaReturn = AktierListBlankaReturn.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
                if (_AKtieBlankaReturn == null)
                {
                    var _AktieLow = AktierListLow.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);

                    if ((( _Aktie.Pris / _AktieLow.Pris) > Convert.ToDecimal(1.02)
                    || (_AKtieBlanka.Pris * Convert.ToDecimal(0.95)) > _Aktie.Pris)
                    || (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 28, 00)))

                    {
                        AktierListBlankaReturn.Add(_Aktie);
                        db.Insert("BlankaReturn", "Id", _Aktie);

                        //Buyer(db, _Aktie);
                        AntalBlanka--;
                        retu = true;
                    }
                }
            }
            return retu;
        }
    }


}

