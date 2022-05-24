using System.Collections.Generic;

namespace TickitNewFace.Models
{
    public class TickitDataChevalet 
    {
        public string rangeChevalet { get; set; }
        public string typePrix { get; set; }
        public decimal? pourcentageReduction { get; set; }
        public string formatImpressionEtiquettesSimples { get; set; }
        public string originePanier { get; set; }
        public List<TickitDataProduit> produitsData { get; set; }

        // Cillia

        public string typeTarifCbr { get; set; }
    }
}