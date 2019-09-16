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
    class CheckBlankning
    {
        public Boolean CheckBlankningar( Database db
                                     , Aktie _Aktie
                                     , int AntalBlanka
                                     , List<Aktie> AktierListHigh
                                     , List<Aktie> AktierListBlanka
                                     )
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
        public Boolean CheckBlankningReturn( Database db
                                     , Aktie _Aktie
                                     , int AntalBlanka
                                     , List<Aktie> AktierListBlankaReturn
                                     , List<Aktie> AktierListBlanka
                                     , List<Aktie> AktierListLow)
        {
            var retu = false;
            var _AKtieBlanka = AktierListBlanka.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
            if (_AKtieBlanka != null)
            {
                var _AKtieBlankaReturn = AktierListBlankaReturn.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);
                if (_AKtieBlankaReturn == null)
                {
                    var _AktieLow = AktierListLow.Find(item => item.Aktie_ID == _Aktie.Aktie_ID);

                    if (((_Aktie.Pris / _AktieLow.Pris) > Convert.ToDecimal(1.02)
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

