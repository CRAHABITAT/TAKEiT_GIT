using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TickitNewFace.Models;

namespace TickitNewFace.Managers
{
    public class CsvResponse
    {
        /// <summary>
        /// renvoie la liste des prix
        /// </summary>
        /// <param name="sku">langage ID</param>
        /// <returns></returns>
        public static string getAllPrixEditionCsv(int langageId)
        {
            string lineCsv = "";
            lineCsv = lineCsv + "Sku" + ";";
            lineCsv = lineCsv + "RangeName" + ";";
            lineCsv = lineCsv + "VariationName" + ";";
            lineCsv = lineCsv + "Famille" + ";";
            lineCsv = lineCsv + "SousFamille" + ";";
            lineCsv = lineCsv + "SousSousFamille" + ";";
            lineCsv = lineCsv + "produit" + ";";
            lineCsv = lineCsv + "Date_debut" + ";";
            lineCsv = lineCsv + "Date_fin" + ";";
            lineCsv = lineCsv + "Type_promo";
            lineCsv = lineCsv + "\r\n";

            List<T_PrixEdition> allPrix = DAO.PrixDao.getAllPrixEdition(langageId);
            foreach (T_PrixEdition prix in allPrix) 
            {
                lineCsv = lineCsv + prix.Sku + ";";
                lineCsv = lineCsv + prix.RangeName + ";";
                lineCsv = lineCsv + prix.VariationName + ";";
                lineCsv = lineCsv + prix.Famille + ";";
                lineCsv = lineCsv + prix.SousFamille + ";";
                lineCsv = lineCsv + prix.SousSousFamille + ";";
                lineCsv = lineCsv + prix.Prix_produit + ";";
                lineCsv = lineCsv + prix.Date_debut + ";";
                lineCsv = lineCsv + prix.Date_fin + ";";
                lineCsv = lineCsv + prix.Type_promo;
                lineCsv = lineCsv + "\r\n";
            }

            return lineCsv;
        }

        /// <summary>
        /// renvoie la liste des prix
        /// </summary>
        /// <param name="sku">langage ID</param>
        /// <returns></returns>
        public static string getAllPrixEditionCsvBis(int langageId)
        {
            string lineCsv = "";
            lineCsv = lineCsv + "Sku" + ";";
            lineCsv = lineCsv + "RangeName" + ";";
            lineCsv = lineCsv + "VariationName" + ";";
            lineCsv = lineCsv + "Famille" + ";";
            lineCsv = lineCsv + "SousFamille" + ";";
            lineCsv = lineCsv + "SousSousFamille" + ";";
            lineCsv = lineCsv + "produit" + ";";
            lineCsv = lineCsv + "Date_debut" + ";";
            lineCsv = lineCsv + "Date_fin" + ";";
            lineCsv = lineCsv + "Type_promo";
            lineCsv = lineCsv + "\r\n";

            List<string> allPrix = DAO.PrixDao.getAllPrixEditionBis(langageId);
            foreach (string prix in allPrix)
            {
                lineCsv = lineCsv + prix + "\r\n";
            }

            return lineCsv;
        }
    }
}