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
                var msg = new DAL.Message()
                {
                 DateTime = DateTime.Now 
                ,Program = "PengarMaskin" 
                ,Severity = s.ToString() 
                ,Text = text 
                };
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
