using System;
using System.Configuration;
using System.Data.SqlClient;
using TickitNewFace.Models;
using System.Web;
using TickitNewFace.Utils;

namespace TickitNewFace.DAO
{
    /// <summary>
    /// Classe de gestion de la configuration globale de l'application.
    /// </summary>
    public static class EvenementDao
    {
        /// <summary>
        /// insertion nouvel évenement.
        /// </summary>
        /// <param name="evenement"></param>
        public static void insertEvenement(T_Evenement objEve)
        {
            string sqlQuery = "Insert into Evenement values ('" + objEve.Login + "', '" + objEve.Eve + "', GETDATE())";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }
}