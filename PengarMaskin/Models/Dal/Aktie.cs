using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PengarMaskin
{
    public class Aktie : ICloneable
    {
        public Int32 Aktie_ID { get; set; }
        public String Namn { get; set; }
        public Decimal Pris { get; set; }
        public Decimal Change { get; set; }
        public Decimal Procent { get; set; }
        public Int64 Omsatt { get; set; }
        public DateTime DateTime { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    public class AktieUtveckling : ICloneable
    {
        public Int32 Aktie_ID { get; set; }
        public String Namn { get; set; }
        public Decimal Idag { get; set; }
        public Decimal EnVecka { get; set; }
        public Decimal EnMan { get; set; }
        public Decimal TreMan { get; set; }
        public Decimal SexMan { get; set; }
        public Decimal EttAr { get; set; }
        public Decimal TvaAr { get; set; }
        public Decimal TreAr { get; set; }
        public Decimal FemAr { get; set; }
        public DateTime DateTime { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
