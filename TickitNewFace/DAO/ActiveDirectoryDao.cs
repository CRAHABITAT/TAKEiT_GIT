using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.DirectoryServices;
using TickitNewFace.Models;
using TickitNewFace.Utils;
using System.Text.RegularExpressions;
using TickitNewFace.Const;

namespace TickitNewFace.DAO
{
    /// <summary>
    /// Classe DAO de récuperation des données de l'activeDirectory.
    /// </summary>
    public class ActiveDirectory
    {


        /// <summary>
        /// Retourne le magasin rattaché au compte AD
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public static string getMagasinByLogin(string userLogin)
        {

            DirectoryEntry DEUser = getDirectoryEntry(userLogin);
            String magasin = "ERROR_MAG";

            if (null == DEUser)
                return magasin;
            try
            {
                object[] myList = DEUser.Properties["memberOf"].Value as object[];

                foreach (string value in myList)
                {
                    System.Text.RegularExpressions.MatchCollection matches = Regex.Matches((string)value, @"CN=MAG_(.*?),");
                    if (matches.Count >= 1)
                    {
                        magasin = matches[0].Value;
                        return magasin.Replace("CN=MAG_", "").Replace(",", "");
                    }
                }
                return magasin;
            }
            catch (Exception)
            {
                return magasin;
            }
        }




        /// <summary>
        /// </summary>
        /// <param name="Sku"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DirectoryEntry getDirectoryEntry(string userLogin)
        {
            string ldapPath = "LDAP://" + ConfigurationManager.AppSettings["ldapIp"] + "/DC=habitat,DC=local";
            string login = ConfigurationManager.AppSettings["ldapLogin"];
            string password = ConfigurationManager.AppSettings["ldapPassword"];

            DirectoryEntry ldap = new DirectoryEntry(ldapPath, login, password);
            DirectorySearcher searcher = new DirectorySearcher(ldap);
            searcher.Filter = "(SAMAccountName=" + userLogin + ")";

            SearchResult result = searcher.FindOne();

            if (null == result)
            {
                return null;
            }
            else
            {
                return result.GetDirectoryEntry();
            }
        }

        public static Boolean isFranchise(string userLogin)
        {
            DirectoryEntry DEUser = getDirectoryEntry(userLogin);
            Regex isFranchise = new Regex(@"OU=Franchises");

            if (null != DEUser)
                return isFranchise.IsMatch((string)DEUser.Properties["distinguishedName"].Value);
            else
                return false;
        }

        public static String getFranchiseName(string userLogin)
        {
            DirectoryEntry DEUser = getDirectoryEntry(userLogin);

            if (null == DEUser)
                return null;

            var matches = Regex.Matches((string)DEUser.Properties["distinguishedName"].Value, @"OU=(.*?),");
            String match;

            if (matches.Count > 1)
                match = matches[1].Value;
            else
                match = matches[0].Value;

            return isFranchise(userLogin) ? match.Substring(3, match.Length - 4) : ApplicationConsts.nonFranchise;
        }

        public static Boolean isAdmin(string userLogin)
        {
            DirectoryEntry DEUser = getDirectoryEntry(userLogin);

            if (null == DEUser)
                return false;

            try
            {

                object[] myList = DEUser.Properties["memberOf"].Value as object[];

                foreach (string value in myList)
                {
                    var matches = Regex.Matches((string)value, @"CN=Admin.Takeit(.*?),");
                    if (matches.Count >= 1)
                        return true;
                }
                //gestion manuelle
                //si personne admin mais pas dans AD encore (a supprimer plus tard)
                if (userLogin == "msridi"
                    || userLogin == "fseelbach"
                    || userLogin == "bskrbic"
                    || userLogin == "lyr1"
                    || userLogin == "cpauthenet"
                    || userLogin == "mgatimel"
                    || userLogin == "mfauvage"
                    || userLogin == "hseynave"
                    || userLogin == "tviaud"
                    || userLogin == "atzirini"
                    || isAdminFranchise(userLogin)
                    )
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Boolean isAdminFranchise(string userLogin)
        {
            DirectoryEntry DEUser = getDirectoryEntry(userLogin);

            if (null == DEUser)
                return false;

            try
            {

                object[] myList = DEUser.Properties["memberOf"].Value as object[];

                foreach (string value in myList)
                {
                    System.Text.RegularExpressions.MatchCollection matches = Regex.Matches((string)value, @"CN=GG(.*?)Admininistrators,");
                    if (matches.Count >= 1)
                        return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
