using System.Management;

namespace KOR.Updater
{
    public class Info
    {
        #region Get CPU Info

        /// <summary>
        /// Get current machine on installed CPU ID
        /// </summary>
        /// <returns></returns>
        public static string GetCpuid()
        {
            string ret = string.Empty;

            try
            {
                ManagementClass managClass = new ManagementClass("win32_processor");
                ManagementObjectCollection managCollec = managClass.GetInstances();

                foreach (ManagementObject managObj in managCollec)
                {
                    ret = managObj.Properties["processorId"].Value.ToString();
                }
            }
            catch (ManagementException manaExp)
            {
                // Save error log
            }

            return ret;
        }

        #endregion
    }
}
