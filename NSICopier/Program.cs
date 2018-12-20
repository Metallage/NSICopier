using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSICopier
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Logica nsiSender = new Logica();
                nsiSender.copyToAll();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
        }
    }
}
