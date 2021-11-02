using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using TickitNewFace.Models;

namespace TickitNewFace.DAO
{
    public class Description_PlusDao
    {
        /// <summary>
        /// Retourne les listePlus d'un produits
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static List<T_Description_Plus> getListePlusBySku(string Sku, int langageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select Sku, LangageId, Plus, Position from Description_Plus ";
            sqlQuery = sqlQuery + " Where Sku = " + Sku;
            sqlQuery = sqlQuery + " and langageId = " + langageId;
            sqlQuery = sqlQuery + " Order by Position";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<T_Description_Plus> listePlus = new List<T_Description_Plus>();

            while (reader.Read())
            {
                T_Description_Plus plus = new T_Description_Plus();

                plus.Sku = (string)reader.GetValue(0);
                plus.LangageId = (int)reader.GetValue(1);
                plus.Plus = (string)reader.GetValue(2);
                plus.Position = (int)reader.GetValue(3);

                listePlus.Add(plus);
            }

            reader.Close();
            cmd.Dispose();
            
            return listePlus;
        }

        /// <summary>
        /// Insère un listePlus pour un produit
        /// </summary>
        /// <param name="listePlus"></param>
        public static void insertPlus(T_Description_Plus plus)
        {
            plus.Plus = plus.Plus.Replace("'", "''");
            
            string sqlQuery = "Insert into Description_Plus values ('" + plus.Sku + "', " + plus.LangageId + ", '" + plus.Plus + "', " + plus.Position + ")";
    
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// Supprime tous les listePlus d'un produit qui sonjt saisis dans une langue représentée par 'LangageId'
        /// </summary>
        /// <param name="Sku"></param>
        /// <param name="LangageId"></param>
        public static void supprimerPlusByLang(string Sku, int LangageId, int position)
        {
            string sqlQuery = "delete from Description_Plus where Sku = '" + Sku + "' and LangageId = " + LangageId + " and position = " + position;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }
    }
}