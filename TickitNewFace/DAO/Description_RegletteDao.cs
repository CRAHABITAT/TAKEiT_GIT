using System;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using TickitNewFace.Models;

namespace TickitNewFace.DAO
{
    public class Description_RegletteDao
    {
        /// <summary>
        /// Retourne la description Reglette d'un produit
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static T_Description_Reglette getDescRegletteBySku(string Sku, int LangageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select Sku, LangageId, Description from Description_Reglette ";
            sqlQuery = sqlQuery + " Where Sku = '" + Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + LangageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            T_Description_Reglette descReglette = null;
            if (reader.HasRows)
            {
                reader.Read();
                descReglette = new T_Description_Reglette();
                descReglette.Sku = (string)reader.GetValue(0);
                descReglette.LangageId = reader.GetValue(1) == DBNull.Value ? null : (int?)reader.GetValue(1);
                descReglette.Description = (string)reader.GetValue(2);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return descReglette;
        }

        /// <summary>
        /// Met à jour une description Reglette.
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updateDescReglette(T_Description_Reglette descReglette)
        {
            descReglette.Description = descReglette.Description.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " Update Description_Reglette set ";
            sqlQuery = sqlQuery + " Description =  '" + descReglette.Description + "' where";
            sqlQuery = sqlQuery + " Sku = '" + descReglette.Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + descReglette.LangageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// insert un nouveau record description Reglette
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void insertDescReglette(T_Description_Reglette descReglette)
        {
            descReglette.Description = descReglette.Description.Replace("'", "''");
            string sqlQuery = "Insert into Description_Reglette values ('" + descReglette.Sku + "', " + descReglette.LangageId + ", '" + descReglette.Description + "')";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// Supprime un record description Reglette
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void deleteDescReglette(string Sku, int langageId, string Description)
        {
            string sqlQuery = "delete from Description_Reglette ";
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
