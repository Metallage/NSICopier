using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NSICopier
{
    class NSIDestination
    {
        //Куда собственно копируем
        string destinationPath;
        //Нужно ли архивировать перед отправкой
        bool needArch;


        public NSIDestination(string destinationPath, bool needArch)
        {
            this.destinationPath = destinationPath;
            this.needArch = needArch;
        }


        //В перспективе при неудаче она должна повторно пытаться создать директорию
        private bool CreateOrCheckDestination()
        {
            bool exists = false;


            if (Directory.Exists(destinationPath))
            {
                exists = true;
            }
            else
            {
                Directory.CreateDirectory(destinationPath);
                exists = true;
            }

            return exists;
        }

       public void CopyToDestination(string sourceDir, string archPath)
        {
            bool isPossible = CreateOrCheckDestination();

            if(isPossible)
            {
                if(needArch)
                {
                    FileInfo archNSI = new FileInfo(archPath);
                    CopyArch(archNSI);
                }
                else
                {
                    DirectoryInfo sDir = new DirectoryInfo(sourceDir);
                    CopyNoArch(sDir);
                }
            }

        }

        private void CopyArch(FileInfo archNSI)
        {
            archNSI.CopyTo(destinationPath + archNSI.Name);
        }

        private void CopyNoArch(DirectoryInfo dir)
        {
            foreach(FileInfo nsiFile in dir.GetFiles())
            {
                nsiFile.CopyTo(destinationPath+nsiFile.Name);
            }

        }

        /// <summary>
        /// Нужно ли архивировать
        /// </summary>
        public bool NeedArch
        { get; }
    }
}
