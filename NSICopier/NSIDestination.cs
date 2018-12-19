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

        private void CopyToDestination(string file)
        {
            FileInfo copingFile = new FileInfo(file);
            FileInfo destFile = new FileInfo(destinationPath + copingFile.Name);
            copingFile.CopyTo(destFile.FullName, true);
            //FileStream fsCopy = new FileStream(destFile.FullName, FileMode.Truncate);
            //if(fsCopy.CanWrite)
            //{
            //    fsCopy.Wri
            //}

        }

        /// <summary>
        /// Нужно ли архивировать
        /// </summary>
        public bool NeedArch
        { get; }
    }
}
