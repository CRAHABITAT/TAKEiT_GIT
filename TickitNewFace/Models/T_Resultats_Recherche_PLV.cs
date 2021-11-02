using System;

namespace TickitNewFace.Models
{
    public class T_Resultats_Recherche_PLV
    {
        public string sousGammes { get; set; }
        public string Skus { get; set; }
        public string Gamme { get; set; }
        public string Format { get; set; }
        public string DescriptionDgccrf  { get; set; }
        public DateTime Date_debut { get; set; }
        public DateTime Date_fin { get; set; }
        public int? CountryCode { get; set; }
    }
}
