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
                                     , int AntalAffarer
                                     , List<Aktie> AktierListHigh
                                     , List<Aktie> AktierListBuy
                                     , List<Aktie> AktierListLow)
        {
            var retu = false;
            var _AktieLow = AktierListLow.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            var _AKtieBuy = AktierListBuy.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            var _AktieHigh = AktierListHigh.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            Decimal _faktor_del1 = Convert.ToDecimal(0.99);

            if (_AKtieBuy == null)
            {

                if (( //((_Aktie.Pris / _AktieLow.Pris) > Convert.ToDecimal(1.002))  
                      // (_Aktie.Procent > Convert.ToDecimal(0.3))
                      //& (_Aktie.Procent < Convert.ToDecimal(4.0))
                  (_Aktie.Omsatt > 100000)
                & (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 10, 00))
                & (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 25, 00))
                & _Aktie.Aktie_ID != 4870 //Fingerprint))
                & _Aktie.Namn.TrimEnd().Substring(_Aktie.Namn.Length - 1, 1) != "A")
                //|| (ind <=5
                //    & _Aktie.Namn.TrimEnd().Substring(_Aktie.Namn.Length - 1,1) != "A"
                //    & (_Aktie.Omsatt > 100000))
                    )
                {

                    var x = new DAL();
                    DAL.Trend _trend = x.GetTrend(db, _Aktie.Aktie_ID);

                    int MaxAntalKop = int.Parse(ConfigurationManager.AppSettings["MaxAntalKop"]);
                    //if (AntalAffarer < MaxAntalKop)
                    //{
                    //    if (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 00, 00))
                    //    {
                    //        if (AntalAffarer < MaxAntalKop & _faktor_del1 <= 1)
                    //        {
                    //            _faktor_del1 = _faktor_del1 + Convert.ToDecimal(0.001);
                    //        }
                    //        else
                    //        {
                    //            _faktor_del1 = Convert.ToDecimal(0.99);
                    //        }
                    //    }
                    //}
                    decimal faktor;

                    if (_trend.TrendNU < 0)
                    {
                        faktor = (_trend.TrendNU * Convert.ToDecimal(0.01)) + _faktor_del1;
                    }
                    else
                    {
                        faktor = _faktor_del1;
                    }

                    if (_Aktie.Pris < _trend.MinDagFore * faktor)
                    {


                        if (AntalAffarer < MaxAntalKop)
                        {
                            Message.Log(MessageType.Info, string.Format("Lägre än igår köper {0} Pris = {1} TrendNU = {2} faktor = {3} MinDagFore = {4}", _Aktie.Namn, _Aktie.Pris.ToString(), _trend.TrendNU.ToString(), faktor.ToString(), _trend.MinDagFore.ToString()));
                            Buy.Buyer(_driver, _Aktie);
                            AktierListBuy.Add(_Aktie);
                            db.Insert("Buy", "Id", _Aktie);
                            AntalAffarer++;

                            var sText = (string.Format("Köper = {0} pris_min = {2} pris_max = {12} Min20 = {3} Min15 = {4} Min05 = {5} Min01 = {6} pris = {1} trend20 = {7} trend15 = {8} trend10 = {9} trend05 = {10} trendNU = {11} listnr = {12} Under_high = {13}"
                                                , _Aktie.Namn.ToString(), _Aktie.Pris.ToString(), _AktieLow.Pris.ToString()
                                                , _trend.Min20.ToString().ToString(), _trend.Min15.ToString(), _trend.Min05.ToString()
                                                , _trend.Min01.ToString(), _trend.Trend20.ToString(), _trend.Trend15.ToString()
                                                , _trend.Trend10.ToString(), _trend.Trend05.ToString(), _trend.TrendNU.ToString()
                                                , ind, (_AktieHigh.Pris / _Aktie.Pris)));
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

                }
            }
            return retu;
        }
    }
}
