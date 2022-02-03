using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using TickitNewFace.Models;
using System.Web;

namespace TickitNewFace.DAO
{
    public class RangeDao
    {
        /// <summary>
        /// retourne le nom du range depuis son ID
        /// </summary>
        /// <param name="idRange"></param>
        /// <returns></returns>
        public static string getRangeNameById(int idRange)
        {
            string sqlQuery = "select RangeName from Range where Id = " + idRange;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            reader.Read();

            string rangeName = (string)reader.GetValue(0);

            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            
            return rangeName;
        }

        /// <summary>
        /// retourne le nom du range depuis son ID
        /// </summary>
        /// <param name="idRange"></param>
        /// <returns></returns>
        public static List<T_Range> getRangeByName(string rangeName)
        {
            string sqlQuery = "select id, RangeName from Range where RangeName = '" + rangeName.ToUpper() + "'";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<T_Range> listeRange = new List<T_Range>();

            while (reader.Read())
            {
                T_Range range = new T_Range();

                int id = (int)reader.GetValue(0);
                string name = (string)reader.GetValue(1);

                range.Id = id;
                range.RangeName = name;

                listeRange.Add(range);
            }
            
            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            
            return listeRange;
        }

        /// <summary>
        /// Renvoie la description globale de la gamme
        /// </summary>
        /// <param name="rangeName"></param>
        /// <returns></returns>
        public static bool isRangeBarAtissu(int rangeId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " select ISNULL(Bar_a_tissu, 'N') as Bar_a_tissu from range ";
            sqlQuery = sqlQuery + " where ";
            sqlQuery = sqlQuery + " Id = " + rangeId;


            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            bool isBarATissu = false;

            if (reader.HasRows)
            {
                reader.Read();
                if ((string)reader.GetValue(0) == "O")
                    isBarATissu = true;
            
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return isBarATissu;
            
        }


        //Cillia 
        public static bool isRangeTissu_Cuir_A_partir(int rangeId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " select ISNULL(T_C_A_Partir, 'N') as T_C_A_Partir from range ";
            sqlQuery = sqlQuery + " where ";
            sqlQuery = sqlQuery + " Id = " + rangeId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            bool isT_C = false;

            if (reader.HasRows)
            {
                reader.Read();
                if ((string)reader.GetValue(0) == "O")
                    isT_C = true;

            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return isT_C;
        }


        /// <summary>
        /// Renvoie la description globale de la gamme
        /// </summary>
        /// <param name="rangeName"></param>
        /// <returns></returns>
        public static string getDescriptionPlusByRangeName(string rangeName, int langageId, string divisionRange)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select Description from Description_Range dr, Range r ";
            sqlQuery = sqlQuery + " where ";
            sqlQuery = sqlQuery + " r.Id = dr.RangeId ";
            sqlQuery = sqlQuery + " and r.RangeName = '" + rangeName.ToUpper() + "'";
            sqlQuery = sqlQuery + " and dr.langageId = " + langageId;
            sqlQuery = sqlQuery + " and ( ";
            sqlQuery = sqlQuery + " dr.F_SF_SSF = '" + divisionRange + "'";
            sqlQuery = sqlQuery + " or dr.F_SF_SSF = '' )";
            sqlQuery = sqlQuery + " order by dr.F_SF_SSF desc";

            SqlConnection connection ;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            string descriptionPlus = null;

            if (reader.HasRows)
            {
                reader.Read();
                descriptionPlus = (string)reader.GetValue(0);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return descriptionPlus;
        }

        /// <summary>
        /// Renvoie la description globale de la gamme par ID
        /// </summary>
        /// <param name="rangeName"></param>
        /// <returns></returns>
        public static string getDescriptionPlusByRangeID(int rangeId, int langageId)
        {
            string sqlQuery = "";

            sqlQuery = sqlQuery + " Select Description from Description_Range ";
            sqlQuery = sqlQuery + " where ";
            sqlQuery = sqlQuery + " RangeId = " + rangeId;
            sqlQuery = sqlQuery + " and langageId = " + langageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            string descriptionPlus = null;

            if (reader.HasRows)
            {
                reader.Read();
                descriptionPlus = (string)reader.GetValue(0);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return descriptionPlus;
        }

        /// <summary>
        /// Mise à jour de la description du Range
        /// </summary>
        /// <param name="rangeName"></param>
        /// <param name="plusRange"></param>
        /// <param name="langageId"></param>
        public static void insertOrUpdatePlusRange(string rangeName, string plusRange, int langageId, string F_SF_SSF)
        {
            plusRange = plusRange.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " if exists( ";
            sqlQuery = sqlQuery + " select * from Description_Range, Range ";
            sqlQuery = sqlQuery + " where ";
            sqlQuery = sqlQuery + " range.Id = Description_Range.RangeId ";
            sqlQuery = sqlQuery + " and ";
            sqlQuery = sqlQuery + " RangeName = '" + rangeName + "'";
            sqlQuery = sqlQuery + " and langageId = " + langageId;
            sqlQuery = sqlQuery + " and F_SF_SSF = '" + F_SF_SSF + "'";
            sqlQuery = sqlQuery + " ) ";
            sqlQuery = sqlQuery + " update Description_Range ";
            sqlQuery = sqlQuery + " set Description = '" + plusRange + "'";
            sqlQuery = sqlQuery + " where RangeId in ( ";
            sqlQuery = sqlQuery + " select id from Range where RangeName = '" + rangeName + "')";
            sqlQuery = sqlQuery + " and langageId = " + langageId;
            sqlQuery = sqlQuery + " and F_SF_SSF = '" + F_SF_SSF + "'";

            sqlQuery = sqlQuery + " ELSE ";
            sqlQuery = sqlQuery + " BEGIN ";

            foreach (T_Range obj in getRangeByName(rangeName))
            {
                sqlQuery = sqlQuery + " INSERT into Description_Range values (" + obj.Id + "," + langageId + ",'" + plusRange + "', '" + F_SF_SSF + "') ";
            }

            sqlQuery = sqlQuery + " END ";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// Renvoie la description globale de la gamme
        /// </summary>
        /// <param name="rangeName"></param>
        /// <returns></returns>
        public static string getLibelleByRangeName(string rangeName, int langageId, string divisionRange)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select Libelle from Libelle_Range dr, Range r ";
            sqlQuery = sqlQuery + " where ";
            sqlQuery = sqlQuery + " r.Id = dr.RangeId ";
            sqlQuery = sqlQuery + " and r.RangeName = '" + rangeName.ToUpper() + "'";
            sqlQuery = sqlQuery + " and dr.langageId = " + langageId;

            sqlQuery = sqlQuery + " and ( ";
            sqlQuery = sqlQuery + " dr.F_SF_SSF = '" + divisionRange + "'";
            sqlQuery = sqlQuery + " or dr.F_SF_SSF = '' )";
            sqlQuery = sqlQuery + " order by dr.F_SF_SSF desc";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            string libelleRange = null;
            if (reader.HasRows)
            {
                reader.Read();
                libelleRange = (string)reader.GetValue(0);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return libelleRange;
        }

        /// <summary>
        /// MAJ du libellé du Range.
        /// </summary>
        /// <param name="rangeName"></param>
        /// <param name="plusRange"></param>
        /// <param name="langageId"></param>
        public static void insertOrUpdateLibelleRange(string rangeName, string libelleRange, int langageId, string F_SF_SSF)
        {
            libelleRange = libelleRange.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " if exists( ";
            sqlQuery = sqlQuery + " select * from Libelle_Range, Range ";
            sqlQuery = sqlQuery + " where ";
            sqlQuery = sqlQuery + " range.Id = Libelle_Range.RangeId ";
            sqlQuery = sqlQuery + " and ";
            sqlQuery = sqlQuery + " RangeName = '" + rangeName + "'";
            sqlQuery = sqlQuery + " and langageId = " + langageId;
            sqlQuery = sqlQuery + " and F_SF_SSF = '" + F_SF_SSF + "'";
            sqlQuery = sqlQuery + " ) ";
            sqlQuery = sqlQuery + " update Libelle_Range ";
            sqlQuery = sqlQuery + " set Libelle = '" + libelleRange + "'";
            sqlQuery = sqlQuery + " where RangeId in ( ";
            sqlQuery = sqlQuery + " select id from Range where RangeName = '" + rangeName + "')";
            sqlQuery = sqlQuery + " and langageId = " + langageId;
            sqlQuery = sqlQuery + " and F_SF_SSF = '" + F_SF_SSF + "'";

            sqlQuery = sqlQuery + " ELSE ";
            sqlQuery = sqlQuery + " BEGIN ";

            foreach (T_Range obj in getRangeByName(rangeName))
            {
                sqlQuery = sqlQuery + " INSERT into Libelle_Range values (" + obj.Id + "," + langageId + ",'" + libelleRange + "', '" + F_SF_SSF + "') ";
            }

            sqlQuery = sqlQuery + " END ";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }


        /// <summary>
        /// Renvoie les 3 descriptions plus d'une gamme.
        /// </summary>
        /// <param name="rangeName"></param>
        /// <returns></returns>
        public static List<string> getPlusByRangeId(int RangeId, int langageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " select Plus from Description_Plus_Range";
            sqlQuery = sqlQuery + " where ";
            sqlQuery = sqlQuery + " langageId = " + langageId;
            sqlQuery = sqlQuery + " and RangeId = " + RangeId;
            sqlQuery = sqlQuery + " and Plus is not null ";
            sqlQuery = sqlQuery + " and Plus <> '' ";
            sqlQuery = sqlQuery + " order by position asc";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<string> listPlus = new List<string>();

            while (reader.Read())
            {
                string currentPlus = (string)reader.GetValue(0);
                listPlus.Add(currentPlus);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return listPlus;
        }

        /// <summary>
        /// Renvoie les 3 descriptions plus d'une gamme.
        /// </summary>
        /// <param name="rangeName"></param>
        /// <returns></returns>
        public static string getDescriptionCompositionByRangeId(int RangeId, int langageId, int paragraphSection)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " select '<strong>' + commposition_key + '</strong>' + ' ' + commposition_value + ' ' from Description_Composition ";
            sqlQuery = sqlQuery + " where ";
            sqlQuery = sqlQuery + " LangageId = " + langageId;
            sqlQuery = sqlQuery + " and RangeId = " + RangeId;
            sqlQuery = sqlQuery + " and Paragraph_section = " + paragraphSection;


            sqlQuery = sqlQuery + " order by Paragraph_position ";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            string compositionDesc = "";

            while (reader.Read())
            {
                compositionDesc = compositionDesc + (string)reader.GetValue(0);        
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return compositionDesc;
        }
    }
}
