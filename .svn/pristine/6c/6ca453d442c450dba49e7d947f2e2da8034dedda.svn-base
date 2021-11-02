using System;
using System.Collections.Generic;
using TickitNewFace.DAO;
using TickitNewFace.Models;
using TickitNewFace.Form;
using System.Web;

namespace TickitNewFace.Managers
{
    /// <summary>
    /// Classe Manager de gestion d'une fiche produit.
    /// </summary>
    public static class FicheProduitManager
    {

        public static string getDrescMaketingProduitBySku(string sku, int langageId)
        {
            return ProduitDao.getDrescMaketingProduitBySku(sku, langageId);
        }

        public static string getDesignerProduitBySku(string sku, int langageId)
        {
            return ProduitDao.getDesignerProduitBySku(sku, langageId);
        }

        public static T_IPLV_Details getIPLVDetailsProduitBySku(string sku, int langageId)
        {
            return ProduitDao.getIPLVDetailsProduitBySku(sku, langageId);
        }
        
        public static List<T_IPLV_Association> getSkusAssociesProduitBySku(string sku, int langageId)
        {
            return ProduitDao.getSkusAssociesProduitBySku(sku, langageId);
        }

        public static List<T_IPLV_Association> getSkusGroupeProduitBySku(string sku, int langageId)
        {
            return ProduitDao.getSkusGroupeProduitBySku(sku, langageId);
        }

        /// <summary>
        /// retourne un produit à partir de son identifiant unique
        /// </summary>
        /// <param name="sku">ID du produit</param>
        /// <returns></returns>
        public static T_Produit getProduitBySku(string sku, int langageId)
        {
            return ProduitDao.getProduitBySku(sku, langageId);
        }

        /// <summary>
        /// Retourne les différents formats d'impression d'un produit.
        /// </summary>
        /// <param name="sku">ID du produit</param>
        /// <returns></returns>
        public static List<string> getFormatsImpressionPorduit(string sku)
        {
            return ProduitDao.getFormatsImpressionBySku(sku);
        }

        /// <summary>
        /// Retourne la variation d'un produit en tenant compte de la langue
        /// </summary>
        /// <param name="sku">ID du produit</param>
        /// <param name="langageId">ID de la langue</param>
        /// <returns></returns>
        public static T_Variation getVariationBySku(string sku, int langageId)
        {
            return VariationDao.getVariationBySku(sku, langageId);
        }

        /// <summary>
        /// retourne le prixCal de référence (permanent) d'un produit précedant le prixCal passé en paramètre.
        /// </summary>
        /// <param name="prixNew"></param>
        /// <returns></returns>
        public static T_Prix getPrixPermanentPrecedent(T_Prix prix)
        {
            return PrixDao.getPrixPermanentPrecedent(prix);
        }

        /// <summary>
        /// retourne le prixCal d'un produit à une date T.
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="langageId"></param>
        /// <returns></returns>
        public static T_Prix getPrixBySkuAndDate(string sku, int langageId, DateTime date)
        {
            return PrixDao.getPrixBySkuAndDate(sku, langageId, date);
        }

        /// <summary>
        /// Retourne la description légale d'un produit
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="langageId"></param>
        /// <returns></returns>
        public static T_Description_Dgccrf getDgccrfBySku(string sku, int langageId)
        {
            return Description_DgccrfDao.getDgccrfBySku(sku, langageId);
        }

        /// <summary>
        /// Retourne les listePlus d'un produit
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="langageId"></param>
        /// <returns></returns>
        public static List<T_Description_Plus> getPlusBySku(string sku, int langageId)
        {
            return Description_PlusDao.getListePlusBySku(sku, langageId);
        }

        
 /// <summary>
        /// MAJ du produit et de la description légale qui lui est rattachée.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="langageId"></param>
        public static int updateProduitIPLV(List<T_IPLV_Association> Skusgroupes, List<T_IPLV_Association> Skusassocies, List<string> Couleurs, List<string> Matieres, EditProduitForm form, int langageId)
        {
            int modif = 0;
            if (form.Sku == null) {
                return 1;
            }
            string Sku = form.Sku;

            //recuperation des infos
            String DrescMaketing = Managers.FicheProduitManager.getDrescMaketingProduitBySku(Sku, langageId);
            T_IPLV_Details IPLVD = Managers.FicheProduitManager.getIPLVDetailsProduitBySku(Sku, langageId);

            List<T_IPLV_Association> SkusAssocies = Managers.FicheProduitManager.getSkusAssociesProduitBySku(Sku, langageId);
            List<T_IPLV_Association> SkusGroupes = Managers.FicheProduitManager.getSkusGroupeProduitBySku(Sku, langageId);
            String Designer = Managers.FicheProduitManager.getDesignerProduitBySku(Sku, langageId);

            // Produit à mettre à jour.
            T_Produit produitToUpdate = DAO.ProduitDao.getProduitBySku(form.Sku, langageId);

            //designed_habitat
            if (IPLVD.designed_habitat == null) { 
                //creation de la ligne si new
                DAO.ProduitDao.insertPorduitDH(form.designed_habitat, form.Sku, langageId); //maj
                modif = 1;
            }
            else
            {
                //comparaison
                if (IPLVD.designed_habitat != form.designed_habitat)
                {
                    DAO.ProduitDao.updatePorduitDH(form.designed_habitat, form.Sku, langageId); //maj
                    modif = 1;
                }
            }     
            //Dmark
            if (DrescMaketing == null)
            {
                //creation de la ligne si new
               DAO.ProduitDao.insertPorduitDmark(form.Dmark, form.Sku, langageId); //maj
                modif = 1;
            }
            else
            {
                //comparaison
                if (DrescMaketing != form.Dmark)
                {
                    DAO.ProduitDao.updatePorduitDmark(form.Dmark, form.Sku, langageId); //maj
                    modif = 1;
                }
            }
            //Designer
            if (Designer == null)
            {
                //creation de la ligne si new
                DAO.ProduitDao.insertPorduitDesigner(form.Designer, form.Sku, langageId);
                modif = 1;
            }
            else
            {
                //comparaison
                if (Designer != form.Designer)
                {
                    if (form.Designer != null)
                    {
                        DAO.ProduitDao.updatePorduitDesigner(form.Designer, form.Sku, langageId); //maj
                        modif = 1;
                    }
                }
            }
            //dimprod
                //comparaison
            if (IPLVD.dimension_produit != form.dimprod)
            {
                DAO.ProduitDao.updatePorduitdimprod(form.dimprod, form.Sku, langageId); //maj
                modif = 1;
            }

            //dimcolis
                //comparaison
            if (IPLVD.dimension_colis != form.dimcolis)
            {
                DAO.ProduitDao.updatePorduitdimcolis(form.dimcolis, form.Sku, langageId); //maj
                modif = 1;
            }

            //Matieres
            if (IPLVD.matieres != null)
            {
                int cptid;
                int cptdif = 0;
                foreach (var item in Matieres)
                {
                    cptid = 0;
                    foreach (var item2 in IPLVD.matieres)
                    {
                        if (item == item2)// matiere trouvé?
                        {
                            cptid++;
                        }
                    }
                    if (cptid == 0)
                    { //la matiere a t elle est retrouvé
                        cptdif++;
                    }

                }
                if (cptdif != 0 || (int)IPLVD.matieres.Length != (int)Matieres.Count)
                {
                    //une matiere est differente (ajout ou supression d une matiere)
                    DAO.ProduitDao.updatePorduitMatieres(Matieres, form.Sku, langageId); //maj
                    modif = 1;
                }
            }
            else {
                if (null != Matieres)
                {
                    if (0 != (int)Matieres.Count)
                    {
                        //une matiere est differente (ajout ou supression d une matiere)
                        DAO.ProduitDao.updatePorduitMatieres(Matieres, form.Sku, langageId); //maj
                        modif = 1;
                    }
                }
            }

            //couleurs
            if (IPLVD.matieres != null)
            {
                int cptid2;
                int cptdif2 = 0;
                foreach (var item in Couleurs)
                {
                    cptid2 = 0;
                    foreach (var item2 in IPLVD.couleurs)
                    {
                        if (item == item2)// couleur trouvé?
                        {
                            cptid2++;
                        }
                    }
                    if (cptid2 == 0)
                    { //la couleur a t elle est retrouvé
                        cptdif2++;
                    }

                }
                if (cptdif2 != 0 || (int)IPLVD.couleurs.Length != (int)Couleurs.Count)
                {
                    //une couleur est differente (ajout ou supression d une matiere)
                    DAO.ProduitDao.updatePorduitcouleurs(Couleurs, form.Sku, langageId); //maj
                    modif = 1;
                }
            }
            else {
                if (Couleurs != null)
                {
                    if (0 != (int)Couleurs.Count)
                    {
                        //une couleur est differente (ajout ou supression d une matiere)
                        DAO.ProduitDao.updatePorduitcouleurs(Couleurs, form.Sku, langageId); //maj
                        modif = 1;
                    }
                }
            }
            int modif2 = 0;
            //Skusassocies
            if (SkusAssocies != null)
            {
                int cptid2;
                int cptdif2 = 0;
                foreach (var item in Skusassocies)
                {
                    cptid2 = 0;
                    foreach (var item2 in SkusAssocies)
                    {
                        if (item.sku == item2.sku && item.rang == item2.rang)// Skusassocies trouvé?
                        {
                            cptid2++;
                        }
                    }
                    if (cptid2 == 0)
                    { //le Sku associe a t il ete retrouvé
                        cptdif2++;
                    }

                }
                if (cptdif2 != 0 || (int)SkusAssocies.Count != (int)Skusassocies.Count)
                {
                    //un skusAssocies est different (ajout ou supression d un skusAssocies) //maj
                    DAO.ProduitDao.deletePorduitskusassocies( form.Sku, langageId);
                    foreach(var item in Skusassocies){
                        DAO.ProduitDao.insertPorduitskusassocies(item, form.Sku, langageId);
                    }

  
                    modif2 = 2;
                }
            }
            else
            {
                if (Skusassocies != null)
                {
                    if (0 != (int)Skusassocies.Count)
                    {
                        //un skusAssocies est different (ajout ou supression d un skusAssocies) //maj
                        DAO.ProduitDao.deletePorduitskusassocies(form.Sku, langageId);
                        foreach (var item in Skusassocies)
                        {
                            DAO.ProduitDao.insertPorduitskusassocies(item, form.Sku, langageId);
                        }
                        modif2 = 2;
                    }
                }
            }

            int modif3 = 0;
            //SkusGroupes
            if (SkusGroupes != null)
            {
                int cptid2;
                int cptdif2 = 0;
                foreach (var item in Skusgroupes)
                {
                    cptid2 = 0;
                    foreach (var item2 in SkusGroupes)
                    {
                        if (item.sku == item2.sku && item.rang == item2.rang && item.nom == item2.nom)// Skusassocies trouvé?
                        {
                            cptid2++;
                        }
                    }
                    if (cptid2 == 0)
                    { //le Sku associe a t il ete retrouvé
                        cptdif2++;
                    }

                }
                if (cptdif2 != 0 || (int)SkusGroupes.Count != (int)Skusgroupes.Count)
                {
                    //un skusAssocies est different (ajout ou supression d un skusAssocies) //maj
                    if (SkusGroupes != null)
                    {
                        if ((int)SkusGroupes.Count != 0)
                        {
                            DAO.ProduitDao.deletePorduitskusgroupes(SkusGroupes[0].nom, form.Sku, langageId);
                        }
                    }
                    foreach (var item in Skusgroupes)
                    {
                        DAO.ProduitDao.insertPorduitskusgroupes(item, form.Sku, langageId);
                    }
                    modif3 = 3;
                }
            }
            else
            {
                if (0 != (int)Skusgroupes.Count)
                {
                    //un skusAssocies est different (ajout ou supression d un skusAssocies) //maj
                    if (SkusGroupes != null)
                    {
                        if((int)SkusGroupes.Count != 0){
                            DAO.ProduitDao.deletePorduitskusgroupes(SkusGroupes[0].nom, form.Sku, langageId);
                        }
                    }
                    foreach (var item in Skusgroupes)
                    {
                        DAO.ProduitDao.insertPorduitskusgroupes(item, form.Sku, langageId);
                    }
                    modif3 = 3;
                }
            }




            //date pour update
            if (modif == 1)
            {
                string d = DAO.ProduitDao.selectPorduitIPLVupdate("DETAILS_PRODUITS", Sku);
                if (d != null){
                    DAO.ProduitDao.updatePorduitIPLV("DETAILS_PRODUITS", Sku);
                }else {
                    DAO.ProduitDao.insertPorduitIPLV("DETAILS_PRODUITS", Sku);
                }
            }
            if (modif2 == 2)
            {
                string d = DAO.ProduitDao.selectPorduitIPLVupdate("PRODUITS_ASSOCIES", Sku);
                if (d != null)
                {
                    DAO.ProduitDao.updatePorduitIPLV("PRODUITS_ASSOCIES", Sku);
                }
                else
                {
                    DAO.ProduitDao.insertPorduitIPLV("PRODUITS_ASSOCIES", Sku);
                }
            }
            if (modif3 == 3)
            {
                string d = DAO.ProduitDao.selectPorduitIPLVupdate("PRODUITS_AMBIANCE", Sku);
                if (d != null)
                {
                    DAO.ProduitDao.updatePorduitIPLV("PRODUITS_AMBIANCE", Sku);
                }
                else
                {
                    DAO.ProduitDao.insertPorduitIPLV("PRODUITS_AMBIANCE", Sku);
                }
            }
            // Hsitorisation de l'évenement connexion.
            T_Evenement objEve = new T_Evenement();
            objEve.Dateve = DateTime.Now;
            objEve.Eve = "INSERT - MAJ IPLV";
            objEve.Login = (string)HttpContext.Current.Session["userName"];
            EvenementDao.insertEvenement(objEve);
            return 0;
        }

        /// <summary>
        /// MAJ du produit et de la description légale qui lui est rattachée.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="langageId"></param>
        public static int updateProduit(int modif, EditProduitForm form, int langageId)
        {
            // Produit à mettre à jour.
            T_Produit produitToUpdate = DAO.ProduitDao.getProduitBySku(form.Sku, langageId);

            produitToUpdate.Hauteur = null;
            if (form.Height != null)
            {
                form.Height = form.Height.Replace(",", ".");//securité
                produitToUpdate.Hauteur = decimal.Parse(form.Height);
            }

            produitToUpdate.Longueur = null;
            if (form.Length != null)
            {
                form.Length = form.Length.Replace(",", ".");//securité
                produitToUpdate.Longueur = decimal.Parse(form.Length);
            }

            produitToUpdate.Largeur = null;
            if (form.Width != null)
            {
                form.Width = form.Width.Replace(",", ".");//securité
                produitToUpdate.Largeur = decimal.Parse(form.Width);
            }


            produitToUpdate.Profondeur = null;
            if (form.Profondeur != null)
            {
                form.Profondeur = form.Profondeur.Replace(",", ".");//securité
                produitToUpdate.Profondeur = decimal.Parse(form.Profondeur);
            }

            produitToUpdate.Diametre = null;
            if (form.Diametre != null)
            {
                form.Diametre = form.Diametre.Replace(",", ".");//securité
                produitToUpdate.Diametre = decimal.Parse(form.Diametre);
            }


            produitToUpdate.Volume = null;
            if (form.Volume != null)
            {
                form.Volume = form.Volume.Replace(",", ".");//securité
                produitToUpdate.Volume = decimal.Parse(form.Volume);
            }

            produitToUpdate.Self_Assembly = null;
            if (form.Montage != null)
            {
                produitToUpdate.Self_Assembly = form.Montage;
            }

            // MAJ du produit
            DAO.ProduitDao.updatePorduit(produitToUpdate);

            // DGCCRF à mettre à jour
            T_Description_Dgccrf dgccrf = DAO.Description_DgccrfDao.getDgccrfBySku(form.Sku, langageId);

            // Si il n'existe pas de DGCCRF associée à ce produit, en créer une.
            if (dgccrf == null)
            {
                if (form.Dgccrf != "" && form.Dgccrf != null)
                {
                    T_Description_Dgccrf newDgccrf = new T_Description_Dgccrf();
                    newDgccrf.Sku = form.Sku;
                    newDgccrf.LangageId = langageId;
                    newDgccrf.LegalDescription = form.Dgccrf;
                    DAO.Description_DgccrfDao.insertDgccrf(newDgccrf);
                }
            }
            else
            {
                dgccrf.LegalDescription = form.Dgccrf;
                DAO.Description_DgccrfDao.updateDgccrf(dgccrf);
            }


            // Variation à mettre à jour
            T_Variation variation = DAO.VariationDao.getVariationBySku(form.Sku, langageId);

            // Si il n'existe pas de DGCCRF associée à ce produit, en créer une.
            if (variation == null)
            {
                if (form.Variation != "" && form.Variation != null)
                {
                    T_Variation newVariation = new T_Variation();
                    newVariation.Sku = form.Sku;
                    newVariation.LangageId = langageId;
                    newVariation.VariationName = form.Variation;
                    DAO.VariationDao.insertVariation(newVariation);
                }
            }
            else
            {
                variation.VariationName = form.Variation;
                DAO.VariationDao.updateVariation(variation);
            }

            // Suppression des plus avant traitement.
            DAO.Description_PlusDao.supprimerPlusByLang(form.Sku, langageId, 1);
            DAO.Description_PlusDao.supprimerPlusByLang(form.Sku, langageId, 2);
            DAO.Description_PlusDao.supprimerPlusByLang(form.Sku, langageId, 3);

            // les plus
            int position = 1;

            // Update des nouveau.
            if (form.Plus1 != null && (form.Plus1.Length < 50))
            {   
                T_Description_Plus plus1 = new T_Description_Plus();
                plus1.Plus = form.Plus1;
                plus1.LangageId = langageId;
                plus1.Sku = form.Sku;
                plus1.Position = position;
                DAO.Description_PlusDao.insertPlus(plus1);
                position++;
            }
            if (form.Plus2 != null && (form.Plus2.Length < 50))
            {
                T_Description_Plus plus2 = new T_Description_Plus();
                plus2.Plus = form.Plus2;
                plus2.LangageId = langageId;
                plus2.Sku = form.Sku;
                plus2.Position = position;
                DAO.Description_PlusDao.insertPlus(plus2);
                position++;
            }
            if (form.Plus3 != null && (form.Plus3.Length < 50))
            {
                T_Description_Plus plus3 = new T_Description_Plus();
                plus3.Plus = form.Plus3;
                plus3.LangageId = langageId;
                plus3.Sku = form.Sku;
                plus3.Position = position;
                DAO.Description_PlusDao.insertPlus(plus3);
                position++;
            }

            //les formats
            //supression des plus existant
            DAO.ProduitDao.supprimertoutFormatImpressionBySku(form.Sku);
            //update des nouveau
            if (form.formatA5RV != null)
            {
                DAO.ProduitDao.ajoutFormatImpressionBySku(form.Sku, form.formatA5RV);
            }
            if (form.formatA5S != null)
            {
                DAO.ProduitDao.ajoutFormatImpressionBySku(form.Sku, form.formatA5S);
            }

            if (form.formatA6RV != null)
            {
                DAO.ProduitDao.ajoutFormatImpressionBySku(form.Sku, form.formatA6RV);
            }
            if (form.formatA6S != null)
            {
                DAO.ProduitDao.ajoutFormatImpressionBySku(form.Sku, form.formatA6S);
            }

            if (form.formatA7RV != null)
            {
                DAO.ProduitDao.ajoutFormatImpressionBySku(form.Sku, form.formatA7RV);
            }
            if (form.formatA7S != null)
            {
                DAO.ProduitDao.ajoutFormatImpressionBySku(form.Sku, form.formatA7S);
            }

            if (modif == 1)
            {
                string d = DAO.ProduitDao.selectPorduitIPLVupdate("DETAILS_PRODUITS", form.Sku);
                if (d != null)
                {
                    DAO.ProduitDao.updatePorduitIPLV("DETAILS_PRODUITS", form.Sku);
                }
                else
                {
                    DAO.ProduitDao.insertPorduitIPLV("DETAILS_PRODUITS", form.Sku);
                }
            }

            // Hsitorisation de l'évenement connexion.
            T_Evenement objEve = new T_Evenement();
            objEve.Dateve = DateTime.Now;
            objEve.Eve = "INSERT - MAJ du produit et de la description légale qui lui est rattachée";
            objEve.Login = (string)HttpContext.Current.Session["userName"];
            EvenementDao.insertEvenement(objEve);
            return 0;
        }
    }
}
