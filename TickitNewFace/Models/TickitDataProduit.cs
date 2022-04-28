using System;
using System.Collections.Generic;

namespace TickitNewFace.Models
{
    /*
     * Test commentaire
     */
    public class TickitDataProduit
    {
        public string sku { get; set; }

        public string division { get; set; }

        public string variation { get; set; }
        public string libelleProduit { get; set; }
        public string range { get; set; }
        public string prix { get; set; }
        public string prixSansTaxeAvantVirgule { get; set; }
        public string prixSansTaxeApresVirgule { get; set; }
        public string prixAvecTaxe { get; set; }
        public string prixPermanent { get; set; }
        public List<string> plus { get; set; }
        public string dimension { get; set; }
        public string DGCCRF { get; set; }
        public string aMonterSoiMeme { get; set; }
        public string pourcentage { get; set; }
        public string livraison { get; set; }
        public string Taxe_eco { get; set; }
        public string Nombre_colis { get; set; }
        public string Made_In { get; set; }
        public Boolean isPromoSoldes { get; set; }
        public string mentionHautPlvPrixStandard { get; set; }
        public string mentionHautPlvPrixHabitant { get; set; }
        public string formatImpressionEtiquetteSimple { get; set; }
        public string demarqueLocale { get; set; }

        public string prixSoldePrecedent { get; set; }

        public string typeTarifCbr { get; set; }
        //Cillia 
       // public string Type_promo { get; set; }

    
    }
}
