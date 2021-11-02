using System;
using TickitNewFace.Models;
using System.Collections.Generic;

namespace TickitNewFace.Models
{
    public class T_IPLV_Details
    {
        public string sku { get; set; }
        public string dimension_produit { get; set; }
        public string dimension_colis { get; set; }
        public string[] couleurs { get; set; }
        public string[] matieres { get; set; }
        public string designed_habitat { get; set; }
    }
}