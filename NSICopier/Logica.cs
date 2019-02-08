using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

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

        /// <summary>
        /// Массив целевых папок
        /// </summary>
        List<NSIDestination> destinations = new List<NSIDestination>();

        bool clearSource = false;


        public Logica(string settingsPath)
        {
            ParseSettings(settingsPath);

            if(!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
        }

        /// <summary>
        /// Копируем всё 
        /// </summary>
        public void copyToAll()
        {
            string archPATH = tempPath + "archNSI_" + DateTime.Now.ToShortDateString() + ".7z";

            AddToArchive(archiver,sourceFiles+"*.*",archPATH);

            foreach(NSIDestination destination in this.destinations)
            {
                destination.CopyToDestination(sourceFiles, archPATH);
            }

            ClearAll();
        }
        
        
        
        /// <summary>
        /// Нашел в гугле, функция для архивании с помощью 7z
        /// </summary>
        /// <param name="archiver">путь к архиватору</param>
        /// <param name="fileNames">файлы которые нужно загнать в архив (можно *.*)</param>
        /// <param name="archiveName">путь к архиву</param>
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

        /// <summary>
        /// Чтение настроек из XML
        /// </summary>
        /// <param name="fileName">xml файл с настройками</param>
        private void ParseSettings(string fileName)
        {
            //Читаем и определяем корень
            XmlDocument settings = new XmlDocument();
            settings.Load(fileName); 
            XmlElement rootSetting = settings.DocumentElement;

            //Чтение параметров временной папки
            XmlNode tempSetting = rootSetting.SelectSingleNode("//options/temp");
            tempPath = tempSetting.InnerText;

            //Чтение параметров папки с файлами для копирования
            XmlNode sourceSetting = rootSetting.SelectSingleNode("//options/source");
            sourceFiles = sourceSetting.InnerText;

            //Чтение пути к архиватору
            XmlNode archiverSetting = rootSetting.SelectSingleNode("//options/archiver");
            archiver = archiverSetting.InnerText;

            //Нужно ли удалять исходники
            XmlNode clearSetting = rootSetting.SelectSingleNode("//options/clearSource");
            if (clearSetting.InnerText.ToLower() == "yes")
            {
                clearSource = true;
            }
            

            //Чтение настроек целевых папок
            XmlNodeList destinations = rootSetting.SelectNodes("//destinations/destination");

            foreach(XmlNode destination in destinations)
            {
                bool needArch = false;

                if(destination.SelectSingleNode("needArch").InnerText.ToLower() == "yes")
                {
                    needArch = true;
                }

                this.destinations.Add(new NSIDestination(destination.SelectSingleNode("directory").InnerText, needArch));
                
            }
        }

        /// <summary>
        /// Удаляет исходники и темпы
        /// </summary>
        private void ClearAll()
        {
            if (clearSource)
            {
                DirectoryInfo scf = new DirectoryInfo(sourceFiles);
                foreach (FileInfo fi in scf.GetFiles())
                {
                    fi.Delete();
                }
            }

            DirectoryInfo tmp = new DirectoryInfo(tempPath);
            foreach(FileInfo fi2 in tmp.GetFiles())
            {
                fi2.Delete();
            }           
        }
    }
}
