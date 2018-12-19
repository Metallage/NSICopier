using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NSICopier
{
    class Logica
    {
        //Откуда копируем
        string sourceFiles;

        //Временная папка
        string tempPath;

        //Путь к 7z 
        string archiver;

        public Logica()
        {
            tempPath = @"c:\temp\copyNsi\";
            sourceFiles = @"c:\temp\fromNsi\";
            archiver = @"C:\temp\unp\7z\7z.exe";

            if(!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
        }

        public void copyToAll()
        {
            List<NSIDestination> destinations = new List<NSIDestination>();

            NSIDestination nsiDes = new NSIDestination(@"c:\temp\nsiout\", false);
            destinations.Add(nsiDes);
        }


        
    }
}
