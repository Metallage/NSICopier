using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

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
            string archPATH = tempPath + "archNSI_" + DateTime.Now.ToShortDateString() + ".7z";

            AddToArchive(archiver,sourceFiles+"*.*",archPATH);

            List<NSIDestination> destinations = new List<NSIDestination>();

            NSIDestination nsiDes = new NSIDestination(@"c:\temp\nsiout\", false);
            destinations.Add(nsiDes);

            foreach(NSIDestination destination in destinations)
            {
                destination.CopyToDestination(sourceFiles, archPATH);
            }
        }

        private void AddToArchive(string archiver, string fileNames, string archiveName)
        {
            try
            {
                // Предварительные проверки
                if (!File.Exists(archiver))
                    throw new Exception("Архиватор 7z по пути \"" + archiver +
                    "\" не найден");

                // Формируем параметры вызова 7z
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = archiver;
                // добавить в архив с максимальным сжатием
                startInfo.Arguments = " a -mx9 ";
                // имя архива
                startInfo.Arguments += "\"" + archiveName + "\"";
                // файлы для запаковки
                startInfo.Arguments += " \"" + fileNames + "\"";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                int sevenZipExitCode = 0;
                using (Process sevenZip = Process.Start(startInfo))
                {
                    sevenZip.WaitForExit();
                    sevenZipExitCode = sevenZip.ExitCode;
                }
                // Если с первого раза не получилось,
                //пробуем еще раз через 1 секунду
                if (sevenZipExitCode != 0 && sevenZipExitCode != 1)
                {
                    using (Process sevenZip = Process.Start(startInfo))
                    {
                        Thread.Sleep(1000);
                        sevenZip.WaitForExit();
                        switch (sevenZip.ExitCode)
                        {
                            case 0: return; // Без ошибок и предупреждений
                            case 1: return; // Есть некритичные предупреждения
                            case 2: throw new Exception("Фатальная ошибка");
                            case 7: throw new Exception("Ошибка в командной строке");
                            case 8:
                                throw new Exception("Недостаточно памяти для выполнения операции");
                            case 225:
                                throw new Exception("Пользователь отменил выполнение операции");
                            default:
                                throw new Exception("Архиватор 7z вернул недокументированный код ошибки: " + sevenZip.ExitCode.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("SevenZip.AddToArchive: " + e.Message);
            }
        }

    }
}
