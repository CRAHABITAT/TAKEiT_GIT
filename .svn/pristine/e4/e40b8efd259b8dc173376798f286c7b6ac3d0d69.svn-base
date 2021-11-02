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
    public static class ConfigurationBisDao
    {
        /// <summary>
        /// Renvoie la configuration globale de l'application qui est stockée en base de données.
        /// </summary>
        /// <returns></returns>
        public static string getValeurByCleAndMagasinId(string cle, int magasinId)
        {
            string sqlQuery = "select valeur from Configuration_bis where cle = '" + cle + "' and magasin_id = " + magasinId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            string valeur = "";
            if (reader.Read())
            {
                valeur = reader.GetValue(0) == DBNull.Value ? "" : (string)reader.GetValue(0);
            }

            reader.Dispose(); 
            reader.Close();
            cmd.Dispose();

            return valeur;
        }
    }
}
