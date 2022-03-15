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
    public static class PlvA6RectoVerso
    {
        /// Fonction permetant de génerer le document PDF pour l impression format A6 recto verso
        /// 08/10/2020
        /// Mehdi SRIDI
        /// <param name="chevalet"></param>
        /// <param name="format"></param>
        /// <param name="magasinId"></param>
        /// <param name="dateQuery"></param>
        /// <returns></returns>
        static public string getHtmlA6(TickitDataChevalet chevalet, String format, int magasinId, DateTime dateQuery)
        {
            string baseUrlFonts = "http://ean.habitat.fr/TAKEIT/webfonts/";
            string baseUrlImages = "http://ean.habitat.fr/TAKEIT/A6/images/";

            string codeHtml = "";
            codeHtml = codeHtml + "<html>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "<head>";
            codeHtml = codeHtml + "	<meta charset=\"UTF-8\">";
            codeHtml = codeHtml + "	<title>Habitat</title>";
            codeHtml = codeHtml + "	<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">";
            codeHtml = codeHtml + "	<style>";
            codeHtml = codeHtml + "	.divReglette {";
            codeHtml = codeHtml + "		grid-area: auto;";
            codeHtml = codeHtml + "		width: 600px;";
            codeHtml = codeHtml + "		height: 500px;";
            codeHtml = codeHtml + "		margin: 0px;";
            codeHtml = codeHtml + "		padding: 0px;";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-size: cover;";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "	.promo_rose {";
            codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "pastille_rose.png);";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-origin: border-box;";
            codeHtml = codeHtml + "		padding-top: 18px;";
            codeHtml = codeHtml + "		padding-right: 11px;";
            codeHtml = codeHtml + "		background-size: 80%;";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	";

            codeHtml = codeHtml + "	.promo_rouge {";
            codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "pastille_rouge.png);";
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
            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "	html,";
            codeHtml = codeHtml + "	body {";
            codeHtml = codeHtml + "		height: 100%;";
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
            codeHtml = codeHtml + "	<table width=\"2481\" height=\"1754\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";

            foreach (TickitDataProduit data in chevalet.produitsData)
            {
                string imageMadeIn = "";
                if (data.Made_In != "" && data.Made_In != null)
                {
                    imageMadeIn = "<object data=\"" + baseUrlImages + "flag_" + data.Made_In + ".jpg\" width=\"75\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\"><img src=\"" + baseUrlImages + "blanc.png\" width=\"75\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\"/> </object>";
                }

                T_Produit pro = DAO.ProduitDao.getProduitBySku(data.sku, 4);
                int RangeId = pro.RangeId;

                List<string> listPlus = new List<string>();
                listPlus = DAO.RangeDao.getPlusByRangeId(RangeId, magasinId);

                bool isbarATissu = DAO.RangeDao.isRangeBarAtissu(RangeId);


                string texteDur1 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A6_TEXTE_DUR_1", magasinId);
                string texteDur2 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A6_TEXTE_DUR_2", magasinId);
                string texteDur3 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A6_TEXTE_DUR_3", magasinId);
                string texteDur4 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A6_TEXTE_DUR_4", magasinId);

                //string madeIn = DAO.ProduitDao.getMadeInByRangeId(RangeId);
                List<string> madeInList = DAO.ProduitDao.getListeMadeIn();
                //List<string> listePlus = DAO.RangeDao.getPlusByRangeId(RangeId, magasinId);

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

                //string comp1 = DAO.RangeDao.getDescriptionCompositionByRangeId(RangeId, magasinId, 1);
                //string comp2 = DAO.RangeDao.getDescriptionCompositionByRangeId(RangeId, magasinId, 2);

                //Cillia soit on utilise la table dgccrf


                //  T_Description_Dgccrf comp1 = DAO.Description_DgccrfDao.getDgccrfBySku(pro.Sku, magasinId);

                // string comp = comp1.LegalDescription;


                //Cillia soit on utilise la table Description_composition_Produit

                string comp = DAO.ProduitDao.getDescriptionCompositionBySku(pro.Sku, magasinId);

                string texte_colis = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A5_TEXTE_DUR_6", magasinId);

                string APartirDe = "";
                if (DAO.RangeDao.isRangeBarAtissu(RangeId))
                {
                    APartirDe = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A6_TEXTE_DUR_5", magasinId);
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
                    Nombre_colis = "1" + texte_colis;
                }

                codeHtml = codeHtml + "		<tr>";
                codeHtml = codeHtml + "			<td align=\"center\" style=\"padding: 0px 0 0 20px\" valign=\"top\">";
                codeHtml = codeHtml + "				<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\" style=\"height: 910px;\">";
                codeHtml = codeHtml + "					<tr>";
                codeHtml = codeHtml + "						<td align=\"center\" style=\"padding: 40px 0 0 20px\" valign=\"top\">";
                codeHtml = codeHtml + "							<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "								<tr style=\"height: 225px;\">";
                codeHtml = codeHtml + "									<td align=\"center\">";
                codeHtml = codeHtml + "										<table width=\"100\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td align=\"center\" padding=\"20px 0 0 0\">";
                codeHtml = codeHtml + "													<table align=\"center\" width=\"150\" heigth=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td align=\"center\" padding=\"0 0 0 0\"> <img src=\"" + baseUrlImages + "logo.png\" width=\"75\" border=\"0\" alt=\"habitat\" style=\"display:block; height:auto\"> </td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "										<table width=\"300\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td align=\"left\">";
                codeHtml = codeHtml + "													<table align=\"left\" width=\"500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\">";
                codeHtml = codeHtml + "														<tr>";

                string descriptionRange = DAO.RangeDao.getDescriptionPlusByRangeID(RangeId, magasinId);

                codeHtml = codeHtml + "															<td height=\"34\" colspan=\"3\" valign=\"bottom\" style=\"text-align: left; font-family: dinhabbold; font-size: 65px; text-transform: uppercase\">" + data.range + "</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td align=\"left\">";
                codeHtml = codeHtml + "													<table align=\"left\" width=\"700\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td height=\"50\" style=\"text-align: left; font-family: DINHabrg; font-size: 65px; color: #000000; \">" + data.variation + "</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td align=\"center\" style=\"padding: 20px 0 80px 0\">";
                codeHtml = codeHtml + "													<table align=\"left\" width=\"800\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td class=\"filet\" width=\"1%\" rowspan=\"4\"> </td>";
                codeHtml = codeHtml + "															<td width=\"3%\" rowspan=\"4\"></td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td width=\"500\" style=\"text-align: left; font-family: DINHabRg; font-size: 40px;\"> " + plus1 + " &nbsp;</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td width=\"500\" style=\"text-align: left; font-family: DINHabRg; font-size: 40px;\"> " + plus2 + "</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td width=\"500\" style=\"text-align: left; font-family: DINHabRg; font-size: 40px;\"> " + plus3 + "</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "										<table width=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td align=\"center\" style=\"padding: 0 0 0 0\">";
                codeHtml = codeHtml + "													<table align=\"center\" width=\"200\" heigth=\"150\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" valign=\"top\">";
                codeHtml = codeHtml + "														<tr>";

                string typePastille = "";
                if (chevalet.typePrix == ApplicationConsts.typePrix_demarqueLocale || chevalet.typePrix == ApplicationConsts.typePrix_promo) typePastille = ApplicationConsts.typePastillePromoReglette;
                if (chevalet.typePrix == ApplicationConsts.typePrix_solde) typePastille = ApplicationConsts.typePastilleSoldeReglette;

                string pourcentagetexte = "";
                if (chevalet.pourcentageReduction != null)
                {
                    pourcentagetexte = "-" + chevalet.pourcentageReduction + "%";
                }

                codeHtml = codeHtml + "															<td width=\"200\" height=\"160\" align=\"center\" valign=\"top\" class=\"" + typePastille + "\" style=\"text-align: center; font-family: dinhabbold; font-size: 40pt; color: #FFFFFF; line-height: 120px;\">" + pourcentagetexte + " &nbsp;&nbsp;</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "									</td>";
                codeHtml = codeHtml + "								</tr>";
                codeHtml = codeHtml + "							</table>";
                codeHtml = codeHtml + "							<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "								<tr valign=\"top\" style=\"height: 750px;\">";
                codeHtml = codeHtml + "									<td align=\"left\" style=\"padding: 30px 0 0 150px\">";
                codeHtml = codeHtml + "										<div class=\"divReglette\">";
                codeHtml = codeHtml + "											<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td>";
                codeHtml = codeHtml + "														<table width=\"1000\" height=\"600\">";
                codeHtml = codeHtml + "															<tr width=\"1000\" height=\"600\">";
                codeHtml = codeHtml + "																<td align=\"left\">";
                codeHtml = codeHtml + "																	<object data=\"" + baseUrlImages + "filaires/" + "filaire_A6_" + data.sku + ".png\" width=\"600\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\" type=\"image/png\"><img src=\"" + baseUrlImages + "blanc.png\" width=\"295\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> </object>";
                codeHtml = codeHtml + "																</td>";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</div>";
                codeHtml = codeHtml + "									</td>";
                codeHtml = codeHtml + "								</tr>";
                codeHtml = codeHtml + "								<tr valign=\"top\" style=\"height: 300px;\">";
                codeHtml = codeHtml + "									<td align=\"left\" style=\"padding: 30px 0 0 150px\">";
                codeHtml = codeHtml + "										<div>";
                codeHtml = codeHtml + "											<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td>";
                codeHtml = codeHtml + "														<table>";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td width=\"80\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 100px;\">" + prixGauche.Replace(".00", "") + "</td>";
                codeHtml = codeHtml + "																<td width=\"20\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 50px; \">&nbsp;</td>";
                codeHtml = codeHtml + "																<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 70px; text-decoration: line-through; text-align: left;\">" + prixDroite.Replace(".00", "") + "</td>";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														<table>";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 30px; padding: 0 0; line-height: 0\">" + data.Taxe_eco + "&nbsp;</td>";
                codeHtml = codeHtml + "	                                                            <td  width=\"650\"  height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family:DINHabRg; font-size: 30px; padding: 0 0; line-height:22px \"></td>";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														<table>";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabbold; font-size: 50px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																	<br>";
                codeHtml = codeHtml + "																	<br> " + data.dimension + " &nbsp;</td>";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</div>";
                codeHtml = codeHtml + "									</td>";
                codeHtml = codeHtml + "								</tr>";
                codeHtml = codeHtml + "							</table>";
                codeHtml = codeHtml + "							<div class=\"footer\">";
                codeHtml = codeHtml + "								<table align=\"left\" width=\"1200\" valign=\"top\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "									<tr style=\"height: 125px;\">";
                codeHtml = codeHtml + "										<td style=\"padding:60px 0 60px 50px\">";
                codeHtml = codeHtml + "											<table align=\"left\" width=\"1000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td align=\"center\" style=\"padding:0px 0 0 0\" width=\"300\">";
                codeHtml = codeHtml + "														<hr width=\"300\"> </td>";
                codeHtml = codeHtml + "													<td height=\"10\" valign=\"bottom\" style=\"text-align: center; font-family: dinhabbold; font-size: 22px;\" width=\"400\">" + texteDur2 + "</td>";
                codeHtml = codeHtml + "													<td align=\"center\" style=\"padding:10px 0 0 0\" width=\"300\">";
                codeHtml = codeHtml + "														<hr width=\"300\"> </td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "											<table align=\"left\" width=\"1000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td width=\"75\" align=\"center\" style=\"padding:0px 0 0px 40px\">" + imageMadeIn + "</td>";
                codeHtml = codeHtml + "													<td width=\"800\" align=\"right\" style=\"text-align: right; font-family: DINHabRg; font-size: 25px; padding: 0px 0 0px 0px\">" + texteDur3 + "";
                codeHtml = codeHtml + "														<br><strong>" + texteDur4 + "</strong> </td>";
                codeHtml = codeHtml + "													<td width=\"100\" align=\"left\"><img src=\"" + baseUrlImages + "Camion_livraison.png\" width=\"100\" border=\"0\" alt=\"camion\" title=\"camion livraison\" style=\"display:block; padding: 0px 0 0px 10px\"></td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</div>";
                codeHtml = codeHtml + "						</td>";
                codeHtml = codeHtml + "					</tr>";
                codeHtml = codeHtml + "				</table>";
                codeHtml = codeHtml + "			</td>";
                codeHtml = codeHtml + "			<td align=\"center\" style=\"padding: 0px 0 0 20px\" valign=\"top\">";
                codeHtml = codeHtml + "				<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\" style=\"height: 910px;\">";
                codeHtml = codeHtml + "					<tr>";
                codeHtml = codeHtml + "						<td align=\"center\" style=\"padding: 40px 0 0 20px\" valign=\"top\">";
                codeHtml = codeHtml + "							<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "								<tr style=\"height: 225px;\">";
                codeHtml = codeHtml + "									<td align=\"center\">";
                codeHtml = codeHtml + "										<table width=\"100\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" class=\"full\">";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td align=\"center\" padding=\"20px 0 0 0\">";
                codeHtml = codeHtml + "													<table align=\"center\" width=\"150\" heigth=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td align=\"center\" padding=\"0 0 0 0\"> <img src=\"" + baseUrlImages + "logo.png\" width=\"75\" border=\"0\" alt=\"habitat\" style=\"display:block; height:auto\"> </td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "										<table width=\"500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td align=\"left\" style=\"padding: 0 30px 0 0\">";
                codeHtml = codeHtml + "													<table align=\"left\" width=\"500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\">";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td height=\"34\" colspan=\"3\" valign=\"bottom\" style=\"text-align: left; font-family: dinhabbold; font-size: 65px; text-transform: uppercase\">" + data.range + "</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td align=\"left\">";
                codeHtml = codeHtml + "													<table align=\"left\" width=\"700\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td height=\"50\" style=\"text-align: left; font-family: DINHabrg; font-size: 65px; color: #000000; \">" + data.variation + "</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td align=\"center\" style=\"padding: 20px 0 20px 0\">";
                codeHtml = codeHtml + "													<table align=\"left\" width=\"800\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td class=\"filet\" width=\"1%\" rowspan=\"4\"> </td>";
                codeHtml = codeHtml + "															<td width=\"3%\" rowspan=\"4\"></td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td width=\"500\" style=\"text-align: left; font-family: DINHabRg; font-size: 40px;\"> " + plus1 + " &nbsp;</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td width=\"500\" style=\"text-align: left; font-family: DINHabRg; font-size: 40px;\"> " + plus2 + "</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td width=\"500\" style=\"text-align: left; font-family: DINHabRg; font-size: 40px;\"> " + plus3 + "</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "										<table width=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td align=\"center\" style=\"padding: 0 0 0 0\">";
                codeHtml = codeHtml + "													<table align=\"center\" width=\"200\" heigth=\"150\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" valign=\"top\">";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td width=\"200\" height=\"160\" align=\"center\" valign=\"top\" class=\"" + typePastille + "\" style=\"text-align: center; font-family: dinhabbold; font-size: 40pt; color: #FFFFFF; line-height: 120px;\">" + pourcentagetexte + " &nbsp;&nbsp;</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "									</td>";
                codeHtml = codeHtml + "								</tr>";
                codeHtml = codeHtml + "							</table>";
                codeHtml = codeHtml + "							<table width=\"1200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" style=\"padding-top: 20px;\">";
                codeHtml = codeHtml + "								<tr style=\"height: 790px;\">";
                codeHtml = codeHtml + "									<td valign=\"top\" style=\"padding: 50px 30px 60px 0px\">";
                codeHtml = codeHtml + "										<table align=\"center\" width=\"850\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "	                                           <td width=\"850\" style=\"text-align: left; font-family: DINHabbold; font-size: 38px; text-align: justify; padding: 0 0 0 0\">";
                codeHtml = codeHtml + "													  " + texteDur1 + " ";
                codeHtml = codeHtml + "												</td></tr>";
                codeHtml = codeHtml + "												<tr><td width=\"850\" style=\"vertical-align: top;text-align: left; font-family: DINHabRg; font-size: 32px; text-align: justify; padding: 0 0\">" + comp + "</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "									</td>";
                codeHtml = codeHtml + "								</tr>";
                codeHtml = codeHtml + "								<tr valign=\"top\" style=\"height: 365px\">";
                codeHtml = codeHtml + "									<td align=\"left\" style=\"padding: 30px 0 0 150px; width : 10000px\" valign=\"top\">";
                codeHtml = codeHtml + "										<table>";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td>";
                codeHtml = codeHtml + "													<table   width=\"800\" height=\"35\"> ";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td width=\"200\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 100px;  padding:0 0 0 0; line-height:65px;\">" + prixGauche.Replace(".00", "") + "</td>";
                // codeHtml = codeHtml + "															<td width=\"20\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 50px; \">&nbsp;</td>";
                codeHtml = codeHtml + "															<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 55px; text-decoration: line-through; text-align: le; line-height:30px;\">" + prixDroite.Replace(".00", "") + "</td>";
                // codeHtml = codeHtml + "	                                                        <td width=\"20\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 50px; \">&nbsp;</td>";
                codeHtml = codeHtml + "                                                         <td width=\"50\"  height=\"22\"  valign=\"bottom\" style=\"text-align: right; font-family:  DINHabRg; font-size: 28px; padding: 0 0; line-height:30px \">" + Nombre_colis + "</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "													<table   width=\"800\" height=\"100\">";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td  style=\"text-align: left; font-family: DINHabRg; font-size: 30px; padding: 0 0; line-height: 0\">" + data.Taxe_eco + "&nbsp;</td>";
                codeHtml = codeHtml + "                                                         <td  style=\"text-align: right; font-family:DINHabRg; font-size: 30px; padding: 0 0; line-height:0 \">Réf." + data.sku + "</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "													<table>";
                codeHtml = codeHtml + "														<tr>";
                codeHtml = codeHtml + "															<td style=\"text-align: left; font-family: DINHabbold; font-size: 50px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																<br>";
                codeHtml = codeHtml + "																<br> " + data.dimension + " &nbsp;</td>";
                codeHtml = codeHtml + "														</tr>";
                codeHtml = codeHtml + "													</table>";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "									</td>";
                codeHtml = codeHtml + "								</tr>";
                codeHtml = codeHtml + "							</table>";
                codeHtml = codeHtml + "							<div class=\"footer\">";
                codeHtml = codeHtml + "								<table align=\"left\" width=\"1200\" valign=\"top\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "									<tr style=\"height: 125px;\">";
                codeHtml = codeHtml + "										<td style=\"padding:0px 0 60px 50px\">";
                codeHtml = codeHtml + "											<table align=\"left\" width=\"1000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td align=\"center\" style=\"padding:0px 0 0 0\" width=\"300\">";
                codeHtml = codeHtml + "														<hr width=\"300\"> </td>";
                codeHtml = codeHtml + "													<td height=\"10\" valign=\"bottom\" style=\"text-align: center; font-family: dinhabbold; font-size: 22px;\" width=\"400\">" + texteDur2 + "</td>";
                codeHtml = codeHtml + "													<td align=\"center\" style=\"padding:10px 0 0 0\" width=\"300\">";
                codeHtml = codeHtml + "														<hr width=\"300\"> </td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "											<table align=\"left\" width=\"1000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td width=\"75\" align=\"center\" style=\"padding:0px 0 0px 40px\">" + imageMadeIn + "</td>";
                codeHtml = codeHtml + "													<td width=\"800\" align=\"right\" style=\"text-align: right; font-family: DINHabRg; font-size: 25px; padding: 0px 0 0px 0px\">" + texteDur3 + "";
                codeHtml = codeHtml + "														<br><strong>" + texteDur4 + "</strong> </td>";
                codeHtml = codeHtml + "													<td width=\"100\" align=\"left\"><img src=\"" + baseUrlImages + "Camion_livraison.png\" width=\"100\" border=\"0\" alt=\"camion\" title=\"camion livraison\" style=\"display:block; padding: 0px 0 0px 10px\"></td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</div>";
                codeHtml = codeHtml + "						</td>";
                codeHtml = codeHtml + "					</tr>";
                codeHtml = codeHtml + "				</table>";
                codeHtml = codeHtml + "			</td>";
                codeHtml = codeHtml + "		</tr>";
            }

            //Cillia
            codeHtml = codeHtml + "			<tr><td align=\"center\" style=\"padding: 0 0 20px 0\" valign=\"top\">";
            codeHtml = codeHtml + "	         <table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
            codeHtml = codeHtml + "			<tr>";
            codeHtml = codeHtml + "			<td style=\"font-size: 5px; padding-bottom: 50px\"> </td> </tr></table></td></tr>";

            codeHtml = codeHtml + "	</table>";
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
            int nbMaxPlvParPage = 2;

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
            htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;

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