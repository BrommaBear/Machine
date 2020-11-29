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
    class Check
    {
        public static Boolean Buyer(IWebDriver _driver
                                     , Database db
                                     , Aktie _Aktie, int ind
                                     , ref int AntalAffarer
                                     , List<Aktie> AktierListHigh
                                     , ref List<Aktie> AktierListBuy
                                     , List<Aktie> AktierListLow
                                     , List<AktieURL> AktierListUrl)
                                     
        {
            var retu = false;
            decimal faktor;
            var _AktieLow = AktierListLow.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            var _AKtieBuy = AktierListBuy.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            var _AktieHigh = AktierListHigh.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            var _AktieUrl = AktierListUrl.Find(item => item.Id == _Aktie.Aktie_ID);
            Decimal faktor1 = Convert.ToDecimal(0.985);

            if (_AKtieBuy == null)
            {
                if ((_Aktie.Omsatt > 100000)
                & (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 10, 00))
                & (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 25, 00))
                & _Aktie.Aktie_ID != 4870 //Fingerprint))
                & _Aktie.Namn.TrimEnd().Substring(_Aktie.Namn.Length - 1, 1) != "A")
                {
                    var x = new DAL();
                    DAL.Trend _trend = x.GetTrend(db, _Aktie.Aktie_ID);

                    int MaxAntalKop = int.Parse(ConfigurationManager.AppSettings["MaxAntalKop"]);

                    var fakt = ConfigurationManager.AppSettings["Faktor"];
                    faktor1 = Convert.ToDecimal(fakt);

                    if (_trend.TrendNU < 0)
                    {
                        faktor = (_trend.TrendNU * Convert.ToDecimal(0.01)) + faktor1;
                    }
                    else
                    {
                        faktor = faktor1;
                    }

                    if (_Aktie.Pris < _trend.MinDagFore * faktor)
                    {
                        if (AntalAffarer < MaxAntalKop)
                        {
                            Message.Log(MessageType.Info, string.Format("Lägre än igår köper {0} Pris = {1} TrendNU = {2} faktor = {3} MinDagFore = {4}", _Aktie.Namn, _Aktie.Pris.ToString(), _trend.TrendNU.ToString(), faktor.ToString(), _trend.MinDagFore.ToString()));
                            
                            Buy.Buyer(_driver, _Aktie,_AktieUrl);
                            AktierListBuy.Add(_Aktie);
                            db.Insert("Buy", "Id", _Aktie);
                            AntalAffarer++;

                            var sText = (string.Format("Köper = {0} pris = {1} pris_min = {2} pris_max = {16} Min20 = {3} Min15 = {4} Min05 = {5} Min01 = {6}  trend20 = {7} trend15 = {8} trend10 = {9} trend05 = {10} trendNU = {11} listnr = {12} Under_high = {13} AntalAffärer = {14} MaxAntalAffärer = {15}"
                                                , _Aktie.Namn.ToString(), _Aktie.Pris.ToString(), _AktieLow.Pris.ToString()
                                                , _trend.Min20.ToString().ToString(), _trend.Min15.ToString(), _trend.Min05.ToString()
                                                , _trend.Min01.ToString(), _trend.Trend20.ToString(), _trend.Trend15.ToString()
                                                , _trend.Trend10.ToString(), _trend.Trend05.ToString(), _trend.TrendNU.ToString()
                                                , ind, (_AktieHigh.Pris / _Aktie.Pris), AntalAffarer, MaxAntalKop, _AktieHigh.Pris.ToString()));
                            Message.Log(MessageType.Info, sText);
                            _AktieHigh.Pris = _Aktie.Pris;
                            _AktieLow.Pris = _Aktie.Pris;

                            var newprice = (_Aktie.Pris * Convert.ToDecimal(1.013));

                            retu = true;
                        }
                        else
                        {
                            var sText = string.Format("Skulle köpt {0} Pris = {1} TrendNU = {2} faktor = {3} MinDagFore = {4}", _Aktie.Namn, _Aktie.Pris.ToString(), _trend.TrendNU.ToString(), faktor.ToString(), _trend.MinDagFore.ToString());
                            Message.Log(MessageType.Info, sText);

                        }
                    }
                    if (
                        (_Aktie.Pris < _trend.Min15)
                       & (_Aktie.Pris <= _trend.Min10)                       
                       & (_trend.Min05 < _trend.Min10)
                       & (_trend.Min10 < _trend.Min20)
                       & (_trend.Min10 < _trend.Min15)
                       & (_Aktie.Pris > _trend.Min05)
                       & (_Aktie.Pris > _trend.Min03)
                       & (_Aktie.Pris > _trend.Min01)
                       & (_Aktie.Procent > Convert.ToDecimal(1.2))
                       & (_Aktie.Procent < Convert.ToDecimal(2.0))
                       & (_trend.TrendNU > Convert.ToDecimal(0.2))

                       & (_Aktie.Pris > _AktieLow.Pris)
                       & (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 30, 00))
                       & (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 30, 00))
                        )
                    {
                        if (AntalAffarer < MaxAntalKop)
                        {
                            Message.Log(MessageType.Info, string.Format("Över 1,0 och under 2,0 Köper {0} Pris = {1} TrendNU = {2} faktor = {3} MinDagFore = {4}", _Aktie.Namn, _Aktie.Pris.ToString(), _trend.TrendNU.ToString(), faktor.ToString(), _trend.MinDagFore.ToString()));
                            Buy.Buyer(_driver, _Aktie,_AktieUrl);
                            AktierListBuy.Add(_Aktie);
                            db.Insert("Buy", "Id", _Aktie);
                            AntalAffarer++;

                            var sText = (string.Format("Köper =  {0} pris = {1} pris_min = {2} pris_max = {12} Min20 = {3} Min15 = {4} Min05 = {5} Min02 = {6}  trend20 = {7} trend15 = {8} trend10 = {9} trend05 = {10} trendNU = {11} listnr = {12} Under_high = {13} AntalAffärer = {14} MaxAntalAffärer = {15}"
                                                , _Aktie.Namn.ToString(), _Aktie.Pris.ToString(), _AktieLow.Pris.ToString()
                                                , _trend.Min20.ToString().ToString(), _trend.Min15.ToString(), _trend.Min05.ToString()
                                                , _trend.Min02.ToString(), _trend.Trend20.ToString(), _trend.Trend15.ToString()
                                                , _trend.Trend10.ToString(), _trend.Trend05.ToString(), _trend.TrendNU.ToString()
                                                , ind, (_AktieHigh.Pris / _Aktie.Pris), AntalAffarer, MaxAntalKop, _AktieHigh.Pris.ToString()));
                            Message.Log(MessageType.Info, sText);
                            _AktieHigh.Pris = _Aktie.Pris;
                            _AktieLow.Pris = _Aktie.Pris;

                            var newprice = (_Aktie.Pris * Convert.ToDecimal(1.013));

                            retu = true;
                        }
                        else
                        {
                            var sText = string.Format("Skulle köpt Över 1,0 och under 2,0 {0} Pris = {1} TrendNU = {2} faktor = {3} MinDagFore = {4} AntalAffärer = {5} MaxAntalAffärer = {6}", _Aktie.Namn, _Aktie.Pris.ToString(), _trend.TrendNU.ToString(), faktor.ToString(), _trend.MinDagFore.ToString(), AntalAffarer, MaxAntalKop);
                            Message.Log(MessageType.Info, sText);
                            
                        
                        }
                    }
                    if ((_Aktie.Pris / _trend.Min02) >= Convert.ToDecimal(1.006)
                      & (_trend.TrendNU > Convert.ToDecimal(-0.2))
                       & (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 05, 00))
                       & (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 30, 00))
                        )
                    {
                        if (AntalAffarer < MaxAntalKop)
                        {
                            Message.Log(MessageType.Info, string.Format("Över 1,0 och under 2,0 Köper {0} Pris = {1} TrendNU = {2} faktor = {3} MinDagFore = {4}", _Aktie.Namn, _Aktie.Pris.ToString(), _trend.TrendNU.ToString(), faktor.ToString(), _trend.MinDagFore.ToString()));
                            Buy.Buyer(_driver, _Aktie,_AktieUrl);
                            AktierListBuy.Add(_Aktie);
                            db.Insert("Buy", "Id", _Aktie);
                            AntalAffarer++;

                            var sText = (string.Format("Köper =  {0} pris = {1} pris_min = {2} pris_max = {12} Min20 = {3} Min15 = {4} Min05 = {5} Min02 = {6}  trend20 = {7} trend15 = {8} trend10 = {9} trend05 = {10} trendNU = {11} listnr = {12} Under_high = {13} AntalAffärer = {14} MaxAntalAffärer = {15}"
                                                , _Aktie.Namn.ToString(), _Aktie.Pris.ToString(), _AktieLow.Pris.ToString()
                                                , _trend.Min20.ToString().ToString(), _trend.Min15.ToString(), _trend.Min05.ToString()
                                                , _trend.Min02.ToString(), _trend.Trend20.ToString(), _trend.Trend15.ToString()
                                                , _trend.Trend10.ToString(), _trend.Trend05.ToString(), _trend.TrendNU.ToString()
                                                , ind, (_AktieHigh.Pris / _Aktie.Pris), AntalAffarer, MaxAntalKop, _AktieHigh.Pris.ToString()));
                            Message.Log(MessageType.Info, sText);
                            _AktieHigh.Pris = _Aktie.Pris;
                            _AktieLow.Pris = _Aktie.Pris;

                            var newprice = (_Aktie.Pris * Convert.ToDecimal(1.013));

                            retu = true;
                        }
                        else
                        {
                            var sText = string.Format("Skulle köpt Över 0,5 sista 2 min {0} Pris = {1} TrendNU = {2} faktor = {3} MinDagFore = {4} AntalAffärer = {5} MaxAntalAffärer = {6}", _Aktie.Namn, _Aktie.Pris.ToString(), _trend.TrendNU.ToString(), faktor.ToString(), _trend.MinDagFore.ToString(), AntalAffarer, MaxAntalKop);
                            Message.Log(MessageType.Info, sText);


                        }
                    }

                }
            }
            return retu;
        }
        
         public static Boolean Seller(IWebDriver _driver
                                     , Database db
                                     , Aktie _Aktie
                                     , ref int AntalAffarer
                                     , List<Aktie> AktierListHigh
                                     , ref List<Aktie> AktierListBuy
                                     , ref List<Aktie> AktierListSell
                                     , List<Aktie> AktierListLow)
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

                    //decimal underMax;
                    decimal overbuy;
                    decimal overbuy2;
                    decimal underbuy;
                    //if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 00, 00))
                    //{
                    overbuy = Convert.ToDecimal(1.015);
                    overbuy2 = Convert.ToDecimal(1.008);
                    underbuy = Convert.ToDecimal(1.04);
                   
                    var sellType = 0;
                    var daysadd1 = 2;
                    var daysadd2 = 4;

                    if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                    {
                        daysadd1 = 4;
                        daysadd2 = 6;
                    }

                    if ((_AKtieBuy.Pris / _Aktie.Pris) > Convert.ToDecimal(underbuy))
                    {
                        sellType = 1;
                    }
                    if ((_Aktie.Pris / _AKtieBuy.Pris) > Convert.ToDecimal(overbuy)) // & _Aktie.Pris < _trend.Min01)
                    {
                        sellType = 2;
                    }

                    //if ((DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 20, 00))
                     if ((_Aktie.Pris / _AKtieBuy.Pris) > Convert.ToDecimal(overbuy2) // & _Aktie.Pris < _trend.Min01)
                      & (_AKtieBuy.DateTime.AddDays(daysadd1) < DateTime.Now))
                    {
                        sellType = 3;
                    }

                    if ((_Aktie.Procent > Convert.ToDecimal(1.5))
                     & (_AKtieBuy.DateTime.AddDays(daysadd1) < DateTime.Now))
                    {
                        sellType = 4;
                    }

                    if ((_Aktie.Procent > Convert.ToDecimal(1.0))
                     & ((_AKtieBuy.DateTime.AddDays(daysadd2) < DateTime.Now)))
                    // & (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 15, 00))))
                    {
                        sellType = 5;
                    }

                    if (sellType > 0
                        & _trend.Min10 <= _trend.Min05
                        & _trend.Min05 <= _trend.Min03
                        & _trend.Min03 <= _trend.Min01
                        & _trend.Min01 <= _Aktie.Pris)
                    {
                        sellType = 0;
                        Message.Log(MessageType.Info, string.Format("Ska inte sälja = {0} pris_min20 = {1} pris_min10 = {2}  pris_min05 = {3} pris_nu = {4}", _Aktie.Namn, _trend.Min20, _trend.Min10, _trend.Min05, _Aktie.Pris));
                    }

                    if (sellType > 0)
                    {
                        AktierListSell.Add(_Aktie);
                        db.Insert("Sell", "Id", _Aktie);

                        var _AKtieLow = AktierListLow.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
                        Sell.Seller(_driver, _Aktie, true);
                        AntalAffarer--;

                        var sText = (string.Format("Säljer = {0} aktie_procent = {13} pris_min = {12} pris_max = {2} Min20 = {3} Min15 = {4} Min05 = {5} Min01 = {6} pris = {1} trend30 = {14} trend25 = {13} trend20 = {7} trend15 = {8} trend10 = {9} trend05 = {10} trendNU = {11}  selltype = {16} overbuy = {17}"
                                            , _Aktie.Namn.ToString(), _Aktie.Pris.ToString(), _AKtieHigh.Pris.ToString()
                                            , _trend.Min20.ToString(), _trend.Min15.ToString(), _trend.Min05.ToString()
                                            , _trend.Min01.ToString(), _trend.Trend20.ToString(), _trend.Trend15.ToString()
                                            , _trend.Trend10.ToString(), _trend.Trend05.ToString(), _trend.TrendNU.ToString()
                                            , _AKtieLow.Pris.ToString(), _trend.Trend25.ToString(), _trend.Trend30.ToString()
                                            , _Aktie.Procent, sellType.ToString(), overbuy.ToString()));
                        Message.Log(MessageType.Info, sText);
                        //AktierListBuy.Remove(_Aktie);
                        retu = true;
                    }
                }
            }
            return retu;
        }

        public static void CheckLow( Aktie _Aktie
                                    ,List<Aktie> AktierListLow)
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
        public static void CheckHigh(Aktie _Aktie
                                     , List<Aktie> AktierListHigh
                                     )
        {
            var _AktieHigh = AktierListHigh.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            if (_AktieHigh == null)
            {
                _AktieHigh = (Aktie)_Aktie.Clone();
                AktierListHigh.Add(_AktieHigh);
            }
            if (_AktieHigh.Pris < _Aktie.Pris)
            {
                _AktieHigh.Pris = _Aktie.Pris;
            }
        }
    }
}
