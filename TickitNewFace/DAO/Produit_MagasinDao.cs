using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using TickitNewFace.Models;
using TickitNewFace.Utils;

namespace TickitNewFace.DAO
{
    public class Produit_MagasinDao
    {
        /// <summary>
        /// Retourne les listePlus d'un produits
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static List<string> getSkusByMagasinIdDivision(string magId, int MagasinId, string division, DateTime date, string TypePrix)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select distinct Produit_Magasin.Sku, Produit.Division from Produit_Magasin, Produit, prix";
            sqlQuery = sqlQuery + " where Produit_Magasin.id_magasin = " + MagasinId + "and Produit_Magasin.code_magasin = '" + magId + "'";
            sqlQuery = sqlQuery + " and produit.Sku = Produit_Magasin.Sku";
            sqlQuery = sqlQuery + " and produit.Sku = prix.Sku";
            sqlQuery = sqlQuery + " and produit.Division like '" + division + "%'";
            sqlQuery = sqlQuery + " and prix.Code_Pays = " + MagasinId ;
            sqlQuery = sqlQuery + " and prix.Type_promo = '" + TypePrix + "'";
            sqlQuery = sqlQuery + " and '" + DateUtils.getFormatDateAng(date) + "' between Prix.Date_debut and Prix.Date_fin ";
            sqlQuery = sqlQuery + " order by produit.division asc";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<string> listePlus = new List<string>();
            
            while (reader.Read())
            {
                listePlus.Add((string)reader.GetValue(0));
            }

            reader.Close();
            cmd.Dispose();
            
            return listePlus;
        }

        /// <summary>
        /// Renvoie liste magasin select imp masse
        /// </summary>
        /// <param name="listePlus"></param>
        public static List<T_magasin> selecidMagasins(int lid)
        {
            string sqlQuery = "Select Magasin_id, Magasin_nom, Pays_id from dbo.Liste_Magasin where Pays_id ='" + lid + "';";
            
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<T_magasin> magasins = new List<T_magasin>();

            while (reader.Read())
            {
                T_magasin magasin = new T_magasin();
                magasin.Magasin_id = (string)reader.GetValue(0);
                magasin.Magasin_nom = (string)reader.GetValue(1);
                magasin.Pays_id = (int)reader.GetValue(2);

                magasins.Add(magasin);
            }

            reader.Dispose();
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            return magasins;
        }



        /// <summary>
        /// Renvoie liste magasin a update
        /// </summary>
        /// <param name="listePlus"></param>
        public static T_magasin selecidMagasin(int lid,string choix)
        {
            string sqlQuery;
            if (lid == 4 || lid == 5 || lid == 6)
            {
                sqlQuery = "Select Magasin_id, Magasin_nom, Pays_id from dbo.Liste_Magasin where Magasin_id ='" + choix + "';";
            }
            else
            {
                sqlQuery = "Select Magasin_id, Magasin_nom, Pays_id from dbo.Liste_Magasin where Magasin_id ='" + choix + "' or Pays_id ='" + choix + "';";
            }
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<T_magasin> magasins = new List<T_magasin>();

            while (reader.Read())
            {
                T_magasin magasin = new T_magasin();
                magasin.Magasin_id = (string)reader.GetValue(0);
                magasin.Magasin_nom = (string)reader.GetValue(1);
                magasin.Pays_id = (int)reader.GetValue(2);

                magasins.Add(magasin);
            }

            reader.Dispose();
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            return magasins[0];
        }

        /// <summary>
        /// Insère un couple Sku magasin
        /// </summary>
        /// <param name="listePlus"></param>
        public static void insertSkuMagasin(string Sku, int MagasinId, string magId)
        {
            string sqlQuery = "Insert into Produit_Magasin values ('" + Sku + "', '" + MagasinId + "', '" + magId + "')";
    
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// Supprime les skus liés à un code magasin
        /// </summary>
        /// <param name="Sku"></param>
        /// <param name="LangageId"></param>
        public static void supprimerSkusByMagasinId(int MagasinId, string magId)
        {
            string sqlQuery = "delete from Produit_Magasin where id_magasin = " + MagasinId + "and code_magasin = '" + magId + "' or code_magasin = '" + MagasinId + "' ";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }
        public static void editSkusProduitMagasins(int MagasinId, string magIds)
        {
            string sqlQuery = "EXECUTE dbo.remplirProduitMmagasins '" + magIds + "','" + MagasinId + "';";
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.CommandTimeout = 1000;
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }
    }
}