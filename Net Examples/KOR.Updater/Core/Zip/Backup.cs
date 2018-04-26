using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace KOR.Updater.Core.Helpers
{
    public class Backup
    {
        #region Backup Constructors

        // Important settings

        /// <summary>
        /// Backup storing folder path
        /// </summary>
        public string BackupStoreFolder { get; set; }

        /// <summary>
        /// Compressin master folder
        /// </summary>
        public string BackupFolder { get; set; }

        /// <summary>
        /// Backup file name formatting, default value is "backup-{0:yyyy-MM-dd_hh-mm-ss-tt}.zip"
        /// </summary>
        public string BackupFileNameFormat = "backup-{0:yyyy-MM-dd_hh-mm-ss-tt}.zip";

        /// <summary>
        /// password for compressing
        /// </summary>
        public string BackupFilePassword { get; set; }

        /// <summary>
        /// Extraction state info
        /// </summary>
        public bool BackupState { get; set; }

        /// <summary>
        /// Zip file password
        /// </summary>
        public string ZipFilePassword { get; set; }

        // Other settings

        /// <summary>
        /// Directories to be ignored during compression
        /// </summary>
        public List<DirectoryInfo> IgnoreDirs = new List<DirectoryInfo>();

        /// <summary>
        /// Files to be ignored during compression
        /// </summary>
        public List<FileInfo> IgnoreFiles = new List<FileInfo>();

        /// <summary>
        /// Compression level default 3, min 0, max 9
        /// </summary>
        public int CompressionLevel = 3;

        #endregion

        #region Backup

        /// <summary>
        /// Compresses the files in the nominated folder, and creates a zip file on disk named as BackupStoreFoldername.
        /// </summary>
        /// <returns></returns>
        public void CreateBackup()
        {
            if (!Directory.Exists(BackupStoreFolder))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(BackupStoreFolder));
            }

            string backupfile = BackupStoreFolder + @"\" + string.Format(BackupFileNameFormat, DateTime.Now);
            FileStream fsOut = File.Create(backupfile);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(CompressionLevel); //0-9, 9 being the highest level of compression

            zipStream.Password = BackupFilePassword;  // optional. Null is the same as not setting. Required if using AES.

            // This setting will strip the leading part of the folder path in the entries, to
            // make the entries relative to the starting folder.
            // To include the full path for each entry up to the drive root, assign folderOffset = 0.
            int folderOffset = BackupFolder.Length + (BackupFolder.EndsWith("\\") ? 0 : 1);

            CompressFolder(BackupFolder, zipStream, folderOffset);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();

            BackupState =  File.Exists(backupfile) ? true : false;
        }

        // Recurses down the folder structure
        //
        void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {

            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            string[] folders = Directory.GetDirectories(path);

            if (IgnoreDirs != null)
            {
                foreach (string folder in folders)
                {
                    foreach (var ignoredir in IgnoreDirs)
                    {
                        if (ignoredir.FullName != folder)
                        {
                            CompressFolder(folder, zipStream, folderOffset);
                        }
                    }
                }
            }
            else
            {
                foreach (string folder in folders)
                {
                    CompressFolder(folder, zipStream, folderOffset);
                }
            }
        }

        #endregion
    }
}
