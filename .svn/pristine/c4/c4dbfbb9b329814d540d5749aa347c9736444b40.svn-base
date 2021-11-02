using System;

namespace TickitNewFace.Utils
{
    /// <summary>
    /// Classe de gestion des types de dates.
    /// </summary>
    public static class DateUtils
    {
        /// <summary>
        /// Retourne une date en format anglais.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string getFormatDateAng(DateTime date)
        {
            int year = date.Year;
            int day = date.Day;
            int month = date.Month;

            string dateString;

            dateString = year.ToString();

            if (month < 10)
            {
                dateString = dateString + "/" + "0" + month;
            }
            else
            {
                dateString = dateString + "/" + month;
            }
            if (day < 10)
            {
                dateString = dateString + "/" + "0" + day;
            }
            else
            {
                dateString = dateString + "/" + day;
            }

            return dateString;
        }

        /// <summary>
        /// Retourne une date en format anglais.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string getFormatDateFr(DateTime date)
        {
            int year = date.Year;
            int day = date.Day;
            int month = date.Month;

            string dateString = "";

            if (day < 10)
            {
                dateString = dateString + "0" + day;
            }
            else
            {
                dateString = dateString + day;
            }

            if (month < 10)
            {
                dateString = dateString + "/" + "0" + month;
            }
            else
            {
                dateString = dateString + "/" + month;
            }

            dateString = dateString + "/" + year.ToString();

            return dateString;
        }
    }
}