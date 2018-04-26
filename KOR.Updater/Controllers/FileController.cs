using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace KOR.Updater.Controllers
{
    /// <summary>
    /// File Controller contains
    /// </summary>
    public class FileController
    {
        #region File Using Check

        /// <summary>
        /// Control file is using
        /// </summary>
        /// <param name="file">File info</param>
        /// <returns></returns>
        public static bool IsFileBusy(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return false;
        }

        /// <summary>
        /// Deleting dwonloaded archive
        /// </summary>
        /// <param name="archivefilename">downloaded archive file</param>
        /// <returns></returns>
        public static async Task<bool> DeleteFile(string archivefilename)
        {
            bool archiveisdeleted = false;

            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    FileInfo file = new FileInfo(archivefilename);
                    while (FileController.IsFileBusy(file)) await Task.Delay(500); file.Delete();
                })
                .ContinueWith((T) =>
                {
                    if (!File.Exists(archivefilename))
                    {
                        archiveisdeleted = true;
                    }
                });
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Error from Delete File Task:--->" + exp.ToString());
                throw;
            }

            return archiveisdeleted;
        }

        #endregion
    }
}
