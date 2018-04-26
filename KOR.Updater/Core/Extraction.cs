using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using KOR.Updater.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOR.Updater.Core
{
    public class Extractor
    {
        #region Extraction

        #region Extraction Constructors

        /// <summary>
        /// Extraction state
        /// </summary>
        public bool _DownloadedFileIsExtracted;
        public bool DownloadedFileIsExtracted
        {
            get { return _DownloadedFileIsExtracted; }
            set
            {
                if (_DownloadedFileIsExtracted != value)
                {
                    _DownloadedFileIsExtracted = value;
                }
            }
        }

        /// <summary>
        /// Extraction state info
        /// </summary>
        public string _ExtractionState;
        public string ExtractionState
        {
            get { return _ExtractionState; }
            set
            {
                if (_ExtractionState != value)
                {
                    _ExtractionState = value;
                }
            }
        }

        /// <summary>
        /// Zip file password
        /// </summary>
        public string _ZipFilePassword;
        public string ZipFilePassword
        {
            get { return _ZipFilePassword; }
            set
            {
                if (_ZipFilePassword != value)
                {
                    _ZipFilePassword = value;
                }
            }
        }

        #endregion

        #region Extraction Methods

        /// <summary>
        /// Exreact xip file in file or folders
        /// </summary>
        /// <param name="archivefilename">dwonloaded archive file</param>
        /// <param name="password">archive password</param>
        /// <param name="outputfolder">output folder</param>
        /// <returns></returns>
        public async Task ExtractZipFile(string archivefilename, string password, string outputfolder)
        {
            // if file exist
            if (File.Exists(archivefilename))
            {
                Download.DownloadedFileIsExist = true;

                // setting a null zip file
                ZipFile zipfile = null;
                try
                {
                    // strring FileStream to file open and read
                    FileStream filestream = File.OpenRead(archivefilename);
                    zipfile = new ZipFile(filestream);
                    if (!string.IsNullOrEmpty(password))
                    {
                        // setting password
                        zipfile.Password = password;
                        ExtractionState = "Dosya İçeriği Taranıyor...";
                    }

                    // foreach every zip entry in zipfile
                    foreach (ZipEntry zipentry in zipfile)
                    {
                        if (!zipentry.IsFile)
                        {
                            continue;
                        }

                        string enrtyfilename = zipentry.Name;
                        ExtractionState = zipentry.Name + " Çıkartılıyor...";

                        await Task.Delay(500);
                        // buffer for memory leak
                        byte[] buffer = new byte[4096];
                        Stream zipStream = zipfile.GetInputStream(zipentry);
                        string fullZipToPath = Path.Combine(outputfolder, enrtyfilename);
                        string directoryName = Path.GetDirectoryName(fullZipToPath);
                        if (directoryName.Length > 0) Directory.CreateDirectory(directoryName);

                        // stream writer
                        using (FileStream streamWriter = File.Create(fullZipToPath))
                        {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }
                    }
                }
                finally
                {
                    if (zipfile != null)
                    {
                        zipfile.IsStreamOwner = true;
                        zipfile.Close();
                    }

                    ExtractionState = "Güncellemeden arta kalan dosyalar temizleniyor...";
                    await Task.Delay(1000);

                    // delete downloaded file
                    if (await FileController.DeleteFile(archivefilename))
                    {
                        // Close updater and start main exe
                    }
                }

            }
        }

        #endregion

        #endregion
    }
}
