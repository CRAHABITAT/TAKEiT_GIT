using System;
using System.Collections.Generic;
using TickitNewFace.Models;
using TickitNewFace.DAO;
using System.IO;
using TickitNewFace.Const;
using TickitNewFace.Utils;
using System.Globalization;

namespace TickitNewFace.Managers
{
    /// <summary>
    /// Classe Manager retourne les informations imprimables sur un ticket.
    /// </summary>
    public static class TickitDataManager
    {
        /// <summary>
        /// Retourne un Memory stream selon le type de PLV choisi
        /// </summary>
        /// <param name="ticketToSend"></param>
        /// <param name="format"></param>
        /// <returns>ms</returns>
        public static MemoryStream getTickitTypeStream(List<TickitDataProduit> listeTicketsToSend, string format, int magasinId)
        {
            MemoryStream ms = new MemoryStream();
            // Cas d'une PLV soldes / promotion...
            if (listeTicketsToSend[0].isPromoSoldes)
            {
                if (format == ApplicationConsts.format_A5_recto_verso || format == ApplicationConsts.format_A6_recto_verso || format == ApplicationConsts.format_A7_recto_verso)
                {
                    ms = PDFUtils.PlvSoldesUtils.GenerateDocumentPlvRectoVerso(listeTicketsToSend, format, magasinId);
                }
                else if (format == ApplicationConsts.format_A5_simple || format == ApplicationConsts.format_A6_simple || format == ApplicationConsts.format_A7_simple)
                {
                    ms = PDFUtils.PlvSoldesUtils.GenerateDocumentPlvSimple(listeTicketsToSend, format, magasinId);
                }
            }
            
            else
            {
                if (format == ApplicationConsts.format_A5_recto_verso || format == ApplicationConsts.format_A6_recto_verso || format == ApplicationConsts.format_A7_recto_verso)
                {
                    ms = PDFUtils.PlvPermanenteUtils.GenerateDocumentPlvRectoVerso(listeTicketsToSend, format, magasinId);
                }
                else if (format == ApplicationConsts.format_A5_simple || format == ApplicationConsts.format_A6_simple || format == ApplicationConsts.format_A7_simple)
                {
                    ms = PDFUtils.PlvPermanenteUtils.GenerateDocumentPlvSimple(listeTicketsToSend, format, magasinId);
                }
            }

            return ms;
        }
        
        /// <summary>
        /// Retourne un objet TickitData contenant toutes les informations affichable dans un ticket
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public static TickitDataProduit getTickitData(string sku, int langageId, string demarqueLocale, DateTime date)
        {
            T_Produit produit = ProduitDao.getProduitBySku(sku, langageId);
            List<T_Description_Plus> listePlus = Description_PlusDao.getListePlusBySku(sku, langageId);
            T_Description_Dgccrf descriptionDgccrf = Description_DgccrfDao.getDgccrfBySku(sku, langageId);
            T_Variation variation = VariationDao.getVariationBySku(sku, langageId);

            TickitDataProduit ticket = new TickitDataProduit();

            String motRef = "Réf. ";
            if (langageId == 5) motRef = "";

            ticket.sku = motRef + produit.Sku.ToString();
            ticket.variation = variation.VariationName;
            ticket.Made_In = produit.Made_In;

            if (descriptionDgccrf != null)
            {
                ticket.DGCCRF = descriptionDgccrf.LegalDescription;
            }
            
            ticket.range = DAO.RangeDao.getRangeNameById(produit.RangeId);

            T_Prix prixAtDate = PrixDao.getPrixBySkuAndDate(sku, langageId, date);
            // remplissage type tarif
            ticket.typeTarifCbr = prixAtDate.TypeTarifCbr;

            // ce prix servira à determiner si la livraison est incluse ou pas.
            decimal prixVente;
            
            // 1er cas : Si soldes
            if (prixAtDate.Type_promo != ApplicationConsts.typePrix_permanent)
            {
                ticket.isPromoSoldes = true;
                T_Prix prixPermanent = PrixDao.getPrixPermanentPrecedent(prixAtDate);
                
                // Si pas de démarque locale
                if (demarqueLocale == "")
                {
                    //Decimal? pourcentage = 100 - (((prixAtDate.Prix_produit - (produit.Eco_mobilier == null ? 0 : produit.Eco_mobilier)) * 100) / prixPermanent.Prix_produit);

                    //Cillia 04/01/2022

                    Decimal? pourcentage = 100 - (((prixAtDate.Prix_produit - (produit.Eco_mobilier == null ? 0 : produit.Eco_mobilier)) * 100) / (prixPermanent.Prix_produit -(produit.Eco_mobilier == null ? 0 : produit.Eco_mobilier)) );


                    pourcentage = SpecificMathUtils.getRoundDecimal(pourcentage);

                    int posPoint2 = pourcentage.ToString().IndexOf(".");
                    if (posPoint2 != -1)
                    {
                        ticket.pourcentage = "-" + pourcentage.ToString().Substring(0, posPoint2);
                    }
                    else
                    {
                        int posPointVirgule = pourcentage.ToString().IndexOf(",");
                        if (posPointVirgule != -1)
                        {
                            ticket.pourcentage = "-" + pourcentage.ToString().Substring(0, posPointVirgule);
                        }
                        else
                        {
                            ticket.pourcentage = "-" + pourcentage.ToString();
                        }
                    }
                    
                    // MSRIDI 14122015 Cas d'une deuxième ou 3ème démarque en SOLDE
                    if (prixAtDate.Type_promo == ApplicationConsts.typePrix_solde) 
                    {
                        T_Prix prixSoldePrcedent = PrixDao.getPrixSoldePrecedent(prixAtDate);
                        ticket.prixSoldePrecedent = prixSoldePrcedent == null ? "" : prixSoldePrcedent.Prix_produit.ToString();
                    }
                    
                    int posPoint = prixAtDate.Prix_produit.ToString().IndexOf(".");
                    if (posPoint == -1)
                    {
                        posPoint = prixAtDate.Prix_produit.ToString().IndexOf(",");
                    }
                    prixVente = prixAtDate.Prix_produit;
                    ticket.prixSansTaxeAvantVirgule = prixAtDate.Prix_produit.ToString().Substring(0, posPoint);
                    ticket.prixSansTaxeApresVirgule = prixAtDate.Prix_produit.ToString().Substring(posPoint, prixAtDate.Prix_produit.ToString().Length - posPoint) ;
                    ticket.prixPermanent = prixPermanent.Prix_produit.ToString();
                }

                // Cas d'une démarque locale
                else
                {
                    ticket.prixPermanent = prixPermanent.Prix_produit.ToString();
                    ticket.pourcentage = "-" + demarqueLocale;

                    Decimal prixApresDemarque = prixPermanent.Prix_produit - (prixPermanent.Prix_produit * Decimal.Parse(demarqueLocale)) / 100;

                    int posPoint = prixApresDemarque.ToString().IndexOf(".");
                    if (posPoint == -1)
                    {
                        posPoint = prixApresDemarque.ToString().IndexOf(",");
                    }

                    prixVente = prixApresDemarque;
                    ticket.prixSansTaxeAvantVirgule = prixApresDemarque.ToString().Substring(0, posPoint);
                    ticket.prixSansTaxeApresVirgule = prixApresDemarque.ToString().Substring(posPoint, prixApresDemarque.ToString().Length - posPoint) ;
                }
            }

            // Si le prix n'est pas soldé
            else
            {
                // Si pas de démarque locale
                if (demarqueLocale == "")
                {
                    ticket.isPromoSoldes = false;
                    int posPoint = prixAtDate.Prix_produit.ToString().IndexOf(".");
                    if (posPoint == -1)
                    {
                        posPoint = prixAtDate.Prix_produit.ToString().IndexOf(",");
                    }

                    prixVente = prixAtDate.Prix_produit;
                    ticket.prixSansTaxeAvantVirgule = prixAtDate.Prix_produit.ToString().Substring(0, posPoint);
                    ticket.prixSansTaxeApresVirgule = prixAtDate.Prix_produit.ToString().Substring(posPoint, prixAtDate.Prix_produit.ToString().Length - posPoint) ;
                }
                
                // Cas d'une démarque locale
                else
                {
                    ticket.isPromoSoldes = true;
                    
                    ticket.prixPermanent = prixAtDate.Prix_produit.ToString();
                    if(SpecificMathUtils.isInteger(prixAtDate.Prix_produit)){
                        ticket.prixPermanent = ((int)prixAtDate.Prix_produit).ToString();
                    }
                    
                    ticket.pourcentage = "-" + demarqueLocale;

                    Decimal prixApresDemarque = prixAtDate.Prix_produit - (prixAtDate.Prix_produit * Decimal.Parse(demarqueLocale)) / 100;

                    int posPoint = prixApresDemarque.ToString().IndexOf(".");
                    if (posPoint == -1)
                    {
                        posPoint = prixApresDemarque.ToString().IndexOf(",");
                    }
                    
                    prixVente = prixApresDemarque;
                    ticket.prixSansTaxeAvantVirgule = prixApresDemarque.ToString().Substring(0, posPoint);
                    ticket.prixSansTaxeApresVirgule = prixApresDemarque.ToString().Substring(posPoint, prixApresDemarque.ToString().Length - posPoint) ;
                }
            }
            
            
            
            /*
            if ((ticket.prixSansTaxeApresVirgule == ".0" + LangueDao.getCodeMonnaieByMagasinId(langageId)) || (ticket.prixSansTaxeApresVirgule == ".00" + LangueDao.getCodeMonnaieByMagasinId(langageId)) || (ticket.prixSansTaxeApresVirgule == ".000" + LangueDao.getCodeMonnaieByMagasinId(langageId)))
            {
                ticket.prixSansTaxeAvantVirgule = ticket.prixSansTaxeAvantVirgule + LangueDao.getCodeMonnaieByMagasinId(langageId);
                ticket.prixSansTaxeApresVirgule = "";
            }
            */
            

            // Règle de gestion : 
            // La livraison est incluse uniquement dans le prix des produits appartenant à la division 5/6 et le prix est > seuil
            // Le seuil est actuellement fixé à 700€
            // Le terme livraison incluse ou à emporter n'est applicable que si le produit est en division 5 ou 6
            if (produit.Division == 5 || produit.Division == 6)
            {
                ticket.Nombre_colis = Resources.Langue.NombreColis + " " + produit.Nombre_colis.ToString();
                if (prixVente > ApplicationConsts.config.Seuil_Minimal_Livraison_Incluse && (produit.Division == 5 || produit.Division == 6))
                {
                    ticket.livraison = Resources.Langue.TickitDataManager_PrixLivrer;
                }
                else
                {
                    ticket.livraison = Resources.Langue.TickitDataManager_PrixAEmporter;
                }
            }
            
            // Calcul du prix taxé : MODIF 13/02/2014
            decimal? prixAvecTaxe = 0;
            ticket.Taxe_eco = "";
            if (produit.Eco_emballage != null)
            {
                prixAvecTaxe = prixVente + produit.Eco_emballage;
                ticket.Taxe_eco = "dont " + produit.Eco_emballage.ToString() + LangueDao.getCodeMonnaieByMagasinId(langageId) + " d'éco-emballage";
            }

            if (produit.Eco_part != null)
            {
                prixAvecTaxe = prixVente + produit.Eco_part;
                ticket.Taxe_eco = "dont " + produit.Eco_part.ToString() + LangueDao.getCodeMonnaieByMagasinId(langageId) + " d'éco-part";
            }
            
            if (produit.Eco_mobilier != null)
            {
                // MSRIDI 24022016
                // prixAvecTaxe = prixVente + produit.Eco_mobilier;
                // ticket.prixAvecTaxe = prixAvecTaxe.ToString() + LangueDao.getCodeMonnaieByMagasinId(langageId);
                ticket.Taxe_eco = produit.Eco_mobilier.ToString() + LangueDao.getCodeMonnaieByMagasinId(langageId);   
            }

            // set dimensions
            ticket = setDimensionsProduit(ticket, produit);

            ticket.plus = new List<string>();
            foreach (T_Description_Plus element in listePlus)
            {
                ticket.plus.Add("- " + element.Plus);
            }
            

            // Spécifique au Madagascar
            if(langageId == 23)
            {
                NumberFormatInfo nfi = new CultureInfo("fr-FR", false).NumberFormat;
                NumberFormatInfo MGA = new NumberFormatInfo();
                MGA = (NumberFormatInfo)nfi.Clone();
                MGA.CurrencySymbol = "";

                if (null != ticket.prixPermanent)
                {
                    ticket.prixPermanent = decimal.Parse(ticket.prixPermanent).ToString("C", MGA);
                    ticket.prixPermanent = ticket.prixPermanent.Replace(",00", "");
                    ticket.livraison = "";
                }
                ticket.prixSansTaxeAvantVirgule = decimal.Parse(ticket.prixSansTaxeAvantVirgule).ToString("C", MGA);
                ticket.prixSansTaxeAvantVirgule = ticket.prixSansTaxeAvantVirgule.Replace(",00", "");
                ticket.livraison = "";
            }
            // Spécifique Finlande
            if (langageId == 22)
            {
                ticket.livraison = "";
            }
            
            // Concaténation avec le symbole de la monnaie
            if ((ticket.prixSansTaxeApresVirgule == ".0" || (ticket.prixSansTaxeApresVirgule == ".00" || (ticket.prixSansTaxeApresVirgule == ".000" ))))
            {
                ticket.prixSansTaxeAvantVirgule = ticket.prixSansTaxeAvantVirgule + LangueDao.getCodeMonnaieByMagasinId(langageId);
                ticket.prixSansTaxeApresVirgule = "";
            }
            else
            {
                ticket.prixSansTaxeApresVirgule = ticket.prixSansTaxeApresVirgule + LangueDao.getCodeMonnaieByMagasinId(langageId);
            }
            
            // MSRIDI 20042016 a supprimer dès fin de la promo.
            
            if (ticket.typeTarifCbr == "HABHFR") {
                ticket.DGCCRF = ticket.DGCCRF + " * Prix avec carte Habitant." ;
                ticket.mentionHautPlvPrixStandard = "Prix standard.";
                ticket.mentionHautPlvPrixHabitant = "Prix Habitant.";
            }
            
            return ticket;
        }
        
        /// <summary>
        /// Retourne un objet TickitData contenant les informations affichables dans un chevalet de gamme.
        /// </summary>
        /// <param name="Sku"></param>
        /// <param name="langageId"></param>
        /// <returns></returns>
        public static TickitDataProduit getTickitDataPourChevalet(string Sku, int langageId, DateTime dateQuery, string format, string demarqueLocale)
        {
            T_Produit produit = Managers.FicheProduitManager.getProduitBySku(Sku, langageId);
            T_Variation variation = Managers.FicheProduitManager.getVariationBySku(Sku, langageId);

            // Prix a changer par date.
            T_Prix prix = Managers.FicheProduitManager.getPrixBySkuAndDate(Sku, langageId, dateQuery);

            TickitDataProduit data = new TickitDataProduit();
            data.division = produit.Division.ToString();
            data.sku = produit.Sku;
            data.range = DAO.RangeDao.getRangeNameById(produit.RangeId);
            data.variation = variation.VariationName;
            data.prix = prix.Prix_produit.ToString() + LangueDao.getCodeMonnaieByMagasinId(langageId);
            data.Made_In = produit.Made_In;

            if (produit.Nombre_colis != null) {
                data.Nombre_colis = produit.Nombre_colis.ToString();
            }
            
            if (langageId == 23) {
                data.prix = StringUtils.getAriaryMonnaieFormat(prix.Prix_produit) + LangueDao.getCodeMonnaieByMagasinId(langageId);
            }
            
            if (produit.Eco_mobilier != null)
            {
                data.Taxe_eco = "dont " + produit.Eco_mobilier.ToString() + " " + LangueDao.getCodeMonnaieByMagasinId(langageId) + " d'Eco-part"; // Avant eco-mobilier
            }
            
            if (prix.Type_promo == ApplicationConsts.typePrix_promo || prix.Type_promo == ApplicationConsts.typePrix_solde)
            {
                
                data.prixPermanent = Managers.FicheProduitManager.getPrixPermanentPrecedent(prix).Prix_produit.ToString() + LangueDao.getCodeMonnaieByMagasinId(langageId);
                if (langageId == 23)
                {
                    data.prixPermanent = StringUtils.getAriaryMonnaieFormat(Managers.FicheProduitManager.getPrixPermanentPrecedent(prix).Prix_produit) + LangueDao.getCodeMonnaieByMagasinId(langageId);
                }
              
               // decimal? pourcentage = 100 - (((prix.Prix_produit - (produit.Eco_mobilier == null ? 0 : produit.Eco_mobilier)) * 100) / Managers.FicheProduitManager.getPrixPermanentPrecedent(prix).Prix_produit);

                //Cillia

                decimal? pourcentage = 100 - (((prix.Prix_produit - (produit.Eco_mobilier == null ? 0 : produit.Eco_mobilier)) * 100) / ((Managers.FicheProduitManager.getPrixPermanentPrecedent(prix).Prix_produit) - (produit.Eco_mobilier == null ? 0 : produit.Eco_mobilier)));


                pourcentage = Utils.SpecificMathUtils.getRoundDecimal(pourcentage);
                data.pourcentage = pourcentage.ToString();
            }

            // MSRIDI 17122015 
            if (prix.Type_promo == ApplicationConsts.typePrix_solde)
            {
                T_Prix prix2 = PrixDao.getPrixSoldePrecedent(prix);
                data.prixSoldePrecedent = prix2 == null ? "" : prix2.Prix_produit.ToString() + LangueDao.getCodeMonnaieByMagasinId(langageId) ;
            }

            // set dimensions.
            data = setDimensionsProduit(data, produit);

            data.formatImpressionEtiquetteSimple = format;
            data.demarqueLocale = demarqueLocale;

            return data;
        }

        public static TickitDataProduit setDimensionsProduit(TickitDataProduit data, T_Produit produit)
        {
            // Dimension à afficher dans le ticket
            data.dimension = "";

            // Largeur
            if (produit.Largeur != null)
            {
                if (produit.Largeur == (int)produit.Largeur)
                {
                    data.dimension = data.dimension + ApplicationConsts.symboleDimensionLargeur + " " + (int)produit.Largeur;
                }
                else
                {
                    data.dimension = data.dimension + ApplicationConsts.symboleDimensionLargeur + " " + produit.Largeur;
                }
            }

            // Diamètre
            if (produit.Diametre != null)
            {
                if (produit.Diametre == (int)produit.Diametre)
                {
                    data.dimension = data.dimension + ApplicationConsts.symboleDimensionDiametre + " " + (int)produit.Diametre;
                }
                else
                {
                    data.dimension = data.dimension + ApplicationConsts.symboleDimensionDiametre + " " + produit.Diametre;
                }
            }

            // Hauteur
            if (produit.Hauteur != null)
            {
                if (produit.Hauteur == (int)produit.Hauteur)
                {
                    data.dimension = data.dimension + " " + ApplicationConsts.symboleSeparateurDimension + " " + ApplicationConsts.symboleDimensionHauteur + " " + (int)produit.Hauteur;
                }
                else
                {
                    data.dimension = data.dimension + " " + ApplicationConsts.symboleSeparateurDimension + " " + ApplicationConsts.symboleDimensionHauteur + " " + produit.Hauteur;
                }
            }

            // Longueur
            if (produit.Longueur != null)
            {
                if (produit.Longueur == (int)produit.Longueur)
                {
                    data.dimension = data.dimension + " " + ApplicationConsts.symboleSeparateurDimension + " " + ApplicationConsts.symboleDimensionLongueur + " " + (int)produit.Longueur;
                }
                else
                {
                    data.dimension = data.dimension + " " + ApplicationConsts.symboleSeparateurDimension + " " + ApplicationConsts.symboleDimensionLongueur + " " + produit.Longueur;
                }
            }

            // Profondeur
            if (produit.Profondeur != null)
            {
                if (produit.Profondeur == (int)produit.Profondeur)
                {
                    data.dimension = data.dimension + " " + ApplicationConsts.symboleSeparateurDimension + " " + ApplicationConsts.symboleDimensionProfondeur + " " + (int)produit.Profondeur;
                }
                else
                {
                    data.dimension = data.dimension + " " + ApplicationConsts.symboleSeparateurDimension + " " + ApplicationConsts.symboleDimensionProfondeur + " " + produit.Profondeur;
                }
            }

            // Dimension
            if (data.dimension != "")
            {
                data.dimension = data.dimension + " " + ApplicationConsts.dimensionUniteMesure;
            }

            if (produit.Self_Assembly == "YES")
            {
                data.aMonterSoiMeme = Resources.Langue.TickitDataManager_aMonterSoiMeme;
            }

            return data;
        }

        /// <summary>
        /// Retourne les plus d'un range
        /// </summary>
        /// <param name="RangeName"></param>
        /// <param name="langageId"></param>
        /// <returns></returns>
        public static string getPlusOfRange(string RangeName, int langageId, string divisionRange)
        {
            return DAO.RangeDao.getDescriptionPlusByRangeName(RangeName, langageId, divisionRange);
        }
    }
}