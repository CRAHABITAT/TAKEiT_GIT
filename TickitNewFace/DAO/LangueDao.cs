using System;
using System.Configuration;
using System.Data.SqlClient;
using TickitNewFace.Const;
using System.Web;
using System.Collections.Generic;
namespace TickitNewFace.DAO
{
    public class LangueDao
    {
        /// <summary>
        /// methode interne renvoyant le identifiant de la langue.
        /// </summary>
        /// <param name="codePays"></param>
        /// <returns></returns>
        public static int getLangageIdByCode(string codePays)
        {
            string sqlQuery = "";
            sqlQuery += " Select id from langue ";
            sqlQuery += " Where CountryCode = '" + codePays.ToUpper() + "'";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            int id = (int)reader.GetValue(0);
            
            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            
            return id;
        }

        public static string getLangageByCode(string codeAd)
        {
            string sqlQuery = "";
            sqlQuery += " Select Langue from langue ";
            sqlQuery += " Where CountryCode = '" + codeAd.ToUpper() + "'";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            string langage = (string)reader.GetValue(0);

            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            
            return langage.ToLower();
        }

        public static string getCodeMonnaieByMagasinId(int magasinId)
        {
            string sqlQuery = "";
            sqlQuery += " Select CodeMonnaie from langue ";
            sqlQuery += " Where ID = " + magasinId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            string langage = (string)reader.GetValue(0);

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            //langage = "฿";
            //langage = "غ";
            return langage ;
        }

        public static void testIM()
        {
            
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand("Select id,idLangueOrigine from Langue", connection);
            SqlDataReader reader = cmd.ExecuteReader();

            SqlCommand cmd2 = new SqlCommand("duplication_descriptions", connection);
            cmd2.CommandType = System.Data.CommandType.StoredProcedure;



            while (reader.Read())
            {
                if (reader["idLangueOrigine"] != null && (reader["idLangueOrigine"] != reader["id"]))
	            {
                    cmd2.Parameters.Add(new SqlParameter("@code_pays_source", reader["idLangueOrigine"]));
                    cmd2.Parameters.Add(new SqlParameter("@code_pays_destination", reader["id"]));

                    if (cmd2 != null)
                    {
                        cmd2.Dispose();
                    }
	            }

            }

           

        }
    }
}
