using System;
using System.Configuration;
using System.Data.SqlClient;
using TickitNewFace.Models;
using System.Web;

namespace TickitNewFace.DAO
{
    /// <summary>
    /// Classe de gestion de la configuration globale de l'application.
    /// </summary>
    public static class ConfigurationDao
    {
        /// <summary>
        /// Renvoie la configuration globale de l'application qui est stockée en base de données.
        /// </summary>
        /// <returns></returns>
        public static T_Configuration getConfigurationAppication()
        {
            string sqlQuery = "select Seuil_minimal_livraison_incluse from Configuration";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            T_Configuration config = null;
            if (reader.Read())
            {
                config = new T_Configuration();
                config.Seuil_Minimal_Livraison_Incluse = reader.GetValue(0) == DBNull.Value ? null : (int?)reader.GetValue(0);
            }

            reader.Dispose(); 
            reader.Close();
            cmd.Dispose();

            return config;
        }

        /// <summary>
        /// MAJ de la configuration de l'application.
        /// </summary>
        /// <param name="config"></param>
        public static void updateConfiguration(T_Configuration config)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Update Configuration ";
            sqlQuery = sqlQuery + " Set Seuil_minimal_livraison_incluse = " + config.Seuil_Minimal_Livraison_Incluse;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public static void insertTest(string bla)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " insert into BLA values ('" + bla + "')";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }
}
