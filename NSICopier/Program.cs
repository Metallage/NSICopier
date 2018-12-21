using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NSICopier
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string settingsPath = "Settings.xml";
                if (args.Length > 0)
                {
                    settingsPath = args[0];
                }

                if (File.Exists(settingsPath))
                {
                    Logica nsiSender = new Logica(settingsPath);
                    nsiSender.copyToAll();
                }
                else
                {
                    Console.WriteLine(settingsPath + " не найден");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
        }
    }
}
