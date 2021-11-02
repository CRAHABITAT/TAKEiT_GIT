using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using TickitNewFace.Models;
using System.Web;

namespace TickitNewFace.DAO
{
    public class Resultats_RechercheDao
    {
        /// <summary>
        /// Renvoie les résultats de recherche
        /// </summary>
        /// <param name="rechercheText"></param>
        /// <returns></returns>
        public static List<T_Resultats_Recherche> getResultsRechByCriteria(string rechercheText, int langageId, DateTime date)
        {
            // NB : il faut changer les libellés solde et promotion dès lors quand les change dans la base de données.
            string sqlQuery = "";
            sqlQuery = sqlQuery + " SELECT  distinct TOP 1000 Sku ,RangeName ,VariationName ,Libelle ,Prix_produit - ISNULL(CASE WHEN (CountryCode in (4,10,17,19,20,21,37,38)) THEN eco_mobilier ELSE 0 END,0), DescriptionType ,";
            sqlQuery = sqlQuery + " Date_debut ,Date_fin ,DescriptionStatut ,CountryCode, Division, NombreFormatsImpressionDisponibles, Type_promo , ";

            sqlQuery = sqlQuery + " CASE WHEN (Type_promo = 'P' or Type_promo = 'S') THEN ";
            sqlQuery = sqlQuery + " (";
            sqlQuery = sqlQuery + " Select top 1 - CONVERT(Decimal(9,2),((Pr.Prix_produit- rech.Prix_produit) / (Pr.Prix_produit - ISNULL(CASE WHEN (CountryCode in (4,10,17,19,20,21,37,38)) THEN eco_mobilier ELSE 0 END,0))) * 100) from Prix Pr";
            sqlQuery = sqlQuery + " Where Sku = rech.Sku";
            sqlQuery = sqlQuery + " and Code_pays = " + langageId;
            sqlQuery = sqlQuery + " and Date_debut <= '" + date.Year + "-" + date.Month + "-" + date.Day + "'";
            sqlQuery = sqlQuery + " and Date_fin >= '" + date.Year + "-" + date.Month + "-" + date.Day + "'";
            sqlQuery = sqlQuery + " and Type_promo = 'N'";
            sqlQuery = sqlQuery + " order by Date_debut desc ";
            sqlQuery = sqlQuery + " )END AS Pourcentage_reduction ";

            sqlQuery = sqlQuery + " FROM resultats_recherche rech where ";
            sqlQuery = sqlQuery + " ( ";
            sqlQuery = sqlQuery + " RangeName like '%" + rechercheText.ToUpper().Trim() + "%' ";
            sqlQuery = sqlQuery + " Or Sku like '" + rechercheText.Trim() + "' ";
            sqlQuery = sqlQuery + " Or VariationName like '%" + rechercheText.ToUpper().Trim() + "%' ";
            sqlQuery = sqlQuery + " ) ";

            sqlQuery = sqlQuery + " and ";
            sqlQuery = sqlQuery + " '" + date.Year + "-" + date.Month + "-" + date.Day + "' between Date_debut and Date_fin ";
            sqlQuery = sqlQuery + " and ";
            sqlQuery = sqlQuery + " CountryCode = " + langageId;
            sqlQuery = sqlQuery + " order by Date_debut desc";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<T_Resultats_Recherche> resultats = new List<T_Resultats_Recherche>();

            while (reader.Read())
            {
                T_Resultats_Recherche resultat = new T_Resultats_Recherche();

                resultat.Sku = (string)reader.GetValue(0);
                resultat.RangeName = (string)reader.GetValue(1);
                resultat.VariationName = (string)reader.GetValue(2);
                resultat.Libelle = (string)reader.GetValue(3);
                resultat.Prix_produit = (Decimal)reader.GetValue(4);
                resultat.Type_promo = (string)reader.GetValue(5);
                resultat.Date_debut = (DateTime)reader.GetValue(6);
                resultat.Date_fin = (DateTime)reader.GetValue(7);

                string strObj = reader.GetValue(8) == DBNull.Value ? null : (string)reader.GetValue(8);
                resultat.Statut = strObj;

                resultat.CountryCode = reader.GetValue(9) == DBNull.Value ? null : (int?)reader.GetValue(9);

                int? divClassSousFamille = reader.GetValue(10) == DBNull.Value ? null : (int?)reader.GetValue(10);
                int? division = null;

                if (divClassSousFamille != null)
                {
                    string divClassSousFamilleString = divClassSousFamille.ToString();
                    string divisionString = divClassSousFamilleString[0].ToString();
                    division = int.Parse(divisionString);
                }

                resultat.Division = division;

                resultat.NombreFormatsImpressionDisponibles = (int?)reader.GetValue(11);
                resultat.Pourcentage_reduction = reader.GetValue(13) == DBNull.Value ? null : (decimal?)reader.GetValue(13);

                resultat.Pourcentage_reduction = Utils.SpecificMathUtils.getRoundDecimal(resultat.Pourcentage_reduction);

                // éviter d'avoir les lignes résiduelles : ne prendre que la première ligne dans le cas de plusieurs lignes par SKU.
                Boolean SkuNotExists = true;
                foreach (T_Resultats_Recherche element in resultats)
                {
                    if (element.Sku == resultat.Sku) { SkuNotExists = false; }
                }

                if (SkuNotExists)
                {
                    resultats.Add(resultat);
                }
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return resultats;
        }
    }
}
