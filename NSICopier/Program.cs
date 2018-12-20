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
                if (File.Exists("Settings.xml"))
                {
                    Logica nsiSender = new Logica();
                    nsiSender.copyToAll();
                }
                else
                {
                    Console.WriteLine("Settings.xml не найден");
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
