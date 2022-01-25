using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web;
using TickitNewFace.Models;

namespace TickitNewFace.DAO
{
    public static class ProduitDao
    {
        public static T_IPLV_Details getIPLVDetailsProduitBySku(string sku, int langageId)
        {
            string sqlQuery = "SELECT sku,dimension_produit_txt,dimension_colis_txt,couleurs,matieres,designed_habitat FROM [plv_details_produit] where Sku = '" + sku + "' and LangageId = " + langageId;
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();


            T_IPLV_Details DIPLV = new T_IPLV_Details();
            while (reader.Read())
            {
                DIPLV.sku = (string)reader.GetValue(0);
                DIPLV.dimension_produit = (string)reader.GetValue(1);
                DIPLV.dimension_colis = (string)reader.GetValue(2);
                string couleur = (string)reader.GetValue(3);
                DIPLV.couleurs = couleur.Split(',');
                string matiere = (string)reader.GetValue(4);
                if (matiere != "") { 
                    DIPLV.matieres = matiere.Split(',');
                }
                DIPLV.designed_habitat = (string)reader.GetValue(5);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return DIPLV;
        }

        public static List<T_IPLV_Association> getSkusGroupeProduitBySku(string sku, int langageId)
        {
            string sqlQuery = "select sku_associe,position, nom_ambiance from Ambiance where sku_principal = " + sku + " order by position";
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();


            List<T_IPLV_Association> LAPIPLV = new List<T_IPLV_Association>();
            while (reader.Read())
            {
                T_IPLV_Association APIPLV = new T_IPLV_Association();
                APIPLV.sku = (string)reader.GetValue(0);
                APIPLV.rang = (int)reader.GetValue(1);
                APIPLV.nom = (string)reader.GetValue(2);
                List<string> limg = Managers.RechercheManager.getIMG(APIPLV.sku);
                APIPLV.limg = limg;
                LAPIPLV.Add(APIPLV);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return LAPIPLV;
        }

        public static List<T_IPLV_Association> getSkusAssociesProduitBySku(string sku, int langageId)
        {
            string sqlQuery = "select sku_associe,position from Association where sku_principal = " + sku + " order by position";
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();


            List<T_IPLV_Association> LAPIPLV = new List<T_IPLV_Association>();
            while (reader.Read())
            {
                T_IPLV_Association APIPLV = new T_IPLV_Association();
                APIPLV.sku = (string)reader.GetValue(0);
                APIPLV.rang = (int)reader.GetValue(1);
                List<string> limg = Managers.RechercheManager.getIMG(APIPLV.sku);
                APIPLV.limg = limg;
                LAPIPLV.Add(APIPLV);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return LAPIPLV;
        }


        public static string getDesignerProduitBySku(string sku, int langageId)
        {
            string sqlQuery = "select Designer from designer where Sku = " + sku;
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();


            string DM = null;
            while (reader.Read())
            {
                DM = (string)reader.GetValue(0);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return DM;
        }

        public static string getOrientationBySku(string sku, int langageId)
        {
            string sqlQuery = " select orientation from Description_orientation ";
            sqlQuery = sqlQuery + " where Sku = '" + sku + "'";
            sqlQuery = sqlQuery + " and LangageId = " + langageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();


            string orientation = "";
            while (reader.Read())
            {
                orientation = (string)reader.GetValue(0);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return orientation;
        }
        
        public static string getMadeInByRangeId(int RangeId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " select top 1 Made_in from produit";
            sqlQuery = sqlQuery + " where";
            sqlQuery = sqlQuery + " RangeId = " + RangeId;
            sqlQuery = sqlQuery + " and Made_in <> ''";
            sqlQuery = sqlQuery + " and Made_in is not null";

            
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();


            string madeInGamme = "";
            while (reader.Read())
            {
                madeInGamme = (string)reader.GetValue(0);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return getMadeInEffectif(madeInGamme);
        }


        public static List<string> getListeMadeIn()
        {
            string sqlQuery = "select distinct Made_in from Produit where Made_in is not null";
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();


            List<string> madeInList = new List<string>();
            while (reader.Read())
            {
                madeInList.Add(getMadeInEffectif((string)reader.GetValue(0)));
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return madeInList;
        }


        /// <summary>
        /// description marketing
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="langageId"></param>
        /// <returns></returns>
        public static string getDrescriptionConvertibleBySku(string sku, int langageId, string typeDimension)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery +" select Description from Description_Dimensions_Convertibles";
            sqlQuery = sqlQuery +" where";
            sqlQuery = sqlQuery +" sku = '" + sku + "'" ;
            sqlQuery = sqlQuery +" and Type_dimension = '" + typeDimension + "'";
            sqlQuery = sqlQuery + "and LangageId = " + langageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            string DM = "";
            while (reader.Read())
            {
                DM = (string)reader.GetValue(0);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return DM;
        }


        /// <summary>
        /// description marketing
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="langageId"></param>
        /// <returns></returns>
        public static string getDrescMaketingProduitBySku(string sku, int langageId)
        {
            string sqlQuery = "select LegalDescription from Description_Marketing where Sku = '" + sku + "' and LangageId = " + langageId;
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();


            string DM = null;
            while (reader.Read())
            {
                DM = (string)reader.GetValue(0);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return DM;
        }

        /// <summary>
        /// Retourne un produit
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static T_Produit getProduitBySku(string Sku, int langageId)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + " Select Sku, Longueur, Largeur, Hauteur, Poids, Volume, RangeId, Self_Assembly, statut_produit.DescriptionStatut, Eco_mobilier, Eco_emballage, Eco_part, Nombre_colis, Division, Made_in, Profondeur, Diametre ";
            sqlQuery = sqlQuery + " From Produit left outer join statut_produit on Produit.Statut = statut_produit.CodeStatut and langageId = " + langageId;
            sqlQuery = sqlQuery + " Where Sku = " + Sku;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            T_Produit produit = null;

            if (reader.HasRows)
            {
                reader.Read();
                
                produit = new T_Produit();
                produit.Sku = reader.GetValue(0) == DBNull.Value ? null : (string)reader.GetValue(0);
                produit.Longueur = reader.GetValue(1) == DBNull.Value ? null : (decimal?)reader.GetValue(1);
                produit.Largeur = reader.GetValue(2) == DBNull.Value ? null : (decimal?)reader.GetValue(2);
                produit.Hauteur = reader.GetValue(3) == DBNull.Value ? null : (decimal?)reader.GetValue(3);

                produit.Poids = reader.GetValue(4) == DBNull.Value ? null : (decimal?)reader.GetValue(4);
                produit.Volume = reader.GetValue(5) == DBNull.Value ? null : (decimal?)reader.GetValue(5);

                produit.RangeId = (int)reader.GetValue(6);

                produit.Self_Assembly = reader.GetValue(7) == DBNull.Value ? null : (string)reader.GetValue(7);
                produit.Statut = reader.GetValue(8) == DBNull.Value ? null : (string)reader.GetValue(8);

                produit.Eco_mobilier = reader.GetValue(9) == DBNull.Value ? null : (decimal?)reader.GetValue(9);
                produit.Eco_emballage = reader.GetValue(10) == DBNull.Value ? null : (decimal?)reader.GetValue(10);
                produit.Eco_part = reader.GetValue(11) == DBNull.Value ? null : (decimal?)reader.GetValue(11);

                // les taxes ne sont effectives que pour la France pour le moment.
                // solution de rapidité mais crade. A bien faire dès que possible.
                if (produit.Eco_mobilier == 0 || !(langageId == 4 || langageId == 10 || langageId == 17 || langageId == 19 || langageId == 20 || langageId == 21 || langageId == 37 || langageId == 38))
                {
                    produit.Eco_mobilier = null;
                }
                if (produit.Eco_mobilier == 0 || !(langageId == 4 || langageId == 10 || langageId == 17 || langageId == 19 || langageId == 20 || langageId == 21 || langageId == 37 || langageId == 38))
                {
                    produit.Eco_emballage = null;
                }
                if (produit.Eco_mobilier == 0 || !(langageId == 4 || langageId == 10 || langageId == 17 || langageId == 19 || langageId == 20 || langageId == 21 || langageId == 37 || langageId == 38))
                {
                    produit.Eco_part = null;
                }

                produit.Nombre_colis = reader.GetValue(12) == DBNull.Value ? null : (int?)reader.GetValue(12);

                int? divClassSousFamille = reader.GetValue(13) == DBNull.Value ? null : (int?)reader.GetValue(13);
                int? division = null;

                if (divClassSousFamille != null)
                {
                    string divClassSousFamilleString = divClassSousFamille.ToString();
                    string divisionString = divClassSousFamilleString[0].ToString();
                    division = int.Parse(divisionString);
                }
                produit.Division = division;

                produit.Made_In = reader.GetValue(14) == DBNull.Value ? null : getMadeInEffectif((string)reader.GetValue(14));
                produit.Profondeur = reader.GetValue(15) == DBNull.Value ? null : (decimal?)reader.GetValue(15);
                produit.Diametre = reader.GetValue(16) == DBNull.Value ? null : (decimal?)reader.GetValue(16);
            }
            
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return produit;
        }

        public static void ajoutFormatImpressionBySku(string Sku, string format)
        {
            string sqlQuery = "IF NOT EXISTS (select * from Produit_Impression where Sku = '" + Sku + "' and Format = '" + format + "') insert into Produit_Impression values ('" + Sku + "','" + format + "')";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();

            cmd.Dispose();
        }

        public static void supprimerFormatImpressionBySku(string Sku, string format)
        {
            string sqlQuery = "delete from Produit_Impression where Sku = '" + Sku + "' and Format = '" + format + "'";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public static void supprimertoutFormatImpressionBySku(string Sku)
        {
            string sqlQuery = "delete from Produit_Impression where Sku = '" + Sku + "'";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
        

          /// <summary>
        /// insert IPLV designed_habitat
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void insertPorduitDH(string designed_habitat, string Sku, int langageId)
        {
            if (designed_habitat == null)
                designed_habitat = "";
            designed_habitat = designed_habitat.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " INSERT INTO [plv_details_produit] VALUES (";
            sqlQuery = sqlQuery + "'" + Sku + "'";
            sqlQuery = sqlQuery + ",'" + langageId + "'";
            sqlQuery = sqlQuery + ",'','','',''";
            sqlQuery = sqlQuery + ",'" + designed_habitat + "')";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }


          /// <summary>
        /// Met à jour IPLV designed_habitat
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updatePorduitDH(string designed_habitat, string Sku, int langageId)
        {
            designed_habitat = designed_habitat.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " Update [plv_details_produit] set ";
            sqlQuery = sqlQuery + " designed_habitat = '" + designed_habitat + "' where";
            sqlQuery = sqlQuery + " Sku = '" + Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + langageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }


        /// <summary>
        /// Met à jour IPLV dimprod
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updatePorduitdimprod(string dimprod, string Sku, int langageId)
        {
            dimprod = dimprod.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " update [plv_details_produit] set ";
            sqlQuery = sqlQuery + " dimension_produit_txt = '" + dimprod + "' where";
            sqlQuery = sqlQuery + " Sku = '" + Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + langageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }


        /// <summary>
        /// Met à jour IPLV dimcolis
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updatePorduitdimcolis(string dimcolis, string Sku, int langageId)
        {
            dimcolis = dimcolis.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " update [plv_details_produit] set ";
            sqlQuery = sqlQuery + " dimension_colis_txt = '" + dimcolis + "' where";
            sqlQuery = sqlQuery + " Sku = '" + Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + langageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }


        /// <summary>
        /// Met à jour IPLV Matieres
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updatePorduitMatieres(List<string> Matieres, string Sku, int langageId)
        {
            string s = "";
            for (int i = 0; i < Matieres.Count; i++) {
                if (i != 0)
                    s += ",";
                s+= Matieres[i].Replace("'", "''");
            }

            string sqlQuery = "";
            sqlQuery = sqlQuery + " update [plv_details_produit] set ";
            sqlQuery = sqlQuery + " matieres = '" + s + "' where";
            sqlQuery = sqlQuery + " Sku = '" + Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + langageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }

        /// <summary>
        /// Met à jour IPLV Couleurs
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void insertPorduitskusassocies(T_IPLV_Association skusassocie, string Sku, int langageId)
        {
            string s = skusassocie.sku.Replace("'", "''");
     

            string sqlQuery = "";
            sqlQuery = sqlQuery + " insert into Association values ('";
            sqlQuery = sqlQuery + Sku + "','";
            sqlQuery = sqlQuery + skusassocie.rang + "','";
            sqlQuery = sqlQuery + s + "');";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public static void deletePorduitskusassocies(string Sku, int langageId)
        {

            string sqlQuery = "";
            sqlQuery = sqlQuery + " delete from Association where sku_principal = '";
            sqlQuery = sqlQuery + Sku + "';";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }



        /// <summary>
        /// Met à jour IPLV Couleurs
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void insertPorduitskusgroupes(T_IPLV_Association skusassocie, string Sku, int langageId)
        {
            if (skusassocie.nom == null)
                skusassocie.nom = "";
            string s = skusassocie.nom.Replace("'", "''");
            string s2 = skusassocie.sku.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " insert into Ambiance values ('";
            sqlQuery = sqlQuery + s + "','";
            sqlQuery = sqlQuery + Sku + "','";
            sqlQuery = sqlQuery + skusassocie.rang + "','";
            sqlQuery = sqlQuery + s2 + "');";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public static void deletePorduitskusgroupes(string nom, string Sku, int langageId)
        {

            string sqlQuery = "";
            sqlQuery = sqlQuery + " delete from Ambiance where sku_principal = '";
            sqlQuery = sqlQuery + Sku + "' and nom_ambiance = '";
            sqlQuery = sqlQuery + nom + "';";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }



        /// <summary>
        /// Met à jour IPLV Couleurs
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updatePorduitcouleurs(List<string> couleurs, string Sku, int langageId)
        {
            string s = "";
            for (int i = 0; i < couleurs.Count; i++)
            {
                if (i != 0)
                    s += ",";
                s += couleurs[i].Replace("'", "''");
            }

            string sqlQuery = "";
            sqlQuery = sqlQuery + " update [plv_details_produit] set ";
            sqlQuery = sqlQuery + " couleurs = '" + s + "' where";
            sqlQuery = sqlQuery + " Sku = '" + Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + langageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        
        /// <summary>
        /// insert IPLV Dmark
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void insertPorduitDesigner(string Designer, string Sku, int langageId)
        {
            if (Designer == null)
                Designer = "";
            Designer = Designer.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + "INSERT INTO designer VALUES(";
            sqlQuery = sqlQuery + "'" + Sku + "'";
            sqlQuery = sqlQuery + ",'" + Designer + "')";
            

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }


        /// <summary>
        /// Met à jour IPLV Dmark
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updatePorduitDesigner(string Designer, string Sku, int langageId)
        {
            Designer = Designer.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " update designer set ";
            sqlQuery = sqlQuery + " Designer = '" + Designer + "' where";
            sqlQuery = sqlQuery + " Sku = '" + Sku + "' ";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }

        
            /// <summary>
        /// insert IPLV Dmark
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void insertPorduitDmark(string Dmark, string Sku, int langageId)
        {
            if (Dmark == null)
                Dmark = "";
            Dmark = Dmark.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " INSERT INTO Description_Marketing VALUES (";
            sqlQuery = sqlQuery + "'" + Sku + "'";
            sqlQuery = sqlQuery + ",'" + langageId + "'";
            sqlQuery = sqlQuery + ",'" + Dmark + "')";
           
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }


        /// <summary>
        /// Met à jour IPLV Dmark
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updatePorduitDmark(string Dmark, string Sku, int langageId)
        {
            Dmark = Dmark.Replace("'", "''");

            string sqlQuery = "";
            sqlQuery = sqlQuery + " update Description_Marketing set ";
            sqlQuery = sqlQuery + " LegalDescription = '" + Dmark + "' where";
            sqlQuery = sqlQuery + " Sku = '" + Sku + "' ";
            sqlQuery = sqlQuery + " and LangageId = " + langageId;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();   
        }


        public static void insertPorduitIPLV(string detailProduit, string Sku)
        {
            string sqlQuery = "INSERT INTO plv_maj VALUES ('" + Sku + "','" + detailProduit + "',GETDATE())";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }


        /// <summary>
        /// Met à jour IPLV.
        /// </summary>
        /// <param name="Dgccrf"></param>
        public static void updatePorduitIPLV(string detailProduit, string Sku)
        {
            string sqlQuery = "update plv_maj set date_maj = GETDATE() where Sku = '" + Sku + "'and type_donnee = '" + detailProduit + "'";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }


        public static string selectPorduitIPLVupdate(string detailProduit, string Sku)
        {
            string sqlQuery = "select Sku from plv_maj where Sku = '" + Sku + "'and type_donnee = '" + detailProduit + "'";
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);

            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();


            string DM = null;
            while (reader.Read())
            {
                DM = (string)reader.GetValue(0);
            }
            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            return DM;
        }


        /// <summary>
        /// Met à jour un produit
        /// </summary>
        /// <param name="produit"></param>
        public static void updatePorduit(T_Produit produit)
        {
            string profondeur = produit.Profondeur == null ? "null" : produit.Profondeur.ToString();
            string diametre = produit.Diametre == null ? "null" : produit.Diametre.ToString();
            string Largeur = produit.Largeur == null ? "null" : produit.Largeur.ToString();
            string Hauteur = produit.Hauteur == null ? "null" : produit.Hauteur.ToString();
            string Longueur = produit.Longueur == null ? "null" : produit.Longueur.ToString();
            string Poids = produit.Poids == null ? "null" : produit.Poids.ToString();
            string Volume = produit.Volume == null ? "null" : produit.Volume.ToString();
            string Self_Assembly = produit.Self_Assembly == null ? "null" : produit.Self_Assembly.ToString();

            string Eco_mobilier = produit.Eco_mobilier == null ? "null" : produit.Eco_mobilier.ToString();
            string Eco_emballage = produit.Eco_emballage == null ? "null" : produit.Eco_emballage.ToString();
            string Eco_part = produit.Eco_part == null ? "null" : produit.Eco_part.ToString();
            string Made_In = produit.Made_In == null ? "null" : produit.Made_In;

            Eco_mobilier = Eco_mobilier.Replace(',', '.');
            Eco_emballage = Eco_emballage.Replace(',', '.');
            Eco_part = Eco_part.Replace(',', '.');

            string sqlQuery = "";
            sqlQuery = sqlQuery + " Update Produit set ";

            sqlQuery = sqlQuery + " Profondeur =  " + profondeur + ",";
            sqlQuery = sqlQuery + " Diametre =  " + diametre + ",";
            sqlQuery = sqlQuery + " Longueur =  " + Longueur + ",";
            sqlQuery = sqlQuery + " Largeur =  " + Largeur + ",";
            sqlQuery = sqlQuery + " Hauteur =  " + Hauteur + ",";
            sqlQuery = sqlQuery + " Poids =  " + Poids + ",";
            sqlQuery = sqlQuery + " Volume =  " + Volume + ",";
            sqlQuery = sqlQuery + " Self_Assembly =  '" + Self_Assembly + "',";
            sqlQuery = sqlQuery + " Eco_mobilier = " + Eco_mobilier + ",";
            sqlQuery = sqlQuery + " Eco_emballage = " + Eco_emballage + ",";
            sqlQuery = sqlQuery + " Eco_part = " + Eco_part + ",";
            sqlQuery = sqlQuery + " Made_in = '" + Made_In + "'";
            sqlQuery = sqlQuery + " where Sku = " + produit.Sku;

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// Renvoie les formats d'impression possibles pour un produit
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        public static List<string> getFormatsImpressionBySku(string Sku)
        {
            string sqlQuery = "";
            sqlQuery = sqlQuery + "select Format from Produit_Impression ";
            sqlQuery = sqlQuery + "Where Sku = '" + Sku + "'";

            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<string> formats = new List<string>();

            while (reader.Read())
            {
                String format = (string)reader.GetValue(0);
                formats.Add(format);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            
            return formats;
        }


        // 1/2/3/4 reglette
        // D3 : A7_simple / D5 : A6_recto_verso

        // Spécifique Guyane
        
        public static List<string> getProduitsDenisBouchene()
        {
            string sqlQuery = "";
            //sqlQuery = sqlQuery + "select distinct Produit.Sku, Produit.division from Produit, Prix where Produit.Sku = Prix.Sku and (produit.division like '1%' or produit.division like '2%' or produit.division like '3%' or produit.division like '4%') and Prix.Code_pays = 22 and produit.Sku in ('917282','912141','912138','970733','912140','912139','912137','908381','908382','916906','916907','916905','908416','908417','907919','907915','907918','907916','907917','909610','909609','904305','904306','904307','904309','913135','503436','909135','909136','914754','908053','908051','908052','914753','914752','917285','917286','916842','916841','912975','912976','909594','909598','912977','912978','909597','909595','909599','909596','916824','902813','915809','915807','915821','915814','915810','915815','915817','915805','915816','915813','909903','909902','909901','909900','916773','916772','907285','907282','907279','907275','907284','907277','907281','907278','907276','907912','907911','907280','907283','909133','909134','913309','913302','913303','913305','913307','913310','909973','977905','915280','601632','601608','601616','601551','502103','996495','601640','285323','285331','285234','285404','955426','917041','917043','912175','912176','912152','913325','913328','913326','913330','913323','913329','912091','912131','912132','912133','912159','912154','912153','912155','912156','912098','915387','984490','912362','912092','912094','916888','977530','912986','912985','912983','912987','912988','912984','971318','972428','912645','912097','990552','993003','990537','990538','990554','993004','953573','990539','951578','990550','990551','990555','990553','973115','917283','912375','912640','912641','912642','912643','912644','901147','912112','912761','912764','912758','912759','912760','912762','912768','914275','914275','914276','914277','914276','914277','916910','917268','916908','916912','917271','916911','916909','917270','917269','905677','951788','951789','908572','908568','962811','974654','909962','909963','909964','909965','909966','909967','909968','909969','909970','909972','916721','914719','914720','914722','914721','914832','906447','906445','905352','906446','970598','914724','916750','916753','916748','916751','916755','916749','916752','916754','953674','916577','909612','909611','909613','909619','917341','917342','917347','917343','917344','916059','907913','916800','916801','916802','916803','915832','915829','915833','915830','915831','916065','963977','996972','916790','916789','916791','916856','916858','916859','916857','990891','990890','990866','990867','990868','915836','915835','959335','959318','959322','918095','918097','912143','912142','912144','912145','971314','971315','914829','915131','914729','914170','914169','965757','965758','997761','916689','916691','977133','997775','977132','977116','977140','977141','977137','977131','977119','997776','977139','997778','968957','968956','968950','905555','916805','990681','912158','912166','955432','970600','996695','996605','962211','912135','997880','916779','916780','916781','916725','916731','916729','916735','916727','916733','909575','909587','909570','909584','909580','909574','909578','909577','909583','991143','951267','951268','952509','952515','951263','951264','995928','995933','995934','995925','914823','914824','914822','960766','960765','960767','960759','902851','902853','902854','909929','902852','914835','914836','902108','910893','910894','910895','962175','968984','917495','976796','976798','976799','981385','912084','975800','957759','950126','980211','914862','902925','970608','961735','979596','979597','979595','979594','813346','976813','962812','471135','908419','908424','908421','915102','915101','914755','958279','908990','908987','903377','970994','970995','908413','958157','321370','321389','321397','321400','908986','916983','916990','916991','916969','916970','916971','912544','912542','912549','912550','912551','912552','912553','912543','912541','908408','908400','908407','908405','908055','908056','916979','912717','912718','912719','912722','912724','912725','912727','956976','956968','956978','956972','912300','915048','915044','915047','915049','915045','915046','973392','974683','974684','974685','909706','912320','912321','912322','912323','912319','914935','909703','909701','912409','909707','912535','912536','917000','917001','917003','917004','917017','917016','903388','905714','910437','906895','906427','906421','906425','906423','912399','916987','915271','912225','912942','912939','912941','912943','912944','912940','912237','912191','912192','969400','969401','912235','912236','912190','912193','912234','912194','916925','963625','990032','909716','909717','972780','972781','969524','912938','912284','912285','912281','961454','961455','973844','973845','913631','913632','913630','913626','913627','913625','913594','913593','913592','913629','913628','912324','916431','916430','912212','912213','912214','912215','913028','913029','913024','913025','913026','913027','913342','202452','912302','912303','915265','915264','912196','912197','912703','912705','912706','912702','916984','916985','916986','916989','916988','912931','912932','912930','912933','995756','912694','912693','912935','912936','914933','914934','914882','913317','913313','982380','912259','912260','912252','912261','912262','912253','912258','912254','912255','912247','912248','912250','912257','912699','912382','912384','912389','912392','912386','912387','912829','912830','912847','913345','913344','912309','912310','912311','914890','914911','914930','914932','914931','914929','908567','914941','914937','914938','914939','914936','914896','914897','917008','914883','914884','912561','912560','916948','912555','912556','912557','916949','916950','916951','916952','915336','961981','965845','976214','973391','914902','914903','914898','914899','915256','915254','915072','917471','915087','916545','916546','915882','915884','915881','915883','916446','916447','916982','916975','916573','916574','917005','917013','917033','917034','917985','917981','917983','917468','916585','916586','916587','917982','917469','916922','916924','916931','916921','916923','916542','916543','916544','916946','916947','916942','916943','916944','916945','916518','916519','916468','916469','916470','916471','916981','916980','917011','917012','917009','917010','917984','917980','917467','915872','916451','916452','916453','916575','916576','916577','916966','916967','916548','916549','917025','917026','917027','917028','917029','917030','917031','917032','917021','917022','917023','917024','917020','917459','917465','917466','916961','916962','916976','916977','916978','916998','916999','916505','916506','916507','916919','916920','916917','916918','917470','916592','916593','916589','916590','916591','901380','955411','909030','977267','909016','909021','909026','916508','916509','916510','916511','916512','916513','977268','909034','915433','915432','915434','906175','903136','998334','968486','968177','913046','913048','913049','913050','913051','913043','901119','978709','963465','963466','983135','916568','916570','910661','910665','966468','907997','970758','970363','970362','906970','916938','916939','915243','915240','915244','916555','916554','916552','916409','916412','912238','916408','916411','916410','916413','985052','985056','986877','986875','986879','986882','986883','958815','991132','984107','969339','969337','914419','914420','965969','903487','903488','965780','903495','966439','966438','965967','903489','905431','975628','914881','970832','917002','917019','976719','512761','135240','135291','593494','593605','593729','957324','916965','916963','916964','960821','961279','961273','960820','960824','960822','916955','916956','903279','963792','961483','906591','983896','961484','961481','962064','905939','903299','913255','913259','907720','907721','907722','909333','909330','909332','909375','909374','909380','909341','909338','909340','909370','909371','909337','909334','909336','909343','913451','913452','913465','913466','913467','913468','909404','909405','909366','909367','909392','909393','909400','909401','909396','909397','909362','909363','909384','909385','909382','909383','912396','912397','916940','916941','908410','908411','908409','905397','967429','967431','967432','967433','967434','914886','914885','914888','914887','916406','916958','916959','915077','915078','970148','968514','981189','981210','915262','915263','916972','916973','916974','905637','905638','900397','916957','916960','966753','972585','914889','969426','915577','912398','994300','994303','994302','994301','994271','994304','957989','957990','912195','916954','915079','917035','917036','978701','916968','915424','915423','915426','915425','915420','915421','915422','915427','994094','915024','915026','915020','915022','915023','915021','908600','908651','908650','911969','908599','908596','916338','904977','904978','904980','980350','908598','913221','913372','911935','911936','911806','911807','913376','972508','911638','908459','916614','966429','955252','966432','916615','209600','209627','209716','988098','908967','913640','911706','915174','915175','915176','911710','909298','911914','911915','998122','908911','998129','998126','998130','908912','998124','998123','914515','914516','914511','914512','914513','914514','912187','912188','911911','911912','911913','914405','914390','914392','914376','914377','914448','914449','914451','914372','914389','915933','915935','800171','800173','800176','800178','800181','800182','968009','980401','964719','914379','917298','917301','917299','917295','917296','905005','914406','916348','911971','911972','212078','229281','916351','966517','966519','904965','201812','201836','913202','963729','963728','200365','200426','960200','882536','017426','914520','914521','914522','978437','978439','978435','975712','980209','979914','961414','911955','902301','393869','393920','912820','961245','961244','978047','961215','965359','914500','914502','914501','914503','916616','966535','977875','916611','913635','913638','910207','910206','908717','908718','994232','916349','990307','990311','978427','915990','915991','917725','911811','911813','909504','909505','909500','909501','909502','909503','911957','911974','911960','911975','911976','972494','964698','910911','912070','910910','996995','913200','913201','913643','913645','913648','310282','904345','984961','984962','994936','994937','915549','915552','915555','915551','915550','915561','904966','987340','902215','987341','914367','902216','908714','908715','916595','908710','908712','916594','994664','994666','995055','915533','912601','966559','966565','966566','912600','966555','966557','966558','902218','902219','911962','911967','916350','978419','914976','997056','961239','905185','905584','980886','973925','903017','908732','958326','998239','998237','958319','998233','998235','908729','908728','908334','913621','908261','908328','908302','916308','911879','911886','911880','907576','907577','908270','908271','908272','908292','917054','917052','977491','977492','976500','976501','908075','917064','908074','917058','908076','917066','908082','917060','908110','908114','908116','917057','917063','908124','917068','908123','917062','908125','908078','908084','908086','908090','908092','908098','908100','908102','910113','917061','917067','917065','917059','911556','911557','911558','911559','910845','910850','910855','910860','910870','910875','910885','910846','910851','910856','910861','910871','910876','910886','910848','910853','910858','910863','910873','910878','910888','910849','910854','910859','910864','910874','910879','910889','910847','910852','910857','910862','910872','910877','910887','911928','911929','911930','910371','911765','952882','952883','952898','952899','952905','916305','961203','904145','904146','908323','908326','910302','912508','912509','910162','910163','910164','907571','907572','907574','907575','911887','912519','915504','915506','915507','915516','915517','915518','917585','917586','917587','915519','915520','915508','917593','917594','915524','915501','915502','915513','917588','911512','911520','911522','910189','910190','910191','910192','914535','913035','916271','916272','916284','916286','916282','916274','916275','912056','914622','912054','912055','912059','914621','912057','912058','912517','913036','911885','911882','911941','911942','970353','915615','915637','915616','915617','915638','915639','915612','915634','902298','908950','908952','902296','908966','914590','914591','914592','915030','914544','914536','914541','914639','915038','914560','914554','914561','914555','914562','914556','914565','914559','914563','914557','914564','914558','914623','910092','910160','912521','915033','914542','914664','914666','914543','914645','914646','914647','914650','914655','914656','914677','914678','914679','914675','914676','914177','902951','902952','908953','908954','964932','911683','911684','911685','911679','911680','911694','914640','917215','917206','917205','914570','914571','917226','917227','916266','916265','917086','917081','917082','917208','917087','916300','916306','916295','917083','917084','916288','916290','916292','915467','915468','915488','915489','915469','915485','915490','915491','915486','915487','915492','915493','916057','916056','916054','916053','916055','914680','910184','970349','979333','988020','911691','911692','911693','911697','911698','914877','914878','914875','914876','969345','965965','980611','911545','911573','911578','911583','911588','911574','911579','911584','911589','911599','911600','911595','911596','911570','911575','911580','911585','911571','911576','911581','911586','911572','911577','911582','911587','952850','990560','912518','902705','964004','917676','917677','917678','917682','917683','917684','917689','917690','917693','917694','917679','917680','917685','917686','916382','916383','916384','916385','908309','908311','912505','912506','964542','984112','909008','910168','956315','965826','980302','996798','996799','972981','972982','972979','972978','915452','915453','915454','915463','915449','915450','915458','902676','902677','902678','902679','900419','900418','905080','905081','900415','900416','900417','916049','916050','911890','911891','908580','964296','911830','994954','956994','956995','956997','956999','987867','996165','987869','996166','987881','996167','987883','996168','909143','909144','909145','909148','961493','998061','967543','911667','911668','911669','911670','911664','911665','911671','916276','908148','917051','968533','968534','654191','876615','900308','900310','900312','900314','900316','901982','913477','913478','913479','913480','913481','913482','957422','962159','993764','951429','958201','902757','902758','712442','712450','955570','981838','902156','971901','902158','976320','979067','979090','909560','903515','916354','908655','991638','991639','913504','913505','913499','913500','913501','984449','984451','908442','908443','905222','907013','911714','911716','911718','916606','911555','913291','908061','908062','908063','916601','916602','916603','902200','965179','902198','965096','965157','907526','965119','965116','965114','965153','965109','910039','909881','914602','910081','911982','911980','911981','917240','917241','911951','911952','915362','915363','915595','915598','915596','915597','915599','915600','913285','913286','913287','913288','915220','915213','915214','915215','915216','915217','915218','914597','915300','915301','991891','915170','915172','915173','915171','910751','909531','910750','991636','910752','909533','990677','911810','913446','971543','967387','916334','916335','916336','917100','985410','914465','915356','914361','917304','917303','916634','916635','916636','902772','902773','914100','956542','956545','956546','956105','956119','956132','956103','917321','917322','917323','915949','912813','915948','915946','916326','917488','917489','916600','917952','917953','917954','917950','917960','917961','917962','917963','917956','917957','917958','916361','917458','917101','911896','917251','917252','917250','917246','917247','917248','915751','915753','962440','917236','915142','915143','953587','953588','911261','911262','902034','901995','901997','901999','902036','902014','902015','902018','902021','902024','902025','902027','902030','905245','909567','905244','905241','905242','913695','915750','960341','960352','909202','909204','909200','909201','992048','962284','962283','917329','917330','905706','952004','909229','904711','902051','902048','902049','906990','917309','917310','902534','902521','902524','901411','901412','966897','968431','966851','966890','968091','900083','900084','916197','916191','916192','916193','916201','916198','916200','916196','917437','917433','917435','917431','917438','986889','960939','960944','960937','960938','960925','960927','960929','960924','907617','907618','907616','958480','971613','958469','904536','904537','904539','909542','909543','985051','985055','909539','909540','912637','912638','912635','912636','979541','917490','910015','910016','910017','989991','989990','976803','900413','900414','955976','955977','956698','956699','962758','962759','962773','962774','910365','910009','910010','910021','910011','910012','910022','974899','974917','993316','995901','993318','964835','964847','910961','910962','910963','910964','910965','911023','911017','911018','911019','911020','911021','911022','912811','954132','956961','951175','951176','951177','996081','996082','996083','951412','951180','996078','996079','985594','985595','985596','985597','996452','996453','996454','953667','953668','916337','913385','911730','901547','953653','953654','917456','917457','978699','917102','952946','952945','952929','981752','907586','962955','962956','917046','917047','900372','900375','900376','973964','973962','917749','917755','910970','910972','908657','908656','900559','900558','960856','967346','957264','967342','983730','800221','967447','973960','980880','981874','996684','976994','981872','982154','909885','909894','990529','990542','913273','913274','912850','917700','913075','913073','913071','907757','959715','917555','916210','916211','916207','916206','952427','952514','800044','800043','800250','800249','903168','903177','993792','913402','900565','952206','951987','917998','908933','956139','915794','916119','915793','913747','913748','913749','903085','903173','903079','917220','913063','913061','914111','914112','914110','914122','914123','914124','910732','910817','910816','914114','914115','914116') order by produit.division asc";
            sqlQuery = sqlQuery + "select distinct Produit.Sku, Produit.division from Produit, Prix where Produit.Sku = Prix.Sku and (produit.division like '5%') and Prix.Code_pays = 4 and produit.Sku in ('988337','916354','916353','916606','913291','911555','916328','917241','915363','915362','913286','913285','913287','915279','916326','916361','909567','905243','905242','913449','914271','801471','957123','917491','958469','971613','958480','904537','904539','904536','800256','800255','914604','917490','989991','901549','974918','974899','912811','802506','802509','954132','953668','953667','801272','901547','981752','952945','952929','952946','907586','962956','962955','917046','973962','973964','957264','911632','967346','712523','983730','800221','967447','968728','968742','996684','996633','913557','913549','960622','983558','908443','911719','915598','916333','916633','902771','915947','912813','917951','917959','917955','910987','917245','911263','915750','960340','900138','902554','989990','956697','962775','996451','911730','910977','978703','902756','902156','902158','979067','979090','971901','976320','911714','908061','908062','908063','910039','911952','911925','910752','909533','916322','990677','913446','912069','985410','915360','910830','917950','917458','914440','915751','971048','983800','913448','905294','917329','917330','905257','909229','902051','902048','902049','906990','905706','904711','952004','986889','900412','976323','908657','908656','981874','980880','913560','911626','911625','965109','965150','965179','965108','907526','965131','965175','965099','965153','965157','965096','907525','965116','965159','965115','965118','902199','902198','911982','911980','911981','915596','915597','915219','915212','915170','915171','911810','915359','915358','915357','915356','956069','956076','956065','956072','956077','908219','908204','907875','912918','912923','916600','909202','973176','909201','992048','991728','902536','902508','901410','968419','968098','968096','968090','917439','917426','974199','985059','910016','910015','900559','900558','960856','907011','907002','910081','917320','913450','900084','900083','977700','977708','11754','916190','916200','916194','912637','912636','912635','979541','910009','910010','910021','910012','917455','917102','983420','900362','913476','900293','955570','712442','712450','800608','800610','800609','902020','902034','902010','901993','953612','910796','910968','911023','911024','964814','955647','977160','951177','958180','982926','996077','960938','960939','960924','960927','960925','960937','907619','957422','951429','995901','993316','976994','981872') order by produit.division asc";
            
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(HttpContext.Current.Session.SessionID, out connection);
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<string> produits = new List<string>();
            
            while (reader.Read())
            {
                String format = (string)reader.GetValue(0);
                produits.Add(format);
            }

            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            
            return produits;
        }

        private static string getMadeInEffectif(string Made_In)
        {
            string madeInOut = "";
            if (
                Made_In == Const.ApplicationConsts.made_in_EU ||
                Made_In == Const.ApplicationConsts.made_in_FR ||
                Made_In == Const.ApplicationConsts.made_in_DE ||
                Made_In == Const.ApplicationConsts.made_in_ES ||
                Made_In == Const.ApplicationConsts.made_in_PT ||
                Made_In == Const.ApplicationConsts.made_in_BE ||
                Made_In == Const.ApplicationConsts.made_in_IT ||
                Made_In == Const.ApplicationConsts.made_in_UE || //ajouté par Cillia
                Made_In == Const.ApplicationConsts.made_in_IN ||
                Made_In == Const.ApplicationConsts.made_in_JP ||
                Made_In == Const.ApplicationConsts.made_in_TH ||
                Made_In == Const.ApplicationConsts.made_in_GB ||
                Made_In == Const.ApplicationConsts.made_in_VN ||
                Made_In == Const.ApplicationConsts.made_in_EG 

                )
                {
                    madeInOut = Made_In;
                }
            return madeInOut;
        }
    }
}