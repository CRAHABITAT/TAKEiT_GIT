using System;
using System.Collections.Generic;
using TickitNewFace.Models;
using System.IO;
using System.Linq;

namespace TickitNewFace.Managers
{
    public static class RechercheManager
    {
        /// <summary>
        /// Service qui renvoie la objectsToUpdate des résultats depuis les DAOs
        /// </summary>
        /// <param name="rechercheText"></param>
        /// <returns></returns>
        public static List<T_Resultats_Recherche> getProduits(String rechercheText, DateTime dateObj, int langageId)
        {
            List<T_Resultats_Recherche> resultats = DAO.Resultats_RechercheDao.getResultsRechByCriteria(rechercheText, langageId, dateObj);
            return resultats;
        }

        public static List<T_Resultats_Recherche_PLV> getPLV(String rechercheText, DateTime dateObj, int langageId)
        {
            List<T_Resultats_Recherche_PLV> resultats = DAO.Resultats_RecherchePLVDao.getResultsRechByCriteriaPLV(rechercheText, langageId, dateObj);
            return resultats;
        }

        public static List<string> getIMG(string sku) {
            //<img src="http://images.habitat.fr/picturesProducts/ImagesSmall/805835/805835_01.png" alt="Smiley face" height="42" width="42">
            
            List<string> l = new List<string>();
            string dirPath = @"\\2997FR-ECOM01\Ecom\Images_Small";
            string dirPath2 = @"\\2997FR-ECOM01\Ecom\Images_Small" + "\\" + sku;
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
            int cpt = 0;
            foreach (var dir in dirs)
            {
                if (dir == dirPath2)
                    cpt++;
            }
            // repertoire trouvé
            if (cpt != 0)
            {
                DirectoryInfo di = new DirectoryInfo(dirPath2);
                foreach (var fi in di.GetFiles())
                {
                    string s = fi.Name;
                    int no = 0;
                    foreach (var c in s){
                        if(c == ' '){
                            no++;
                        }
                    }
                    if(no == 0){
                    l.Add("http://habitat:habitatimages@images.habitat.fr/picturesProducts/ImagesSmall/" + sku + "/" + fi.Name); //ajout addresse des image
                    }
                }
                /*
                string dirPath2 = @"\\2997FR-ECOM01\Ecom\Images_Small" + "\\" + sku;
                List<string> dirs2 = new List<string>(Directory.EnumerateDirectories(dirPath2));
                foreach (var img in dirs2)
                {
                    l.Add("http://images.habitat.fr/picturesProducts/ImagesSmall/" + sku + "/" + img); //ajout addresse des image
                }*/
            }
            return l;
        }
    }
}