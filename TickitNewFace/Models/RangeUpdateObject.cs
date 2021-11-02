namespace TickitNewFace.Models
{
    public class RangeUpdateObject
    {
        public string rangeName { get; set; }

        // Famille / sous famille / sous sous famille
        public string F_SF_SSF { get; set; }

        public string plus_FR { get; set; }
        public string plus_GB { get; set; }
        public string plus_ES { get; set; }
        public string plus_DE { get; set; }

        public string libelle_FR { get; set; }
        public string libelle_GB { get; set; }
        public string libelle_ES { get; set; }
        public string libelle_DE { get; set; }
    }
}