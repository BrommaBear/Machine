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
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
           // Application.SetCompatibleTextRenderingDefault(false);
           
            Console.WriteLine("Press the Enter key to exit the program at any time... ");
            Console.ReadLine();

            Application.Run(new Form1());
           
        }
        
       



    }

    
    
}
