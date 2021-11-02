using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using TickitNewFace.Models;
using System.Web;

namespace TickitNewFace.DAO
{
    public class Resultats_RecherchePLVDao
    {
        /// <summary>
        /// insert une plv de gamme
        /// </summary>
        /// <param name="Gamme"></param>
        /// <param name="langageId"></param>
        public static void insertPLVGamme(T_Presentation_Gamme_PLV_tmp Gamme, int langageId)
        {
            //string sqlQuery = "insert into list_gamme_impression_PLV values('" + Gamme.nomGamme + "','" + Gamme.sousTitreGamme + "','" + Gamme.nomSGamme + "','" + Gamme.descSGamme + "','" + Gamme.skus + "','" + Gamme.options + "','" + langageId + "','" + Gamme.logoGamme + "','" + Gamme.dgccrfGamme + "','" + Gamme.formatGamme + "');";
            Gamme.nomGamme = Gamme.nomGamme.Replace("'", "''");
            Gamme.sousTitreGamme = Gamme.sousTitreGamme.Replace("'", "''");
            Gamme.nomSGamme = Gamme.nomSGamme.Replace("'", "''");
            Gamme.descSGamme = Gamme.descSGamme.Replace("'", "''");
            Gamme.skus = Gamme.skus.Replace("'", "''");
            Gamme.options = Gamme.options.Replace("'", "''");
            Gamme.dgccrfGamme = Gamme.dgccrfGamme.Replace("'", "''");
            Gamme.formatGamme = Gamme.formatGamme.Replace("'", "''");
            string sqlQuery = "EXECUTE dbo.insertPlvGamme'" + Gamme.nomGamme + "','" + Gamme.sousTitreGamme + "','" + Gamme.nomSGamme + "','" + Gamme.descSGamme + "','" + Gamme.skus + "','" + Gamme.options + "','" + langageId + "','" + Gamme.logoGamme + "','" + Gamme.dgccrfGamme + "','" + Gamme.formatGamme + "';";
            
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }


        /// <summary>
        /// Renvoie les résultats de recherche pour selection
        /// </summary>
        /// <param name="rechercheText"></param>
        /// <returns></returns>
        public static List<T_Resultats_Recherche_PLV> getResultsRechByCriteriaPLV(string rechercheText, int langageId, DateTime date)
        {
            // NB : il faut changer les libellés solde et promotion dès lors quand les change dans la base de données.
            string sqlQuery = "Select TOP 1000 Gamme, SousTitreGamme, NomGlobal, NomOption,LangageId, Logo, DescriptionDgccrf, Format, Skus from list_gamme_impression_PLV  Where Gamme like '%" + rechercheText + "%' and LangageId = " + langageId + ";";
           // sqlQuery = sqlQuery + " and Date_debut <= '" + date.Year + "-" + date.Month + "-" + date.Day + "'";
           // sqlQuery = sqlQuery + " and Date_fin >= '" + date.Year + "-" + date.Month + "-" + date.Day + "'";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<T_Resultats_Recherche_PLV> resultats = new List<T_Resultats_Recherche_PLV>();
            while (reader.Read())
            {
                T_Resultats_Recherche_PLV resultat = new T_Resultats_Recherche_PLV();

                resultat.Gamme = (string)reader.GetValue(0);
                resultat.sousGammes = (string)reader.GetValue(2);
                resultat.Skus = (string)reader.GetValue(8);
                resultat.DescriptionDgccrf = (string)reader.GetValue(6);
                resultat.Format = (string)reader.GetValue(7);
                resultats.Add(resultat);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            return resultats;
        }

        /// <summary>
        /// Renvoie les résultats de recherche pour impression
        /// </summary>
        /// <param name="rechercheText"></param>
        /// <returns></returns>
        public static T_Presentation_Gamme_PLV RemplirPresentationGammePLV(string Gamme, int langageId)
        {
            Gamme = Gamme.Replace("'", "''");
            // NB : il faut changer les libellés solde et promotion dès lors quand les change dans la base de données.
            string sqlQuery = "Select Gamme, SousTitreGamme, NomGlobal, DescriptionGlobal, Skus, NomOption, LangageId, Logo, DescriptionDgccrf, Format from list_gamme_impression_PLV  Where Gamme = '" + Gamme + "' and LangageId = " + langageId + ";";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            int nombreOption = 0;
            T_Presentation_Gamme_PLV_tmp GammeTmp = new T_Presentation_Gamme_PLV_tmp();
            List<string> Option = new List<string>();//liste des options sans doublons (arg unique)

            //lecture de BDD
            while (reader.Read())
            {
                //gestion option
                GammeTmp.options = (string)reader.GetValue(5);
                GammeTmp.options.Replace(";;","; ;");
                string[] tmpNomOption = GammeTmp.options.Split(';', ',');//separation des opt et des sous opts
                nombreOption = tmpNomOption.Length;//nombre option
                int existDeja = 0;
                foreach (string opt in tmpNomOption)
                {
                    existDeja = 0;
                    foreach (string opt2 in Option)
                    {
                        if (opt2 == opt)
                            existDeja = 1;
                    }
                    if (existDeja == 0)
                    {

                        Option.Add(opt);
                        
                    }
                        
                }
                for (int k = 0; k < Option.Count; k++) { if (Option[k] == "") { Option[k] = " "; } }

                //les autres arg
                GammeTmp.nomGamme = (string)reader.GetValue(0);
                GammeTmp.sousTitreGamme = (string)reader.GetValue(1);
                GammeTmp.nomSGamme = (string)reader.GetValue(2);
                GammeTmp.descSGamme = (string)reader.GetValue(3);
                GammeTmp.skus = (string)reader.GetValue(4);
                GammeTmp.logoGamme = (int)reader.GetValue(7);
                GammeTmp.dgccrfGamme = (string)reader.GetValue(8);
                GammeTmp.formatGamme = (string)reader.GetValue(9);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            // remplissage de T_Presentation_Gamme_PLV
            T_Presentation_Gamme_PLV GammeRemplie = new T_Presentation_Gamme_PLV();
            GammeRemplie.nomGamme = GammeTmp.nomGamme;
            GammeRemplie.sousTitreGamme = GammeTmp.sousTitreGamme;
            GammeRemplie.logoGamme = GammeTmp.logoGamme;
            GammeRemplie.dgccrfGamme = GammeTmp.dgccrfGamme;
            GammeRemplie.formatGamme = GammeTmp.formatGamme;
            GammeRemplie.options = Option;

            // remplissage des T_Presentation_Sous_Gamme_PLV
            string[] nomSGammes;
            nomSGammes = GammeTmp.nomSGamme.Split(';');
            string[] descSGammes;
            descSGammes = GammeTmp.descSGamme.Split(';');
            //separation opts
            string[] optionskustmp = GammeTmp.options.Split(';');
            int i;
            string[][] optionskus = new string[optionskustmp.Length][];
            for(i = 0; i<optionskustmp.Length; i++)
            {
                optionskus[i] = optionskustmp[i].Split(',');
            }
            for (i = 0; i < optionskus.Length; i++) { for (int j = 0; j < optionskus[i].Length; j++) { if (optionskus[i][j] == "") { optionskus[i][j] = " "; } } }
            //separation skus
            string[] Tskutmp = GammeTmp.skus.Split(';');
            string[][] Tsku = new string[Tskutmp.Length][];
            for (i = 0; i < Tskutmp.Length; i++)
            {
                Tsku[i] = Tskutmp[i].Split(',');
            }
            List<T_Presentation_Sous_Gamme_PLV> LSGammeRemplie = new List<T_Presentation_Sous_Gamme_PLV>();
            
            for(i = 0; i<nomSGammes.Length; i++)
            {
                T_Presentation_Sous_Gamme_PLV SGammeRemplie = new T_Presentation_Sous_Gamme_PLV();
                SGammeRemplie.nomSGamme = nomSGammes[i];
                SGammeRemplie.descSGamme = descSGammes[i];
                int j,h;
                string sku = " ";
                List<string> Lsku = new List<string>();
                for (j = 0; j < Option.Count; j++){
                    sku = " ";
                    for (h = 0; h < Tsku[i].Length; h++)
                    {
                        if (Option[j] == optionskus[i][h])
                        {
                            sku = Tsku[i][h];
                            if (sku == "")
                            {
                                Lsku.Add(" ");
                            }
                            else
                            {  
                                Lsku.Add(sku);
                            }
                        }
                        
                    }
                    if (sku == " ") {
                        Lsku.Add(sku);
                    }
                }
                SGammeRemplie.skus = Lsku;
                LSGammeRemplie.Add(SGammeRemplie);
            }
            GammeRemplie.sousGammes = LSGammeRemplie;
            return GammeRemplie;
        }


        /// <summary>
        /// Renvoie les résultats de recherche (ancienne version)
        /// </summary>
        /// <param name="rechercheText"></param>
        /// <returns></returns>
        public static Tuple<List<T_Gamme_PLV>, List<T_Sous_Gamme_PLV>> RemplirGammePLV(string Gamme, int langageId)
        {
            // NB : il faut changer les libellés solde et promotion dès lors quand les change dans la base de données.
            Gamme = Gamme.Replace("'", "''");
            string sqlQuery = "Select Gamme, NomGlobal, DescriptionGlobal, Skus, NomOption, LangageId, Logo, DescriptionDgccrf, Format from list_gamme_impression_PLV  Where Gamme = '" + Gamme + "' and LangageId = " + langageId + ";";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<T_Gamme_PLV> Gammes = new List<T_Gamme_PLV>();
            List<T_Sous_Gamme_PLV> SousGammes = new List<T_Sous_Gamme_PLV>();

            while (reader.Read())
            {
                T_Gamme_PLV GammeCourante = new T_Gamme_PLV();

                GammeCourante.nomGamme = (string)reader.GetValue(0);
                GammeCourante.sousGammes = (string)reader.GetValue(1);
                GammeCourante.logoGamme = (int)reader.GetValue(6);
                GammeCourante.dgccrfGamme = (string)reader.GetValue(7);
                GammeCourante.formatGamme = (string)reader.GetValue(8);
                string stmpNomGlobal = (string)reader.GetValue(1);
                string[] tmpNomGlobal = stmpNomGlobal.Split(';');
                string stmpDescriptionGlobal = (string)reader.GetValue(2);
                string[] tmpDescriptionGlobal = stmpDescriptionGlobal.Split(';');
                string stmpSkus = (string)reader.GetValue(3);
                string[] tmpSkus = stmpSkus.Split(';');
                string stmpNomOption = (string)reader.GetValue(4);
                string[] tmpNomOption = stmpNomOption.Split(';');
                int cpt = 0;
                foreach(var item in tmpNomGlobal)
                {
                    T_Sous_Gamme_PLV SGammeCourante = new T_Sous_Gamme_PLV();
                    SGammeCourante.nomSGamme = item;
                    SGammeCourante.descSGamme = tmpDescriptionGlobal[cpt];
                    SGammeCourante.skus = tmpSkus[cpt];
                    SGammeCourante.options = tmpNomOption[cpt];
                    cpt++;
                    SousGammes.Add(SGammeCourante);
                }

                Gammes.Add(GammeCourante);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            return Tuple.Create(Gammes, SousGammes);
        }
    }
}

