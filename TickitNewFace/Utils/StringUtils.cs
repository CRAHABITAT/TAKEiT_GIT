using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Net;

namespace TickitNewFace.Utils
{
    public static class StringUtils
    {
        /// <summary>
        /// Convertit un mot majuscule en minuscule sauf la première lettre
        /// </summary>
        /// <param name="oldstring"></param>
        /// <returns></returns>
        public static string convertStringMajusjToMinus(string oldstring)
        {
            /*
            String[] substrings = oldstring.Split(' ');

            string newstring = "";

            if (substrings.Count() == 1)
            {
                newstring = oldstring[0].ToString().ToUpper() + oldstring.Substring(1).ToLower();
            }
            else
            {
                string premierMot = substrings[0];
                string deuxiemeMot = substrings[1];

                newstring = premierMot[0].ToString().ToUpper() + premierMot.Substring(1).ToLower() + " " + deuxiemeMot;
            }

            return newstring;*/
            return oldstring;
        }

        /// <summary>
        /// Renvoie un decimal au format Monnaie Malgache
        /// </summary>
        /// <param name="oldstring"></param>
        /// <returns></returns>
        public static string getAriaryMonnaieFormat(decimal dec)
        {
            NumberFormatInfo nfi = new CultureInfo("fr-FR", false).NumberFormat;
            NumberFormatInfo MGA = new NumberFormatInfo();
            MGA = (NumberFormatInfo)nfi.Clone();
            MGA.CurrencySymbol = "";

            string result;
            result = dec.ToString("C", MGA);
            result = result.Replace(",00", "");
            return result;
        }

        /// <summary>
        /// permet de savoir si une ressource est disponible sur un serveur http distant.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static public bool URLExists(string url)
        {
            bool result = false;

            WebRequest webRequest = WebRequest.Create(url);
            
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                result = true;
            }
            catch (WebException)
            {
                result = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;
        }
    }
}