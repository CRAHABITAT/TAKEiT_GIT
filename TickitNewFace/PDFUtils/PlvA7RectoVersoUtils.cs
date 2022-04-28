using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Web.Mvc;
using TickitNewFace.Models;
using TickitNewFace.Const;
using TickitNewFace.Utils;
using System.Xml.Linq;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.html;
using HiQPdf;
using System.Net;

namespace TickitNewFace.PDFUtils
{
    public static class PlvA7RectoVerso
    {
        /// Fonction permetant de génerer le document PDF pour l impression format A7 recto verso
        /// 27/10/2020
        /// Mehdi SRIDI
        /// <param name="chevalet"></param>
        /// <param name="format"></param>
        /// <param name="magasinId"></param>
        /// <param name="dateQuery"></param>
        /// <returns></returns>
        static public string getHtmlA7(TickitDataChevalet chevalet, String format, int magasinId, DateTime dateQuery)
        {
            string baseUrlFonts = "http://ean.habitat.fr/TAKEIT/webfonts/";
            string baseUrlImages = "http://ean.habitat.fr/TAKEIT/A7/images/";

            string codeHtml = "";
            codeHtml = codeHtml + "<html>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "<head>";
            codeHtml = codeHtml + "	<meta charset=\"UTF-8\">";
            codeHtml = codeHtml + "	<style>";
            codeHtml = codeHtml + "	.divReglette {";
            codeHtml = codeHtml + "		grid-area: auto;";
            codeHtml = codeHtml + "		width: 105mm;";
            codeHtml = codeHtml + "		height: 148mm;";
            codeHtml = codeHtml + "		margin: 0px;";
            codeHtml = codeHtml + "		padding: 0px;";
            codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "corps.jpg);";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-size: cover;";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "	.promo_rose {";
            codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "pastille_rose.jpg);";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-origin: border-box;";
            codeHtml = codeHtml + "		padding-top: 18px;";
            codeHtml = codeHtml + "		padding-right: 11px;";
            codeHtml = codeHtml + "		background-size: 80%;";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "	.promo_rouge {";
            codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "pastille_rouge.jpg);";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-origin: border-box;";
            codeHtml = codeHtml + "		padding-top: 18px;";
            codeHtml = codeHtml + "		padding-right: 11px;";
            codeHtml = codeHtml + "		background-size: 80%;";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	";
            //Cillia 

            codeHtml = codeHtml + "	.promo_bleue {";
            codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "pastille_bleue.jpg);";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-origin: border-box;";
            codeHtml = codeHtml + "		padding-top: 18px;";
            codeHtml = codeHtml + "		padding-right: 11px;";
            codeHtml = codeHtml + "		background-size: 80%;";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	";



            foreach (TickitDataProduit data in chevalet.produitsData)
            {
                if (data.Made_In != "")
                {
                    codeHtml = codeHtml + "	#flag_" + data.Made_In + " {";
                    codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "flag_" + data.Made_In + ".jpg);";
                    codeHtml = codeHtml + "		background-repeat: no-repeat;";
                    codeHtml = codeHtml + "		background-origin: border-box;";
                    codeHtml = codeHtml + "		background-size: 90%;";
                    codeHtml = codeHtml + "	}";
                }
            }

            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "	.filet {";
            codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "filet.jpg);";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-size: contain;";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	</style>";
            codeHtml = codeHtml + "	<style type=\"text/css\">";
            codeHtml = codeHtml + "	@font-face {";
            codeHtml = codeHtml + "		font-family: 'dinhabbold';";
            codeHtml = codeHtml + "		src: url('" + baseUrlFonts + "DINHabBd.ttf') format('truetype');";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "	@font-face {";
            codeHtml = codeHtml + "		font-family: 'DINHabRg';";
            codeHtml = codeHtml + "		src: url('" + baseUrlFonts + "DINHabRg.ttf') format('truetype');";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	</style>";
            codeHtml = codeHtml + "</head>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "<body>";

            int iteration = 1;
            foreach (TickitDataProduit data in chevalet.produitsData)
            {
                string imageMadeIn = "";
                if (data.Made_In != "" && data.Made_In != null)
                {
                    imageMadeIn = "<object data=\"" + baseUrlImages + "flag_" + data.Made_In + ".jpg\" width=\"40\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\"><img src=\"" + baseUrlImages + "blanc.png\" width=\"40\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\"/> </object>";
                }

                T_Produit pro = DAO.ProduitDao.getProduitBySku(data.sku, 4);
                int RangeId = pro.RangeId;

                List<string> listPlus = new List<string>();
                listPlus = DAO.RangeDao.getPlusByRangeId(RangeId, magasinId);

                bool isbarATissu = DAO.RangeDao.isRangeBarAtissu(RangeId);

                string descriptionRange = DAO.RangeDao.getDescriptionPlusByRangeID(RangeId, magasinId);

                string texteDur1 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A7_TEXTE_DUR_1", magasinId);
                string texteDur2 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A7_TEXTE_DUR_2", magasinId);
                string texteDur3 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A7_TEXTE_DUR_3", magasinId);
                string texteDur4 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A7_TEXTE_DUR_4", magasinId);

                string madeIn = DAO.ProduitDao.getMadeInByRangeId(RangeId);

                // List<string> listePlus = DAO.RangeDao.getPlusByRangeId(RangeId, magasinId);


                //Cillia 
                List<T_Description_Plus> listePlus = DAO.Description_PlusDao.getListePlusBySku(pro.Sku, magasinId);
                string plus1 = "";
                string plus2 = "";
                string plus3 = "";

                if (listePlus.Count == 1)
                {
                    plus1 = "&bull; &nbsp; " + listePlus[0].Plus;
                }
                if (listePlus.Count == 2)
                {
                    plus1 = "&bull; &nbsp; " + listePlus[0].Plus;
                    plus2 = "&bull; &nbsp; " + listePlus[1].Plus;

                }
                if (listePlus.Count == 3)
                {
                    plus1 = "&bull; &nbsp; " + listePlus[0].Plus;
                    plus2 = "&bull; &nbsp; " + listePlus[1].Plus;
                    plus3 = "&bull; &nbsp; " + listePlus[2].Plus;
                }

                // string comp = DAO.RangeDao.getDescriptionCompositionByRangeId(RangeId, magasinId, 1);

                //Cillia on utilise la table Description_Composition_Produit

                string comp = DAO.ProduitDao.getDescriptionCompositionBySku(pro.Sku, magasinId);


                string texte_colis = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A5_TEXTE_DUR_6", magasinId);

                string APartirDe = "";
                if (DAO.RangeDao.isRangeBarAtissu(RangeId))
                {
                    APartirDe = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A7_TEXTE_DUR_5", magasinId);
                }


                string prixDroite = "";
                string prixGauche = "";
                if (data.pourcentage != null)
                {
                    prixDroite = data.prixPermanent;
                    prixGauche = data.prix;
                }
                else
                {
                    prixGauche = data.prix;
                }

                //Cillia 
                if (chevalet.typePrix == ApplicationConsts.typePrix_permanent && data.pourcentage != null)
                {
                    prixGauche = data.prixPermanent;
                    prixDroite = "";

                }


                //Cillia

                 T_Prix prix= DAO.PrixDao.getPrixBySkuAndDate(pro.Sku, magasinId, dateQuery);

                string  typeTarifCbr = prix.TypeTarifCbr;

                string Orientation = DAO.ProduitDao.getOrientationBySku(data.sku, magasinId);

                string DimensionsDeplie = DAO.ProduitDao.getDrescriptionConvertibleBySku(data.sku, magasinId, "L");
                string DimensionsCouchage = DAO.ProduitDao.getDrescriptionConvertibleBySku(data.sku, magasinId, "C");

                string Nombre_colis = "";

                if (data.Nombre_colis != null && data.Nombre_colis != "" && data.Nombre_colis != "0")
                {
                    Nombre_colis = data.Nombre_colis + texte_colis;
                }
                if (data.Nombre_colis == "0")
                {
                    Nombre_colis = "1 " + texte_colis;
                }

                if (iteration == 1 || iteration == 3)
                {
                codeHtml = codeHtml + "	<table>";
                codeHtml = codeHtml + "		<tr>";
                }
                codeHtml = codeHtml + "			<td align=\"left\" style=\"padding:40px 0 30px 25px\">";
                codeHtml = codeHtml + "				<div class=\"divReglette\">";
                codeHtml = codeHtml + "					<table align=\"left\" width=\"325\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"center\" padding=\"0 0 0 0\">";
                codeHtml = codeHtml + "								<table width=\"350\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"left\" padding=\"0 0 0 0\">";
                codeHtml = codeHtml + "											<table align=\"center\" width=\"40\" heigth=\"40\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td align=\"left\" padding=\"0 0 0 0\"> <img src=\"" + baseUrlImages + "logo.png\" width=\"40\" heigth=\"40\" border=\"0\" alt=\"habitat\" style=\"display:block height:auto\"> </td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "										<td align=\"left\" padding=\"20px 0 0 50px\">";
                codeHtml = codeHtml + "											<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td height=\"34\"  padding=\"0 0 0 5px\" colspan=\"3\" valign=\"bottom\" style=\"text-align: left; font-family: dinhabbold; font-size: 26px; text-transform: uppercase\">" + data.range + "</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td height=\"15\"  padding=\"0 0 0 5px\"  colspan=\"3\" valign=\"top\" style=\"text-align: left; font-family: DINHabRg; font-size: 20px;\">" + data.variation + "</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "										<td align=\"right\" padding=\"0 0 0 90px\">";
                codeHtml = codeHtml + "											<table align=\"right\" width=\"80\" heigth=\"80\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" valign=\"top\">";
                codeHtml = codeHtml + "												<tr>";



                string pourcentagetexte = "";
                if (data.pourcentage != null)
                {
                    pourcentagetexte = "-" + data.pourcentage + "%";
                }


              string typePastille = "";

              //  if (typeTarifCbr == "HABHFR" && chevalet.typePrix == ApplicationConsts.typePrix_promo)
               // { typePastille = ApplicationConsts.typePastillePromoHab;

               // codeHtml = codeHtml + "										     <td width=\"80\" height=\"80\" align=\"center\" valign=\"top\" class=\"" + typePastille + "\" style=\"text-align: center; font-family: dinhabbold; font-size: 14pt; color: #FFFFFF; line-height: 50px; \">" + pourcentagetexte + " &nbsp;</td>";
                
              //  }

                if (chevalet.typePrix == ApplicationConsts.typePrix_demarqueLocale || chevalet.typePrix == ApplicationConsts.typePrix_promo) { typePastille = ApplicationConsts.typePastillePromoReglette; }
                else  if (chevalet.typePrix == ApplicationConsts.typePrix_solde) { typePastille = ApplicationConsts.typePastilleSoldeReglette; }
               
           

               
                
                codeHtml = codeHtml + "													<td width=\"80\" height=\"80\" align=\"center\" valign=\"top\" class=\"" + typePastille + "\" style=\"text-align: center; font-family: dinhabbold; font-size: 18pt; color: #FFFFFF; line-height: 60px; \">" + pourcentagetexte + " &nbsp;</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"center\" style=\"padding:0 0 0 50px\">";
                codeHtml = codeHtml + "								<table align=\"left\" width=\"350\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td class=\"filet\" width=\"1%\" rowspan=\"3\"> </td>";
                codeHtml = codeHtml + "										<td width=\"3%\" rowspan=\"3\"></td>";
                codeHtml = codeHtml + "										<td width=\"200\" style=\"text-align: left; font-family: DINHabRg; font-size: 16px;\">" + plus1 + " &nbsp;</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td width=\"200\" style=\"text-align: left; font-family: DINHabRg; font-size: 16px;\">" + plus2 + " &nbsp;</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td width=\"200\" style=\"text-align: left; font-family: DINHabRg; font-size: 16px;\">" + plus3 + " &nbsp;</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"left\" style=\"padding: 0 0 0 50px\">";
                codeHtml = codeHtml + "								<table width=\"350\" height=\"245\">";
                codeHtml = codeHtml + "									<tr width=\"200\" height=\"245\">";
                codeHtml = codeHtml + "										<td align=\"left\"  height=\"245\" >";
                codeHtml = codeHtml + "											<object data=\"" + baseUrlImages + "filaires/" + "filaire_A7_" + data.sku + ".png\" width=\"140\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\" type=\"image/png\"><img src=\"http://ean.habitat.fr/TAKEIT/A7/images/blanc.png\" width=\"140\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> </object>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"left\" style=\"padding: 0 0 0 50px\">";
                codeHtml = codeHtml + "								<table width=\"350\" height=\"35\">";
                codeHtml = codeHtml + "									<tr width=\"350\" height=\"35\">";
                codeHtml = codeHtml + "										<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0px\" width=\"266\">" + prixGauche.Replace(".00", "") + " <span style=\"font-family: DINHabRg; font-size: 20px; text-decoration: line-through; text-align: left; line-height:20px;\">" + prixDroite.Replace(".00", "") + "</span></td>";
                //  codeHtml = codeHtml + "										<td height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 14px; text-align: left; line-height:20px;\">" + Nombre_colis + "</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "								<table width=\"325\" height=\"18\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td style=\"text-align: left; font-family: DINHabRg; font-size: 11px; padding: 0 0px; line-height: 1\" valign=\"top\">" + data.Taxe_eco + "</td>";
                //codeHtml = codeHtml + "										<td width=\"68\" valign=\"top\" style=\"font-family: DINHabRg; font-size: 11px; text-align: left; line-height:1;\">Réf. " + data.sku + "</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "								<table width=\"350\" height=\"20\">";
                codeHtml = codeHtml + "									<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "										<td style=\"text-align: left; font-family: DINHabbold; font-size: 20px; padding: 0 0px; line-height: 0\">";
                codeHtml = codeHtml + "											<br> " + data.dimension + " &nbsp;</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"left\" style=\"padding: 0px 0 0px 20px\">";
                codeHtml = codeHtml + "								<table align=\"left\" width=\"350\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\">";
                codeHtml = codeHtml + "									<tr>";

                //codeHtml = codeHtml + "											<td align=\"left\" style=\"padding:0px 0 0 0\" width=\"100\"><hr width=\"100\"> </td>";
                //codeHtml = codeHtml + "											<td height=\"10\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 8px;\" width=\"300\">" + texteDur2 + "</td>";			
                //codeHtml = codeHtml + "                                     <td align=\"left\" style=\"padding:0px 0 0 0\" width=\"100\"><hr width=\"100\"> </td></tr></table>";
                codeHtml = codeHtml + "									  <td width=\"40\" align=\"center\" style=\"padding:10px 0 0 0px\">" + imageMadeIn + "</td>";

                codeHtml = codeHtml + "	                                   <td width=\"350\" align=\"right\" style=\"text-align: right; font-family: DINHabRg; font-size: 7px; padding:0 0 0 0\">" + texteDur3 + "";
                codeHtml = codeHtml + "											<br><strong>" + texteDur4 + "</strong> </td>";
                codeHtml = codeHtml + "										<td width=\"55\" align=\"left\"><img src=\"" + baseUrlImages + "Camion_livraison.png\" width=\"40\" border=\"0\" alt=\"camion\" title=\"camion livraison\" style=\"display:block; padding: 5px 0 0px 10px\"></td>";
                //codeHtml = codeHtml + "									</tr></table>";

                // codeHtml = codeHtml + "										<td width=\"800\" align=\"right\" style=\"text-align: right; font-family: DINHabRg; font-size: 14px; padding: 5px 0 0px 0px\"> </td>";
                //codeHtml = codeHtml + "										<td align=\"left\" width=\"50\" border=\"0\" alt=\"\" title=\"\" style=\"display:block; padding: 5px 0 0 10px\"></td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "				</div>";
                codeHtml = codeHtml + "			</td>";
                codeHtml = codeHtml + "			<td align=\"right\" style=\"padding:40px 0 30px 70px\">";
                codeHtml = codeHtml + "				<div class=\"divReglette\">";
                codeHtml = codeHtml + "					<table align=\"left\" width=\"325\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"center\"  padding=\"0 0 0 0\">";
                codeHtml = codeHtml + "								<table width=\"350\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"left\" padding=\"0 0 0 0\">";
                codeHtml = codeHtml + "											<table align=\"center\" width=\"40\" heigth=\"40\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td align=\"left\" padding=\"0 0 0 0\"> <img src=\"" + baseUrlImages + "logo.png\" width=\"40\" heigth=\"40\" border=\"0\" alt=\"habitat\" style=\"display:block; height:auto\"> </td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "										<td align=\"left\" padding=\"20px 0 0 0\">";
                codeHtml = codeHtml + "											<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td height=\"34\" colspan=\"3\" valign=\"bottom\" style=\"text-align: left; font-family: dinhabbold; font-size: 26px; text-transform: uppercase\">" + data.range + " </td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td height=\"15\" colspan=\"3\" valign=\"top\" style=\"text-align: left; font-family: DINHabRg; font-size: 20px;\">" + data.variation + "</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "										<td align=\"right\" padding=\"0 0 0 90px\">";
                codeHtml = codeHtml + "											<table align=\"right\" width=\"80\" heigth=\"80\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" valign=\"top\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td width=\"80\" height=\"80\" align=\"center\" valign=\"top\" class=\"" + typePastille + "\" style=\"text-align: center; font-family: dinhabbold; font-size: 18pt; color: #FFFFFF; line-height: 60px; \">" + pourcentagetexte + " &nbsp;</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"center\" style=\"padding:0 0 0 50px\">";
                codeHtml = codeHtml + "								<table align=\"left\" width=\"350\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td class=\"filet\" width=\"1%\" rowspan=\"3\"></td>";
                codeHtml = codeHtml + "										<td width=\"3%\" rowspan=\"3\"></td>";
                codeHtml = codeHtml + "										<td width=\"200\" style=\"text-align: left; font-family: DINHabRg; font-size: 16px;\">" + plus1 + " &nbsp;</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td width=\"200\" style=\"text-align: left; font-family: DINHabRg; font-size: 16px;\">" + plus2 + "</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td width=\"200\" style=\"text-align: left; font-family: DINHabRg; font-size: 16px;\">" + plus3 + "</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"center\" style=\"padding: 0 0 0 20px\">";
                codeHtml = codeHtml + "								<table width=\"320\" height=\"250\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" class=\"full\">";
                codeHtml = codeHtml + "									<tr  width=\"200\" height=\"250\">";
                codeHtml = codeHtml + "										<td width=\"150\" height=\"250\" style=\"text-align: left; font-family: DINHabRg; font-size: 14px; text-align: justify; padding: 0 0 0 35px\"> " + comp + "";
                codeHtml = codeHtml + "										 </td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"left\" style=\"padding: 0 0 0 50px\">";
                codeHtml = codeHtml + "								<table width=\"350\" height=\"35\">";
                codeHtml = codeHtml + "									<tr width=\"350\" height=\"35\">";
                codeHtml = codeHtml + "										<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0px\" width=\"266\">" + prixGauche.Replace(".00", "") + " &nbsp; <span style=\"font-family: DINHabRg; font-size: 20px; text-decoration: line-through; text-align: left; line-height:20px;\">" + prixDroite.Replace(".00", "") + " </span></td>";
                codeHtml = codeHtml + "										<td height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 14px; text-align: left; line-height:20px;\">" + Nombre_colis + "</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "								<table width=\"325\" height=\"18\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td style=\"text-align: left; font-family: DINHabRg; font-size: 11px; padding: 0 0px; line-height: 1\" valign=\"top\">" + data.Taxe_eco + " &nbsp;</td>";
                codeHtml = codeHtml + "										<td width=\"68\" valign=\"top\" style=\"font-family: DINHabRg; font-size: 11px; text-align: left; line-height:1;\">Réf. " + data.sku +"</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "								<table width=\"350\" height=\"20\">";
                codeHtml = codeHtml + "									<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "										<td style=\"text-align: left; font-family: DINHabbold; font-size: 20px; padding: 0 0px; line-height: 0\">";
                codeHtml = codeHtml + "											<br> " + data.dimension + " &nbsp;</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"left\" style=\"padding: 0px 0 0 20px\">";
                codeHtml = codeHtml + "								<table align=\"left\" width=\"350\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td width=\"40\" align=\"center\" style=\"padding:5px 0 0 30px\">" + imageMadeIn + "</td>";
                codeHtml = codeHtml + "										<td width=\"800\" align=\"right\" style=\"text-align: right; font-family: DINHabRg; font-size: 7px; padding:0 0 0 0\">" + texteDur3 + "";
                codeHtml = codeHtml + "											<br><strong>" + texteDur4 + "</strong> </td>";
                codeHtml = codeHtml + "										<td width=\"55\" align=\"left\"><img src=\"" + baseUrlImages + "Camion_livraison.png\" width=\"40\" border=\"0\" alt=\"camion\" title=\"camion livraison\" style=\"display:block; padding: 5px 0 0px 10px\"></td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "				</div>";
                codeHtml = codeHtml + "			</td>";

                 if (chevalet.produitsData.Count == 1 || (chevalet.produitsData.Count == 3 && iteration == 3))
                {
                    codeHtml = codeHtml + "  			<td  align=\"left\" style=\"padding: 40px 0 30px 70px\">";
                     codeHtml = codeHtml + "				<div class=\"divReglette\">";
                      codeHtml = codeHtml + "					<table align=\"left\" width=\"325\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\"></table></div>";
                     codeHtml = codeHtml + "			</td>";
                     codeHtml = codeHtml + "			<td  align=\"right\" style=\"padding: 40px 0 30px 70px\">";
                     codeHtml = codeHtml + "				<div class=\"divReglette\"></div>";
                     codeHtml = codeHtml + "					<table align=\"left\" width=\"325\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\"></table></div>";
                     codeHtml = codeHtml + "			</td>";

                     codeHtml = codeHtml + "		</tr>";
                     codeHtml = codeHtml + "	</table>";
                 }

                 if (iteration == 2 || iteration == 4)
                 {
                     codeHtml = codeHtml + "		</tr>";
                     codeHtml = codeHtml + "	</table>";
                 }

                 iteration ++;
             }
                codeHtml = codeHtml + "		</tr>";
                codeHtml = codeHtml + "	</table>";
               // codeHtml = codeHtml + "</body>";
               // codeHtml = codeHtml + "";
                //codeHtml = codeHtml + "</html>";
                //return codeHtml;
            
            codeHtml = codeHtml + "</body>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "</html>";
            return codeHtml;
        }

        /// <summary>
        /// Cette methode renvoie une liste de chevalets dans le but de génrer différents fichiers PDFs.
        /// </summary>
        /// <param name="decimalString"></param>
        /// <param name="symbole"></param>
        /// <returns></returns>
        public static List<TickitDataChevalet> splitChevalet(TickitDataChevalet chevalet)
        {
            List<TickitDataChevalet> chevalets = new List<TickitDataChevalet>();

            int nbLoops;
            int nbPartieEntiere;
            int nbMaxPlvParPage = 4;

            nbPartieEntiere = chevalet.produitsData.Count % nbMaxPlvParPage;
            nbLoops = chevalet.produitsData.Count / nbMaxPlvParPage;

            if (nbPartieEntiere != 0) nbLoops++;

            for (int i = 1; i <= nbLoops; i++)
            {
                TickitDataChevalet chevaletCurrentPage = new TickitDataChevalet();
                chevaletCurrentPage.originePanier = chevalet.originePanier;
                chevaletCurrentPage.pourcentageReduction = chevalet.pourcentageReduction;
                chevaletCurrentPage.rangeChevalet = chevalet.rangeChevalet;
                chevaletCurrentPage.typePrix = chevalet.typePrix;
                chevaletCurrentPage.formatImpressionEtiquettesSimples = chevalet.formatImpressionEtiquettesSimples;
                chevaletCurrentPage.produitsData = new List<TickitDataProduit>();

                int nbMaxIterationsProduitsParChevalet;

                if (i < nbLoops) nbMaxIterationsProduitsParChevalet = nbMaxPlvParPage; else nbMaxIterationsProduitsParChevalet = nbPartieEntiere;
                if ((i == nbLoops) && (nbPartieEntiere == 0)) nbMaxIterationsProduitsParChevalet = nbMaxPlvParPage;

                for (int i_pro = 0; i_pro < nbMaxIterationsProduitsParChevalet; i_pro++)
                {
                    int pos = ((i - 1) * nbMaxPlvParPage) + i_pro;
                    chevaletCurrentPage.produitsData.Add(chevalet.produitsData[((i - 1) * nbMaxPlvParPage) + i_pro]);
                }
                chevalets.Add(chevaletCurrentPage);
            }

            return chevalets;
        }

        /// <summary>
        /// Renvoie le modèle de convertisseur html.
        /// </summary>
        /// <returns></returns>
        public static HtmlToPdf getHtmlToPdfModel()
        {
            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set browser width
            htmlToPdfConverter.BrowserWidth = 1200;

            // set browser height if specified, otherwise use the default
            // htmlToPdfConverter.BrowserHeight = 1400;

            // set HTML Load timeout
            htmlToPdfConverter.HtmlLoadedTimeout = 120;

            // set PDF page size and orientation
            htmlToPdfConverter.Document.PageSize = PdfPageSize.A4;

            // set the PDF standard used by the document
            htmlToPdfConverter.Document.PdfStandard = PdfStandard.Pdf;

            // PORTRAIT vs LANDSCAPE(
            htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Landscape;

            // set PDF page margins
            htmlToPdfConverter.Document.Margins = new PdfMargins(5);

            // set whether to embed the true type font in PDF
            htmlToPdfConverter.Document.FontEmbedding = true;

            // set triggering mode; for WaitTime mode set the wait time before convert
            htmlToPdfConverter.TriggerMode = ConversionTriggerMode.Auto;

            // set the document security
            htmlToPdfConverter.Document.Security.AllowPrinting = true;

            // set the permissions password too if an open password was set
            if (htmlToPdfConverter.Document.Security.OpenPassword != null && htmlToPdfConverter.Document.Security.OpenPassword != String.Empty)
                htmlToPdfConverter.Document.Security.PermissionsPassword = htmlToPdfConverter.Document.Security.OpenPassword + "_admin";

            return htmlToPdfConverter;
        }
    }
}