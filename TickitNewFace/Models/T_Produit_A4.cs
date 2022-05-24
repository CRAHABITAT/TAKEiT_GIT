using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TickitNewFace.Models
{
    public class T_Produit_A4
    {
        public string Sku { get; set; }
        public string ImageFilaire { get; set; }
        public string Variation { get; set; }
        public string Orientation { get; set; }
        public string APartirDe { get; set; }
        public string prixGauche { get; set; }
        public string prixDroite { get; set; }
        public string EcoPart { get; set; }
        public string Dimenions { get; set; }
        public string DimensionsDeplie { get; set; }
        public string DimensionsCouchage { get; set; }
        public string Nombre_colis { get; set; }

        //Cillia 12/05/22

        public string typeTarifCbr { get; set; }

        //
        static public T_Produit_A4 initializeProduit()
        {
            T_Produit_A4 pro = new T_Produit_A4();
            pro.Sku = "";
            pro.ImageFilaire = "";
            pro.Variation = "";
            pro.Orientation = "";
            pro.APartirDe = "";
            pro.prixGauche = "";
            pro.prixDroite = "";
            pro.EcoPart = "";
            pro.Dimenions = "";
            pro.DimensionsDeplie = "";
            pro.DimensionsCouchage = "";
            pro.Nombre_colis = "";
            pro.typeTarifCbr = "";

            return pro;
        }
    }
}