using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using TickitNewFace.Models;

namespace TickitNewFace.DAO
{
    public class LibelleProduitDao
    {
        /// <summary>
        /// Renvoie  la libelle d'un produit.
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static T_Libelle_Produit getLibelleProduitBySku(string Sku, int langageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select Sku, LangageId, libelle from libelle_produit";
            sqlQuery = sqlQuery + " Where Sku = " + Sku;
            sqlQuery = sqlQuery + " and langageId = " + langageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            T_Libelle_Produit libelle = null;

            if (reader.HasRows)
            {
                reader.Read();
                libelle = new T_Libelle_Produit();

                libelle.Sku = (string)reader.GetValue(0);
                libelle.LangageId = (int)reader.GetValue(1);
                libelle.Libelle = (string)reader.GetValue(2);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            
            return libelle;
        }


        /// <summary>
        /// MAJ Variation
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updatelibelleproduit(T_Libelle_Produit libelleObj)
        {
            libelleObj.Libelle = libelleObj.Libelle.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " Update libelle_produit set ";
            sqlQuery = sqlQuery + " libelle =  '" + libelleObj.Libelle + "' where";
            sqlQuery = sqlQuery + " Sku = '" + libelleObj.Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + libelleObj.LangageId;

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
        public static void insertLibelleProduit(T_Libelle_Produit libelleObj)
        {
            libelleObj.Libelle = libelleObj.Libelle.Replace("'", "''");
            string sqlQuery = "Insert into libelle_produit values ('" + libelleObj.Sku + "', " + libelleObj.LangageId + ", '" + libelleObj.Libelle + "')";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// insert un nouveau record libelle
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void deleteLibelleProduit(string Sku, int langageId, string libelle)
        {
            string sqlQuery = "delete from libelle_produit ";
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