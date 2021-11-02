using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using TickitNewFace.Models;
using TickitNewFace.Const;

namespace TickitNewFace.DAO
{
    public class VariationDao
    {
        /// <summary>
        /// Renvoie  la variation d'un produit.
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static T_Variation getVariationBySku(string Sku, int langageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select Sku, LangageId, VariationName from Variation ";
            sqlQuery = sqlQuery + " Where Sku = '" + Sku;
            sqlQuery = sqlQuery + "' and langageId = " + langageId;

            SqlConnection connection;
            ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            T_Variation variation = null;
            if (reader.HasRows)
            {
                reader.Read();
                variation = new T_Variation();

                variation.Sku = (string)reader.GetValue(0);
                variation.LangageId = (int)reader.GetValue(1);
                variation.VariationName = (string)reader.GetValue(2);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return variation;
        }

        /// <summary>
        /// MAJ Variation
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updateVariation(T_Variation variation)
        {
            variation.VariationName = variation.VariationName.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " Update Variation set ";
            sqlQuery = sqlQuery + " VariationName =  '" + variation.VariationName + "' where";
            sqlQuery = sqlQuery + " Sku = '" + variation.Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + variation.LangageId;

            SqlConnection connection;
            ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// insert un nouveau record DGCCRF
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void insertVariation(T_Variation variation)
        {
            variation.VariationName = variation.VariationName.Replace("'", "''");
            string sqlQuery = "Insert into Variation values ('" + variation.Sku + "', " + variation.LangageId + ", '" + variation.VariationName + "')";

            SqlConnection connection;
            ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// insert un nouveau record DGCCRF
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void deleteVariation(string Sku, int langageId, string variationName)
        {
            string sqlQuery = "delete from variation ";
            sqlQuery = sqlQuery + "where ";
            sqlQuery = sqlQuery + "Sku = '" + Sku + "' and ";
            sqlQuery = sqlQuery + "LangageId = " + langageId;

            SqlConnection connection;
            ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }
}