using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace PengarMaskin
{
    static class Message
    {
        public static void Log(MessageType s, string text)
        {
            using (var db = new Database("Bjorn"))
            {
                var msg = new DAL.Message();
                msg.DateTime = DateTime.Now;
                msg.Program = "PengarMaskin";
                msg.Severity = s.ToString();
                msg.Text = text;
                db.Insert("Message", "Id", msg);
            }


        }
        
    }
    public enum MessageType
    {
        Info, 
        Error
    }

}
