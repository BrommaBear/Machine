using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco; 

namespace PengarMaskin
{
    class DAL 
    {
        readonly Database _db ;
        public DAL()
        {
            _db = new Database("Bjorn");
        }
        public Trend GetTrend(Database db,int Aktie_ID) {
            var dt = DateTime.Now;
            var today = DateTime.Today;
            var DagFore = today.AddDays(-1);
            if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
            {
                 DagFore = today.AddDays(-3);
            }
          
            //dt = DateTime.Now.AddHours(-6);
            var sql = new PetaPoco.Sql()
           .Append("Select ")
           .Append("Trend30 = (select top 1 procent from BorsTrend  where DateTime > dateadd(mi,-30,@0))", dt)
           .Append(",Trend25 = (select top 1 procent from BorsTrend  where DateTime > dateadd(mi,-25,@0))", dt)
           .Append(",Trend20 = (select top 1 procent from BorsTrend  where DateTime > dateadd(mi,-20,@0))", dt)
           .Append(",Trend15 = (select top 1 procent from BorsTrend  where DateTime > dateadd(mi,-15,@0))", dt)
           .Append(",Trend10 = (select top 1 procent from BorsTrend  where DateTime > dateadd(mi,-10,@0))", dt)
           .Append(",Trend05 = (select top 1 procent from BorsTrend  where DateTime > dateadd(mi,-5,@0))", dt)
           .Append(",TrendNU = (select procent from BorsTrend where DateTime = (Select max(DateTime) from BorsTrend))")
           .Append(",Min20   = (select top 1 pris from history where aktie_id = @0 and DateTime > dateadd(mi,-20,@1))", Aktie_ID, dt)
           .Append(",Min15   = (select top 1 pris from history where aktie_id = @0 and DateTime > dateadd(mi,-15,@1))", Aktie_ID, dt)
           .Append(",Min10   = (select top 1 pris from history where aktie_id = @0 and DateTime > dateadd(mi,-10,@1))", Aktie_ID, dt)
           .Append(",Min07   = (select top 1 pris from history where aktie_id = @0 and DateTime > dateadd(mi,-07,@1))", Aktie_ID, dt)
           .Append(",Min05   = (select top 1 pris from history where aktie_id = @0 and DateTime > dateadd(mi,-05,@1))", Aktie_ID, dt)
           .Append(",Min03   = (select top 1 pris from history where aktie_id = @0 and DateTime > dateadd(mi,-03,@1))", Aktie_ID, dt)
           .Append(",Min01   = (select top 1 pris from history where aktie_id = @0 and DateTime > dateadd(mi,-01,@1))", Aktie_ID, dt)
           .Append(",MinDagFore = (select min(pris) from history where aktie_id = @0 and DateTime > @1 and DateTime < @2)", Aktie_ID, DagFore,DagFore.AddDays(+1))
           
           ;

            List<Trend> ret =  db.Query<Trend>(sql).ToList();
            return ret[0];
        }

        public AktieID GetAktieID(string Name)
        {
            var sql = new PetaPoco.Sql()
            .Append("SELECT Aktie_ID ")
            .Append(",Namn ")
            .Append("FROM Aktie ")
            .Append("Where Namn = @0", Name);
                    
            List <AktieID> ret = _db.Query<AktieID>(sql).ToList();
            return ret[0];
        }






        [TableName("History")]
        [PrimaryKey("Id")]
        public class History
        {
            public int Id { get; set; }
            public int Aktie_ID { get; set; }
            public string Namn { get; set; }
            public decimal Pris { get; set; }
            public decimal Change { get; set; }
            public decimal Procent { get; set; }
            public DateTime DateTime{ get; set; }
        }

        [TableName("AktieUtv")]
        [PrimaryKey("Id")]
        public class AktieUtv
        {
            public int Id { get; set; }
            public int Aktie_ID { get; set; }
            public string Namn { get; set; }
            public decimal Idag { get; set; }
            public decimal EnVecka { get; set; }
            public decimal EnMan { get; set; }
            public decimal TreMan { get; set; }
            public decimal SexMan { get; set; }
            public decimal EttAr { get; set; }
            public decimal TvaAr { get; set; }
            public decimal TreAr { get; set; }
            public decimal FemAr { get; set; }      
            public DateTime DateTime { get; set; }
        }

        [TableName("Aktie")]
        [PrimaryKey("Id")]
        public class AktieID
        {
            public int Id { get; set; }
            public int Aktie_ID { get; set; }
            public string Namn { get; set; }
            
        }


        [TableName("Buy")]
        [PrimaryKey("Id")]
        public class Buy
        {
            public int Id { get; set; }
            public int Aktie_ID { get; set; }
            public string Namn { get; set; }
            public decimal Pris { get; set; }
            public decimal Change { get; set; }
            public decimal Procent { get; set; }
            public DateTime DateTime { get; set; }
        }

        [TableName("BlankaReturn")]
        [PrimaryKey("Id")]
        public class BlankaReturn
        {
            public int Id { get; set; }
            public int Aktie_ID { get; set; }
            public string Namn { get; set; }
            public decimal Pris { get; set; }
            public decimal Change { get; set; }
            public decimal Procent { get; set; }
            public DateTime DateTime { get; set; }
        }

        [TableName("Blanka")]
        [PrimaryKey("Id")]
        public class Blanka
        {
            public int Id { get; set; }
            public int Aktie_ID { get; set; }
            public string Namn { get; set; }
            public decimal Pris { get; set; }
            public decimal Change { get; set; }
            public decimal Procent { get; set; }
            public DateTime DateTime { get; set; }
        }

        [TableName("Sell")]
        [PrimaryKey("Id")]
        public class Sell
        {
            public int Id { get; set; }
            public int Aktie_ID { get; set; }
            public string Namn { get; set; }
            public decimal Pris { get; set; }
            public decimal Change { get; set; }
            public decimal Procent { get; set; }
            public DateTime DateTime { get; set; }
        }

        [TableName("Message")]
        [PrimaryKey("Id")]
        public class Message
        {
            public int Id { get; set; }
            public DateTime DateTime { get; set; }
            public string Severity { get; set; }
            public string Program { get; set; }
            public string Text { get; set; }
        }

        [TableName("BorsTrend")]
        [PrimaryKey("Id")]
        public class BorsTrend
        {
            public int Id { get; set; }
            public decimal Procent { get; set; }
            public DateTime DateTime { get; set; }
        }

        public class Trend
        {
            public int Id { get; set; }
            public decimal Trend30 { get; set; }
            public decimal Trend25 { get; set; }
            public decimal Trend20 { get; set; }
            public decimal Trend15 { get; set; }
            public decimal Trend10 { get; set; }
            public decimal Trend05 { get; set; }
            public decimal TrendNU { get; set; }
            public decimal Min20 { get; set; }
            public decimal Min15 { get; set; }
            public decimal Min10 { get; set; }
            public decimal Min07 { get; set; }
            public decimal Min05 { get; set; }
            public decimal Min03 { get; set; }
            public decimal Min01 { get; set; }
            public decimal MinDagFore { get; set; }          
        }


    }
}
