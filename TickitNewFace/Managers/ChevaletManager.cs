using System;
using System.Linq;
using TickitNewFace.Models;
using System.Collections.Generic;

namespace TickitNewFace.Managers
{
    public static class ChevaletManager
    {
        /// <summary>
        /// Cette methode permet de savoir s'il s'agit d'un chevalet de gamme ou pas.
        /// </summary>
        /// <param name="chevalet"></param>
        /// <returns></returns>
        public static Boolean isChevaletDeGamme(TickitDataChevalet chevalet)
        {
            string rangeChevalet = chevalet.rangeChevalet;

            // un chevalet de gamme doit avoir une gamme. Sinon il l'est pas.
            if (chevalet.rangeChevalet == null)
            {
                return false;
            }

            foreach (TickitDataProduit data in chevalet.produitsData)
            {
                if (data.range.ToUpper() != rangeChevalet.ToUpper())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Cette methode permet de récuperer le range d'un chevalet.
        /// </summary>
        /// <param name="chevalet"></param>
        /// <returns></returns>
        public static string getRangeOfChevalet(TickitDataChevalet chevalet)
        {
            if (chevalet.produitsData.Count == 0)
            {
                return null;
            }
            else
            {
                List<string> ranges = new List<string>();
                foreach (TickitDataProduit produitData in chevalet.produitsData)
                {
                    ranges.Add(produitData.range);
                }

                var distinctRanges = (from r in ranges select r).Distinct();
                List<string> ls = distinctRanges.ToList();

                if (ls.Count == 1)
                {
                    return ls[0];
                }
                return null;
            }
        }

        /// <summary>
        /// Cette methode permet de récuperer le range ID d'un chevalet.
        /// </summary>
        /// <param name="chevalet"></param>
        /// <returns></returns>
        public static int getRangeIDOfChevalet(TickitDataChevalet chevalet)
        {
            int rangeId ;
            if (chevalet.produitsData.Count == 0)
            {
                rangeId = 0;
            }
            else
            {
                string sku = chevalet.produitsData[0].sku;
                T_Produit pro = DAO.ProduitDao.getProduitBySku(sku, 4);
                rangeId = pro.RangeId;
            }
            return rangeId;
        }

        /// <summary>
        /// Cette methode permet de récuperer le pourcentage d'un chevalet.
        /// </summary>
        /// <param name="chevalet"></param>
        /// <returns></returns>
        public static decimal? getPourcentageReductionOfChevalet(TickitDataChevalet chevalet)
        {
            if (chevalet.produitsData.Count == 0)
            {
                return null;
            }
            return chevalet.pourcentageReduction;
        }

        /// <summary>
        /// Cette methode permet de récuperer le pourcentage d'un chevalet.
        /// </summary>
        /// <param name="chevalet"></param>
        /// <returns></returns>
        public static string getTypePrixOfChevalet(TickitDataChevalet chevalet)
        {
            if (chevalet.produitsData.Count == 0)
            {
                return null;
            }
            return chevalet.typePrix;
        }

        /// <summary>
        /// Vérifie si un produit "Sku" existe dans le chevalet ou pas.
        /// </summary>
        /// <param name="Sku"></param>
        /// <param name="chevalet"></param>
        /// <returns></returns>
        public static Boolean isProduitExistsInChevalet(string Sku, TickitDataChevalet chevalet)
        {
            foreach (TickitDataProduit produit in chevalet.produitsData)
            {
                if (produit.sku == Sku)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
