using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TickitNewFace.Models
{
    /// <summary>
    /// Classe produit.
    /// </summary>
    public class TickitDataLuminaire
    {
        public TickitDataProduit produitDessus { get; set; }
        public TickitDataProduit produitDessous { get; set; }
    }
}
