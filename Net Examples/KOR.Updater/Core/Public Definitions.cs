using System.Security.Principal;

namespace KOR.Updater.Core
{

    public class Errors
    {
        /// <summary>
        /// Error time (utc)
        /// </summary>
        public static string ErrorTime { get; set; }

        /// <summary>
        /// Error localtion (eg. function, process)
        /// </summary>
        public static string ErrorLocation { get; set; }

        /// <summary>
        /// Specific error name
        /// </summary>
        public static string ErrorName { get; set; }

        /// <summary>
        /// Error full content
        /// </summary>
        public static string ErrorContent { get; set; }
    }
}
