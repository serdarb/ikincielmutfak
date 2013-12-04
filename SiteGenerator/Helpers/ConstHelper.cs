using System.Globalization;

namespace SiteGenerator
{
    public class ConstHelper
    {
        private static CultureInfo _cultureTR;
        public static CultureInfo CultureTR
        {
            get { return _cultureTR ?? (_cultureTR = new CultureInfo("tr-TR")); }
        }
    }
}
