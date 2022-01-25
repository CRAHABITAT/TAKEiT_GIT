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
    public static class PlvChevaletA6Utils
    {
        /// Fonction permetant de génerer le document PDF pour l impression format A6
        /// 18/07/2020
        /// Mehdi SRIDI
        /// <param name="chevalet"></param>
        /// <param name="format"></param>
        /// <param name="magasinId"></param>
        /// <param name="dateQuery"></param>
        /// <returns></returns>
        static public string getHtmlA6(TickitDataChevalet chevalet, String format, int magasinId, DateTime dateQuery)
        {
            string baseUrlImages = "http://2997fr-mssql04/product/Content/maquette_A6/images/";
            string baseUrlFonts = "http://2997fr-mssql04/product/Content/maquette_A4/webfonts/";

            baseUrlFonts = "http://ean.habitat.fr/TAKEIT/webfonts/";
            baseUrlImages = "http://ean.habitat.fr/TAKEIT/A6/images/";

            string prefixFilaire = "filaire_A6_";

            int RangeId = Managers.ChevaletManager.getRangeIDOfChevalet(chevalet);

            List<string> listPlus = new List<string>();
            listPlus = DAO.RangeDao.getPlusByRangeId(RangeId, magasinId);

            bool isbarATissu = DAO.RangeDao.isRangeBarAtissu(RangeId);

            List<T_Produit_A4> lista = getTableauProduitsFromChevalet(chevalet, magasinId, RangeId);

            T_Produit_A4 pro1 = lista[0];
            T_Produit_A4 pro2 = lista[1];
            T_Produit_A4 pro3 = lista[2];
            T_Produit_A4 pro4 = lista[3];
            T_Produit_A4 pro5 = lista[4];
            T_Produit_A4 pro6 = lista[5];
            T_Produit_A4 pro7 = lista[6];
            T_Produit_A4 pro8 = lista[7];

            string texteDur1 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A6_TEXTE_DUR_1", magasinId);
            string texteDur2 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A6_TEXTE_DUR_2", magasinId);
            string texteDur3 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A6_TEXTE_DUR_3", magasinId);
            string texteDur4 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A6_TEXTE_DUR_4", magasinId);

            string imageMadeIn = "";
            string madeIn = DAO.ProduitDao.getMadeInByRangeId(RangeId);

            List<string> listePlus = DAO.RangeDao.getPlusByRangeId(RangeId, magasinId);

            string plus1 = "";
            string plus2 = "";
            string plus3 = "";

            if (listePlus.Count == 1)
            {
                plus1 = "&bull; &nbsp; " + listePlus[0];
            }
            if (listePlus.Count == 2)
            {
                plus1 = "&bull; &nbsp; " + listePlus[0];
                plus2 = "&bull; &nbsp; " + listePlus[1];

            }
            if (listePlus.Count == 3)
            {
                plus1 = "&bull; &nbsp; " + listePlus[0];
                plus2 = "&bull; &nbsp; " + listePlus[1];
                plus3 = "&bull; &nbsp; " + listePlus[2];
            }
          

            string comp = DAO.RangeDao.getDescriptionCompositionByRangeId(RangeId, magasinId, 1);

            if (madeIn != "")
            {
                imageMadeIn = "<object data=\"" + baseUrlImages + "Flag_" + madeIn + ".png\" width=\"75\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\"><img src=\"" + baseUrlImages + "blanc.png\" width=\"75\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\"/> </object>";
            }

            string codeHtml = "";
            codeHtml = codeHtml + "<html lang=\"fr\">";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "<head>";
            codeHtml = codeHtml + "	<meta charset=\"UTF-8\">";
            codeHtml = codeHtml + "	<title>Habitat</title>";
            codeHtml = codeHtml + "	<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">";
            codeHtml = codeHtml + "	<style>";
            codeHtml = codeHtml + "		";
            codeHtml = codeHtml + "		.divReglette {";
            codeHtml = codeHtml + "		grid-area: auto;";
            codeHtml = codeHtml + "		width: 600px;";
            codeHtml = codeHtml + "		height: 500px;";
            codeHtml = codeHtml + "		margin: 0px;";
            codeHtml = codeHtml + "		padding: 0px;";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-size: cover;";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "		.divReglette2 {";
            codeHtml = codeHtml + "		grid-area: auto;";
            codeHtml = codeHtml + "		width: 800px;";
            codeHtml = codeHtml + "		height: 400px;";
            codeHtml = codeHtml + "		margin: 0px;";
            codeHtml = codeHtml + "		padding: 0px;";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-size: cover;";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "	.promo_rose {";
            codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "pastille_rose.png);";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-origin: border-box;";
            codeHtml = codeHtml + "		padding-top: 18px;";
            codeHtml = codeHtml + "		padding-right: 11px;";
            codeHtml = codeHtml + "		background-size: 80%;";
            codeHtml = codeHtml + "	}";

            codeHtml = codeHtml + "	.promo_rouge {";
            codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "pastille_rouge.png);";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-origin: border-box;";
            codeHtml = codeHtml + "		padding-top: 18px;";
            codeHtml = codeHtml + "		padding-right: 11px;";
            codeHtml = codeHtml + "		background-size: 80%;";
            codeHtml = codeHtml + "	}";

            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "	#flag_" + madeIn + " {";
            codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "flag_" + madeIn + ".jpg);";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-origin: border-box;";
            codeHtml = codeHtml + "		background-size: 90%;";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "	.filet {";
            codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "filet.jpg);";
            codeHtml = codeHtml + "		background-repeat: no-repeat;";
            codeHtml = codeHtml + "		background-size: contain;";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "		html, body{";
            codeHtml = codeHtml + "		height: 100%;";
            codeHtml = codeHtml + "}";
            codeHtml = codeHtml + "		";
            codeHtml = codeHtml + "	.contenu{";
            codeHtml = codeHtml + "	min-height: 100%;";
            codeHtml = codeHtml + "	position: relative;";
            codeHtml = codeHtml + "}";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "	footer{";
            codeHtml = codeHtml + "	height: 1px; ";
            codeHtml = codeHtml + "	position: absolute;";
            codeHtml = codeHtml + "	bottom: 0px;";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "}";
            codeHtml = codeHtml + "";
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
            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "</head>";
            codeHtml = codeHtml + "	<body>";
            codeHtml = codeHtml + "		<table  width=\"2481\" height=\"1754\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "		   <tr>";
            codeHtml = codeHtml + "			<td align=\"center\" style=\"padding: 40px 0 0 20px\" valign=\"top\" >";
            codeHtml = codeHtml + "				<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\" style=\"height: 910px;\">";
            codeHtml = codeHtml + "					<tr>";
            codeHtml = codeHtml + "					<td align=\"center\" style=\"padding: 40px 0 0 20px\" valign=\"top\" >";
            codeHtml = codeHtml + "							<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "								<tr style=\"height: 225px;\">";
            codeHtml = codeHtml + "									<td align=\"center\">";
            codeHtml = codeHtml + "										<table width=\"100\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td align=\"center\" padding=\"20px 0 0 0\">";
            codeHtml = codeHtml + "													<table align=\"center\" width=\"150\" heigth=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "														<tr>";
            codeHtml = codeHtml + "															<td align=\"center\" padding=\"0 0 0 0\"> <img src=\"" + baseUrlImages + "logo.png\" width=\"75\" border=\"0\" alt=\"habitat\" style=\"display:block; height:auto\" > </td>";
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
            codeHtml = codeHtml + "															<td height=\"34\" colspan=\"3\" valign=\"bottom\" style=\"text-align: left; font-family: dinhabbold; font-size: 65px; text-transform: uppercase\">" + chevalet.rangeChevalet + "</td>";
            codeHtml = codeHtml + "														</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "												</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td align=\"left\">";
            codeHtml = codeHtml + "													<table align=\"left\" width=\"700\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" >";
            codeHtml = codeHtml + "														<tr>";

            string descriptionRange = DAO.RangeDao.getDescriptionPlusByRangeID(RangeId, magasinId);

            codeHtml = codeHtml + "															<td height=\"50\" style=\"text-align: left; font-family: DINHabrg; font-size: 65px; color: #000000; \">" + descriptionRange + "</td>";
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
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "									</td>";
            codeHtml = codeHtml + "								</tr>";
            codeHtml = codeHtml + "							</table>";
            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "							<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "								<tr style=\"height: 525px;\">";
            codeHtml = codeHtml + "									<td align=\"left\" style=\"padding: 0 0 0 100px\">";
            codeHtml = codeHtml + "										<table>";
            codeHtml = codeHtml + "										<tr>";
            codeHtml = codeHtml + "											<td>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												<div class=\"divReglette\">";
            codeHtml = codeHtml + "										<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td>";
            codeHtml = codeHtml + "													<table width=\"500\" height=\"300\">";
            codeHtml = codeHtml + "													<tr width=\"400\" height=\"300\">";
            codeHtml = codeHtml + "												<td align=\"left\"> <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro1.Sku + ".png\" width=\"295\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\" type=\"image/png\"><img src=\"" + baseUrlImages + "blanc.png\" width=\"295\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> </object></td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "													<table width=\"350\" height=\"30\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">" + pro1.Variation + "</td>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "																	<table width=\"420\" height=\"35\">";
            codeHtml = codeHtml + "																		<tr>";
            codeHtml = codeHtml + "																			<td width=\"80\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">" + pro1.prixGauche + "</td>";
            codeHtml = codeHtml + "																			<td width=\"20\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">&nbsp;</td>";
            codeHtml = codeHtml + "																			<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">" + pro1.prixDroite + "</td>";
            codeHtml = codeHtml + "																			<td width=\"10\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">&nbsp;</td>";
            codeHtml = codeHtml + "																			<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">" + pro1.Nombre_colis + "</td>";
            codeHtml = codeHtml + "																		</tr>";
            codeHtml = codeHtml + "																	</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "													<table width=\"400\" height=\"18\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >" + pro1.EcoPart + "&nbsp;</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "												<table>";
            codeHtml = codeHtml + "											<tr width=\"400\" height=\"20\">";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
            codeHtml = codeHtml + "													<br> " + pro1.Dimenions + "  &nbsp;</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "										</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												</div>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "											</td>";
            codeHtml = codeHtml + "											<td>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												<div class=\"divReglette\">";
            codeHtml = codeHtml + "										<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td>";
            codeHtml = codeHtml + "													<table width=\"500\" height=\"300\">";
            codeHtml = codeHtml + "													<tr width=\"400\" height=\"300\">";
            codeHtml = codeHtml + "												<td align=\"left\"> <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro2.Sku + ".png\" width=\"295\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\" type=\"image/png\"><img src=\"" + baseUrlImages + "blanc.png\" width=\"295\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> </object></td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "													<table width=\"350\" height=\"30\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">" + pro2.Variation + "</td>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "																	<table width=\"420\" height=\"35\">";
            codeHtml = codeHtml + "																		<tr>";
            codeHtml = codeHtml + "																			<td width=\"80\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">" + pro2.prixGauche + "</td>";
            codeHtml = codeHtml + "																			<td width=\"20\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">&nbsp;</td>";
            codeHtml = codeHtml + "																			<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">" + pro2.prixDroite + "</td>";
            codeHtml = codeHtml + "																			<td width=\"10\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">&nbsp;</td>";
            codeHtml = codeHtml + "																			<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">" + pro2.Nombre_colis + "</td>";
            codeHtml = codeHtml + "																		</tr>";
            codeHtml = codeHtml + "																	</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "													<table width=\"400\" height=\"18\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >" + pro2.EcoPart + "&nbsp;</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "												<table>";
            codeHtml = codeHtml + "											<tr width=\"400\" height=\"20\">";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
            codeHtml = codeHtml + "													<br> " + pro2.Dimenions + "  &nbsp;</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "										</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												</div>";
            codeHtml = codeHtml + "											</td>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "										</table>";
            codeHtml = codeHtml + "									</td>";
            codeHtml = codeHtml + "								</tr>";
            codeHtml = codeHtml + "							</table>";
            codeHtml = codeHtml + "							<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "								<tr style=\"height: 530px;\">";
            codeHtml = codeHtml + "									<td align=\"left\" style=\"padding: 0 0 0 100px\">";
            codeHtml = codeHtml + "										<table>";
            codeHtml = codeHtml + "										<tr>";
            codeHtml = codeHtml + "											<td>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												<div class=\"divReglette\">";
            codeHtml = codeHtml + "										<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td>";
            codeHtml = codeHtml + "													<table width=\"500\" height=\"300\">";
            codeHtml = codeHtml + "													<tr width=\"400\" height=\"300\">";
            codeHtml = codeHtml + "												<td align=\"left\"> <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro3.Sku + ".png\" width=\"295\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\" type=\"image/png\"><img src=\"" + baseUrlImages + "blanc.png\" width=\"295\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> </object></td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "													<table width=\"350\" height=\"30\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">" + pro3.Variation + "</td>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "																	<table width=\"420\" height=\"35\">";
            codeHtml = codeHtml + "																		<tr>";
            codeHtml = codeHtml + "																			<td width=\"80\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">" + pro3.prixGauche + "</td>";
            codeHtml = codeHtml + "																			<td width=\"20\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">&nbsp;</td>";
            codeHtml = codeHtml + "																			<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">" + pro3.prixDroite + "</td>";
            codeHtml = codeHtml + "																			<td width=\"10\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">&nbsp;</td>";
            codeHtml = codeHtml + "																			<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">" + pro3.Nombre_colis + "</td>";
            codeHtml = codeHtml + "																		</tr>";
            codeHtml = codeHtml + "																	</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "													<table width=\"400\" height=\"18\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >" + pro3.EcoPart + "&nbsp;</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "												<table>";
            codeHtml = codeHtml + "											<tr width=\"400\" height=\"20\">";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
            codeHtml = codeHtml + "													<br> " + pro3.Dimenions + "  &nbsp;</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "										</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												</div>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "											</td>";
            codeHtml = codeHtml + "											<td>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												<div class=\"divReglette\">";
            codeHtml = codeHtml + "										<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" style=\"height: 525px;\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td>";
            codeHtml = codeHtml + "													<table width=\"500\" height=\"300\">";
            codeHtml = codeHtml + "													<tr width=\"400\" height=\"300\">";
            codeHtml = codeHtml + "												<td align=\"left\"> <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro4.Sku + ".png\" width=\"295\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\" type=\"image/png\"><img src=\"" + baseUrlImages + "blanc.png\" width=\"295\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> </object></td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "													<table width=\"350\" height=\"30\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">" + pro4.Variation + "</td>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "																	<table width=\"420\" height=\"35\">";
            codeHtml = codeHtml + "																		<tr>";
            codeHtml = codeHtml + "																			<td width=\"80\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">" + pro4.prixGauche + "</td>";
            codeHtml = codeHtml + "																			<td width=\"20\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">&nbsp;</td>";
            codeHtml = codeHtml + "																			<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">" + pro4.prixDroite + "</td>";
            codeHtml = codeHtml + "																			<td width=\"10\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">&nbsp;</td>";
            codeHtml = codeHtml + "																			<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">" + pro4.Nombre_colis + "</td>";
            codeHtml = codeHtml + "																		</tr>";
            codeHtml = codeHtml + "																	</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "													<table width=\"400\" height=\"18\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >" + pro4.EcoPart + "&nbsp;</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "												<table>";
            codeHtml = codeHtml + "											<tr width=\"400\" height=\"20\">";
            codeHtml = codeHtml + "												<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
            codeHtml = codeHtml + "													<br> " + pro4.Dimenions + "  &nbsp;</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "										</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "												</div>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "											</td>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "										</table>";
            codeHtml = codeHtml + "									</td>";
            codeHtml = codeHtml + "								</tr>";
            codeHtml = codeHtml + "							</table>";
            codeHtml = codeHtml + "			";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "							<div class=\"footer\">";
            codeHtml = codeHtml + "										<table align=\"left\" width=\"1200\" valign=\"top\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
            codeHtml = codeHtml + "											<tr style=\"height: 125px;\">";
            codeHtml = codeHtml + "												<td style=\"padding:120px 0 0 50px\">";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "														<table align=\"left\" width=\"1000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
            codeHtml = codeHtml + "															<tr>";
            codeHtml = codeHtml + "														<td align=\"center\" style=\"padding:10px 0 0 0\" width=\"300\"><hr width=\"300\"> </td>";
            codeHtml = codeHtml + "														<td height=\"10\" valign=\"bottom\" style=\"text-align: center; font-family: dinhabbold; font-size: 22px;\" width=\"400\">" + texteDur2 + "</td>";
            codeHtml = codeHtml + "														<td align=\"center\" style=\"padding:10px 0 0 0\" width=\"300\"><hr width=\"300\"> </td>";
            codeHtml = codeHtml + "															</tr>";
            codeHtml = codeHtml + "														</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "														<table align=\"left\" width=\"1000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" >";
            codeHtml = codeHtml + "														<tr>";
            codeHtml = codeHtml + "														<td width=\"75\" align=\"center\" style=\"padding:20px 0 0 40px\"> " + imageMadeIn + " </td>";
            codeHtml = codeHtml + "														<td width=\"800\" align=\"right\" style=\"text-align: right; font-family: DINHabRg; font-size: 20px; padding: 20px 0 0px 0px\">" + texteDur3 + "";
            codeHtml = codeHtml + "														<br><strong>" + texteDur4 + "</strong> </td>";
            codeHtml = codeHtml + "														<td width=\"55\" align=\"left\"><img src=\"" + baseUrlImages + "Camion_livraison.png\" width=\"80\" border=\"0\" alt=\"camion\" title=\"camion livraison\" style=\"display:block; padding: 20px 0 0px 10px\"></td>";
            codeHtml = codeHtml + "														</tr>";
            codeHtml = codeHtml + "														</table>";
            codeHtml = codeHtml + "													</td>";
            codeHtml = codeHtml + "												</tr>";
            codeHtml = codeHtml + "										</table>";
            codeHtml = codeHtml + "									</div>";
            codeHtml = codeHtml + "						";
            codeHtml = codeHtml + "						</td>";
            codeHtml = codeHtml + "					</tr>";
            codeHtml = codeHtml + "				</table>";
            codeHtml = codeHtml + "			</td>";
            codeHtml = codeHtml + "			";
            codeHtml = codeHtml + "			<td align=\"center\" style=\"padding: 40px 0 0 20px\" valign=\"top\" >";
            codeHtml = codeHtml + "				<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\" style=\"height: 910px;\">";
            codeHtml = codeHtml + "					<tr>";
            codeHtml = codeHtml + "					<td align=\"center\" style=\"padding: 40px 0 0 20px\" valign=\"top\" >";
            codeHtml = codeHtml + "							<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "								<tr style=\"height: 225px;\">";
            codeHtml = codeHtml + "									<td align=\"center\">";
            codeHtml = codeHtml + "										<table width=\"100\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" class=\"full\">";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td align=\"center\" padding=\"20px 0 0 0\">";
            codeHtml = codeHtml + "													<table align=\"center\" width=\"150\" heigth=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "														<tr>";
            codeHtml = codeHtml + "															<td align=\"center\" padding=\"0 0 0 0\"> <img src=\"" + baseUrlImages + "logo.png\" width=\"75\" border=\"0\" alt=\"habitat\" style=\"display:block; height:auto\" > </td>";
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
            codeHtml = codeHtml + "															<td height=\"34\" colspan=\"3\" valign=\"bottom\" style=\"text-align: left; font-family: dinhabbold; font-size: 65px; text-transform: uppercase\">" + chevalet.rangeChevalet + "</td>";
            codeHtml = codeHtml + "														</tr>";
            codeHtml = codeHtml + "													</table>";
            codeHtml = codeHtml + "												</td>";
            codeHtml = codeHtml + "											</tr>";
            codeHtml = codeHtml + "											<tr>";
            codeHtml = codeHtml + "												<td align=\"left\">";
            codeHtml = codeHtml + "													<table align=\"left\" width=\"700\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" >";
            codeHtml = codeHtml + "														<tr>";
            codeHtml = codeHtml + "															<td height=\"50\" style=\"text-align: left; font-family: DINHabrg; font-size: 65px; color: #000000; \">" + descriptionRange + "</td>";
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
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "									</td>";
            codeHtml = codeHtml + "								</tr>";
            codeHtml = codeHtml + "							</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "							<table width=\"1200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" style=\"padding-top: 20px;\">";
            codeHtml = codeHtml + "										<tr style=\"height: 1175px;\">";
            codeHtml = codeHtml + "										";
            codeHtml = codeHtml + "													<td valign=\"top\" style=\"padding: 50px 30px 60px 0px\">";
            codeHtml = codeHtml + "														<table align=\"center\" width=\"800\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "															<tr>";
            codeHtml = codeHtml + "																<td width=\"800\" style=\"vertical-align: top;text-align: left; font-family: DINHabRg; font-size: 32px; text-align: justify; padding: 0 0\">" + comp + "</td>";
            codeHtml = codeHtml + "															</tr>";
            codeHtml = codeHtml + "														</table>";
            codeHtml = codeHtml + "													</td>";
            codeHtml = codeHtml + "												</tr>";
            codeHtml = codeHtml + "											";
            codeHtml = codeHtml + "							</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "							<div class=\"footer\">";
            codeHtml = codeHtml + "										<table align=\"left\" width=\"1200\" valign=\"top\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
            codeHtml = codeHtml + "											<tr style=\"height: 125px;\">";
            codeHtml = codeHtml + "												<td style=\"padding:50px 0 0 50px\">";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "														<table align=\"left\" width=\"1000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
            codeHtml = codeHtml + "															<tr>";
            codeHtml = codeHtml + "														<td align=\"center\" style=\"padding:10px 0 0 0\" width=\"300\"><hr width=\"300\"> </td>";
            codeHtml = codeHtml + "														<td height=\"10\" valign=\"bottom\" style=\"text-align: center; font-family: dinhabbold; font-size: 22px;\" width=\"400\">" + texteDur2 + "</td>";
            codeHtml = codeHtml + "														<td align=\"center\" style=\"padding:10px 0 0 0\" width=\"300\"><hr width=\"300\"> </td>";
            codeHtml = codeHtml + "															</tr>";
            codeHtml = codeHtml + "														</table>";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "														<table align=\"left\" width=\"1000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" >";
            codeHtml = codeHtml + "														<tr>";
            codeHtml = codeHtml + "														<td width=\"75\" align=\"center\" style=\"padding:20px 0 0 40px\">" + imageMadeIn + "</td>";
            codeHtml = codeHtml + "														<td width=\"800\" align=\"right\" style=\"text-align: right; font-family: DINHabRg; font-size: 20px; padding: 20px 0 0px 0px\">Le " + texteDur3 + "";
            codeHtml = codeHtml + "														<br><strong>" + texteDur4 + "</strong> </td>";
            codeHtml = codeHtml + "														<td width=\"55\" align=\"left\"><img src=\"" + baseUrlImages + "Camion_livraison.png\" width=\"80\" border=\"0\" alt=\"camion\" title=\"camion livraison\" style=\"display:block; padding: 20px 0 0px 10px\"></td>";
            codeHtml = codeHtml + "														</tr>";
            codeHtml = codeHtml + "														</table>";
            codeHtml = codeHtml + "													</td>";
            codeHtml = codeHtml + "												</tr>";
            codeHtml = codeHtml + "										</table>";
            codeHtml = codeHtml + "									</div>";
            codeHtml = codeHtml + "						</td>";
            codeHtml = codeHtml + "					</tr>";
            codeHtml = codeHtml + "				</table>";
            codeHtml = codeHtml + "						</td>";
            codeHtml = codeHtml + "					</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "	</body>";
            codeHtml = codeHtml + "</html>";
            return codeHtml;
        }

        static public List<T_Produit_A4> getTableauProduitsFromChevalet(TickitDataChevalet chevalet, int magasinId, int RangeId)
        {
            List<T_Produit_A4> listeProduitsA4 = new List<T_Produit_A4>();

            T_Produit_A4 pro1 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro2 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro3 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro4 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro5 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro6 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro7 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro8 = Models.T_Produit_A4.initializeProduit();

            listeProduitsA4.Add(pro1);
            listeProduitsA4.Add(pro2);
            listeProduitsA4.Add(pro3);
            listeProduitsA4.Add(pro4);
            listeProduitsA4.Add(pro5);
            listeProduitsA4.Add(pro6);
            listeProduitsA4.Add(pro7);
            listeProduitsA4.Add(pro8);

            string texte_colis = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A5_TEXTE_DUR_6", magasinId);

            string APartirDe = "";
            if (DAO.RangeDao.isRangeBarAtissu(RangeId))
            {
                APartirDe = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A6_TEXTE_DUR_5", magasinId);
            }

            int i = 0;
            foreach (TickitDataProduit data in chevalet.produitsData)
            {
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

                listeProduitsA4[i].Sku = data.sku;
                listeProduitsA4[i].Variation = data.variation;
                listeProduitsA4[i].Orientation = DAO.ProduitDao.getOrientationBySku(data.sku, magasinId);
                listeProduitsA4[i].APartirDe = APartirDe;
                listeProduitsA4[i].prixGauche = prixGauche.Replace(".00", "");
                listeProduitsA4[i].prixDroite = prixDroite.Replace(".00", "");
                listeProduitsA4[i].EcoPart = data.Taxe_eco;
                listeProduitsA4[i].Dimenions = data.dimension;
                listeProduitsA4[i].DimensionsDeplie = DAO.ProduitDao.getDrescriptionConvertibleBySku(data.sku, magasinId, "L");
                listeProduitsA4[i].DimensionsCouchage = DAO.ProduitDao.getDrescriptionConvertibleBySku(data.sku, magasinId, "C");


                if (data.Nombre_colis != null && data.Nombre_colis != "" && data.Nombre_colis != "0")
                {
                    listeProduitsA4[i].Nombre_colis = data.Nombre_colis + texte_colis;
                }

                i++;
            }

            return listeProduitsA4;
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