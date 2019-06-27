using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace PengarMaskin
{
    class Program
    {
        static void Main()
        {
            PengarMaskin pm  = new PengarMaskin();
            var x = 1;
            while (x == 1)
            {
                try
                {
                    pm.Run();
                }
                catch (WebDriverException ex)
                {
                    Message.Log(MessageType.Info, "Error i ListRefresh");
                    Message.Log(MessageType.Info, ex.Message);
                    Message.Log(MessageType.Error, "Hit ska vi aldrig komma");
                }
            }

        }
    }
}
