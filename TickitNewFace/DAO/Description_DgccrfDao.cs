using System;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using TickitNewFace.Models;

namespace TickitNewFace.DAO
{
    public class Description_DgccrfDao
    {
        /// <summary>
        /// Retourne la description légale d'un produit
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static T_Description_Dgccrf getDgccrfBySku(string Sku, int LangageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select Sku, LangageId, LegalDescription from Description_Dgccrf ";
            sqlQuery = sqlQuery + " Where Sku = '" + Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + LangageId;

            
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            T_Description_Dgccrf dgccrf = null;

            if (reader.HasRows)
            {
                reader.Read();
                dgccrf = new T_Description_Dgccrf();

                dgccrf.Sku = (string)reader.GetValue(0);
                dgccrf.LangageId = reader.GetValue(1) == DBNull.Value ? null : (int?)reader.GetValue(1);
                dgccrf.LegalDescription = (string)reader.GetValue(2);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return dgccrf;
        }

        /// <summary>
        /// Met à jour la description légale.
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updateDgccrf(Models.T_Description_Dgccrf Dgccrf)
        {
            Dgccrf.LegalDescription = Dgccrf.LegalDescription.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " Update Description_Dgccrf set ";
            sqlQuery = sqlQuery + " LegalDescription =  '" + Dgccrf.LegalDescription + "' where";
            sqlQuery = sqlQuery + " Sku = '" + Dgccrf.Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + Dgccrf.LangageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }

        /// <summary>
        /// insert un nouveau record DGCCRF
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void insertDgccrf(Models.T_Description_Dgccrf Dgccrf)
        {   
            Dgccrf.LegalDescription = Dgccrf.LegalDescription.Replace("'", "''");
            string sqlQuery = "Insert into Description_Dgccrf values ('" + Dgccrf.Sku + "', " + Dgccrf.LangageId + ", '" + Dgccrf.LegalDescription + "')";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// insert un nouveau record DGCCRF
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void deleteDgccrf(string Sku, int langageId, string legalDescription)
        {
            string sqlQuery = "delete from Description_Dgccrf ";
            sqlQuery = sqlQuery + "where ";
            sqlQuery = sqlQuery + "Sku = '" + Sku + "' and ";
            sqlQuery = sqlQuery + "LangageId = " + langageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }
}
