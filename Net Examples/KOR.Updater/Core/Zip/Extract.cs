using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using KOR.Updater.Controllers;
using System.IO;
using System.Threading;

namespace KOR.Updater.Core.Helpers
{
    public class Extract
    {
        #region Extraction Constructors

        /// <summary>
        /// Extract file path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// File password for extraction
        /// </summary>
        public string FilePassword { get; set; }

        /// <summary>
        /// Extraction folder
        /// </summary>
        public string OutputFolder { get; set; }

        /// <summary>
        /// Extraction state info
        /// </summary>
        public bool ExtractionState { get; set; }

        /// <summary>
        /// Zip file password
        /// </summary>
        public string ZipFilePassword { get; set; }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract file and folders in .zip file
        /// </summary>
        /// <param name="archivefilename">dwonloaded archive file</param>
        /// <param name="password">archive password</param>
        /// <param name="outputfolder">output folder</param>
        /// <returns></returns>
        public void ExtractZipFile()
        {
            // if file exist
            if (File.Exists(FilePath))
            {
                // setting a null zip file
                ZipFile zipfile = null;

                // strring FileStream to file open and read
                FileStream filestream = File.OpenRead(FilePath);
                zipfile = new ZipFile(filestream);
                if (!string.IsNullOrEmpty(FilePassword))
                {
                    // setting password
                    zipfile.Password = FilePassword;
                }

                // foreach every zip entry in zipfile
                foreach (ZipEntry zipentry in zipfile)
                {
                    if (!zipentry.IsFile)
                    {
                        continue;
                    }

                    string enrtyfilename = zipentry.Name;

                    // buffer for memory leak
                    byte[] buffer = new byte[4096];
                    Stream zipStream = zipfile.GetInputStream(zipentry);
                    string fullZipToPath = Path.Combine(OutputFolder, enrtyfilename);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        if (directoryName.Length > 0) Directory.CreateDirectory(directoryName);
                    }

                    // stream writer
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }

                    Thread.Sleep(500);
                }

                if (zipfile != null)
                {
                    zipfile.IsStreamOwner = true;
                    zipfile.Close();
                }

                // delete extracted file
                FileController.DeleteFile(FilePath);
            }
        }

        #endregion
    }
}
