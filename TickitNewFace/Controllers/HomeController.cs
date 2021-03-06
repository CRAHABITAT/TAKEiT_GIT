
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using TickitNewFace.Models;
using TickitNewFace.Managers;
using TickitNewFace.DAO;
using System.IO;
using TickitNewFace.Form;
using System.Threading;
using System.Globalization;
using TickitNewFace.Const;
using TickitNewFace.Utils;
using System.Web.Routing;
using System.Configuration;
using System.Data.SqlClient;
using iTextSharp.text;
using System.Data;
using System.Text;
using TickitNewFace.PDFUtils;

using HiQPdf;
using System.Net;

namespace TickitNewFace.Controllers
{
    /// <summary>
    /// Classe controller : Interception et traitement des requêtes HTTP.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Retourne la page d'accueil.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Session["onglet"] = "recherche";
            try
            {                
                this.setLang();
                int langageId = (int)Session["langueId"];
                String devise = DAO.LangueDao.getCodeMonnaieByMagasinId(langageId);

                List<TickitDataProduit> produitsData = new List<TickitDataProduit>();
                TickitDataChevalet chevalet = new TickitDataChevalet();

                chevalet.produitsData = produitsData;
                Session["Chevalet"] = chevalet;

                TickitDataLuminaire Luminaire = new TickitDataLuminaire();
                Session["Luminaire"] = Luminaire;
                
                ViewBag.initialDate = DateUtils.getFormatDateFr(DateTime.Now);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }

        public ActionResult PLV()
        {
            Session["onglet"] = "PLV";
            try
            {
                this.setLang();
                int langageId = (int)Session["langueId"];
                String devise = DAO.LangueDao.getCodeMonnaieByMagasinId(langageId);

                List<TickitDataProduit> produitsData = new List<TickitDataProduit>();
                TickitDataChevalet chevalet = new TickitDataChevalet();

                chevalet.produitsData = produitsData;
                Session["Chevalet"] = chevalet;

                TickitDataLuminaire Luminaire = new TickitDataLuminaire();
                Session["Luminaire"] = Luminaire;

                ViewBag.initialDate = DateUtils.getFormatDateFr(DateTime.Now);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }

        /// <summary>
        /// Retourne la page de téléchargement des PLVs vierges.
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadCenter()
        {
            this.setLang();
            return View();
        }

        /// <summary>
        /// Cette methode permet de changer la langue utilisée.
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public ActionResult changeLangue(string lang)
        {
            Session["onglet"] = "recherche";
            Session["lang"] = lang;
            this.setLang(true);
            return View("Index");
        }
        
        /// <summary>
        /// Retourne la objectsToUpdate des produits correspondants au critère de recherche
        /// </summary>
        /// <param name="rechercheText"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Produits(String rechercheText, String rechercheDate)
        {
            try
            {
                Session["onglet"] = "";
                string[] split = rechercheDate.Split(ApplicationConsts.separateurDate);
                DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

                this.setLang();
                int langageId = (int)Session["langueId"];
                CultureInfo ci = Thread.CurrentThread.CurrentCulture;

                // forcer le format des décimaux
                this.setDecimalFormat();

                if (ci.ToString().Length >= 2)
                {
                    ViewBag.lang = ci.ToString().Substring(0, 2);
                }
                else
                {
                    ViewBag.lang = ci.ToString();
                }
                
                List<T_Resultats_Recherche> produits = TickitNewFace.Managers.RechercheManager.getProduits(rechercheText, dateObj, langageId);
                ViewBag.produits = produits;
                ViewBag.dateQuery = rechercheDate;

                return PartialView("Produits");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Author : Msridi
        /// Retourne la objectsToUpdate des produits correspondants au critère de recherche
        /// </summary>
        /// <param name="rechercheText"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GammePLV(String rechercheText, String rechercheDate)
        {
            try
            {
                Session["onglet"] = "";
                string[] split = rechercheDate.Split(ApplicationConsts.separateurDate);
                DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

                this.setLang();
                int langageId = (int)Session["langueId"];
                CultureInfo ci = Thread.CurrentThread.CurrentCulture;

                // forcer le format des décimaux
                this.setDecimalFormat();

                if (ci.ToString().Length >= 2)
               {
                    ViewBag.lang = ci.ToString().Substring(0, 2);
                }
                else
                {
                    ViewBag.lang = ci.ToString();
                }

                List<T_Resultats_Recherche_PLV> gammes = TickitNewFace.Managers.RechercheManager.getPLV(rechercheText, dateObj, langageId);
                
                foreach (T_Resultats_Recherche_PLV gamme in gammes)
                {
                    gamme.sousGammes = gamme.sousGammes.Replace(";","/").Replace(",","/");
                    if (gamme.Format == "A5")
                    {
                        string[] skus = gamme.Skus.Split(';');
                        int cpt = 0;
                        gamme.sousGammes = "";
                        foreach( string sku in skus) {
                            TickitDataProduit ticket = Managers.TickitDataManager.getTickitDataPourChevalet(sku, langageId, dateObj, null, null);
                            string name = ticket.variation;
                            if (cpt != 0)
                                gamme.sousGammes += "/";
                            gamme.sousGammes += name;
                            cpt ++;
                        } 
                    }
                }
                ViewBag.gammes = gammes;
                ViewBag.dateQuery = rechercheDate;

                return PartialView("GammePLV");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }



        /// <summary>
        /// Retourne la fiche d'un produit
        /// </summary>
        /// <param name="Sku">ID produit</param>
        /// <returns></returns>
        public ActionResult Fiche(string Sku)
        {
            try
            {
                Session["onglet"] = "";
                this.setLang();

                int langageId = (int)Session["langueId"];
                string codeMonnaie = DAO.LangueDao.getCodeMonnaieByMagasinId(langageId);

                this.setDecimalFormat();

                T_Produit produit = Managers.FicheProduitManager.getProduitBySku(Sku, langageId);
                ViewBag.produit = produit;
                ViewBag.variation = Managers.FicheProduitManager.getVariationBySku(produit.Sku, langageId);

                ViewBag.rangeName = DAO.RangeDao.getRangeNameById(produit.RangeId);

                T_Prix prix = Managers.FicheProduitManager.getPrixBySkuAndDate(produit.Sku, langageId, DateTime.Now);
                ViewBag.prix = prix;

                T_Description_Dgccrf dgccrf = Managers.FicheProduitManager.getDgccrfBySku(produit.Sku, langageId);
                if (dgccrf != null)
                {
                    ViewBag.dgccrf = dgccrf.LegalDescription;
                }
                // format
                List<string> formats = Managers.FicheProduitManager.getFormatsImpressionPorduit(produit.Sku);
                T_Format_Fiche formatfiche = new T_Format_Fiche();

                formatfiche.formatA5RV = "";
                formatfiche.formatA5S = "";
                formatfiche.formatA6RV = "";
                formatfiche.formatA6S = "";
                formatfiche.formatA7RV = "";
                formatfiche.formatA7S = "";

                foreach (var item in formats)
                {
                    if (item == "A5_recto_verso")
                        formatfiche.formatA5RV = "A5_recto_verso";
                    if (item == "A5_simple")
                        formatfiche.formatA5S = "A5_simple";
                    if (item == "A6_recto_verso")
                        formatfiche.formatA6RV = "A6_recto_verso";
                    if (item == "A6_simple")
                        formatfiche.formatA6S = "A6_simple";
                    if (item == "A7_recto_verso")
                        formatfiche.formatA7RV = "A7_recto_verso";
                    if (item == "A7_simple")
                        formatfiche.formatA7S = "A7_simple";
                }
                ViewBag.formatfiche = formatfiche;
                //plus
                List<T_Description_Plus> listePlus = Managers.FicheProduitManager.getPlusBySku(produit.Sku, langageId);
                ViewBag.plus = listePlus;
                ViewBag.codeMonnaie = codeMonnaie;
                

                String DrescMaketing = Managers.FicheProduitManager.getDrescMaketingProduitBySku(Sku, langageId);
                /*if (DrescMaketing == null){
                    DrescMaketing = "";
                }*/
                ViewBag.DrescMaketing = DrescMaketing;
                String Designer = Managers.FicheProduitManager.getDesignerProduitBySku(Sku, langageId);
                /*if (Designer == null){
                    Designer = "";
                }*/
                ViewBag.Designer = Designer; 
                T_IPLV_Details IPLVD = Managers.FicheProduitManager.getIPLVDetailsProduitBySku(Sku, langageId);
                /*if (IPLVD == null){
                    string[] v = { "" };
                    IPLVD.couleurs = v;
                    IPLVD.matieres = v;
                    IPLVD.dimension_produit = "";
                    IPLVD.dimension_colis = "";
                    IPLVD.designed_habitat = "";
                }*/
                ViewBag.IPLVDetails = IPLVD;
                List<string> limg = Managers.RechercheManager.getIMG(Sku);
                List<T_IPLV_Association> SkusAssocies = Managers.FicheProduitManager.getSkusAssociesProduitBySku(Sku, langageId);
                List<T_IPLV_Association> SkusGroupe = Managers.FicheProduitManager.getSkusGroupeProduitBySku(Sku, langageId);
                ViewBag.limg = limg;
                foreach (var item in SkusAssocies)
                {
                    T_Variation variation = Managers.FicheProduitManager.getVariationBySku(item.sku, langageId);
                    item.variation = variation.VariationName;
                }
                ViewBag.SkusAssocies = SkusAssocies;
                foreach (var item in SkusGroupe) { 
                    T_Variation variation = Managers.FicheProduitManager.getVariationBySku(item.sku, langageId);
                    item.variation = variation.VariationName;
                }
                ViewBag.SkusGroupe = SkusGroupe;
                //http://images.habitat.fr/picturesProducts/ImagesSmall/805837/805837_01.png?login=habitat&pass=habitatimages
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }

        /// <summary>
        /// Met à jour le produit grâce à un formulaire passé en paramètre
        /// </summary>
        /// <param name="form"></param>
        public ActionResult SaveFiche(Form.EditProduitForm form)
        {
            try
            {
                Session["onglet"] = "";
                this.setLang();
                this.setDecimalFormat();
                int langageId = (int)Session["langueId"];
                if (Managers.FicheProduitManager.updateProduit(0,form, langageId) == 0)
                    ViewBag.updateSuccess = "Success";
                return null;
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }


        /// <summary>
        /// Met à jour le produit grâce à un formulaire passé en paramètre
        /// </summary>
        /// <param name="form"></param>
        public ActionResult SaveFicheIPLV(Form.EditProduitForm form)
        {
            try
            {
                List<T_IPLV_Association> Skusassocies = (List<T_IPLV_Association>)Session["Skusassocies"];
                List<T_IPLV_Association> Skusgroupes = (List<T_IPLV_Association>)Session["Skusgroupes"];
                List<string> Matieres = (List<string>)Session["Matieres"];
                List<string> Couleurs = (List<string>)Session["Couleurs"];
                Session["onglet"] = "";
                this.setLang();
                this.setDecimalFormat();
                int langageId = (int)Session["langueId"];
                if (Managers.FicheProduitManager.updateProduit(1, form, langageId) == 0)
                    ViewBag.updateSuccess = "Success";
                if (Managers.FicheProduitManager.updateProduitIPLV(Skusgroupes, Skusassocies, Couleurs, Matieres, form, langageId) == 0)
                    ViewBag.updateSuccess = "Success";
                return null;
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }


        /// <summary>
        /// Retourne la page d'administration et d'enrichissement de la base de données.
        /// </summary>
        /// <returns></returns>
        public ActionResult Administration()
        {
            try
            {
                Session["onglet"] = "admin";
                this.setLang();
                this.setDecimalFormat();
                ViewBag.config = ApplicationConsts.config;
                ViewBag.positionTabs = 0;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }

        /// <summary>
        /// Retourne la page d'administration et d'enrichissement de la base de données.
        /// </summary>
        /// <returns></returns>
        public ActionResult ImpressionEnMasse()
        {
            try
            {
                Session["onglet"] = "ImpressionEnMasse";
                this.setLang();
                this.setDecimalFormat();
                //creation liste des magasins a select
                List<T_magasin> Magasins = new List<T_magasin>();
                //int lid = 4;
                int langageId = (int)Session["langueId"];
                Magasins = Produit_MagasinDao.selecidMagasins(langageId);
                ViewBag.listMagasins = Magasins;
                ViewBag.config = ApplicationConsts.config;
                ViewBag.positionTabs = 0;
                ViewBag.initialDate = DateUtils.getFormatDateFr(DateTime.Now);


                //Users pour les nouvelles PLV + magasins (Nice et Wagram)
                string userName = (string)Session["userName"];
                Boolean viewRegletteNew = false;
                if (
                    userName == "msridi" ||
                   // userName == "crabehi" ||
                    userName == "cpauthenet" ||
                    userName == "mbastide" ||
                    userName == "oouba" ||
                    userName == "mfauvage" ||
                    userName == "kchaudot"  ||
                    userName == "cdoue"  ||
                    userName == "awagram" || 
                    userName == "dwagram" ||
                    userName == "dnice" ||
                    userName == "anice" 
                    )
                {
                    viewRegletteNew = true;
                }
                ViewBag.viewRegletteNew = viewRegletteNew;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }

        /// <summary> TODO
        /// Methode d'upload de fichier SKU / MAGASINS ID
        /// </summary>
        /// <param name="FileUpload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadfileProduitsMagasins(HttpPostedFileBase FileUpload, string Updatetout, string[] control)
        {
            try
            {
                //creation liste des magasins a select
                List<T_magasin> Magasinselect = new List<T_magasin>();
                int lid = 4;
                Magasinselect = Produit_MagasinDao.selecidMagasins(lid);
                ViewBag.listMagasins = Magasinselect;
                ViewBag.initialDate = DateUtils.getFormatDateFr(DateTime.Now);

                Session["onglet"] = "admin";
                this.setLang();
                this.setDecimalFormat(); 
                int langageId = (int)Session["langueId"];
                string magId = (string)Session["magid"];
                int nb_de_mag_update;
                //gestion envoie liste mag admin ou pas admin
                if (langageId != 4 && langageId != 5 && langageId != 6) // pays avec plusieurs magasin
                {
                    magId = langageId.ToString();
                }
                if (control == null)
                {
                    nb_de_mag_update = 1;
                    control = new string[1] { magId };//si pas admin on update le magasin de l user
                }
                else
                {
                    if (control[0] == "ERROR_MAG")
                    {
                        nb_de_mag_update = 1;
                        control[0] = magId;//si admin et aucun magasin est selectionné on update le magasin de l user
                    }
                    else
                    {
                        nb_de_mag_update = control.Length; //si admin et magasin selectionné(s)
                    }
                }
                //creation liste des magasins a update
                List<T_magasin> Magasins = new List<T_magasin>();
                int i;
                for (i=0;i<nb_de_mag_update;i++){
                    T_magasin magasin = new T_magasin();
                    magasin = Produit_MagasinDao.selecidMagasin(langageId, control[i]);
                    Magasins.Add(magasin);
                }
                //lecture fichier a update
                ViewBag.config = ApplicationConsts.config;

                List<string> uploadProduitsErrors;
                //si il y a un fichier
                if (FileUpload != null)
                {
                    string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    string filePath = applicationPath + fileName;

                    FileUpload.SaveAs(filePath);
                  
                    //lecture du fichier
                    List<string> produitAUpdate = UploadFichierManager.convertExcelSkuMagasinsToListUnique(filePath);

                    //liste des magasin a update avec leurs skus
                    List<T_produit_magasin_update> produitMagasins = new List<T_produit_magasin_update>();
                    foreach (T_magasin magselect in Magasins)
                    {
                        T_produit_magasin_update produitMagasin = new T_produit_magasin_update();
                        produitMagasin.Skus = produitAUpdate;
                        produitMagasin.MagasinId = magselect.Pays_id;
                        produitMagasin.magId = magselect.Magasin_id;
                        produitMagasins.Add(produitMagasin); 
                    }

                    if (produitMagasins == null)
                    {
                        uploadProduitsErrors = new List<string>();
                        uploadProduitsErrors.Add(Resources.Langue.UploadFile_ErrorFormat);
                        ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                        ViewBag.positionTabs = 0;
                        return View("ImpressionEnMasse");
                    }
                    //update du magasin dans la bdd
                    ResultatUploadFile resultatUpload = UploadFichierManager.insertProduitMagasin(produitMagasins);

                    uploadProduitsErrors = resultatUpload.uploadErrors;
                    //sortie reussite parametre
                    ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                    ViewBag.nombreProduitsSuccess = resultatUpload.nombreLignesSuccess;
                    ViewBag.nombreMagasinsSuccess = resultatUpload.nombreMagasinsSuccess;
                }
                else
                {
                    //sortie pas de fichier
                    uploadProduitsErrors = new List<string>();
                    uploadProduitsErrors.Add(Resources.Langue.UploadFile_FichierInexistant);
                    ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                }
                ViewBag.positionTabs = 0;

                string userName = (string)Session["userName"];
                Boolean viewRegletteNew = false;
                if (
                    userName == "msridi" ||
                  //  userName == "crabehi" ||
                    userName == "cpauthenet" ||
                    userName == "mbastide" ||
                    userName == "oouba" ||
                    userName == "mfauvage" ||
                    userName == "kchaudot" ||
                    userName == "cdoue"   ||
                    userName == "awagram" || 
                    userName == "dwagram" ||
                    userName == "dnice" ||
                    userName == "anice" )
                {
                    viewRegletteNew = true;
                }
                ViewBag.viewRegletteNew = viewRegletteNew;


                return View("ImpressionEnMasse");
            }
            catch (Exception ex)
            {
                //sortie erreur
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message + "  // " + ex.StackTrace;
                return View("Errors");
            }
        }


        /// <summary>
        /// Met à jour la Configuration
        /// </summary>
        /// <param name="form"></param>
        public ActionResult SaveConfiguration(Models.T_Configuration configForm)
        {
            try
            {
                Session["onglet"] = "admin";
                DAO.ConfigurationDao.updateConfiguration(configForm);
                ApplicationConsts.config = configForm;

                ViewBag.textInfo = "configuration mise à jour avec succès";
                return PartialView("Success");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Methode d'upload de fichier
        /// </summary>
        /// <param name="FileUpload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadfileProduits(HttpPostedFileBase FileUpload)
        {
            try
            {
                Session["onglet"] = "admin";
                this.setLang();
                this.setDecimalFormat();
                int langageId = (int)Session["langueId"];

                ViewBag.config = ApplicationConsts.config;

                List<string> uploadProduitsErrors;

                if (FileUpload != null)
                {
                    string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    string filePath = applicationPath + fileName;

                    FileUpload.SaveAs(filePath);

                    List<TickitUpdateObject> produitsToUpdate = UploadFichierManager.convertExcelProduitsToList(filePath);

                    if (produitsToUpdate == null)
                    {
                        uploadProduitsErrors = new List<string>();
                        uploadProduitsErrors.Add(Resources.Langue.UploadFile_ErrorFormat);
                        ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                        ViewBag.positionTabs = 0;
                        return View("Administration");
                    }

                    ResultatUploadFile resultatUpload = UploadFichierManager.updateProduits(produitsToUpdate, langageId);

                    uploadProduitsErrors = resultatUpload.uploadErrors;
                    
                    ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                    ViewBag.nombreProduitsSuccess = resultatUpload.nombreLignesSuccess;

                    if (resultatUpload.MAJAllLangue)
                    {
                        // Exécution de la Procédure stockée pour mettre à jour les champs sur les autres langues
                    }
                }
                else
                {
                    uploadProduitsErrors = new List<string>();
                    uploadProduitsErrors.Add(Resources.Langue.UploadFile_FichierInexistant);
                    ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                }

                ViewBag.positionTabs = 0;
                return View("Administration");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }



        /// <summary>
        /// Methode d'upload de fichier PLV Gamme
        /// </summary>
        /// <param name="FileUpload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadfilePLVGamme(HttpPostedFileBase FileUpload)
        {
            try
            {
                Session["onglet"] = "admin";
                this.setLang();
                this.setDecimalFormat();
                int langageId = (int)Session["langueId"];
                ViewBag.config = ApplicationConsts.config;

                List<string> uploadProduitsErrors;

                if (FileUpload != null)// si fichier
                {
                    string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    string filePath = applicationPath + fileName;

                    FileUpload.SaveAs(filePath);

                    List<T_Presentation_Gamme_PLV_tmp> produitsToUpdate = UploadFichierManager.convertExcelPlVGammeToList(filePath);

                    if (produitsToUpdate == null) // pas de produit dans le fichier
                    {
                        uploadProduitsErrors = new List<string>();
                        uploadProduitsErrors.Add(Resources.Langue.UploadFile_ErrorFormat);
                        ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                        ViewBag.positionTabs = 0;
                        return View("Administration");
                    }

                    //ResultatUploadFile resultatUpload = UploadFichierManager.updateProduits(produitsToUpdate, langageId);
                    //updatePLVGamme(List<T_Presentation_Gamme_PLV_tmp> produitsToUpdate, int langageId)
                    ResultatUploadFile resultatUpload = UploadFichierManager.updatePLVGamme(produitsToUpdate, langageId);

                    uploadProduitsErrors = resultatUpload.uploadErrors;

                    ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                    ViewBag.nombreProduitsSuccess = resultatUpload.nombreLignesSuccess;
                }
                else // pas de fichier
                {
                    uploadProduitsErrors = new List<string>();
                    uploadProduitsErrors.Add(Resources.Langue.UploadFile_FichierInexistant);
                    ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                }

                ViewBag.positionTabs = 0;
                return View("Administration");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }

        /// <summary>
        /// Methode d'upload de fichier
        /// </summary>
        /// <param name="FileUpload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadfilePrix(HttpPostedFileBase FileUpload)
        {
            try
            {
                Session["onglet"] = "admin";
                this.setLang();
                this.setDecimalFormat();
                int langageId = (int)Session["langueId"];
                ViewBag.config = ApplicationConsts.config;

                List<string> uploadProduitsErrors;

                if (FileUpload != null)
                {
                    string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    string filePath = applicationPath + fileName;

                    FileUpload.SaveAs(filePath);

                    List<T_Prix> prixToUpdate = UploadFichierManager.convertExcelPriceToList(filePath, langageId);

                    if (prixToUpdate == null)
                    {
                        uploadProduitsErrors = new List<string>();
                        uploadProduitsErrors.Add(Resources.Langue.UploadFile_ErrorFormat);
                        ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                        ViewBag.positionTabs = 0;
                        return View("Administration");
                    }

                    ResultatUploadFile resultatUpload = UploadFichierManager.updatePrix(prixToUpdate);

                    uploadProduitsErrors = resultatUpload.uploadErrors;

                    ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                    ViewBag.nombreProduitsSuccess = resultatUpload.nombreLignesSuccess;
                }
                else
                {
                    uploadProduitsErrors = new List<string>();
                    uploadProduitsErrors.Add(Resources.Langue.UploadFile_FichierInexistant);
                    ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                }

                ViewBag.positionTabs = 0;
                return View("Administration");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message  + "  // " + ex.StackTrace;
                return View("Errors");
            }
        }


        /// <summary>
        /// Methode d'upload de fichier
        /// </summary>
        /// <param name="FileUpload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadfileProduitsFranchise(HttpPostedFileBase FileUpload)
        {
            try
            {
                Session["onglet"] = "admin";
                this.setLang();
                this.setDecimalFormat();
                int langageId = (int)Session["langueId"];
                ViewBag.config = ApplicationConsts.config;

                List<string> uploadProduitsErrors;

                if (FileUpload != null)
                {
                    string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    string filePath = applicationPath + fileName;

                    FileUpload.SaveAs(filePath);

                    List<TickitUpdateObjectSimple> produitsToUpdate = UploadFichierManager.convertExcelProduitsFranchiseToList(filePath);

                    if (produitsToUpdate == null)
                    {
                        uploadProduitsErrors = new List<string>();
                        uploadProduitsErrors.Add(Resources.Langue.UploadFile_ErrorFormat);
                        ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                        ViewBag.positionTabs = 0;
                        return View("Administration");
                    }

                    ResultatUploadFile resultatUpload = UploadFichierManager.updateProduitsFranchise(produitsToUpdate, langageId);

                    uploadProduitsErrors = resultatUpload.uploadErrors;

                    ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                    ViewBag.nombreProduitsSuccess = resultatUpload.nombreLignesSuccess;
                }
                else
                {
                    uploadProduitsErrors = new List<string>();
                    uploadProduitsErrors.Add(Resources.Langue.UploadFile_FichierInexistant);
                    ViewBag.uploadProduitsErrors = uploadProduitsErrors;
                }

                ViewBag.positionTabs = 0;
                return View("Administration");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }
        /// <summary>
        /// Methode d'upload de fichier
        /// </summary>
        /// <param name="FileUpload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadfileRange(HttpPostedFileBase FileUpload)
        {
            try
            {
                Session["onglet"] = "admin";
                this.setLang();

                int langageId = (int)Session["langueId"];
                ViewBag.config = ApplicationConsts.config;

                List<string> uploadRangeErrors;

                if (FileUpload != null)
                {
                    string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    string filePath = applicationPath + fileName;

                    FileUpload.SaveAs(filePath);

                    List<RangeUpdateObject> rangesToUpdate = UploadFichierManager.convertExcelRangesToList(filePath);

                    if (rangesToUpdate == null)
                    {
                        uploadRangeErrors = new List<string>();
                        uploadRangeErrors.Add(Resources.Langue.UploadFile_ErrorFormat);
                        ViewBag.uploadRangeErrors = uploadRangeErrors;
                        ViewBag.positionTabs = 1;
                        return View("Administration");
                    }

                    ResultatUploadFile resultatUpload = UploadFichierManager.insertOrUpdatePlusRange(rangesToUpdate);
                    uploadRangeErrors = resultatUpload.uploadErrors;
                    ViewBag.uploadRangeErrors = uploadRangeErrors;
                    ViewBag.nombreRangesSuccess = resultatUpload.nombreLignesSuccess;
                }
                else
                {
                    uploadRangeErrors = new List<string>();
                    uploadRangeErrors.Add(Resources.Langue.UploadFile_FichierInexistant);
                    ViewBag.uploadRangeErrors = uploadRangeErrors;
                }

                ViewBag.positionTabs = 1;
                return View("Administration");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }

        /// <summary>
        /// Methode d'upload de fichier
        /// </summary>
        /// <param name="FileUpload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadfileRangeFranchise(HttpPostedFileBase FileUpload)
        {
            try
            {
                Session["onglet"] = "admin";
                this.setLang();

                int langageId = (int)Session["langueId"];
                ViewBag.config = ApplicationConsts.config;

                List<string> uploadRangeErrors;

                if (FileUpload != null)
                {
                    string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    string filePath = applicationPath + fileName;

                    FileUpload.SaveAs(filePath);

                    List<RangeSimpleUpdateObject> rangesToUpdate = UploadFichierManager.convertExcelRangesFranchiseToList(filePath);

                    if (rangesToUpdate == null)
                    {
                        uploadRangeErrors = new List<string>();
                        uploadRangeErrors.Add(Resources.Langue.UploadFile_ErrorFormat);
                        ViewBag.uploadRangeErrors = uploadRangeErrors;
                        ViewBag.positionTabs = 1;
                        return View("Administration");
                    }
                    
                    ResultatUploadFile resultatUpload = UploadFichierManager.insertOrUpdatePlusRange(rangesToUpdate, langageId);
                    uploadRangeErrors = resultatUpload.uploadErrors;
                    ViewBag.uploadRangeErrors = uploadRangeErrors;
                    ViewBag.nombreRangesSuccess = resultatUpload.nombreLignesSuccess;
                }
                else
                {
                    uploadRangeErrors = new List<string>();
                    uploadRangeErrors.Add(Resources.Langue.UploadFile_FichierInexistant);
                    ViewBag.uploadRangeErrors = uploadRangeErrors;
                }
                ViewBag.positionTabs = 1;
                return View("Administration");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return View("Errors");
            }
        }



        /// <summary>
        /// Gere l'impression d'une PLV.
        /// </summary>
        /// <param name="Gamme"></param> 
        /// <returns>FileStreamResult</returns>
        public FileStreamResult ImpressionPLV(string Gamme, string date)
        {
            //convertion type date
            string[] split = date.Split(ApplicationConsts.separateurDate);
            DateTime datePrint = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));
/*
          /// <summary>
        /// Renvoie la PLV impression en masse
        /// </summary>
        /// <returns>FileStreamResult</returns>
        public FileStreamResult PrintPlvEnMasse(string Division, string Departement, string Classe, string Format, string rechercheDate, string TypePrix)
        {
            string[] split = rechercheDate.Split(ApplicationConsts.separateurDate);
            DateTime datePrint = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            string magId = langageId.ToString();
            if (langageId == 4 || langageId == 5 || langageId == 6) // pays avec plusieurs magasin
            {
                magId = (string)Session["magid"];
            }
            this.setDecimalFormat();

            Division = Division == "" ? "%" : Division;
            Departement = Departement == "" ? "%" : Departement;
            Classe = Classe == "" ? "%" : Classe;

            MemoryStream MS;
            List<string> ProduitsMagasin = DAO.Produit_MagasinDao.getSkusByMagasinIdDivision(magId, langageId, Division + Departement + Classe, datePrint, TypePrix);

            if (ApplicationConsts.format_Reglette == Format)
            {
                TickitDataChevalet chevaletToPrint = new TickitDataChevalet();
                chevaletToPrint.originePanier = "CHEVALET";
                chevaletToPrint.typePrix = "N";
                chevaletToPrint.produitsData = new List<TickitDataProduit>();

                foreach (string pro in ProduitsMagasin)
                {
                    TickitDataProduit dataProduit = Managers.TickitDataManager.getTickitDataPourChevalet(pro, langageId, datePrint, Const.ApplicationConsts.format_A5_recto_verso, null);
                    chevaletToPrint.produitsData.Add(dataProduit);
                }

                MS = PDFUtils.PlvLineaireUtils.GenerateChevaletLineairePdf(chevaletToPrint, Format, langageId, datePrint);
            }
            else
            {
                TickitDataChevalet chevalet = new TickitDataChevalet();
                chevalet.originePanier = "PLVS";
                chevalet.typePrix = "N";
                chevalet.formatImpressionEtiquettesSimples = Format;
                chevalet.produitsData = new List<TickitDataProduit>();
                
                foreach (string pro in ProduitsMagasin)
                {
                    TickitDataProduit dataProduit = Managers.TickitDataManager.getTickitDataPourChevalet(pro, langageId, datePrint, Const.ApplicationConsts.format_A5_recto_verso, null);
                    dataProduit.demarqueLocale = "";
                    dataProduit.isPromoSoldes = false;
                    dataProduit.formatImpressionEtiquetteSimple = Format;
                    chevalet.produitsData.Add(dataProduit);
                }

                List<TickitDataProduit> listeTicketsToSend = new List<TickitDataProduit>();
                foreach (TickitDataProduit produit in chevalet.produitsData)
                {
                    TickitDataProduit ticket = TickitDataManager.getTickitData(produit.sku, langageId, produit.demarqueLocale, datePrint);
                    listeTicketsToSend.Add(ticket);
                }

                MS = TickitDataManager.getTickitTypeStream(listeTicketsToSend, chevalet.produitsData[0].formatImpressionEtiquetteSimple, langageId);
            }

            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=dessus.pdf");
            FileStreamResult file = File(MS, "application/pdf");
            
            return file;
        }
*/
            //language id
            this.setLang();
            int langageId = (int)Session["langueId"];
            List<TickitDataProduit> listeTicketsToSend = new List<TickitDataProduit>();
            TickitDataChevalet chevalet = new TickitDataChevalet();

            T_Presentation_Gamme_PLV test = new T_Presentation_Gamme_PLV();
            test = DAO.Resultats_RecherchePLVDao.RemplirPresentationGammePLV(Gamme, langageId);
            MemoryStream MS2;
            if (test.formatGamme == "A5")
            {
                MS2 = PDFUtils.PLV.GeneratePLVPdf3(test, datePrint, langageId);//genere composition plv
            }
            else {
                MS2 = PDFUtils.PLV.GeneratePLVPdf2(test, datePrint, langageId);//genere composition plv
            }
            List<T_Gamme_PLV> Gammes = new List<T_Gamme_PLV>();
            List<T_Sous_Gamme_PLV> SousGammes = new List<T_Sous_Gamme_PLV>();
            var tuple = new Tuple<List<T_Gamme_PLV>, List<T_Sous_Gamme_PLV>>(Gammes, SousGammes);
            tuple=DAO.Resultats_RecherchePLVDao.RemplirGammePLV(Gamme, langageId);//recupere la composition de la plv

            MemoryStream MS;
            MS = PDFUtils.PLV.GeneratePLVPdf(tuple.Item1, tuple.Item2, datePrint, langageId);//genere composition plv

            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=dessus.pdf");
            FileStreamResult file = File(MS2, "application/pdf");

            return file;
        }


        /// <summary>
        /// Renvoie la vue menu d'impression.
        /// </summary>
        /// <param name="Sku"></param> 
        /// <returns></returns>
        public ActionResult MenuImpression(string Sku, DateTime date)
        {
            try
            {
                this.setLang();
                int langageId = (int)Session["langueId"];
                this.setDecimalFormat();
                T_Variation variation = TickitNewFace.Managers.FicheProduitManager.getVariationBySku(Sku, langageId);
                T_Produit produit = TickitNewFace.Managers.FicheProduitManager.getProduitBySku(Sku, langageId);

                T_Prix prix = TickitNewFace.Managers.FicheProduitManager.getPrixBySkuAndDate(Sku, langageId, date);

                List<string> formats = Managers.FicheProduitManager.getFormatsImpressionPorduit(Sku);

                ViewBag.sku = Sku;
                ViewBag.range = DAO.RangeDao.getRangeNameById(produit.RangeId);
                ViewBag.variation = variation.VariationName;
                ViewBag.prix = prix.Prix_produit;
                ViewBag.formatsImpression = formats;

                ViewBag.initialDate = DateUtils.getFormatDateFr(date);

                ViewBag.prixDeBaseDemarqueLocale = prix.Prix_produit;

                if (prix.Type_promo != ApplicationConsts.typePrix_permanent)
                {
                    T_Prix prixPermanent = TickitNewFace.Managers.FicheProduitManager.getPrixPermanentPrecedent(prix);

                    if (prixPermanent != null)
                    {
                       // a remettre decimal? pourcentage = 100 - (((prix.Prix_produit - (produit.Eco_mobilier == null ? 0 : produit.Eco_mobilier)) * 100) / prixPermanent.Prix_produit);

                        //Cillia
                        decimal? pourcentage = 100 - (((prix.Prix_produit - (produit.Eco_mobilier == null ? 0 : produit.Eco_mobilier)) * 100) / prixPermanent.Prix_produit -(produit.Eco_mobilier == null ? 0 : produit.Eco_mobilier));


                        pourcentage = Utils.SpecificMathUtils.getRoundDecimal(pourcentage);

                        ViewBag.pourcentage = pourcentage;
                        ViewBag.prixPermanent = prixPermanent.Prix_produit;
                        ViewBag.prixDeBaseDemarqueLocale = prixPermanent.Prix_produit;
                    }
                }

                ViewBag.codeMonnaie = DAO.LangueDao.getCodeMonnaieByMagasinId(langageId);
                return PartialView();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Renvoie une etiquette dans un flux PDF.
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="format"></param>
        /// <param name="demarqueLocale"></param>
        /// <param name="isPromo"></param>
        /// <returns></returns>
        public FileStreamResult PrintPdf(string dateQuery)
        {
            string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();
            
            TickitDataChevalet chevalet = (TickitDataChevalet)Session["Chevalet"];

            List<TickitDataProduit> listeTicketsToSend = new List<TickitDataProduit>();
            foreach (TickitDataProduit produit in chevalet.produitsData)
            {
                TickitDataProduit ticket = TickitDataManager.getTickitData(produit.sku, langageId, produit.demarqueLocale, dateObj);
                listeTicketsToSend.Add(ticket);
            }
            
            MemoryStream MS = TickitDataManager.getTickitTypeStream(listeTicketsToSend, chevalet.produitsData[0].formatImpressionEtiquetteSimple, langageId);
            
            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=dessus.pdf");
            FileStreamResult file = File(MS, "application/pdf");

            return file;
        }



        /// <summary>
        /// Renvoie la nouvelle version du format A6 R/V.
        /// MSRIDI 
        /// 28/09/2021
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintA6RectoVerso(string format, string dateQuery)
        {
            string prefixFileName = "A6_recto_verso_";
            string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();

            TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];

            List<TickitDataChevalet> chevalets = PDFUtils.PlvA6RectoVerso.splitChevalet(chevaletToPrint);

            List<string> listePathPdfFiles = new List<string>();
            string urlTest = "http://2997fr-mssql04/product/Content/maquette_A6/maquette.html";
            urlTest = "";
            int iteration = 1;
            foreach (TickitDataChevalet currentChevalet in chevalets)
            {
                string htmlCode = PDFUtils.PlvA6RectoVerso.getHtmlA6(currentChevalet, format, langageId, dateObj);
                HiqPdfManager.ConvertToPdf(PlvA6RectoVerso.getHtmlToPdfModel(), htmlCode, iteration.ToString(), prefixFileName, urlTest);
                listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_" + iteration + ".pdf");
                iteration++;
            }

            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "A6_recto_verso.pdf");
        }

        /// <summary>
        /// Renvoie la nouvelle version du format A7 R/V.
        /// MSRIDI 
        /// 27/10/2021
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintA7RectoVerso(string format, string dateQuery)
        {
            string prefixFileName = "A7_recto_verso_";
            string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();

            TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];

            List<TickitDataChevalet> chevalets = PDFUtils.PlvA7RectoVerso.splitChevalet(chevaletToPrint);

            List<string> listePathPdfFiles = new List<string>();
            string urlTest = "http://2997fr-mssql04/product/Content/maquette_A7/maquette.html";
            urlTest = "";
            int iteration = 1;
            foreach (TickitDataChevalet currentChevalet in chevalets)
            {
                string htmlCode = PDFUtils.PlvA7RectoVerso.getHtmlA7(currentChevalet, format, langageId, dateObj);
                HiqPdfManager.ConvertToPdf(PlvA7RectoVerso.getHtmlToPdfModel(), htmlCode, iteration.ToString(), prefixFileName, urlTest);
                listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_" + iteration + ".pdf");
                iteration++;
            }

            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "A7_recto_verso.pdf");
        }
        
        /// <summary>
        /// Renvoie la page panier d'impression chevalet.
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Chevalet(string Sku, string dateQuery, string format, string demarqueLocale, string origine)
        {
            try
            {
                string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
                DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));
                
                this.setLang();
                
                int langageId = (int)Session["langueId"];
                this.setDecimalFormat();
                TickitDataProduit dataProduit = Managers.TickitDataManager.getTickitDataPourChevalet(Sku, langageId, dateObj, format, demarqueLocale);

                TickitDataChevalet chevalet = (TickitDataChevalet)Session["Chevalet"];
                ViewBag.ProduitDejaExistant = false;
                ViewBag.isChevaletDeGamme = true;
                ViewBag.ChevaletCapaciteAtteinte = false;
                ViewBag.formatsImpressionIncompatibles = false;
                ViewBag.originesDifferentes = false;
                
                // remédier au risque de perte de variables de session.. solution provisoire.
                if (chevalet == null || chevalet.produitsData == null)
                {
                    List<TickitDataProduit> produitsData = new List<TickitDataProduit>();
                    chevalet = new TickitDataChevalet();
                    
                    chevalet.produitsData = produitsData;
                    Session["Chevalet"] = chevalet;
                }
                
                // if panier est de type chevalet
                if (chevalet.originePanier == null)
                {
                    chevalet.originePanier = origine;
                }
                
                if (origine == chevalet.originePanier && origine == "CHEVALET")
                {
                    // Si le nombre de produits excède les 14, nous alertons l'utilisateur et les boutons imprimer chevalet et vitrine seront cachés.
                    if (chevalet.produitsData.Count > 14)
                    {
                        ViewBag.ChevaletCapaciteAtteinte = true;
                    }
                    
                    if (!ChevaletManager.isProduitExistsInChevalet(Sku, chevalet))
                    {
                        T_Prix prixProduit = Managers.FicheProduitManager.getPrixBySkuAndDate(dataProduit.sku, langageId, dateObj);
                        if (prixProduit.Type_promo != ApplicationConsts.typePrix_permanent)
                        {
                            T_Prix prixProduitReference = Managers.FicheProduitManager.getPrixPermanentPrecedent(prixProduit);
                            // decimal? pourcentageReduction = 100 - (((prixProduit.Prix_produit - prixProduit.Eco_mobilier) * 100) / prixProduitReference.Prix_produit);


                            //Cillia 22/11/2021
                            decimal? pourcentageReduction = 100 - (((prixProduit.Prix_produit - prixProduit.Eco_mobilier) * 100) / (prixProduitReference.Prix_produit - prixProduit.Eco_mobilier));
                            
                            // Arrondit un pourcentage très proche de 0 (chiffre après virgule < 0.05)
                            pourcentageReduction = Utils.SpecificMathUtils.getRoundDecimal(pourcentageReduction);
                            
                            if (chevalet.typePrix == null)
                            {
                                chevalet.typePrix = prixProduit.Type_promo;
                                chevalet.pourcentageReduction = pourcentageReduction;
                                chevalet.produitsData.Add(dataProduit);
                            }
                            else
                            {
                                if ((prixProduit.Type_promo == chevalet.typePrix) && (pourcentageReduction == chevalet.pourcentageReduction))
                                {
                                    chevalet.produitsData.Add(dataProduit);
                                }
                                else
                                {
                                    // ne pas ajouter le produit. Vérifier le pourcentage et / ou le type de promo.
                                    ViewBag.typePromoPourcentageIncompatible = true;
                                }
                            }
                        }
                        else
                        {
                            // s'assurer que le chevalet n'a pas de type 
                            if ((chevalet.typePrix == null) || (chevalet.typePrix == ApplicationConsts.typePrix_permanent))
                            {
                                chevalet.typePrix = ApplicationConsts.typePrix_permanent;
                                chevalet.produitsData.Add(dataProduit);
                            }
                            else
                            {
                                // vider le chevalet avant.
                                ViewBag.typePromoPourcentageIncompatible = true;
                            }
                        }
                    }
                    else
                    {
                        ViewBag.ProduitDejaExistant = true;
                    }

                    chevalet.rangeChevalet = ChevaletManager.getRangeOfChevalet(chevalet);

                    // Vérifier après un ajout potentiel que le chevalet est toujours un chevalet de gamme ou pas.
                    ViewBag.isChevaletDeGamme = ChevaletManager.isChevaletDeGamme(chevalet);
                    ViewBag.dateQuery = dateQuery;
                    ViewBag.Chevalet = chevalet;
                }


                if (origine == chevalet.originePanier && origine == "PLVS")
                {
                    T_Prix prixProduit = Managers.FicheProduitManager.getPrixBySkuAndDate(dataProduit.sku, langageId, dateObj);
                    decimal? pourcentageReduction = null;
                    string typePrixProduit = null;

                    // Si démarque locale.
                    if (demarqueLocale != null && demarqueLocale != "")
                    {
                        prixProduit.Type_promo = ApplicationConsts.typePrix_demarqueLocale;
                        pourcentageReduction = decimal.Parse(demarqueLocale);
                        typePrixProduit = "reduction";
  
                    }

                    // Si promo ou solde.
                 //cillia   if (prixProduit.Type_promo == ApplicationConsts.typePrix_promo || prixProduit.Type_promo == ApplicationConsts.typePrix_solde)

                    if (prixProduit.Type_promo != ApplicationConsts.typePrix_permanent && prixProduit.Type_promo !=ApplicationConsts.typePrix_demarqueLocale)
                    {
                        T_Prix prixProduitReference = Managers.FicheProduitManager.getPrixPermanentPrecedent(prixProduit);
                       // pourcentageReduction = 100 - (((prixProduit.Prix_produit - prixProduit.Eco_mobilier) * 100) / prixProduitReference.Prix_produit);

                        //Cillia 04/01/2022
                      pourcentageReduction = 100 - (((prixProduit.Prix_produit - prixProduit.Eco_mobilier) * 100) / (prixProduitReference.Prix_produit - prixProduit.Eco_mobilier));

                        // Arrondit un pourcentage très proche de 0 (chiffre après virgule < 0.05)
                        pourcentageReduction = Utils.SpecificMathUtils.getRoundDecimal(pourcentageReduction);

                        typePrixProduit = "reduction";
                    }

                    // Si prix permanent
                    if (prixProduit.Type_promo == ApplicationConsts.typePrix_permanent)
                    {
                        typePrixProduit = "permanent";
                    }

                    // Si le chevalet n'a pas de deja de type de prix affecté.
                    if (chevalet.typePrix == null || chevalet.typePrix == "")
                    {
                        chevalet.typePrix = typePrixProduit;
                        chevalet.formatImpressionEtiquettesSimples = format;
                    }

                    if ((chevalet.typePrix == ApplicationConsts.typePrix_permanent && typePrixProduit == "permanent") ||
                        (chevalet.typePrix == ApplicationConsts.typePrix_demarqueLocale && typePrixProduit == "reduction") ||
                        (chevalet.typePrix == ApplicationConsts.typePrix_promo && typePrixProduit == "reduction") ||
                        (chevalet.typePrix == ApplicationConsts.typePrix_solde && typePrixProduit == "reduction"))
                    {
                        chevalet.typePrix = typePrixProduit;
                    }

//Cillia
                  // T_Prix prix = DAO.PrixDao.getPrixBySkuAndDate(dataProduit.sku, langageId, dateObj);
                   //  typeTarifCbr = prix.TypeTarifCbr;
                    string typeTarifCbr = "";
                    chevalet.typeTarifCbr = prixProduit.TypeTarifCbr;

                    typeTarifCbr = chevalet.typeTarifCbr;

                   

                    // Si le type de prix du produit est compatible avec celui du chevalet.
                    if (typePrixProduit == chevalet.typePrix && typeTarifCbr == chevalet.typeTarifCbr )
                    {
                        if (format == chevalet.formatImpressionEtiquettesSimples)
                        {
                            chevalet.typePrix = prixProduit.Type_promo;
                            chevalet.pourcentageReduction = pourcentageReduction;
                            chevalet.produitsData.Add(dataProduit);
                        }
                        else
                        {
                            ViewBag.formatsImpressionIncompatibles = true;
                            // formats impression icompatibles
                        }
                    }
                    else
                    {
                        // type de prix incompatible
                        ViewBag.typePromoPourcentageIncompatible = true;
                    }

                    //Cillia 

                 /*   T_Prix prix = DAO.PrixDao.getPrixBySkuAndDate(dataProduit.sku, langageId, dateObj);
                    string typeTarifCbr = prix.TypeTarifCbr;

                    if (typePrixProduit == chevalet.typePrix && typeTarifCbr == "HABHFR")
                    {
                        chevalet.typePrix = prixProduit.Type_promo;
                        chevalet.pourcentageReduction = pourcentageReduction;
                        chevalet.produitsData.Add(dataProduit);
                    }
                    else
                    {
                        // type de prix incompatible
                        ViewBag.typePromoPourcentageIncompatible = true;
                    }

                    //Cillia */




                    chevalet.rangeChevalet = ChevaletManager.getRangeOfChevalet(chevalet);

                    ViewBag.dateQuery = dateQuery;
                    ViewBag.Chevalet = chevalet;
                }

                if (origine != chevalet.originePanier)
                {
                    ViewBag.originesDifferentes = true;
                    ViewBag.dateQuery = dateQuery;
                    ViewBag.Chevalet = chevalet;
                }

                string userName = (string)Session["userName"];
                Boolean viewRegletteNew = false;
                if (
                    userName == "msridi" ||
                  //  userName == "crabehi" ||
                    userName == "cpauthenet" ||
                    userName == "mbastide" ||
                    userName == "oouba" ||
                    userName == "mfauvage" ||
                    userName == "kchaudot" ||
                    userName == "cdoue"  ||
                    userName == "awagram" || 
                    userName == "dwagram" ||
                    userName == "dnice" ||
                    userName == "anice" )
                {
                    viewRegletteNew = true;
                }
                ViewBag.viewRegletteNew = viewRegletteNew;

                return PartialView();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message + " - - " + ex.StackTrace + " - - " + ex.InnerException + " - - " + ex.Data;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Cette methode supprime un produit du chevalet
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RemoveProduitFromChevalet(string Sku, string dateQuery)
        {
            try
            {
                this.setLang();
                List<TickitDataProduit> nouvelleListeProduitsData = new List<TickitDataProduit>();

                TickitDataChevalet chevalet = (TickitDataChevalet)Session["Chevalet"];

                foreach (TickitDataProduit data in chevalet.produitsData)
                {
                    if (data.sku != Sku)
                    {
                        nouvelleListeProduitsData.Add(data);
                    }
                }

                chevalet.produitsData = nouvelleListeProduitsData;
                chevalet.rangeChevalet = ChevaletManager.getRangeOfChevalet(chevalet);
                chevalet.typePrix = ChevaletManager.getTypePrixOfChevalet(chevalet);
                chevalet.pourcentageReduction = ChevaletManager.getPourcentageReductionOfChevalet(chevalet);

                Session["Chevalet"] = chevalet;
                ViewBag.Chevalet = chevalet;
                ViewBag.dateQuery = dateQuery;
                ViewBag.isChevaletDeGamme = ChevaletManager.isChevaletDeGamme(chevalet);

                if (nouvelleListeProduitsData.Count <= 14)
                {
                    ViewBag.ChevaletCapaciteAtteinte = false;
                }
                else
                {
                    ViewBag.ChevaletCapaciteAtteinte = true;
                }

                // S'il n'y a plus aucun produit, réinitialiser le chevalet.
                if (nouvelleListeProduitsData.Count == 0)
                {
                    return ViderChevalet();
                }

                string userName = (string)Session["userName"];
                Boolean viewRegletteNew = false;
                if (
                    userName == "msridi" ||
                   // userName == "crabehi" ||
                    userName == "cpauthenet" ||
                    userName == "mbastide" ||
                    userName == "oouba" ||
                    userName == "mfauvage" ||
                    userName == "kchaudot" ||
                    userName == "cdoue"  ||
                    userName == "awagram" || 
                    userName == "dwagram" ||
                    userName == "dnice" ||
                    userName == "anice" )
                {
                    viewRegletteNew = true;
                }
                ViewBag.viewRegletteNew = viewRegletteNew;


                return PartialView("Chevalet");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Cette methode vide le chevalet.
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ViderChevalet()
        {
            try
            {
                this.setLang();
                List<TickitDataProduit> nouvelleListeProduitsData = new List<TickitDataProduit>();

                List<TickitDataProduit> listeChevalet = new List<TickitDataProduit>();
                TickitDataChevalet chevalet = new TickitDataChevalet();
                chevalet.produitsData = listeChevalet;

                Session["Chevalet"] = chevalet;
                ViewBag.Chevalet = chevalet;

                return PartialView("Chevalet");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Renvoie la PLV chevalet de gamme.
        /// </summary>
        /// <returns></returns>
        public FileStreamResult PrintChevaletDeGammePdf(string format)
        {
            this.setLang();

            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();
            TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];
            MemoryStream MS = PDFUtils.PlvChevaletUtils.GenerateChevaletDeGammePdf(chevaletToPrint, format, langageId);

            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=dessus.pdf");
            FileStreamResult file = File(MS, "application/pdf");

            return file;
        }

        /// <summary>
        /// Renvoie la PLV chevalet
        /// </summary>
        /// <returns></returns>
        public FileStreamResult PrintChevaletVitrinePdf(string format)
        {
            this.setLang();

            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();

            TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];
            MemoryStream MS = PDFUtils.PlvChevaletUtils.GenerateChevaletVitrinePdf(chevaletToPrint, format, langageId);

            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=dessus.pdf");
            FileStreamResult file = File(MS, "application/pdf");

            return file;
        }

        /// <summary>
        /// Renvoie la PLV linéaire
        /// </summary>
        /// <returns></returns>
        public FileStreamResult PrintChevaletLineairePdf(string format, string dateQuery)
        {
            string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();

            TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];

            MemoryStream MS = PDFUtils.PlvLineaireUtils.GenerateChevaletLineairePdf(chevaletToPrint, format, langageId, dateObj);
            
            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=dessus.pdf");
            FileStreamResult file = File(MS, "application/pdf");
            
            return file;
        }

        /// <summary>
        /// Renvoie la nouvelle version de la PLV.
        /// MSRIDI 
        /// 14/07/2020
        /// </summary>
        /// <returns></returns>
        public FileStreamResult PrintChevaletLineaireBisPdf(string format, string dateQuery)
        {
            string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();

            TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];

            MemoryStream MS = PDFUtils.PlvLineaireCarreUtils.GenerateChevaletLineairePdf(chevaletToPrint, format, langageId, dateObj);

            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=dessus.pdf");
            FileStreamResult file = File(MS, "application/pdf");

            return file;
        }

        /// <summary>
        /// Renvoie la nouvelle version de la PLV réglette 6.5 x 3.
        /// MSRIDI 
        /// 03/08/2021
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintChevaletLineaire653Pdf(string format, string dateQuery)
        {
            string prefixFileName = "REGLETTE_NEW_";
            string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();

            TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];
            List<TickitDataChevalet> chevalets = PDFUtils.PlvLineaire653Utils.splitChevalet(chevaletToPrint);

            List<string> listePathPdfFiles = new List<string>();
            int iteration = 1;
            foreach (TickitDataChevalet currentChevalet in chevalets)
            {
                string htmlCode = PDFUtils.PlvLineaire653Utils.getHtmlLineaire653(currentChevalet, format, langageId, dateObj);
                HiqPdfManager.ConvertToPdf(PlvLineaire653Utils.getHtmlToPdfModel(), htmlCode, iteration.ToString(), prefixFileName, "");
                listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_" + iteration + ".pdf");
                iteration++;
            }

            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "REGLETTE.pdf");
        }


        /// <summary>
        /// Renvoie la nouvelle version du format A4.
        /// MSRIDI 
        /// 29/08/2021
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintChevaletA4(string format, string dateQuery)
        {
            string prefixFileName = "A4_NEW_";
            string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();

            TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];
            
            List<string> listePathPdfFiles = new List<string>();
            
            string htmlCode = PDFUtils.PlvChevaletA4Utils.getHtmlA4(chevaletToPrint, format, langageId, dateObj);
            HiqPdfManager.ConvertToPdf(PlvChevaletA4Utils.getHtmlToPdfModel() ,htmlCode, "1", prefixFileName, "");
            listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_1" + ".pdf");
        
            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "A4.pdf");
        }


        /// <summary>
        /// Renvoie la nouvelle version du format A5.
        /// MSRIDI 
        /// 08/09/2021
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintChevaletA5(string format, string dateQuery)
        {
            string prefixFileName = "A5_NEW_";
            string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();

            TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];

            List<string> listePathPdfFiles = new List<string>();

            string htmlCode = PDFUtils.PlvChevaletA5Utils.getHtmlA5(chevaletToPrint, format, langageId, dateObj);
            string urlTest = "http://2997fr-mssql04/product/Content/maquette_A5/maquette.html";
            urlTest = "";
            HiqPdfManager.ConvertToPdf(PlvChevaletA5Utils.getHtmlToPdfModel(), htmlCode, "1", prefixFileName, urlTest);
            listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_1" + ".pdf");

            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "A5.pdf");
        }

        //Cillia 
        //A5 meuble
        public ActionResult PrintChevaletA5meuble(string format, string dateQuery)
        {
            string prefixFileName = "A5_NEW_MEUBLE";
            string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();

            TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];

            List<string> listePathPdfFiles = new List<string>();

            string htmlCode = PDFUtils.PlvChevaletA5meubleUtils.getHtmlA5(chevaletToPrint, format, langageId, dateObj);
            string urlTest = "http://2997fr-mssql04/product/Content/maquette_A5/A5_meuble.html";
            urlTest = "";
            HiqPdfManager.ConvertToPdf(PlvChevaletA5meubleUtils.getHtmlToPdfModel(), htmlCode, "1", prefixFileName, urlTest);
            listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_1" + ".pdf");

            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "A5_meuble.pdf");
        }


      




        /// <summary>
        /// Renvoie la nouvelle version du format A6.
        /// MSRIDI 
        /// 28/09/2021
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintChevaletA6(string format, string dateQuery)
        {
            string prefixFileName = "A6_NEW_";
            string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();

            TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];

            //List<string> listePathPdfFiles = new List<string>();

            List<TickitDataChevalet> chevalets = PDFUtils.PlvChevaletA6Utils.splitChevaletA6(chevaletToPrint);

            List<string> listePathPdfFiles = new List<string>();
            string urlTest = "http://2997fr-mssql04/product/Content/maquette_A6/maquette.html";
            urlTest = "";
            int iteration = 1;
            foreach (TickitDataChevalet currentChevalet in chevalets)
            {
                string htmlCode = PDFUtils.PlvChevaletA6Utils.getHtmlA6(currentChevalet, format, langageId, dateObj);
                HiqPdfManager.ConvertToPdf(PlvChevaletA6Utils.getHtmlToPdfModel(), htmlCode, iteration.ToString(), prefixFileName, urlTest);
                listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_" + iteration + ".pdf");
                iteration++;
            }

            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "A6.pdf");
        }



        /*    string htmlCode = PDFUtils.PlvChevaletA6Utils.getHtmlA6(chevaletToPrint, format, langageId, dateObj);
            string urlTest = "http://2997fr-mssql04/product/Content/maquette_A6/maquette.html";
            urlTest = "";
            HiqPdfManager.ConvertToPdf(PlvChevaletA6Utils.getHtmlToPdfModel(), htmlCode, "1", prefixFileName, urlTest);
            listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_1" + ".pdf");

            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "A6.pdf");
        }*/

        /// <summary>
        /// Cette methode vérifie s'il y'a des produits qui existent
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VerifProduitsMagasinExist(string Division, string Departement, string Classe, string rechercheDate, string TypePrix)
        {
            try
            {
                this.setLang();
                int langageId = (int)Session["langueId"];
                string magId = langageId.ToString();
                if (langageId == 4 || langageId == 5 || langageId == 6) // pays avec plusieurs magasin
                {
                    magId = (string)Session["magid"];
                }

                Division = Division == "" ? "%" : Division;
                Departement = Departement == "" ? "%" : Departement;
                Classe = Classe == "" ? "%" : Classe;

                //DateTime datePrint = DateTime.Today;
                //DateTime datePrint = new DateTime(2015, 12, 28);

                string[] split = rechercheDate.Split(ApplicationConsts.separateurDate);
                DateTime datePrint = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

                //TOREMOVE
                // magId = "MH117";
                List<string> ProduitsMagasin = DAO.Produit_MagasinDao.getSkusByMagasinIdDivision(magId, langageId, Division + Departement + Classe, datePrint, TypePrix);

                ViewBag.NBRE_PRODUITS = ProduitsMagasin.Count;
                if (ProduitsMagasin.Count == 0) {
                    ViewBag.Message = "PRODUITS_MAGASIN_NOT_EXISTS";
                    return PartialView("Message");
                }
                
                ViewBag.Message = "PRODUITS_MAGASIN_EXISTS";
                return PartialView("Message");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Renvoie la PLV impression en masse
        /// </summary>
        /// <returns>FileStreamResult</returns>
        public FileStreamResult PrintPlvEnMasse(string Division, string Departement, string Classe, string Format, string rechercheDate, string TypePrix)
        {
            string[] split = rechercheDate.Split(ApplicationConsts.separateurDate);
            DateTime datePrint = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            string magId = langageId.ToString();
            if (langageId == 4 || langageId == 5 || langageId == 6) // pays avec plusieurs magasin
            {
                magId = (string)Session["magid"];
            }
            this.setDecimalFormat();

            Division = Division == "" ? "%" : Division;
            Departement = Departement == "" ? "%" : Departement;
            Classe = Classe == "" ? "%" : Classe;

            MemoryStream MS;
            //TOREMOVE
            // magId = "MH117";
            List<string> ProduitsMagasin = DAO.Produit_MagasinDao.getSkusByMagasinIdDivision(magId, langageId, Division + Departement + Classe, datePrint, TypePrix);

            if (ApplicationConsts.format_Reglette == Format)
            {
                TickitDataChevalet chevaletToPrint = new TickitDataChevalet();
                chevaletToPrint.originePanier = "CHEVALET";
                chevaletToPrint.typePrix = "N";
                chevaletToPrint.produitsData = new List<TickitDataProduit>();

                foreach (string pro in ProduitsMagasin)
                {
                    TickitDataProduit dataProduit = Managers.TickitDataManager.getTickitDataPourChevalet(pro, langageId, datePrint, Const.ApplicationConsts.format_A5_recto_verso, null);
                    chevaletToPrint.produitsData.Add(dataProduit);
                }

                MS = PDFUtils.PlvLineaireUtils.GenerateChevaletLineairePdf(chevaletToPrint, Format, langageId, datePrint);
            }
            else
            {
                TickitDataChevalet chevalet = new TickitDataChevalet();
                chevalet.originePanier = "PLVS";
                chevalet.typePrix = "N";
                chevalet.formatImpressionEtiquettesSimples = Format;
                chevalet.produitsData = new List<TickitDataProduit>();
                
                foreach (string pro in ProduitsMagasin)
                {
                    TickitDataProduit dataProduit = Managers.TickitDataManager.getTickitDataPourChevalet(pro, langageId, datePrint, Const.ApplicationConsts.format_A5_recto_verso, null);
                    dataProduit.demarqueLocale = "";
                    dataProduit.isPromoSoldes = false;
                    dataProduit.formatImpressionEtiquetteSimple = Format;
                    chevalet.produitsData.Add(dataProduit);
                }

                List<TickitDataProduit> listeTicketsToSend = new List<TickitDataProduit>();
                foreach (TickitDataProduit produit in chevalet.produitsData)
                {
                    TickitDataProduit ticket = TickitDataManager.getTickitData(produit.sku, langageId, produit.demarqueLocale, datePrint);
                    listeTicketsToSend.Add(ticket);
                }

                MS = TickitDataManager.getTickitTypeStream(listeTicketsToSend, chevalet.produitsData[0].formatImpressionEtiquetteSimple, langageId);
            }

            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=dessus.pdf");
            FileStreamResult file = File(MS, "application/pdf");
            
            return file;
        }


        /// <summary>
        /// Renvoie la PLV impression en masse des reglettes 653
        /// MSRIDI : date 01112021
        /// </summary>
        /// <returns>FileStreamResult</returns>
        public ActionResult PrintPlvEnMasseRegletteNew(string Division, string Departement, string Classe, string Format, string rechercheDate, string TypePrix)
        {
            string[] split = rechercheDate.Split(ApplicationConsts.separateurDate);
            DateTime datePrint = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            string magId = langageId.ToString();
            if (langageId == 4 || langageId == 5 || langageId == 6) // pays avec plusieurs magasin
            {
                magId = (string)Session["magid"];
            }
            this.setDecimalFormat();

            Division = Division == "" ? "%" : Division;
            Departement = Departement == "" ? "%" : Departement;
            Classe = Classe == "" ? "%" : Classe;

            //TOREMOVE
            // magId = "MH117";
            List<string> ProduitsMagasin = DAO.Produit_MagasinDao.getSkusByMagasinIdDivision(magId, langageId, Division + Departement + Classe, datePrint, TypePrix);

            TickitDataChevalet chevaletToPrint = new TickitDataChevalet();
            chevaletToPrint.originePanier = "CHEVALET";
            chevaletToPrint.typePrix = TypePrix;
            chevaletToPrint.produitsData = new List<TickitDataProduit>();

            foreach (string pro in ProduitsMagasin)
            {
                TickitDataProduit dataProduit = Managers.TickitDataManager.getTickitDataPourChevalet(pro, langageId, datePrint, Const.ApplicationConsts.format_A5_recto_verso, null);
                chevaletToPrint.produitsData.Add(dataProduit);
            }
            string prefixFileName = "REGLETTE_NEW_";
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            this.setDecimalFormat();

            List<TickitDataChevalet> chevalets = PDFUtils.PlvLineaire653Utils.splitChevalet(chevaletToPrint);

            List<string> listePathPdfFiles = new List<string>();
            int iteration = 1;
            foreach (TickitDataChevalet currentChevalet in chevalets)
            {
                string htmlCode = PDFUtils.PlvLineaire653Utils.getHtmlLineaire653(currentChevalet, Format, langageId, dateObj);
                HiqPdfManager.ConvertToPdf(PlvLineaire653Utils.getHtmlToPdfModel(), htmlCode, iteration.ToString(), prefixFileName, "");
                listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_" + iteration + ".pdf");
                iteration++;
            }

            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "REGLETTE.pdf");

            }
//Cillia
        public ActionResult PrintPlvEnMasseA6rectoverso(string Division, string Departement, string Classe, string Format, string rechercheDate, string TypePrix)
        {
            string[] split = rechercheDate.Split(ApplicationConsts.separateurDate);
            DateTime datePrint = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            string magId = langageId.ToString();
            if (langageId == 4 || langageId == 5 || langageId == 6) // pays avec plusieurs magasin
            {
                magId = (string)Session["magid"];
            }
            this.setDecimalFormat();

            Division = Division == "" ? "%" : Division;
            Departement = Departement == "" ? "%" : Departement;
            Classe = Classe == "" ? "%" : Classe;

            //TOREMOVE
            // magId = "MH117";
            List<string> ProduitsMagasin = DAO.Produit_MagasinDao.getSkusByMagasinIdDivision(magId, langageId, Division + Departement + Classe, datePrint, TypePrix);

            TickitDataChevalet chevaletToPrint = new TickitDataChevalet();
            chevaletToPrint.originePanier = "CHEVALET";
            chevaletToPrint.typePrix = TypePrix;
            chevaletToPrint.produitsData = new List<TickitDataProduit>();

            foreach (string pro in ProduitsMagasin)
            {
                TickitDataProduit dataProduit = Managers.TickitDataManager.getTickitDataPourChevalet(pro, langageId, datePrint, Const.ApplicationConsts.format_A5_recto_verso, null);
                chevaletToPrint.produitsData.Add(dataProduit);
            }
            string prefixFileName = "REGLETTE_NEW_";
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            this.setDecimalFormat();

            List<TickitDataChevalet> chevalets = PDFUtils.PlvA6RectoVerso.splitChevalet(chevaletToPrint);

            List<string> listePathPdfFiles = new List<string>();
            int iteration = 1;
            foreach (TickitDataChevalet currentChevalet in chevalets)
            {
                string htmlCode = PDFUtils.PlvA6RectoVerso.getHtmlA6(currentChevalet, Format, langageId, dateObj);
                HiqPdfManager.ConvertToPdf(PlvA6RectoVerso.getHtmlToPdfModel(), htmlCode, iteration.ToString(), prefixFileName, "");
                listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_" + iteration + ".pdf");
                iteration++;
            }

            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "A6_recto_verso.pdf");
            }

        //Cillia 
        //Impression en masse A7_recto-verso

        public ActionResult PrintPlvEnMasseA7rectoverso(string Division, string Departement, string Classe, string Format, string rechercheDate, string TypePrix)
        {
            string[] split = rechercheDate.Split(ApplicationConsts.separateurDate);
            DateTime datePrint = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            int langageId = (int)Session["langueId"];
            string magId = langageId.ToString();
            if (langageId == 4 || langageId == 5 || langageId == 6) // pays avec plusieurs magasin
            {
                magId = (string)Session["magid"];
            }
            this.setDecimalFormat();

            Division = Division == "" ? "%" : Division;
            Departement = Departement == "" ? "%" : Departement;
            Classe = Classe == "" ? "%" : Classe;

            //TOREMOVE
            // magId = "MH117";
            List<string> ProduitsMagasin = DAO.Produit_MagasinDao.getSkusByMagasinIdDivision(magId, langageId, Division + Departement + Classe, datePrint, TypePrix);

            TickitDataChevalet chevaletToPrint = new TickitDataChevalet();
            chevaletToPrint.originePanier = "CHEVALET";
            chevaletToPrint.typePrix = TypePrix;
            chevaletToPrint.produitsData = new List<TickitDataProduit>();

            foreach (string pro in ProduitsMagasin)
            {
                TickitDataProduit dataProduit = Managers.TickitDataManager.getTickitDataPourChevalet(pro, langageId, datePrint, Const.ApplicationConsts.format_A5_recto_verso, null);
                chevaletToPrint.produitsData.Add(dataProduit);
            }
            string prefixFileName = "REGLETTE_NEW_";
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();
            this.setDecimalFormat();

            List<TickitDataChevalet> chevalets = PDFUtils.PlvA7RectoVerso.splitChevalet(chevaletToPrint);

            List<string> listePathPdfFiles = new List<string>();
            int iteration = 1;
            foreach (TickitDataChevalet currentChevalet in chevalets)
            {
                string htmlCode = PDFUtils.PlvA7RectoVerso.getHtmlA7(currentChevalet, Format, langageId, dateObj);
                HiqPdfManager.ConvertToPdf(PlvA7RectoVerso.getHtmlToPdfModel(), htmlCode, iteration.ToString(), prefixFileName, "");
                listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_" + iteration + ".pdf");
                iteration++;
            }

            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "A7_recto_verso.pdf");

        }


        //Cillia 
        //Impression en masse A4

        public ActionResult PrintPlvEnMasseA4(string Division, string Departement, string Classe, string Format, string rechercheDate, string TypePrix)
        {
           // string prefixFileName = "A4_NEW_";
         //   string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
          //  DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            string[] split = rechercheDate.Split(ApplicationConsts.separateurDate);
            DateTime datePrint = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));
            //DateTime datePrint = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));


            this.setLang();
            int langageId = (int)Session["langueId"];
            string magId = langageId.ToString();
            if (langageId == 4 || langageId == 5 || langageId == 6) // pays avec plusieurs magasin
            {
                magId = (string)Session["magid"];
            }
            this.setDecimalFormat();

            Division = Division == "" ? "%" : Division;
            Departement = Departement == "" ? "%" : Departement;
            Classe = Classe == "" ? "%" : Classe;

            List<string> ProduitsMagasin = DAO.Produit_MagasinDao.getSkusByMagasinIdDivision(magId, langageId, Division + Departement + Classe, datePrint, TypePrix);

            TickitDataChevalet chevaletToPrint = new TickitDataChevalet();
            chevaletToPrint.originePanier = "CHEVALET";

            string prefixFileName = "CHEVALET_A4_";
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));


           // TickitDataChevalet chevaletToPrint = (TickitDataChevalet)Session["Chevalet"];

            List<string> listePathPdfFiles = new List<string>();

            string htmlCode = PDFUtils.PlvChevaletA4Utils.getHtmlA4(chevaletToPrint, Format, langageId, dateObj);
            HiqPdfManager.ConvertToPdf(PlvChevaletA4Utils.getHtmlToPdfModel(), htmlCode, "", prefixFileName, "");
            listePathPdfFiles.Add(Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_1" + ".pdf");

            return HiqPdfManager.mergePdf(listePathPdfFiles, prefixFileName, "A4.pdf");
        }
        

        /// <summary>
        /// Renvoie prix en masse
        /// </summary>
        /// <returns>FileStreamResult</returns>
        public void PrintPrixEnMasse()
        {
            this.setLang();
            int langageId = (int)Session["langueId"];
            string responseCsv = Managers.CsvResponse.getAllPrixEditionCsvBis(langageId);

            string attachment = "attachment; filename=IMPRESSION_TOUS_PRIX.csv";
            HttpContext.Response.Clear();
            HttpContext.Response.ClearHeaders();
            HttpContext.Response.ClearContent();
            HttpContext.Response.AddHeader("content-disposition", attachment);
            HttpContext.Response.ContentType = "text/csv";
            HttpContext.Response.AddHeader("Pragma", "public");
            HttpContext.Response.ContentEncoding = Encoding.GetEncoding("Windows-1252");
            HttpContext.Response.Write(responseCsv);
        }


        //a verifier
        //---------------
        /// <summary>
        /// Menu Skus associes creer le panier des Skus associes
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MenuSkusgroupe(string Sku)
        {
            try
            {
                int langageId = (int)Session["langueId"];
                List<T_IPLV_Association> Skusgroupes = new List<T_IPLV_Association>();
                Skusgroupes = Managers.FicheProduitManager.getSkusGroupeProduitBySku(Sku, langageId);
                foreach (var item in Skusgroupes)
                {
                    T_Variation variation = Managers.FicheProduitManager.getVariationBySku(item.sku, langageId);
                    item.variation = variation.VariationName;
                }
                Session["Skusgroupes"] = Skusgroupes;
                ViewBag.Sku = Sku;
                ViewBag.skusgroupes = Skusgroupes;
                if (Skusgroupes != null){
                    if (Skusgroupes.Count == 0)
                    {
                        ViewBag.nomgroupe = Session["Skusgroupesnomambiance"];
                    }
                    else {
                        ViewBag.nomgroupe = Skusgroupes[0].nom;
                    }
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Cette methode supprime une matiere du menu Skus associes
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RemoveSkusgroupeFromMenuSkusgroupe(string Sku, string skusgroupe)
        {
            try
            {
                string[] stringSeparators = new string[] { "\n" };
                var M = skusgroupe.Split(stringSeparators, StringSplitOptions.None);
                int cpt = 0;
                T_IPLV_Association mar = new T_IPLV_Association();
                List<T_IPLV_Association> Skusgroupes = (List<T_IPLV_Association>)Session["Skusgroupes"];
                Session["Skusgroupesnomambiance"] = Skusgroupes[0].nom;
                foreach (var item in Skusgroupes)
                {
                    if (item.sku == M[0])
                    {
                        mar = item;
                        cpt++;
                    }
                }
                if (cpt != 0)
                    Skusgroupes.Remove(mar);
                ViewBag.Count = cpt;
                Session["Skusgroupes"] = Skusgroupes;
                ViewBag.skusgroupes = Skusgroupes;
                ViewBag.Sku = Sku;
                ViewBag.nomgroupe = Session["Skusgroupesnomambiance"];
                return PartialView("MenuSkusgroupe");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }


        /// <summary>
        /// Cette methode ajoute un Sku associes du menu Skusassocies
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddSkusgroupeFromMenuSkusgroupe(string Rang, string Sku, string skusgroupe)
        {
            try
            {
                int langageId = (int)Session["langueId"];
                List<T_IPLV_Association> Skusgroupes = (List<T_IPLV_Association>)Session["Skusgroupes"];
                T_IPLV_Association Skusgroupe = new T_IPLV_Association();
                List<string> limg = Managers.RechercheManager.getIMG(skusgroupe);
                Skusgroupe.limg = limg;
                if (Rang != "" && Int32.Parse(Rang) > 0)
                {
                    Skusgroupe.rang = Int32.Parse(Rang);
                }
                else
                {
                    Skusgroupe.rang = 1;
                }
                Skusgroupe.sku = skusgroupe;
                T_Variation variation = Managers.FicheProduitManager.getVariationBySku(skusgroupe, langageId);
                if (variation != null)
                {
                    Skusgroupe.variation = variation.VariationName;
                }
                else
                {
                    Skusgroupe.variation = "variation name not found";
                }
                Skusgroupes.Add(Skusgroupe);
                Session["Skusgroupes"] = Skusgroupes;
                ViewBag.skusgroupes = Skusgroupes;
                ViewBag.Sku = Sku;
                ViewBag.nomgroupe = Session["Skusgroupesnomambiance"];
                return PartialView("MenuSkusgroupe");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        
        /// <summary>
        /// Menu Skus associes up en rang un Skus associes
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult editnameSkusgroupeFromMenuSkusgroupe(string nom, string Sku)
        {
            try
            {
                int langageId = (int)Session["langueId"]; 
                List<T_IPLV_Association> Skusgroupes = (List<T_IPLV_Association>)Session["Skusgroupes"];

                if(Skusgroupes.Count == 0){
                    Session["Skusgroupesnomambiance"] = nom;
                }
                foreach (var item in Skusgroupes)
                {
                    item.nom = nom;
                }
                ViewBag.Sku = Sku;
                Session["Skusgroupes"] = Skusgroupes;
                ViewBag.skusgroupes = Skusgroupes;
                ViewBag.nomgroupe = nom;
                return PartialView("MenuSkusgroupe");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Menu Skus associes up en rang un Skus associes
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpSkusgroupeFromMenuSkusgroupe(string Skuaup, string Sku)
        {
            try
            {
                int cpt = 0;
                int langageId = (int)Session["langueId"];
                string[] stringSeparators = new string[] { "\n" };
                var M = Skuaup.Split(stringSeparators, StringSplitOptions.None);
                List<T_IPLV_Association> Skusgroupes = (List<T_IPLV_Association>)Session["Skusgroupes"];
                foreach (var item in Skusgroupes)
                {
                    if (item.sku == M[0] && cpt == 0)
                    {
                        if (item.rang > 0)
                            item.rang--;
                        cpt++;
                    }
                }
                ViewBag.Sku = Sku;
                Session["Skusgroupes"] = Skusgroupes;
                ViewBag.skusgroupes = Skusgroupes;
                ViewBag.nomgroupe = Session["Skusgroupesnomambiance"];
                return PartialView("MenuSkusgroupe");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }


        /// <summary>
        /// Menu Skus associes up en rang un Skus associes
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult downSkusgroupeFromMenuSkusgroupe(string Skuadown, string Sku)
        {
            try
            {
                int cpt = 0;
                int langageId = (int)Session["langueId"];
                string[] stringSeparators = new string[] { "\n" };
                var M = Skuadown.Split(stringSeparators, StringSplitOptions.None);
                List<T_IPLV_Association> Skusgroupes = (List<T_IPLV_Association>)Session["Skusgroupes"];
                foreach (var item in Skusgroupes)
                {
                    if (item.sku == M[0] && cpt == 0)
                    {
                        item.rang++;
                        cpt++;
                    }
                }
                ViewBag.Sku = Sku;
                Session["Skusgroupes"] = Skusgroupes;
                ViewBag.skusgroupes = Skusgroupes;
                ViewBag.nomgroupe = Session["Skusgroupesnomambiance"];
                return PartialView("MenuSkusgroupe");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        
        
        
        
        /// <summary>
        /// Menu Skus associes creer le panier des Skus associes
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MenuSkusassocie(string Sku)
        {
            try
            {
                int langageId = (int)Session["langueId"];
                List<T_IPLV_Association> Skusassocies = new List<T_IPLV_Association>();
                Skusassocies = Managers.FicheProduitManager.getSkusAssociesProduitBySku(Sku, langageId);
                foreach (var item in Skusassocies)
                {
                    T_Variation variation = Managers.FicheProduitManager.getVariationBySku(item.sku, langageId);
                    item.variation = variation.VariationName;
                }
                Session["Skusassocies"] = Skusassocies;
                ViewBag.Sku = Sku;
                ViewBag.skusassocies = Skusassocies;
                return PartialView();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Cette methode supprime une matiere du menu Skus associes
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RemoveSkusassocieFromMenuSkusassocie(string Sku, string skusassocie)
        {
            try
            {
                string[] stringSeparators = new string[] { "\n" };
                var M = skusassocie.Split(stringSeparators, StringSplitOptions.None);
                int cpt = 0;
                T_IPLV_Association mar = new T_IPLV_Association();
                List<T_IPLV_Association> Skusassocies = (List<T_IPLV_Association>)Session["Skusassocies"];
                foreach (var item in Skusassocies)
                {
                    if (item.sku == M[0])
                    {
                        mar = item;
                        cpt++;
                    }
                }
                if (cpt != 0)
                    Skusassocies.Remove(mar);
                ViewBag.Count = cpt;
                Session["Skusassocies"] = Skusassocies;
                ViewBag.Skusassocies = Skusassocies;
                ViewBag.Sku = Sku;
                return PartialView("MenuSkusassocie");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }
        

        /// <summary>
        /// Cette methode ajoute un Sku associes du menu Skusassocies
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddSkusassocieFromMenuSkusassocie(string Rang, string Sku, string skusassocie)
        {
            try
            {
                int langageId = (int)Session["langueId"];
                List<T_IPLV_Association> Skusassocies = (List<T_IPLV_Association>)Session["Skusassocies"];
                T_IPLV_Association Skusassocie = new T_IPLV_Association();
                List<string> limg = Managers.RechercheManager.getIMG(skusassocie);
                Skusassocie.limg = limg;
                if (Rang != "" && Int32.Parse(Rang)>0)
                {
                    Skusassocie.rang = Int32.Parse(Rang);
                }
                else {
                    Skusassocie.rang = 1;
                }
                Skusassocie.sku = skusassocie;
                T_Variation variation = Managers.FicheProduitManager.getVariationBySku(skusassocie, langageId);
                if (variation!= null)
                {
                    Skusassocie.variation = variation.VariationName;
                }else{
                    Skusassocie.variation = "variation name not found";
                }
                Skusassocies.Add(Skusassocie);
                Session["Skusassocies"] = Skusassocies;
                ViewBag.Skusassocies = Skusassocies;
                ViewBag.Sku = Sku;
                return PartialView("MenuSkusassocie");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        
        
        /// <summary>
        /// Menu Skus associes up en rang un Skus associes
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpSkusassocieFromMenuSkusassocie(string Skuaup, string Sku)
        {
            try
            {
                int cpt = 0;
                int langageId = (int)Session["langueId"];
                string[] stringSeparators = new string[] { "\n" };
                var M = Skuaup.Split(stringSeparators, StringSplitOptions.None);
                List<T_IPLV_Association> Skusassocies = (List<T_IPLV_Association>)Session["Skusassocies"];
                foreach (var item in Skusassocies) {
                    if (item.sku == M[0] && cpt == 0)
                    {
                        if (item.rang > 0)
                            item.rang--;
                        cpt++;
                    }
                }
                ViewBag.Sku = Sku;
                Session["Skusassocies"] = Skusassocies;
                ViewBag.skusassocies = Skusassocies;
                return PartialView("MenuSkusassocie");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }


        /// <summary>
        /// Menu Skus associes up en rang un Skus associes
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult downSkusassocieFromMenuSkusassocie(string Skuadown, string Sku)
        {
            try
            {
                int cpt = 0;
                int langageId = (int)Session["langueId"];
                string[] stringSeparators = new string[] { "\n" };
                var M = Skuadown.Split(stringSeparators, StringSplitOptions.None);
                List<T_IPLV_Association> Skusassocies = (List<T_IPLV_Association>)Session["Skusassocies"];
                foreach (var item in Skusassocies)
                {
                    if (item.sku == M[0] && cpt == 0)
                    {
                        item.rang++;
                        cpt++;
                    }
                }
                ViewBag.Sku = Sku;
                Session["Skusassocies"] = Skusassocies;
                ViewBag.skusassocies = Skusassocies;
                return PartialView("MenuSkusassocie");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Menu Couleur creer le panier des matieres
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MenuCouleur(string Sku)
        {
            try
            {
                int langageId = (int)Session["langueId"];
                List<string> Couleurs = new List<string>();

                T_IPLV_Details IPLVD = Managers.FicheProduitManager.getIPLVDetailsProduitBySku(Sku, langageId);
                if (IPLVD.couleurs != null)
                {
                    foreach (var item in IPLVD.couleurs)
                    {
                        Couleurs.Add(item);
                    }
                }
                Session["Couleurs"] = Couleurs;
                ViewBag.Sku = Sku;
                ViewBag.Couleurs = Couleurs;
                return PartialView();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Cette methode supprime une matiere du menu Couleur
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RemoveCouleurFromMenuCouleur(string Sku, string couleur)
        {
            try
            {
                string[] stringSeparators = new string[] { "\n" };
                var M = couleur.Split(stringSeparators, StringSplitOptions.None);
                int cpt = 0;
                string mar = "";
                List<string> Couleurs = (List<string>)Session["Couleurs"];
                foreach (var item in Couleurs)
                {
                    if (item == M[0])
                    {
                        mar = item;
                        cpt++;
                    }
                }
                if (cpt != 0)
                    Couleurs.Remove(mar);
                ViewBag.Count = cpt;
                Session["Couleurs"] = Couleurs;
                ViewBag.Couleurs = Couleurs;
                ViewBag.Sku = Sku;
                return PartialView("MenuCouleur");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }


        /// <summary>
        /// Cette methode supprime une matiere du menu Couleur
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddCouleurFromMenuCouleur(string Sku, string couleur)
        {
            try
            {
                List<string> Couleurs = (List<string>)Session["Couleurs"];
                Couleurs.Add(couleur);
                Session["Couleurs"] = Couleurs;
                ViewBag.Couleurs = Couleurs;
                ViewBag.Sku = Sku;
                return PartialView("MenuCouleur");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }




        /// <summary>
        /// Menu Matiere creer le panier des matieres
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MenuMatiere(string Sku)
        {
            try
            {
                int langageId = (int)Session["langueId"];
                List<string> Matieres = new List<string>();

                T_IPLV_Details IPLVD = Managers.FicheProduitManager.getIPLVDetailsProduitBySku(Sku, langageId);
                if (IPLVD.matieres != null)
                {
                    foreach (var item in IPLVD.matieres)
                    {
                        Matieres.Add(item);
                    }
                }
                Session["Matieres"] = Matieres;
                ViewBag.Sku = Sku;
                ViewBag.Matieres = Matieres;
                return PartialView();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

             /// <summary>
        /// Cette methode supprime une matiere du menu matiere
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RemoveMatiereFromMenuMatiere(string Sku, string matiere)
        {
            try
            {
                string[] stringSeparators = new string[] { "\n" };
                var M = matiere.Split(stringSeparators, StringSplitOptions.None);
                int cpt = 0;
                string mar = "";
                List<string> Matieres = (List<string>)Session["Matieres"];
                foreach (var item in Matieres) {
                    if (item == M[0]) {
                        mar = item;
                        cpt++;
                    }
                }
                if(cpt!=0)
                    Matieres.Remove(mar);
                ViewBag.Count = cpt;
                Session["Matieres"] = Matieres;
                ViewBag.Matieres = Matieres;
                ViewBag.Sku = Sku;
                return PartialView("MenuMatiere");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }


        /// <summary>
        /// Cette methode supprime une matiere du menu matiere
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddMatiereFromMenuMatiere(string Sku, string matiere)
        {
            try
            {
                List<string> Matieres = (List<string>)Session["Matieres"];
                Matieres.Add(matiere);
                Session["Matieres"] = Matieres;
                ViewBag.Matieres = Matieres;
                ViewBag.Sku = Sku;
                return PartialView("MenuMatiere");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        //---------------


        /// <summary>
        /// Renvoie la page panier d'impression luminaire.
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Luminaire(string Sku, string dateQuery)
        {
            try
            {
                string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
                DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

                this.setLang();
                int langageId = (int)Session["langueId"];
                this.setDecimalFormat();

                TickitDataLuminaire luminaire = (TickitDataLuminaire)Session["Luminaire"];

                // remédier au risque de perte de variables de session.. solution provisoire.
                if (luminaire == null)
                {
                    luminaire = new TickitDataLuminaire();
                    Session["Luminaire"] = luminaire;
                }

                if ((luminaire.produitDessous != null && luminaire.produitDessous.sku == Sku) || (luminaire.produitDessus != null && luminaire.produitDessus.sku == Sku))
                {
                    ViewBag.ProduitDejaExistant = true;
                }
                else
                {
                    TickitDataProduit dataProduit = Managers.TickitDataManager.getTickitDataPourChevalet(Sku, langageId, dateObj, null, null);
                    if (luminaire.produitDessous == null)
                    {
                        luminaire.produitDessous = dataProduit;
                    }

                    else if ((luminaire.produitDessous != null) && (luminaire.produitDessus == null))
                    {
                        luminaire.produitDessus = dataProduit;
                    }

                    else if ((luminaire.produitDessous != null) && (luminaire.produitDessus != null))
                    {
                        ViewBag.LuminaireCapaciteAtteinte = true;
                    }
                }

                // nombre de produits dans le luminaire.
                int count = 0;
                if (luminaire.produitDessous != null)
                {
                    count++;
                }
                if (luminaire.produitDessus != null)
                {
                    count++;
                }

                ViewBag.Luminaire = luminaire;
                ViewBag.Count = count;
                ViewBag.dateQuery = dateQuery;

                return PartialView();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Cette methode supprime un produit du luminaire
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RemoveProduitFromLuminaire(string Sku)
        {
            try
            {
                this.setLang();
                TickitDataLuminaire luminaire = (TickitDataLuminaire)Session["Luminaire"];

                if (luminaire.produitDessous != null && luminaire.produitDessous.sku == Sku)
                {
                    luminaire.produitDessous = null;
                }

                if (luminaire.produitDessus != null && luminaire.produitDessus.sku == Sku)
                {
                    luminaire.produitDessus = null;
                }

                int count = 0;

                if (luminaire.produitDessous != null)
                {
                    count++;
                }
                if (luminaire.produitDessus != null)
                {
                    count++;
                }

                ViewBag.Count = count;
                ViewBag.Luminaire = luminaire;
                return PartialView("luminaire");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Cette methode vide le Luminaire.
        /// </summary>
        /// <param name="Sku"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ViderLuminaire()
        {
            try
            {
                this.setLang();
                List<TickitDataProduit> nouvelleListeProduitsData = new List<TickitDataProduit>();

                TickitDataLuminaire luminaire = new TickitDataLuminaire();
                Session["Luminaire"] = luminaire;
                ViewBag.Luminaire = luminaire;

                return PartialView("Luminaire");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Renvoie la PLV Luminaire
        /// </summary>
        /// <returns></returns>
        public FileStreamResult PrintluminairePdf(string format, string dateQuery)
        {
            string[] split = dateQuery.Split(ApplicationConsts.separateurDate);
            DateTime dateObj = new DateTime(int.Parse(split[2]), int.Parse(split[1]), int.Parse(split[0]));

            this.setLang();

            int langageId = (int)Session["langueId"];
            this.setDecimalFormat();

            TickitDataLuminaire luminaire = (TickitDataLuminaire)Session["Luminaire"];

            MemoryStream MS = PDFUtils.PlvLuminaireUtils.GenerateLuminairePdf(luminaire, format, langageId, dateObj);

            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=dessus.pdf");
            FileStreamResult file = File(MS, "application/pdf");

            return file;
        }

        /// <summary>
        /// Renvoie la courbe de l'évolution des prixCal d'un produit
        /// 18/07/2016
        /// <param name="Sku">Réference du produit</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvolutionGraphiquePrix(string Sku)
        {
            try
            {
                int langageId = (int)Session["langueId"];
                string codeMonnaie = DAO.LangueDao.getCodeMonnaieByMagasinId(langageId);
                List<Models.T_Prix> orderedDates = DAO.PrixDao.getAllPrix(Sku, langageId);

//construction liste "dates" de T_Prix_evol (liste de toutes les dates qui seront present sur le graph)
                List<T_Prix_evol> dates = new List<T_Prix_evol>();
                foreach (Models.T_Prix item in orderedDates)
                {
                    //date de debut d'un prix
                    T_Prix_evol date = new T_Prix_evol();
                    date.Date = item.Date_debut;
                    date.Prix_produit = item.Prix_produit;
                    date.Type_promo = item.Type_promo;
                    dates.Add(date);
                    //jour avant
                    T_Prix_evol date3 = new T_Prix_evol();
                    date3.Prix_produit = -1;//prix indeterminé pour instant
                    date3.Date = item.Date_debut.AddDays(-1);//jour de debut -1
                    dates.Add(date3);
                    //date de fin d'un prix
                    T_Prix_evol date2 = new T_Prix_evol();
                    date2.Date = item.Date_fin;
                    date2.Prix_produit = item.Prix_produit;
                    date2.Type_promo = item.Type_promo;
                    dates.Add(date2);
                    //jour apres
                    T_Prix_evol date4 = new T_Prix_evol();
                    date4.Prix_produit = -1;//prix indeterminé pour instant
                    date4.Date = item.Date_fin.AddDays(1);//jour de fin +1
                    dates.Add(date4);
                }
//Rectification des prix
                    //gestion promo multiple qui se chevauche (on prend le prix le plus bas)
                    //assignation des prix indeterminés aux dates qui precedent et qui suivent une periode
                foreach (Models.T_Prix_evol item in dates)
                {
                    foreach (Models.T_Prix item2 in orderedDates)
                    {
                        if ((item2.Prix_produit <= item.Prix_produit || item.Prix_produit == -1) && item2.Date_fin >= item.Date && item2.Date_debut <= item.Date){
                            item.Prix_produit = item2.Prix_produit;
                            item.Type_promo = item2.Type_promo;
                        }
                    }
                }
//tri des dates par ordre chronologique
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

// suppression des dates qui restent avec des prix indeterminés
                dates.Remove(dates[0]);//aucun prix vu qu il se trouve en dehors du plus grand interval (avant)
                dates.Remove(dates[dates.Count - 1]);//aucun prix vu qu il se trouve en dehors du plus grand interval (apres)
                foreach (Models.T_Prix_evol item in dates)
                {
                    if (item.Prix_produit == -1)//si prix indeterminés
                    {
                        dates.Remove(item);//supression de l'élément
                    }
                }

// suppression des doublons (si des bornes d'interval sont identiques)
                for (I = 0; I < dates.Count - 1; I++)
                {
                    if (dates[I].Date == dates[I+1].Date)// si date identique
                    {
                        dates.Remove(dates[I]);
                        I--;//pour louper aucun elt
                    }
                }

//return + viewbag
                ViewBag.dates = dates;
                ViewBag.codeMonnaie = codeMonnaie;
                return PartialView();
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Retourne la description du Range
        /// </summary>
        /// <param name="rechercheText"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RangeDetails(String rechercheText)
        {
            try
            {
                this.setLang();
                CultureInfo ci = Thread.CurrentThread.CurrentCulture;
                int langageId = (int)Session["langueId"];

                if (ci.ToString().Length >= 2)
                {
                    ViewBag.lang = ci.ToString().Substring(0, 2);
                }
                else
                {
                    ViewBag.lang = ci.ToString();
                }

                List<T_Range> listeRange = DAO.RangeDao.getRangeByName(rechercheText);

                if (listeRange.Count == 0)
                {
                    ViewBag.error = "Le Range spécifié n'existe pas dans le référentiel";
                    return PartialView("Errors");
                }
                else
                {
                    string plusRange = DAO.RangeDao.getDescriptionPlusByRangeName(rechercheText, langageId, "");
                    ViewBag.plusRange = plusRange;
                    ViewBag.rangeName = listeRange[0].RangeName;
                    return PartialView("RangeDetails");
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        /// <summary>
        /// Met à jour le produit grâce à un formulaire passé en paramètre
        /// </summary>
        /// <param name="form"></param>
        public ActionResult SaveRange(Form.EditRangeForm form)
        {
            try
            {
                this.setLang();
                int langageId = (int)Session["langueId"];

                DAO.RangeDao.insertOrUpdatePlusRange(form.rangeName, form.plusRange, langageId, "");
                ViewBag.textInfo = TickitNewFace.Resources.Langue.DescriptionRange_SaveSuccess;

                return PartialView("Success");
            }
            catch (Exception ex)
            {
                ViewBag.error = "[ERROR " + ex.Source + "]" + " : " + ex.Message;
                return PartialView("Errors");
            }
        }

        ////////////////////////////////////////////////////////// méthodes privées /////////////////////////////////////////////////////

        /// <summary>
        /// Cette methode permet de fixer la langue pour le Thread courrant.
        /// </summary>
        private void setLang(Boolean force = false)
        {
            if (Session["lang"] != null)
            {
                if (null == Session["langue"] || force)
                    Session["langue"] = DAO.LangueDao.getLangageByCode((string)Session["lang"]);

                if (null == Session["langueId"] || force)
                    Session["langueId"] = DAO.LangueDao.getLangageIdByCode((string)Session["lang"]);
                
                CultureInfo ci = new CultureInfo((string)Session["langue"], false);
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }
            else
            {
                if (null != (string)Session["franchiseName"] && (string)Session["franchiseName"] != ApplicationConsts.nonFranchise)
                {
                    string bla = (string)Session["franchiseName"];

                    Session["langue"] = DAO.LangueDao.getLangageByCode((string)Session["franchiseName"]);
                    Session["langueId"] = DAO.LangueDao.getLangageIdByCode((string)Session["franchiseName"]);
                    
                    CultureInfo ci = new CultureInfo((string)Session["langue"], false);
                    Thread.CurrentThread.CurrentCulture = ci;
                    Thread.CurrentThread.CurrentUICulture = ci;
                }
                else
                {
                    Session["langue"] = DAO.LangueDao.getLangageByCode("fr");
                    Session["langueId"] = DAO.LangueDao.getLangageIdByCode("fr");

                    CultureInfo ci = new CultureInfo((string)Session["langue"], false);
                    Thread.CurrentThread.CurrentCulture = ci;
                    Thread.CurrentThread.CurrentUICulture = ci;
                }
            }
        }

        /// <summary>
        /// force le format des décimaux à "."
        /// </summary>
        private void setDecimalFormat()
        {
            CultureInfo tmp_ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();

            tmp_ci.NumberFormat.NumberDecimalSeparator = ApplicationConsts.separateurDecimal;
            tmp_ci.NumberFormat.NumberGroupSeparator = " ";

            System.Threading.Thread.CurrentThread.CurrentCulture = tmp_ci;
            string separateur = tmp_ci.NumberFormat.NumberDecimalSeparator;
        }

    }
}