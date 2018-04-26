using System;
using System.Diagnostics;

namespace KOR.Updater.System
{
    public class AppKiller
    {
        //
        public bool ProcessKill(string appath)
        {
            bool ret = false;

            try
            {
                Process[] collectionOfProcess = Process.GetProcessesByName("1");
                if (collectionOfProcess.Length >= 1)
                {
                    Process acrProcess = collectionOfProcess[0];

                    string processPath = acrProcess.MainModule.FileName;

                    if (processPath == appath)
                    {
                        acrProcess.Kill();
                        ret = true;
                    }
                }
            }
            catch (Exception exp)
            {
                ret = false;
                // save error log
            }

            return ret;
        }
    }
}
