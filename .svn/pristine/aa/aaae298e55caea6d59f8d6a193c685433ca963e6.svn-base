using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TickitNewFace.Utils
{
    public static class SpecificMathUtils
    {
        /// <summary>
        /// Cette Methode permet d'avoir des arrondis spécifiques selon des règles de gestions.
        /// Exemple : 
        /// - Si inDecimal = 30.02 return 30
        /// - Si inDecimal = 29.98 return 30
        /// - Si inDecimal = 30.50 return 30.50
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static decimal? getRoundDecimal(decimal? inDecimal)
        {
            // on doit pouvoir gérer les null

            if (inDecimal == null) { return null; }
            else
            {
                decimal inDecimalNotNull = (decimal)inDecimal;
                decimal outDecimal = inDecimalNotNull;

                decimal diff = Math.Abs(inDecimalNotNull - Math.Truncate(inDecimalNotNull));
                decimal realDiff;

                if (diff > new decimal(0.5))
                {
                    realDiff = 1 - diff;
                }
                else
                {
                    realDiff = diff;
                }

                if (realDiff < new decimal(0.42))
                {
                    outDecimal = Math.Round(inDecimalNotNull);
                }
                else 
                {
                    outDecimal = Math.Round((decimal)inDecimal, 2);
                }

                return outDecimal;
            }
        }

        public static Boolean isInteger(decimal dec)
        {
            int Int;
            Int = (int)dec;
            if (dec != Int) return false;
            return true;
        }
    }
}
