
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using TickitNewFace.Models;
using TickitNewFace.DAO;
using TickitNewFace.Const;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace TickitNewFace.Managers
{
    /// <summary>
    /// Alimentation de Tickit
    /// </summary>
    public static class UploadFichierManager
    {
        /// <summary>
        /// Permet de charger le contenue d'une feuille excel dans une liste d'objets à partir du chemin et le nom de la feuille désirée
        /// </summary>
        /// <param name="filePath">Chemin du fichier Excel</param>
        /// <returns>Liste des objets à </returns>
        /// <remarks>Aucune gestion d'exception n'est faite à l'intérieur de cette méthode</remarks>
        public static List<T_Presentation_Gamme_PLV_tmp> convertExcelPlVGammeToList(string filePath)
        {
            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 8.0;");
            //OleDbConnection conn = new OleDbConnection(string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";"));
            string req = "SELECT * FROM [PLVGamme$]";
            OleDbCommand command = new OleDbCommand(req, conn);

            //string connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";");
           
            command.Connection.Open();
            OleDbDataReader reader = command.ExecuteReader();

            List<T_Presentation_Gamme_PLV_tmp> objectsToUpdate = new List<T_Presentation_Gamme_PLV_tmp>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (reader.GetValue(0).ToString() != String.Empty)
                    {
                        T_Presentation_Gamme_PLV_tmp obj = new T_Presentation_Gamme_PLV_tmp();

                        // Extraction de la réference du produit
                        obj.nomGamme = reader.GetValue(0).ToString();
                        obj.sousTitreGamme = reader.GetValue(1).ToString();
                        obj.nomSGamme = reader.GetValue(2).ToString().Replace(";", " ").Replace(",", " ");
                        obj.descSGamme = reader.GetValue(3).ToString().Replace(";", " ").Replace(",", " ");
                        obj.skus = reader.GetValue(4).ToString().Replace(";", " ").Replace(",", " ");
                        obj.options = reader.GetValue(5).ToString().Replace(";", " ").Replace(",", " ");
                        obj.logoGamme = decimal.Parse(reader.GetValue(6).ToString());
                        obj.formatGamme = reader.GetValue(7).ToString();
                        obj.dgccrfGamme = reader.GetValue(8).ToString();

                        objectsToUpdate.Add(obj);
                    }
                }
            }

            conn.Close();
            reader.Close();
            command.Dispose();
            // Hsitorisation de l'évenement connexion.
            T_Evenement objEve = new T_Evenement();
            objEve.Dateve = DateTime.Now;
            objEve.Eve = "UPLOAD FICHIER - " + filePath;
            objEve.Login = (string)HttpContext.Current.Session["userName"];
            EvenementDao.insertEvenement(objEve);
            return objectsToUpdate;
        }






        /// <summary>
        /// Permet de charger le contenue d'une feuille excel dans une liste d'objets à partir du chemin et le nom de la feuille désirée
        /// </summary>
        /// <param name="filePath">Chemin du fichier Excel</param>
        /// <returns>Liste des objets à </returns>
        /// <remarks>Aucune gestion d'exception n'est faite à l'intérieur de cette méthode</remarks>
        public static List<TickitUpdateObject> convertExcelProduitsToList(string filePath)
        {
            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 8.0;");
            string req = "SELECT * FROM [Produits$]";
            OleDbCommand command = new OleDbCommand(req, conn);

            command.Connection.Open();
            OleDbDataReader reader = command.ExecuteReader();

            List<TickitUpdateObject> objectsToUpdate = new List<TickitUpdateObject>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (reader.GetValue(0).ToString() != String.Empty)
                    {
                        TickitUpdateObject obj = new TickitUpdateObject();

                        // Extraction de la réference du produit
                        obj.SKU = reader.GetValue(0).ToString();

                        obj.variation_FR = reader.GetValue(2).ToString();
                        obj.variation_DE = reader.GetValue(4).ToString();
                        obj.variation_ES = reader.GetValue(6).ToString();
                        obj.variation_ANG = reader.GetValue(8).ToString();

                        // Extraction des formats d'impression
                        obj.POS_A5_simple = reader.GetValue(11).ToString();
                        obj.POS_A5 = reader.GetValue(12).ToString();

                        obj.POS_A6_simple = reader.GetValue(13).ToString();
                        obj.POS_A6 = reader.GetValue(14).ToString();

                        obj.POS_A7_simple = reader.GetValue(15).ToString();
                        obj.POS_A7 = reader.GetValue(16).ToString();

                        // Extraction des dimensions
                        obj.Diametre = reader.GetValue(17).ToString();
                        obj.Profondeur = reader.GetValue(18).ToString();

                        obj.Largeur = reader.GetValue(19).ToString();
                        obj.Longueur = reader.GetValue(20).ToString();
                        obj.Hauteur = reader.GetValue(21).ToString();
                        obj.Volume = reader.GetValue(22).ToString();

                        // Extraction du type de montage
                        obj.AMonterSoiMeme = reader.GetValue(23).ToString();

                        // Extraction des descriptions reglette en différentes langues
                        obj.DescReglette_FR = reader.GetValue(24).ToString();
                        obj.DescReglette_DE = reader.GetValue(26).ToString();
                        obj.DescReglette_ES = reader.GetValue(28).ToString();
                        obj.DescReglette_ANG = reader.GetValue(30).ToString();

                        // Extraction des descriptions légales en différentes langues
                        obj.DGCCRF_FR = reader.GetValue(32).ToString();
                        obj.DGCCRF_DE = reader.GetValue(34).ToString();
                        obj.DGCCRF_ES = reader.GetValue(36).ToString();
                        obj.DGCCRF_ANG = reader.GetValue(38).ToString();

                        // Extraction des différentes taxes
                        obj.Eco_part = reader.GetValue(40).ToString();
                        obj.Eco_emballage = reader.GetValue(41).ToString();
                        obj.Eco_mobilier = reader.GetValue(42).ToString();

                        // Extraction du lieu de fabrication
                        obj.Made_in = reader.GetValue(43).ToString();

                        // Extraction des plus en différentes langues
                        obj.Plus1_FR = reader.GetValue(44).ToString();
                        obj.Plus2_FR = reader.GetValue(46).ToString();
                        obj.Plus3_FR = reader.GetValue(48).ToString();

                        obj.Plus1_DE = reader.GetValue(50).ToString();
                        obj.Plus2_DE = reader.GetValue(52).ToString();
                        obj.Plus3_DE = reader.GetValue(54).ToString();

                        obj.Plus1_ES = reader.GetValue(56).ToString();
                        obj.Plus2_ES = reader.GetValue(58).ToString();
                        obj.Plus3_ES = reader.GetValue(60).ToString();

                        obj.Plus1_ANG = reader.GetValue(62).ToString();
                        obj.Plus2_ANG = reader.GetValue(64).ToString();
                        obj.Plus3_ANG = reader.GetValue(66).ToString();

                        objectsToUpdate.Add(obj);
                    }
                }
            }

            conn.Close();
            reader.Close();
            command.Dispose();
            // Hsitorisation de l'évenement connexion.
            T_Evenement objEve = new T_Evenement();
            objEve.Dateve = DateTime.Now;
            objEve.Eve = "UPLOAD FICHIER - " + filePath;
            objEve.Login = (string)HttpContext.Current.Session["userName"];
            EvenementDao.insertEvenement(objEve);
            return objectsToUpdate;
        }



        /// <summary>
        /// Permet de mettre à jour des produits à partir du fichier Excel de MAJ PLV de Gamme.
        /// </summary>
        /// <param name="produitsToUpdate"></param>
        public static ResultatUploadFile updatePLVGamme(List<T_Presentation_Gamme_PLV_tmp> produitsToUpdate, int langageId)
        {
            int nombreProduitsMAJ = 0;
            string skuError = "";
            try
            {
                List<T_Presentation_Gamme_PLV_tmp> finalListGammeAUpdate = new List<T_Presentation_Gamme_PLV_tmp>();
                List<List<T_Presentation_Gamme_PLV_tmp>> listGammeAUpdate = new List<List<T_Presentation_Gamme_PLV_tmp>>();
                ResultatUploadFile resultatUpload = new ResultatUploadFile();

                //Liste des erreurs lors du téléchargement.
                List<string> uploadErrors = new List<string>();

                if (produitsToUpdate.Count != 0)
                {
                    // numéro à retourner à l'utilisateur en cas d'erreurs de chargement.
                    int numeroLigne = 2;

                    foreach (T_Presentation_Gamme_PLV_tmp obj in produitsToUpdate)
                    {
                        // Pouvoir determiner assez rapidement la ligne en erreur
                        skuError = obj.skus;

                        // élagage du Sku
                        if (obj.skus.Length == 4)
                        {
                            obj.skus = "00" + obj.skus;
                        }
                        if (obj.skus.Length == 5)
                        {
                            obj.skus = "0" + obj.skus;
                        }
                        if (obj.skus.Contains("\n"))
                        {
                            obj.skus = obj.skus.Replace("\n", "");
                        }

                        // MAJ si et seulement si le produit existe.
                        T_Produit produit = ProduitDao.getProduitBySku(obj.skus, langageId);
                        if (produit != null)
                        {
                            //new gamme?
                            int add = 0;
                            foreach(List<T_Presentation_Gamme_PLV_tmp> gamme in listGammeAUpdate){
                                if(gamme[0].nomGamme == obj.nomGamme ){
                                gamme.Add(obj);
                                add++;
                                }
                            }
                            if(add == 0){
                                List<T_Presentation_Gamme_PLV_tmp> gammenew = new List<T_Presentation_Gamme_PLV_tmp>();
                                gammenew.Add(obj);
                                listGammeAUpdate.Add(gammenew);//insert dans list gamme
                            }
                            
                            nombreProduitsMAJ++;
                        }
                        else
                        {
                            uploadErrors.Add(Resources.Langue.UploadFile_ErrorLigne + numeroLigne + "] : Sku " + obj.skus + Resources.Langue.UploadFile_SkuInexistant);
                            //supresion de la liste
                            produitsToUpdate.Remove(obj);
                        }
                        numeroLigne++;
                    }

                    //remplissage de gammeFinal pour envoyer ap au DAO
                    int cpt3 = 0;//cpt gamme
                    foreach (List<T_Presentation_Gamme_PLV_tmp> gamme in listGammeAUpdate) {
                        T_Presentation_Gamme_PLV_tmp gammeFinal = new T_Presentation_Gamme_PLV_tmp();
                        gammeFinal.formatGamme = gamme[0].formatGamme;
                        gammeFinal.dgccrfGamme = gamme[0].dgccrfGamme;
                        gammeFinal.logoGamme = gamme[0].logoGamme;
                        gammeFinal.sousTitreGamme = gamme[0].sousTitreGamme;
                        gammeFinal.nomGamme = gamme[0].nomGamme;
                        gammeFinal.options = "";
                        gammeFinal.nomSGamme = "";
                        gammeFinal.descSGamme = "";
                        gammeFinal.skus = "";
                        if (gamme[0].formatGamme == "A4")
                        {
                            //liste des ss gammes diff
                            List<T_Presentation_Sous_Gamme_PLV> LSgamme = new List<T_Presentation_Sous_Gamme_PLV>();
                            foreach (T_Presentation_Gamme_PLV_tmp produit in gamme)
                            {
                                int test = 0;
                                foreach (T_Presentation_Sous_Gamme_PLV SG in LSgamme)
                                {
                                    if (SG.nomSGamme == produit.nomSGamme)
                                    {
                                        SG.skus.Add(produit.skus + ";" + produit.options);
                                        test++;
                                    }
                                }
                                if (test == 0)
                                {
                                    T_Presentation_Sous_Gamme_PLV Sgamme = new T_Presentation_Sous_Gamme_PLV();
                                    Sgamme.nomSGamme = produit.nomSGamme;
                                    Sgamme.descSGamme = produit.descSGamme;
                                    List<string> skus = new List<string>();
                                    skus.Add(produit.skus + ";" + produit.options);
                                    Sgamme.skus = skus;
                                    LSgamme.Add(Sgamme);
                                }
                            }
                            int cpt = 0;//cpt opt produit
                            int cpt2 = 0;//cpt ss gamme
                            foreach (T_Presentation_Sous_Gamme_PLV sg in LSgamme)
                            {
                                cpt2 = 0;
                                if (cpt != 0)//si pas premier produit de gamme
                                {
                                    gammeFinal.options += ";";
                                    gammeFinal.skus += ";";
                                    gammeFinal.nomSGamme += ";";
                                    gammeFinal.descSGamme += ";";
                                }
                                gammeFinal.nomSGamme += sg.nomSGamme;
                                gammeFinal.descSGamme += sg.descSGamme;
                                foreach (string item in sg.skus)
                                {
                                    if (cpt2 != 0)//si pas premier produit de sg gamme
                                    {
                                        gammeFinal.options += ",";
                                        gammeFinal.skus += ",";
                                    }
                                    string[] tmp = item.Split(';');
                                    gammeFinal.options += tmp[1];
                                    gammeFinal.skus += tmp[0];
                                    cpt2++;
                                }
                                cpt++;


                                /*
                                gammeFinal.options = ",,;,,;,,";
                                gammeFinal.nomSGamme = ";;";
                                gammeFinal.descSGamme = ";;";
                                gammeFinal.skus = ",,;,,;,,";*/
                            }
                            finalListGammeAUpdate.Add(gammeFinal);
                            cpt3++;
                        }
                        else { //format A5
                            int cpt2 = 0;
                            foreach (T_Presentation_Gamme_PLV_tmp produit in gamme)
                            {
                                if (cpt2 != 0)//si pas premier produit de sg gamme
                                {
                                    gammeFinal.options += ";";
                                    gammeFinal.skus += ";";
                                    gammeFinal.nomSGamme += ";";
                                    gammeFinal.descSGamme += ";";
                                }                              
                                gammeFinal.options += produit.options;
                                gammeFinal.skus += produit.skus;
                                cpt2++;
                            }
                            finalListGammeAUpdate.Add(gammeFinal);
                            /*
                             gammeFinal.options = ";;";
                             gammeFinal.nomSGamme = ";;";
                             gammeFinal.descSGamme = ";;";
                             gammeFinal.skus = ",,;,,;,,";*/
                        }
                    }
                    //DAO insert finalListGammeAUpdate
                    foreach (var flgau in finalListGammeAUpdate)
                    {
                        DAO.Resultats_RecherchePLVDao.insertPLVGamme(flgau, langageId);
                    }
                }
                else
                {
                    uploadErrors.Add(Resources.Langue.UploadFile_FichierVide);
                }

                resultatUpload.uploadErrors = uploadErrors;
                resultatUpload.nombreLignesSuccess = finalListGammeAUpdate.Count;

                // Hsitorisation de l'évenement connexion.
                T_Evenement objEve = new T_Evenement();
                objEve.Dateve = DateTime.Now;
                objEve.Eve = "INSERT - MAJ des PLV de Gamme à partir du fichier Excel de MAJ (" + finalListGammeAUpdate.Count.ToString() + " update)";
                objEve.Login = (string)HttpContext.Current.Session["userName"];
                EvenementDao.insertEvenement(objEve);

                return resultatUpload;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - N° line : " + nombreProduitsMAJ + " - Sku : " + skuError);
            }
        }



        /// <summary>
        /// Permet de mettre à jour des produits à partir du fichier Excel de MAJ.
        /// </summary>
        /// <param name="produitsToUpdate"></param>
        public static ResultatUploadFile updateProduits(List<TickitUpdateObject> produitsToUpdate, int langageId)
        {
            int nombreProduitsMAJ = 0;

            string skuError = "";
            try
            {
                ResultatUploadFile resultatUpload = new ResultatUploadFile();
                resultatUpload.MAJAllLangue = false;

                //Liste des erreurs lors du téléchargement.
                List<string> uploadErrors = new List<string>();

                if (produitsToUpdate.Count != 0)
                {
                    // numéro à retourner à l'utilisateur en cas d'erreurs de chargement.
                    int numeroLigne = 2;

                    foreach (TickitUpdateObject obj in produitsToUpdate)
                    {
                        // Pouvoir determiner assez rapidement la ligne en erreur
                        skuError = obj.SKU;

                        // élagage du Sku
                        if (obj.SKU.Length == 4)
                        {
                            obj.SKU = "00" + obj.SKU;
                        }
                        if (obj.SKU.Length == 5)
                        {
                            obj.SKU = "0" + obj.SKU;
                        }
                        if (obj.SKU.Contains("\n"))
                        {
                            obj.SKU = obj.SKU.Replace("\n", "");
                        }

                        // MAJ si et seulement si le produit existe.
                        T_Produit produit = ProduitDao.getProduitBySku(obj.SKU, langageId);
                        if (produit != null)
                        {
                            if (langageId == ApplicationConsts.codePays_DE || langageId == ApplicationConsts.codePays_ES)
                            {
                                obj.Eco_mobilier = obj.Eco_mobilier.Replace('.', ',');
                                obj.Eco_emballage = obj.Eco_emballage.Replace('.', ',');
                                obj.Eco_part = obj.Eco_part.Replace('.', ',');
                                obj.Diametre = obj.Diametre.Replace('.', ',');
                                obj.Profondeur = obj.Profondeur.Replace('.', ',');
                                obj.Largeur = obj.Largeur.Replace('.', ',');
                                obj.Hauteur = obj.Hauteur.Replace('.', ',');
                                obj.Longueur = obj.Longueur.Replace('.', ',');
                                obj.Volume = obj.Volume.Replace('.', ',');
                            }
                            if (langageId == ApplicationConsts.codePays_FR)
                            {
                                obj.Eco_mobilier = obj.Eco_mobilier.Replace(',', '.');
                                obj.Eco_emballage = obj.Eco_emballage.Replace(',', '.');
                                obj.Eco_part = obj.Eco_part.Replace(',', '.');
                                obj.Diametre = obj.Diametre.Replace(',', '.');
                                obj.Profondeur = obj.Profondeur.Replace(',', '.');
                                obj.Largeur = obj.Largeur.Replace(',', '.');
                                obj.Hauteur = obj.Hauteur.Replace(',', '.');
                                obj.Longueur = obj.Longueur.Replace(',', '.');
                                obj.Volume = obj.Volume.Replace(',', '.');
                            }

                            if (obj.Diametre.Trim() != "") { produit.Diametre = decimal.Parse(obj.Diametre.Trim()); } else { produit.Diametre = null; }
                            if (obj.Profondeur.Trim() != "") { produit.Profondeur = decimal.Parse(obj.Profondeur.Trim()); } else { produit.Profondeur = null; }
                            if (obj.Largeur.Trim() != "") { produit.Largeur = decimal.Parse(obj.Largeur.Trim()); } else { produit.Largeur = null; }
                            if (obj.Hauteur.Trim() != "") { produit.Hauteur = decimal.Parse(obj.Hauteur.Trim()); } else { produit.Hauteur = null; }
                            if (obj.Longueur.Trim() != "") { produit.Longueur = decimal.Parse(obj.Longueur.Trim()); } else { produit.Longueur = null; }
                            if (obj.Volume.Trim() != "") { produit.Volume = decimal.Parse(obj.Volume.Trim()); } else { produit.Volume = null; }

                            // Force l'Eco-mobilier à "" puisqu'elle déscend de l'ERP.
                            obj.Eco_mobilier = "";
                            if (obj.Eco_mobilier.Trim() != "") { produit.Eco_mobilier = decimal.Parse(obj.Eco_mobilier.Trim()); }
                            if (obj.Eco_emballage.Trim() != "") { produit.Eco_emballage = decimal.Parse(obj.Eco_emballage.Trim()); } 
                            if (obj.Eco_part.Trim() != "") { produit.Eco_part = decimal.Parse(obj.Eco_part.Trim()); } 
                            if (obj.Made_in.Trim() != "") { produit.Made_In = obj.Made_in.Trim(); } 

                            produit.Self_Assembly = obj.AMonterSoiMeme.ToUpper().Trim();
                            ProduitDao.updatePorduit(produit);

                            // MAJ formats d'impressions (A5 / A6 / A7 : RV ou simples).
                            ajoutOuSupprimerFormatImpression(obj.POS_A5_simple, obj.SKU, ApplicationConsts.format_A5_simple);
                            ajoutOuSupprimerFormatImpression(obj.POS_A5, obj.SKU, ApplicationConsts.format_A5_recto_verso);

                            ajoutOuSupprimerFormatImpression(obj.POS_A6_simple, obj.SKU, ApplicationConsts.format_A6_simple);
                            ajoutOuSupprimerFormatImpression(obj.POS_A6, obj.SKU, ApplicationConsts.format_A6_recto_verso);

                            ajoutOuSupprimerFormatImpression(obj.POS_A7_simple, obj.SKU, ApplicationConsts.format_A7_simple);
                            ajoutOuSupprimerFormatImpression(obj.POS_A7, obj.SKU, ApplicationConsts.format_A7_recto_verso);

                            // MAJ DGCCRF
                            tryInsertOrUpdateDgccrf(obj.SKU, ApplicationConsts.codePays_GB, obj.DGCCRF_ANG);
                            tryInsertOrUpdateDgccrf(obj.SKU, ApplicationConsts.codePays_FR, obj.DGCCRF_FR);
                            tryInsertOrUpdateDgccrf(obj.SKU, ApplicationConsts.codePays_DE, obj.DGCCRF_DE);
                            tryInsertOrUpdateDgccrf(obj.SKU, ApplicationConsts.codePays_ES, obj.DGCCRF_ES);

                            // MAJ Variation
                            tryInsertOrUpdateVariation(obj.SKU, ApplicationConsts.codePays_GB, obj.variation_ANG);
                            tryInsertOrUpdateVariation(obj.SKU, ApplicationConsts.codePays_FR, obj.variation_FR);
                            tryInsertOrUpdateVariation(obj.SKU, ApplicationConsts.codePays_DE, obj.variation_DE);
                            tryInsertOrUpdateVariation(obj.SKU, ApplicationConsts.codePays_ES, obj.variation_ES);

                            // MAJ Description Reglette
                            tryInsertOrUpdateDescReglette(obj.SKU, ApplicationConsts.codePays_GB, obj.DescReglette_ANG);
                            tryInsertOrUpdateDescReglette(obj.SKU, ApplicationConsts.codePays_FR, obj.DescReglette_FR);
                            tryInsertOrUpdateDescReglette(obj.SKU, ApplicationConsts.codePays_DE, obj.DescReglette_DE);
                            tryInsertOrUpdateDescReglette(obj.SKU, ApplicationConsts.codePays_ES, obj.DescReglette_ES);

                            // MAJ Plus
                            //Description_PlusDao.supprimerPlusByLang(obj.SKU, ApplicationConsts.codePays_FR);
                            tryInsertPlus(obj.SKU, obj.Plus1_FR, ApplicationConsts.codePays_FR, 1);
                            tryInsertPlus(obj.SKU, obj.Plus2_FR, ApplicationConsts.codePays_FR, 2);
                            tryInsertPlus(obj.SKU, obj.Plus3_FR, ApplicationConsts.codePays_FR, 3);

                            //Description_PlusDao.supprimerPlusByLang(obj.SKU, ApplicationConsts.codePays_DE);
                            tryInsertPlus(obj.SKU, obj.Plus1_DE, ApplicationConsts.codePays_DE, 1);
                            tryInsertPlus(obj.SKU, obj.Plus2_DE, ApplicationConsts.codePays_DE, 2);
                            tryInsertPlus(obj.SKU, obj.Plus3_DE, ApplicationConsts.codePays_DE, 3);

                            //Description_PlusDao.supprimerPlusByLang(obj.SKU, ApplicationConsts.codePays_ES);
                            tryInsertPlus(obj.SKU, obj.Plus1_ES, ApplicationConsts.codePays_ES, 1);
                            tryInsertPlus(obj.SKU, obj.Plus2_ES, ApplicationConsts.codePays_ES, 2);
                            tryInsertPlus(obj.SKU, obj.Plus3_ES, ApplicationConsts.codePays_ES, 3);

                            tryInsertPlus(obj.SKU, obj.Plus1_ANG, ApplicationConsts.codePays_GB, 1);
                            tryInsertPlus(obj.SKU, obj.Plus2_ANG, ApplicationConsts.codePays_GB, 2);
                            tryInsertPlus(obj.SKU, obj.Plus3_ANG, ApplicationConsts.codePays_GB, 3);

                            nombreProduitsMAJ++;
                        }
                        else
                        {
                            uploadErrors.Add(Resources.Langue.UploadFile_ErrorLigne + numeroLigne + "] : Sku " + obj.SKU + Resources.Langue.UploadFile_SkuInexistant);
                        }
                        numeroLigne++;
                    }
                }
                else
                {
                    uploadErrors.Add(Resources.Langue.UploadFile_FichierVide);
                }

                resultatUpload.uploadErrors = uploadErrors;
                resultatUpload.nombreLignesSuccess = nombreProduitsMAJ;

                if (nombreProduitsMAJ > 0)
                {
                    resultatUpload.MAJAllLangue = true;
                    

                }

                // Hsitorisation de l'évenement connexion.
                T_Evenement objEve = new T_Evenement();
                objEve.Dateve = DateTime.Now;
                objEve.Eve = "INSERT - MAJ des produits à partir du fichier Excel de MAJ (" + nombreProduitsMAJ.ToString() + " update)";
                objEve.Login = (string)HttpContext.Current.Session["userName"];
                EvenementDao.insertEvenement(objEve);

                return resultatUpload;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - N° line : " + nombreProduitsMAJ + " - Sku : " + skuError);
            }
        }

        /// <summary>
        /// Insère un listePlus
        /// </summary>
        /// <param name="Sku"></param>
        /// <param name="listePlus"></param>
        /// <param name="langageId"></param>
        /// <param name="position"></param>
        private static void tryInsertPlus(string Sku, string plus, int langageId, int position)
        {
            if (plus != null && plus != "")
            {
                Description_PlusDao.supprimerPlusByLang(Sku, langageId, position);

                T_Description_Plus nouvPlus = new T_Description_Plus();
                nouvPlus.Sku = Sku;
                nouvPlus.Plus = plus;
                nouvPlus.LangageId = langageId;
                nouvPlus.Position = position;

                Description_PlusDao.insertPlus(nouvPlus);
            }
        }

        /// <summary>
        /// Insertion ou mise à jour de la description légale d'un produit.
        /// </summary>
        /// <param name="Sku"></param>
        /// <param name="langageId"></param>
        /// <param name="legalDescription"></param>
        private static void tryInsertOrUpdateLibelleProduit(string Sku, int langageId, string libelle)
        {
            if (libelle != null && libelle != "")
            {
                T_Libelle_Produit var = LibelleProduitDao.getLibelleProduitBySku(Sku, langageId);
                if (var == null)
                {
                    T_Libelle_Produit newLibelle = new T_Libelle_Produit();
                    newLibelle.Sku = Sku;
                    newLibelle.LangageId = langageId;
                    newLibelle.Libelle = libelle;

                    LibelleProduitDao.insertLibelleProduit(newLibelle);
                }
                /*else
                {
                    var.Libelle = libelle;
                    LibelleProduitDao.updatelibelleproduit(var);
                }*/
            }
            else
            {
                LibelleProduitDao.deleteLibelleProduit(Sku, langageId, libelle);
            }
        }

        /// <summary>
        /// Insertion ou mise à jour de la description légale d'un produit.
        /// </summary>
        /// <param name="Sku"></param>
        /// <param name="langageId"></param>
        /// <param name="legalDescription"></param>
        private static void tryInsertOrUpdateVariation(string Sku, int langageId, string variation)
        {
            if (variation != null && variation != "")
            {
                T_Variation var = VariationDao.getVariationBySku(Sku, langageId);
                if (var == null)
                {
                    T_Variation newVar = new T_Variation();
                    newVar.Sku = Sku;
                    newVar.LangageId = langageId;
                    newVar.VariationName = variation;

                    VariationDao.insertVariation(newVar);
                }
                else
                {
                    var.VariationName = variation;
                    VariationDao.updateVariation(var);
                }
            }
            /*else
            {
                VariationDao.deleteVariation(Sku, langageId, variation);
            }*/
        }

        /// <summary>
        /// Insertion ou mise à jour de la description légale d'un produit.
        /// </summary>
        /// <param name="Sku"></param>
        /// <param name="langageId"></param>
        /// <param name="legalDescription"></param>
        private static void tryInsertOrUpdateDgccrf(string Sku, int langageId, string legalDescription)
        {
            if (legalDescription != null && legalDescription != "")
            {
                T_Description_Dgccrf dgccrf = Description_DgccrfDao.getDgccrfBySku(Sku, langageId);
                if (dgccrf == null)
                {
                    T_Description_Dgccrf newDgccrf = new T_Description_Dgccrf();
                    newDgccrf.Sku = Sku;
                    newDgccrf.LangageId = langageId;
                    newDgccrf.LegalDescription = legalDescription;

                    Description_DgccrfDao.insertDgccrf(newDgccrf);
                }
                else
                {
                    dgccrf.LegalDescription = legalDescription;
                    Description_DgccrfDao.updateDgccrf(dgccrf);
                }
            }
            
            /*else
            {
                Description_DgccrfDao.deleteDgccrf(Sku, langageId, legalDescription);
            }*/
        }

        /// <summary>
        /// Insertion ou mise à jour de la description Reglette d'un produit.
        /// </summary>
        /// <param name="Sku"></param>
        /// <param name="langageId"></param>
        /// <param name="legalDescription"></param>
        private static void tryInsertOrUpdateDescReglette(string Sku, int langageId, string Description)
        {
            if (Description != null && Description != "")
            {
                T_Description_Reglette descReglette = Description_RegletteDao.getDescRegletteBySku(Sku, langageId);
                if (descReglette == null)
                {
                    T_Description_Reglette newDescReglette = new T_Description_Reglette();
                    newDescReglette.Sku = Sku;
                    newDescReglette.LangageId = langageId;
                    newDescReglette.Description = Description;

                    Description_RegletteDao.insertDescReglette(newDescReglette);
                }
                else
                {
                    descReglette.Description = Description;
                    Description_RegletteDao.updateDescReglette(descReglette);
                }
            }

            /*else
            {
                Description_RegletteDao.deleteDescReglette(Sku, langageId, Description);
            }*/
        }

        /// <summary>
        /// Ajout ou suppression d'un format d'impression à un produit
        /// </summary>
        /// <param name="objFormat"></param>
        /// <param name="Sku"></param>
        /// <param name="formatToInsert"></param>
        private static void ajoutOuSupprimerFormatImpression(string objFormat, string Sku, string formatToInsert)
        {
            if (objFormat.ToUpper().Trim() == ApplicationConsts.ajoutFormatImpression)
            {
                ProduitDao.ajoutFormatImpressionBySku(Sku, formatToInsert);
            }
            if (objFormat.ToUpper().Trim() == ApplicationConsts.suppressionFormatImpression)
            {
                ProduitDao.supprimerFormatImpressionBySku(Sku, formatToInsert);
            }
        }

        /// <summary>
        /// Converti un fichier Excel en liste de données.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<RangeUpdateObject> convertExcelRangesToList(string filePath)
        {
            try
            {
                OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 8.0;");
                string req = "SELECT * FROM [Ranges$]";
                OleDbCommand command = new OleDbCommand(req, conn);

                command.Connection.Open();
                OleDbDataReader reader = command.ExecuteReader();

                List<RangeUpdateObject> objectsToUpdate = new List<RangeUpdateObject>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader.GetValue(0).ToString() != String.Empty)
                        {
                            RangeUpdateObject obj = new RangeUpdateObject();

                            obj.rangeName = reader.GetValue(0).ToString();

                            obj.F_SF_SSF = reader.GetValue(1).ToString();

                            obj.plus_FR = reader.GetValue(2).ToString();
                            obj.plus_DE = reader.GetValue(4).ToString();
                            obj.plus_ES = reader.GetValue(6).ToString();
                            obj.plus_GB = reader.GetValue(8).ToString();

                            obj.libelle_FR = reader.GetValue(10).ToString();
                            obj.libelle_DE = reader.GetValue(12).ToString();
                            obj.libelle_ES = reader.GetValue(14).ToString();
                            obj.libelle_GB = reader.GetValue(16).ToString();

                            objectsToUpdate.Add(obj);
                        }
                    }
                }

                conn.Close();
                reader.Close();
                command.Dispose();

                return objectsToUpdate;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Converti un fichier Excel en liste de données.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<RangeSimpleUpdateObject> convertExcelRangesFranchiseToList(string filePath)
        {
            try
            {
                OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 8.0;");
                string req = "SELECT * FROM [Ranges$]";
                OleDbCommand command = new OleDbCommand(req, conn);

                command.Connection.Open();
                OleDbDataReader reader = command.ExecuteReader();

                List<RangeSimpleUpdateObject> objectsToUpdate = new List<RangeSimpleUpdateObject>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader.GetValue(0).ToString() != String.Empty)
                        {
                            RangeSimpleUpdateObject obj = new RangeSimpleUpdateObject();

                            obj.rangeName = reader.GetValue(0).ToString();
                            obj.plus = reader.GetValue(1).ToString();
                            obj.libelle = reader.GetValue(2).ToString();
                            objectsToUpdate.Add(obj);
                        }
                    }
                }

                conn.Close();
                reader.Close();
                command.Dispose();

                return objectsToUpdate;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// MAJ des description (Plus) d'une gamme.
        /// </summary>
        /// <param name="rangesPlus"></param>
        public static ResultatUploadFile insertOrUpdatePlusRange(List<RangeUpdateObject> rangesPlus)
        {
            ResultatUploadFile resutatUpload = new ResultatUploadFile();
            int nombreProduitsMAJ = 0;

            //Liste des erreurs lors du téléchargement.
            List<string> uploadErrors = new List<string>();

            int numeroLigne = 2;
            foreach (RangeUpdateObject obj in rangesPlus)
            {

                List<T_Range> ranges = DAO.RangeDao.getRangeByName(obj.rangeName);

                if (ranges.Count != 0)
                {
                    // MAJ des description du ranges en divers langues.
                    DAO.RangeDao.insertOrUpdatePlusRange(obj.rangeName, obj.plus_FR, ApplicationConsts.codePays_FR, obj.F_SF_SSF);
                    DAO.RangeDao.insertOrUpdatePlusRange(obj.rangeName, obj.plus_GB, ApplicationConsts.codePays_GB, obj.F_SF_SSF);
                    DAO.RangeDao.insertOrUpdatePlusRange(obj.rangeName, obj.plus_DE, ApplicationConsts.codePays_DE, obj.F_SF_SSF);
                    DAO.RangeDao.insertOrUpdatePlusRange(obj.rangeName, obj.plus_ES, ApplicationConsts.codePays_ES, obj.F_SF_SSF);

                    // MAJ des libellés du ranges en divers langues.
                    DAO.RangeDao.insertOrUpdateLibelleRange(obj.rangeName, obj.libelle_FR, ApplicationConsts.codePays_FR, obj.F_SF_SSF);
                    DAO.RangeDao.insertOrUpdateLibelleRange(obj.rangeName, obj.libelle_GB, ApplicationConsts.codePays_GB, obj.F_SF_SSF);
                    DAO.RangeDao.insertOrUpdateLibelleRange(obj.rangeName, obj.libelle_DE, ApplicationConsts.codePays_DE, obj.F_SF_SSF);
                    DAO.RangeDao.insertOrUpdateLibelleRange(obj.rangeName, obj.libelle_ES, ApplicationConsts.codePays_ES, obj.F_SF_SSF);

                    nombreProduitsMAJ++;
                }
                else
                {
                    uploadErrors.Add("[Error ligne " + numeroLigne + "] - le range " + obj.rangeName + " n'existe pas");
                }
                numeroLigne++;
            }

            resutatUpload.uploadErrors = uploadErrors;
            resutatUpload.nombreLignesSuccess = nombreProduitsMAJ;

            return resutatUpload;

        }

        /// <summary>
        /// MAJ des description (Plus) d'une gamme.
        /// </summary>
        /// <param name="rangesPlus"></param>
        public static ResultatUploadFile insertOrUpdatePlusRange(List<RangeSimpleUpdateObject> rangesPlus, int langueId)
        {
            ResultatUploadFile resutatUpload = new ResultatUploadFile();
            int nombreProduitsMAJ = 0;

            //Liste des erreurs lors du téléchargement.
            List<string> uploadErrors = new List<string>();

            int numeroLigne = 2;
            foreach (RangeSimpleUpdateObject obj in rangesPlus)
            {

                List<T_Range> ranges = DAO.RangeDao.getRangeByName(obj.rangeName);

                if (ranges.Count != 0)
                {
                    // MAJ des description du ranges en divers langues.
                    DAO.RangeDao.insertOrUpdatePlusRange(obj.rangeName, obj.plus, langueId, "");

                    // MAJ des libellés du ranges en divers langues.
                    DAO.RangeDao.insertOrUpdateLibelleRange(obj.rangeName, obj.libelle, langueId, "");

                    nombreProduitsMAJ++;
                }
                else
                {
                    uploadErrors.Add("[Error ligne " + numeroLigne + "] - le range " + obj.rangeName + " n'existe pas");
                }
                numeroLigne++;
            }

            resutatUpload.uploadErrors = uploadErrors;
            resutatUpload.nombreLignesSuccess = nombreProduitsMAJ;

            return resutatUpload;

        }

        public static List<T_Prix> convertExcelPriceToList(string filePath, int countryId)
        {
            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 8.0;");
            string req = "SELECT * FROM [Price$]";
            OleDbCommand command = new OleDbCommand(req, conn);

            command.Connection.Open();
            OleDbDataReader reader = command.ExecuteReader();

            List<T_Prix> objectsToUpdate = new List<T_Prix>();
            
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (reader.GetValue(0).ToString() != String.Empty)
                    {
                        T_Prix obj = new T_Prix();
                        
                        // Extraction de la réference du produit
                        obj.Sku = reader.GetValue(0).ToString();
                        obj.Prix_produit = decimal.Parse(reader.GetValue(1).ToString());
                        obj.Type_promo = reader.GetValue(2).ToString();
                        obj.Date_debut = System.DateTime.Parse(reader.GetValue(3).ToString());
                        obj.Date_fin = System.DateTime.Parse(reader.GetValue(4).ToString());
                        obj.Code_Pays = countryId;
                        obj.Date_maj = System.DateTime.Today;
                        
                        objectsToUpdate.Add(obj);
                    }
                }
            }

            conn.Close();
            reader.Close();
            command.Dispose();
            // Hsitorisation de l'évenement connexion.
            T_Evenement objEve = new T_Evenement();
            objEve.Dateve = DateTime.Now;
            objEve.Eve = "UPLOAD FICHIER - " + filePath;
            objEve.Login = (string)HttpContext.Current.Session["userName"];
            EvenementDao.insertEvenement(objEve);
            return objectsToUpdate;
        }

        public static List<T_Produit_Magasin> convertExcelSkuMagasinsToList(string filePath, int MagasinId, string magId)
        {
            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 8.0;");
            string req = "SELECT * FROM [impression_en_masse$]";
            OleDbCommand command = new OleDbCommand(req, conn);

            command.Connection.Open(); 
            OleDbDataReader reader = command.ExecuteReader();

            List<T_Produit_Magasin> objectsToUpdate = new List<T_Produit_Magasin>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (reader.GetValue(0).ToString() != String.Empty)
                    {
                        T_Produit_Magasin obj = new T_Produit_Magasin();

                        // Extraction de la réference du produit
                        obj.Sku = reader.GetValue(0).ToString();
                        obj.MagasinId = MagasinId;
                        obj.magId = magId;

                        objectsToUpdate.Add(obj);
                    }
                }
            }

            conn.Close();
            reader.Close();
            command.Dispose();

            return objectsToUpdate;
        }
        public static List<string> convertExcelSkuMagasinsToListUnique(string filePath)
        {
            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 8.0;");
            string req = "SELECT * FROM [impression_en_masse$]";
            OleDbCommand command = new OleDbCommand(req, conn);
                
            command.Connection.Open();
            OleDbDataReader reader = command.ExecuteReader();

            List<string> objectsToUpdate = new List<string>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (reader.GetValue(0).ToString() != String.Empty)
                    {
                        string Sku = reader.GetValue(0).ToString();

                        objectsToUpdate.Add(Sku);
                    }
                }
            }

            conn.Close();
            reader.Close();
            command.Dispose();

            // Hsitorisation de l'évenement connexion.
            T_Evenement objEve = new T_Evenement();
            objEve.Dateve = DateTime.Now;
            objEve.Eve = "UPLOAD FICHIER - " + filePath;
            objEve.Login = (string)HttpContext.Current.Session["userName"];
            EvenementDao.insertEvenement(objEve);

            return objectsToUpdate;
        }


        /// <summary>
        /// Permet de mettre à jour des produits à partir du fichier Excel de MAJ. (fichier produit magasin)
        /// </summary>
        /// <param name="produitsToUpdate"></param>
        public static ResultatUploadFile insertProduitMagasin(List<T_produit_magasin_update> produitMagasins)
        {
            ResultatUploadFile resultatUpload = new ResultatUploadFile();

            //Liste des erreurs lors du téléchargement.
            List<string> uploadErrors = new List<string>();
            int nombreProduitsMAJ = 0;
            int nombremagsMAJ = 0;
            int numeroLigne = 0;
                
            if (produitMagasins != null)//si produit mag existe
                {
                    if (produitMagasins.Count != 0)// si produit a update
                    {
                        if (produitMagasins[0].MagasinId == 4) //si francais (pour gestion insert multi magasin)
                        {
                            if (produitMagasins.Count > 1) //si selection de magasin
                            {
                                //init variable local
                                string magIds = "";
                                int MagasinId;
                                MagasinId = produitMagasins[0].MagasinId;
                                int cpt = 0;
                                // on remplis le champ tmp id : 4  code : 4
                                foreach (string produitselect in produitMagasins[0].Skus)
                                {
                                    try
                                    {
                                        DAO.Produit_MagasinDao.insertSkuMagasin(produitselect, MagasinId, MagasinId.ToString());
                                        nombreProduitsMAJ++;
                                        numeroLigne++;
                                    }
                                    catch (Exception ex)
                                    {
                                        uploadErrors.Add("ligne " + numeroLigne + " : " + ex.Message);
                                    }
                                }
                                //les magasins a update
                                foreach (T_produit_magasin_update magasinselect in produitMagasins)
                                {
                                    nombremagsMAJ++;
                                    if (cpt == 0)// si premier magasin pas de separation
                                    {
                                        magIds = magasinselect.magId;
                                    }
                                    else // magasin courant --> separation , pour appel fonction microsoft sql
                                    {
                                        magIds = magIds + "," + magasinselect.magId;
                                    }
                                    cpt++;
                                }
                                //update des magasins
                                Produit_MagasinDao.editSkusProduitMagasins(MagasinId, magIds);
                            }
                            else {//selection d un seul magasin francais
                                foreach (T_produit_magasin_update magasinselect in produitMagasins)
                                {
                                    nombremagsMAJ++;
                                    DAO.Produit_MagasinDao.supprimerSkusByMagasinId(magasinselect.MagasinId, magasinselect.magId);//dell old 
                                    foreach (string Sku in magasinselect.Skus)
                                    {
                                        if (magasinselect != null)
                                        {
                                            try
                                            {
                                                DAO.Produit_MagasinDao.insertSkuMagasin(Sku, magasinselect.MagasinId, magasinselect.magId);//ajout new product
                                                nombreProduitsMAJ++;
                                                numeroLigne++;
                                            }
                                            catch (Exception ex)
                                            {
                                                uploadErrors.Add("ligne " + numeroLigne + " : " + ex.Message);
                                            }
                                        }
                                    }
                                }
                            
                            }
                        }
                        else { 
                        // si pas francais ===> un seul magasin a update
                            foreach (T_produit_magasin_update magasinselect in produitMagasins) {
                                nombremagsMAJ++;
                                DAO.Produit_MagasinDao.supprimerSkusByMagasinId(magasinselect.MagasinId, magasinselect.MagasinId.ToString());//dell old 
                                foreach (string Sku in magasinselect.Skus)
                                {
                                    if (magasinselect != null)
                                    {
                                        try
                                        {
                                            DAO.Produit_MagasinDao.insertSkuMagasin(Sku, magasinselect.MagasinId, magasinselect.MagasinId.ToString());//ajout new product
                                            nombreProduitsMAJ++;
                                            numeroLigne++;
                                        }
                                        catch (Exception ex)
                                        {
                                           uploadErrors.Add("ligne " + numeroLigne + " : " + ex.Message);
                                        }
                                    } 
                                }
                            }
            
                        }   
                    }
                }
                //pas de produit a update
                else
                {
                    uploadErrors.Add(Resources.Langue.UploadFile_FichierVide);
                }

            // Hsitorisation de l'évenement connexion.
            T_Evenement objEve = new T_Evenement();
            objEve.Dateve = DateTime.Now;
            objEve.Eve = "INSERT - MAJ du fichier produit magasin à partir du fichier Excel de MAJ (" + nombreProduitsMAJ.ToString() + " update dans " + nombremagsMAJ + " magasin)";
            objEve.Login = (string)HttpContext.Current.Session["userName"];
            EvenementDao.insertEvenement(objEve);

            resultatUpload.uploadErrors = uploadErrors;
            resultatUpload.nombreLignesSuccess = nombreProduitsMAJ;
            resultatUpload.nombreMagasinsSuccess = nombremagsMAJ;

            return resultatUpload;
        }
        
        
        /// <summary>
        /// Permet de mettre à jour des produits à partir du fichier Excel de MAJ.
        /// </summary>
        /// <param name="produitsToUpdate"></param>
        public static ResultatUploadFile updatePrix(List<T_Prix> objectToUpdate)
        {
            ResultatUploadFile resultatUpload = new ResultatUploadFile();

            //Liste des erreurs lors du téléchargement.
            List<string> uploadErrors = new List<string>();
            int nombreProduitsMAJ = 0;

            if (objectToUpdate.Count != 0)
            {
                // numéro à retourner à l'utilisateur en cas d'erreurs de chargement.
                int numeroLigne = 2;

                foreach (T_Prix obj in objectToUpdate)
                {
                    // MAJ si et seulement si le produit existe.
                    T_Produit produit = ProduitDao.getProduitBySku(obj.Sku, obj.Code_Pays);
                    if (produit != null)
                    {
                        try
                        {
                            DAO.PrixDao.insertPrix(obj);
                            nombreProduitsMAJ++;
                        }
                        catch (Exception ex)
                        {
                            uploadErrors.Add("ligne " + numeroLigne + " : " + ex.Message);
                        }
                    }
                    else
                    {
                        uploadErrors.Add(Resources.Langue.UploadFile_ErrorLigne + numeroLigne + "] : Sku " + obj.Sku + Resources.Langue.UploadFile_SkuInexistant);
                    }
                    numeroLigne++;
                }
            }
            else
            {
                uploadErrors.Add(Resources.Langue.UploadFile_FichierVide);
            }

            // Hsitorisation de l'évenement connexion.
            T_Evenement objEve = new T_Evenement();
            objEve.Dateve = DateTime.Now;
            objEve.Eve = "INSERT - MAJ de prix produit (" + nombreProduitsMAJ.ToString() + " update)";
            objEve.Login = (string)HttpContext.Current.Session["userName"];
            EvenementDao.insertEvenement(objEve);

            resultatUpload.uploadErrors = uploadErrors;
            resultatUpload.nombreLignesSuccess = nombreProduitsMAJ;

            return resultatUpload;
        }

        /// <summary>
        /// Permet de charger le contenue d'une feuille excel dans une liste d'objets à partir du chemin et le nom de la feuille désirée
        /// </summary>
        /// <param name="filePath">Chemin du fichier Excel</param>
        /// <returns>Liste des objets à </returns>
        /// <remarks>Aucune gestion d'exception n'est faite à l'intérieur de cette méthode</remarks>
        public static List<TickitUpdateObjectSimple> convertExcelProduitsFranchiseToList(string filePath)
        {
            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 8.0;");
            string req = "SELECT * FROM [Produits$]";
            OleDbCommand command = new OleDbCommand(req, conn);

            command.Connection.Open();
            OleDbDataReader reader = command.ExecuteReader();

            List<TickitUpdateObjectSimple> objectsToUpdate = new List<TickitUpdateObjectSimple>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (reader.GetValue(0).ToString() != String.Empty)
                    {
                        TickitUpdateObjectSimple obj = new TickitUpdateObjectSimple();

                        // Extraction de la réference du produit
                        obj.SKU = reader.GetValue(0).ToString();

                        // Extraction du type de montage
                        //obj.AMonterSoiMeme = reader.GetValue(1).ToString();

                        // Extraction des descriptions reglette en différentes langues
                        obj.DescReglette = reader.GetValue(1).ToString();

                        // Extraction des descriptions légales en différentes langues
                        obj.DGCCRF = reader.GetValue(2).ToString();

                        // Extraction des plus en différentes langues
                        obj.Plus1 = reader.GetValue(3).ToString();
                        obj.Plus2 = reader.GetValue(4).ToString();
                        obj.Plus3 = reader.GetValue(5).ToString();

                        obj.Variation = reader.GetValue(6).ToString();
                        obj.libelleProduit = reader.GetValue(7).ToString();

                        objectsToUpdate.Add(obj);
                    }
                }
            }

            conn.Close();
            reader.Close();
            command.Dispose();

            // Hsitorisation de l'évenement connexion.
            T_Evenement objEve = new T_Evenement();
            objEve.Dateve = DateTime.Now;
            objEve.Eve = "UPLOAD FICHIER - " + filePath;
            objEve.Login = (string)HttpContext.Current.Session["userName"];
            EvenementDao.insertEvenement(objEve);

            return objectsToUpdate;
        }

        /// <summary>
        /// Permet de mettre à jour des produits à partir du fichier Excel de MAJ.
        /// </summary>
        /// <param name="produitsToUpdate"></param>
        public static ResultatUploadFile updateProduitsFranchise(List<TickitUpdateObjectSimple> produitsToUpdate, int langageId)
        {
            ResultatUploadFile resultatUpload = new ResultatUploadFile();

            //Liste des erreurs lors du téléchargement.
            List<string> uploadErrors = new List<string>();
            int nombreProduitsMAJ = 0;

            if (produitsToUpdate.Count != 0)
            {
                // numéro à retourner à l'utilisateur en cas d'erreurs de chargement.
                int numeroLigne = 2;

                foreach (TickitUpdateObjectSimple obj in produitsToUpdate)
                {
                    // MAJ si et seulement si le produit existe.
                    T_Produit produit = ProduitDao.getProduitBySku(obj.SKU, langageId);
                    if (produit != null)
                    {

                        // MAJ DGCCRF
                        tryInsertOrUpdateDgccrf(obj.SKU, langageId, obj.DGCCRF);


                        // MAJ Description Reglette
                        tryInsertOrUpdateVariation(obj.SKU, langageId, obj.Variation);


                        // MAJ Description libellé produit
                        tryInsertOrUpdateLibelleProduit(obj.SKU, langageId, obj.Variation);

                        // MAJ Description Reglette
                        tryInsertOrUpdateDescReglette(obj.SKU, langageId, obj.DescReglette);

                        // MAJ Plus
                        tryInsertPlus(obj.SKU, obj.Plus1, langageId, 1);
                        tryInsertPlus(obj.SKU, obj.Plus2, langageId, 2);
                        tryInsertPlus(obj.SKU, obj.Plus3, langageId, 3);

                        nombreProduitsMAJ++;
                    }
                    else
                    {
                        uploadErrors.Add(Resources.Langue.UploadFile_ErrorLigne + numeroLigne + "] : Sku " + obj.SKU + Resources.Langue.UploadFile_SkuInexistant);
                    }
                    numeroLigne++;
                }
            }
            else
            {
                uploadErrors.Add(Resources.Langue.UploadFile_FichierVide);
            }

            // Hsitorisation de l'évenement connexion.
            T_Evenement objEve = new T_Evenement();
            objEve.Dateve = DateTime.Now;
            objEve.Eve = "INSERT - MAJ Franchise des produits à partir du fichier Excel de MAJ (" + nombreProduitsMAJ.ToString() + " update)";
            objEve.Login = (string)HttpContext.Current.Session["userName"];
            EvenementDao.insertEvenement(objEve);

            resultatUpload.uploadErrors = uploadErrors;
            resultatUpload.nombreLignesSuccess = nombreProduitsMAJ;

            return resultatUpload;
        }
    }
}
