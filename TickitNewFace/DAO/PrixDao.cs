using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using TickitNewFace.Models;
using TickitNewFace.Utils;
using System.Web;

namespace TickitNewFace.DAO
{
    /// <summary>
    /// Classe DAO de récuperation des données prixCal.
    /// </summary>
    public class PrixDao
    {
        /// <summary>
        /// Retourne le prixCal d'un produit à une date donnée
        /// </summary>
        /// <param name="Sku"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static T_Prix getPrixBySkuAndDate(string Sku, int langageId, DateTime date)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select top 1 Prix.Sku, Prix.Date_debut, Prix.Date_fin, Prix.Prix_produit + ISNULL(CASE WHEN (Prix.Code_pays in (4,10,17,19,20,21,37,38)) THEN produit.eco_mobilier ELSE 0 END,0), Prix.Type_promo, Prix.Code_pays, ";
            sqlQuery = sqlQuery + " ISNULL(CASE WHEN (Prix.Code_pays in (4,10,17,19,20,21,37,38)) THEN produit.eco_mobilier ELSE 0 END,0) as Eco_mobilier , Prix.TypeTarifCbr from Prix, Produit ";
            sqlQuery = sqlQuery + " Where Prix.Sku = " + Sku;
            sqlQuery = sqlQuery + " and Prix.Code_pays = " + langageId;
            sqlQuery = sqlQuery + " and '" + DateUtils.getFormatDateAng(date) + "' between Prix.Date_debut and Prix.Date_fin ";
            sqlQuery = sqlQuery + " and Prix.Sku = Produit.Sku ";
            sqlQuery = sqlQuery + " order by Prix.Date_debut desc";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            T_Prix prix = null;

            if (reader.HasRows)
            {
                reader.Read();

                prix = new T_Prix();
                prix.Sku = (string)reader.GetValue(0);
                prix.Date_debut = (DateTime)reader.GetValue(1);
                prix.Date_fin = (DateTime)reader.GetValue(2);
                prix.Prix_produit = (decimal)reader.GetValue(3);
                prix.Type_promo = reader.GetValue(4) == DBNull.Value ? null : (string)reader.GetValue(4);
                prix.Code_Pays = (int)reader.GetValue(5);
                prix.Eco_mobilier = (decimal)reader.GetValue(6);
                prix.TypeTarifCbr = reader.GetValue(7) == DBNull.Value ? null : (string)reader.GetValue(7);
               //mis en commentaire par Cillia
                if (prix.Type_promo == "N" || prix.Type_promo == "P" || prix.Type_promo == "S")
                {
                     prix.Prix_produit = prix.Prix_produit - prix.Eco_mobilier;
                }
            }
            
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return prix;
        }
        
        /// <summary>
        /// Retourne le prixCal permanent précedent le prixCal passé en paramètre.
        /// </summary>
        /// <param name="prixToCompare"></param>
        /// <returns></returns>
        public static T_Prix getPrixPermanentPrecedent(T_Prix prixToCompare)
        {
            string prixToCompareString = prixToCompare.Prix_produit.ToString().Replace(',', '.');
            
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select top 1 Sku, Date_debut, Date_fin, Prix_produit, Type_promo, Code_pays from Prix ";
            sqlQuery = sqlQuery + " Where Sku = " + prixToCompare.Sku;
            sqlQuery = sqlQuery + " and Code_pays = " + prixToCompare.Code_Pays;
            sqlQuery = sqlQuery + " and Date_debut < '" + DateUtils.getFormatDateAng(prixToCompare.Date_debut) + "'";
            sqlQuery = sqlQuery + " and Type_promo = 'N'";
            sqlQuery = sqlQuery + " and Date_fin >= GETDATE()";
            sqlQuery = sqlQuery + " order by Date_debut desc";

            T_Produit pro = ProduitDao.getProduitBySku(prixToCompare.Sku, prixToCompare.Code_Pays);
            decimal? ecoMobilier = pro.Eco_mobilier;
            
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            
            T_Prix prix = null;

            if (reader.HasRows)
            {
                reader.Read();

                prix = new T_Prix();
                prix.Sku = (string)reader.GetValue(0);
                prix.Date_debut = (DateTime)reader.GetValue(1);
                prix.Date_fin = (DateTime)reader.GetValue(2);
                prix.Type_promo = reader.GetValue(4) == DBNull.Value ? null : (string)reader.GetValue(4);
                prix.Code_Pays = (int)reader.GetValue(5);
                prix.Prix_produit = (Decimal)reader.GetValue(3);

                prix.Prix_produit1 = (Decimal)reader.GetValue(3);

            //mis en commentaire par Cillia //retirer par la suite    
                /*   if (ecoMobilier != null) { 
                prix.Prix_produit = prix.Prix_produit - (decimal)ecoMobilier;
                }*/
            }
            
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return prix;
        }

        /// <summary>
        /// Retourne le prix en promo précédent.
        /// </summary>
        /// <param name="prixToCompare"></param>
        /// <returns></returns>
        public static T_Prix getPrixSoldePrecedent(T_Prix prixToCompare)
        {
            string prixToCompareString = prixToCompare.Prix_produit.ToString().Replace(',', '.');

            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select top 1 Sku, Date_debut, Date_fin, Prix_produit, Type_promo, Code_pays from Prix ";
            sqlQuery = sqlQuery + " Where Sku = " + prixToCompare.Sku;
            sqlQuery = sqlQuery + " and Code_pays = " + prixToCompare.Code_Pays;
            sqlQuery = sqlQuery + " and Date_debut < '" + DateUtils.getFormatDateAng(prixToCompare.Date_debut) + "'";
            sqlQuery = sqlQuery + " and Type_promo = 'S'" ;
            sqlQuery = sqlQuery + " and Date_fin >= GETDATE()";
            sqlQuery = sqlQuery + " order by Date_debut desc";

            T_Produit pro = ProduitDao.getProduitBySku(prixToCompare.Sku, prixToCompare.Code_Pays);
            decimal? ecoMobilier = pro.Eco_mobilier;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            T_Prix prix = null;

            if (reader.HasRows)
            {
                reader.Read();

                prix = new T_Prix();
                prix.Sku = (string)reader.GetValue(0);
                prix.Date_debut = (DateTime)reader.GetValue(1);
                prix.Date_fin = (DateTime)reader.GetValue(2);
                prix.Type_promo = reader.GetValue(4) == DBNull.Value ? null : (string)reader.GetValue(4);
                prix.Code_Pays = (int)reader.GetValue(5);
                prix.Prix_produit = (Decimal)reader.GetValue(3);

            //ajouter par Cillia
               // prix.Prix_produit1 = (Decimal)reader.GetValue(3);

             // mis en commentaire par Cillia (reste à verifier)
               //if (prix.Type_promo=="P" && ecoMobilier != null )
                 //{
                   //  prix.Prix_produit = prix.Prix_produit - (decimal)ecoMobilier;
                 // } 
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return prix;
        }



        /// <summary>
        /// Retourne la objectsToUpdate des dates d'un produit triées par ordre chronologique
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static List<DateTime> getOrderedDatesDebut(string Sku, int langageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select Date_debut from Prix ";
            sqlQuery = sqlQuery + " Where Sku = " + Sku;
            sqlQuery = sqlQuery + " and Code_pays = " + langageId;
            sqlQuery = sqlQuery + " order by Date_debut ";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<DateTime> dates = new List<DateTime>();
            while (reader.Read())
            {
                dates.Add((DateTime)reader.GetValue(0));
            }
            
            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            
            return dates;
        }

        /// <summary>
        /// Retourne liste date; dans l ordre chronogique; pour le graphe sur la fiche produit (plus utilisée)
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static List<T_Prix_evol> getAllPrixDateForGraph(string Sku, int langageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " select Prix_produit, date_debut, date_fin, Type_promo FROM Prix ";
            sqlQuery = sqlQuery + " Where Sku = " + Sku;
            sqlQuery = sqlQuery + " and Code_pays = " + langageId;
            sqlQuery = sqlQuery + " order by Date_debut ";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<T_Prix_evol> dates = new List<T_Prix_evol>();

            while (reader.Read())
            {
                T_Prix_evol objectPrix = new T_Prix_evol();

                objectPrix.Prix_produit = (Decimal)reader.GetValue(0);
                objectPrix.Date = (DateTime)reader.GetValue(1);
                objectPrix.Type_promo = reader.GetValue(3) == DBNull.Value ? null : (string)reader.GetValue(3);
                dates.Add(objectPrix);

                T_Prix_evol objectPrix2 = new T_Prix_evol();
                objectPrix2.Prix_produit = (Decimal)reader.GetValue(0);
                objectPrix2.Date = (DateTime)reader.GetValue(2);
                objectPrix2.Type_promo = reader.GetValue(3) == DBNull.Value ? null : (string)reader.GetValue(3);
                dates.Add(objectPrix2);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            //tri dates par ordre chronologique
            int nbdedates = dates.Count;
            int I, J;//compteur boucle
            for (I = nbdedates - 2; I >= 0; I--)
            {
                for (J = 0; J <= I; J++)
                {
                    if (dates[J + 1].Date < dates[J].Date)//si pas dans l'ordre invertion
                    {
                        T_Prix_evol t = dates[J + 1];
                        dates[J + 1] = dates[J];
                        dates[J] = t;
                    }
                }
            }
            return dates;
        }




        /// <summary>
        /// Retourne l'historique des prixCal sous forme de objectsToUpdate.
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static List<T_Prix> getAllPrix(string Sku, int langageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " select Prix_produit, date_debut, date_fin, Type_promo FROM Prix ";
            sqlQuery = sqlQuery + " Where Sku = " + Sku;
            sqlQuery = sqlQuery + " and Code_pays = " + langageId;
            sqlQuery = sqlQuery + " order by Date_debut ";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<T_Prix> dates = new List<T_Prix>();
            
            while (reader.Read())
            {
                T_Prix objectPrix = new T_Prix();

                objectPrix.Prix_produit = (Decimal)reader.GetValue(0);
                objectPrix.Date_debut = (DateTime)reader.GetValue(1);
                objectPrix.Date_fin = (DateTime)reader.GetValue(2);
                objectPrix.Type_promo = reader.GetValue(3) == DBNull.Value ? null : (string)reader.GetValue(3);

                dates.Add(objectPrix);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            
            return dates;
        }

        /// <summary>
        /// Insère un listePlus pour un produit
        /// </summary>
        /// <param name="listePlus"></param>
        public static void insertPrix(T_Prix prix)
        {
            string sqlQuery = "Insert into Prix values ('" + prix.Sku + "', CAST('" + DateUtils.getFormatDateAng(prix.Date_debut) + "' as datetime), CAST('" + DateUtils.getFormatDateAng(prix.Date_fin) + "' as datetime), '" + prix.Prix_produit + "', '" + prix.Type_promo + "', '" + prix.Code_Pays + "', CAST('" + DateUtils.getFormatDateAng(prix.Date_fin) + "' as datetime), '')";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// Retourne l'historique des prixCal sous forme de objectsToUpdate.
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static List<T_PrixEdition> getAllPrixEdition(int langageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + "select ";
            sqlQuery = sqlQuery + "distinct ";
            sqlQuery = sqlQuery + "Produit.Sku, ";
            sqlQuery = sqlQuery + "Range.RangeName, ";
            sqlQuery = sqlQuery + "Variation.VariationName, ";
            sqlQuery = sqlQuery + "LEFT(Produit.Division, 1) Famille, ";
            sqlQuery = sqlQuery + "LEFT(Produit.Division, 2) Sous_famille, ";
            sqlQuery = sqlQuery + "LEFT(Produit.Division, 3) Sous_Sous_famille, ";
            sqlQuery = sqlQuery + "cast(prix.Prix_produit as varchar), ";
            sqlQuery = sqlQuery + "CONVERT(VARCHAR(10),Prix.Date_debut,103), ";
            sqlQuery = sqlQuery + "CONVERT(VARCHAR(10),Prix.Date_fin,103), ";
            sqlQuery = sqlQuery + "Type_prix.DescriptionType ";
            sqlQuery = sqlQuery + "from ";
            sqlQuery = sqlQuery + "Produit, ";
            sqlQuery = sqlQuery + "Prix, ";
            sqlQuery = sqlQuery + "Range, ";
            sqlQuery = sqlQuery + "Variation, ";
            sqlQuery = sqlQuery + "Type_prix, ";
            sqlQuery = sqlQuery + "pro_gce ";
            sqlQuery = sqlQuery + "where ";
            sqlQuery = sqlQuery + "Prix.Sku  = Produit.Sku ";
            sqlQuery = sqlQuery + "and Produit.RangeId = Range.Id ";
            sqlQuery = sqlQuery + "and Variation.Sku = Produit.Sku ";
            sqlQuery = sqlQuery + "and Type_prix.Type_promo = Prix.Type_promo ";
            sqlQuery = sqlQuery + "and Type_prix.LangageId = Prix.Code_pays ";
            sqlQuery = sqlQuery + "and Variation.LangageId = Prix.Code_pays ";
            sqlQuery = sqlQuery + "and produit.Sku= pro_gce.Sku ";
            sqlQuery = sqlQuery + "and GETDATE() between Date_debut and Date_fin ";
            sqlQuery = sqlQuery + "and Prix.Code_pays = " + langageId + " ";
            sqlQuery = sqlQuery + "order by produit.Sku";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<T_PrixEdition> prixEdition = new List<T_PrixEdition>();

            while (reader.Read())
            {
                T_PrixEdition objectPrix = new T_PrixEdition();

                objectPrix.Sku = (String)reader.GetValue(0);
                objectPrix.RangeName = (String)reader.GetValue(1);
                objectPrix.VariationName = (String)reader.GetValue(2);
                objectPrix.Famille = reader.GetValue(3) == DBNull.Value ? "" : (String)reader.GetValue(3);
                objectPrix.SousFamille = reader.GetValue(4) == DBNull.Value ? "" : (String)reader.GetValue(4);
                objectPrix.SousSousFamille = reader.GetValue(5) == DBNull.Value ? "" : (String)reader.GetValue(5);
                objectPrix.Prix_produit = (String)reader.GetValue(6);
                objectPrix.Date_debut = (String)reader.GetValue(7);
                objectPrix.Date_fin = (String)reader.GetValue(8);
                objectPrix.Type_promo = (String)reader.GetValue(9);

                prixEdition.Add(objectPrix);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return prixEdition;
        }


        /// <summary>
        /// Retourne l'historique des prixCal sous forme de objectsToUpdate.
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static List<string> getAllPrixEditionBis(int langageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + "select ";
            sqlQuery = sqlQuery + "distinct ";
            sqlQuery = sqlQuery + "Produit.Sku + ';' + ";
            sqlQuery = sqlQuery + "Range.RangeName + ';' + ";
            sqlQuery = sqlQuery + "Variation.VariationName + ';' + ";
            sqlQuery = sqlQuery + "LEFT(Produit.Division, 1) + ';' + ";
            sqlQuery = sqlQuery + "LEFT(Produit.Division, 2) + ';' + ";
            sqlQuery = sqlQuery + "LEFT(Produit.Division, 3) + ';' + ";
            sqlQuery = sqlQuery + "cast(prix.Prix_produit as varchar) + ';' + ";
            sqlQuery = sqlQuery + "CONVERT(VARCHAR(10),Prix.Date_debut,103) + ';' + ";
            sqlQuery = sqlQuery + "CONVERT(VARCHAR(10),Prix.Date_fin,103) + ';' + ";
            sqlQuery = sqlQuery + "Type_prix.DescriptionType ";
            sqlQuery = sqlQuery + "from ";
            sqlQuery = sqlQuery + "Produit, ";
            sqlQuery = sqlQuery + "Prix, ";
            sqlQuery = sqlQuery + "Range, ";
            sqlQuery = sqlQuery + "Variation, ";
            sqlQuery = sqlQuery + "Type_prix, ";
            sqlQuery = sqlQuery + "pro_gce ";
            sqlQuery = sqlQuery + "where ";
            sqlQuery = sqlQuery + "Prix.Sku  = Produit.Sku ";
            sqlQuery = sqlQuery + "and Produit.RangeId = Range.Id ";
            sqlQuery = sqlQuery + "and Variation.Sku = Produit.Sku ";
            sqlQuery = sqlQuery + "and Type_prix.Type_promo = Prix.Type_promo ";
            sqlQuery = sqlQuery + "and Type_prix.LangageId = Prix.Code_pays ";
            sqlQuery = sqlQuery + "and Variation.LangageId = Prix.Code_pays ";
            sqlQuery = sqlQuery + "and produit.Sku= pro_gce.Sku ";
            sqlQuery = sqlQuery + "and GETDATE() between Date_debut and Date_fin ";
            sqlQuery = sqlQuery + "and Prix.Code_pays = " + langageId + " ";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<string> objectPrixString = new List<string>();
            while (reader.Read())
            {
                objectPrixString.Add(reader.GetValue(0) == DBNull.Value ? "" : (String)reader.GetValue(0));
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return objectPrixString;
        }


    }
}
