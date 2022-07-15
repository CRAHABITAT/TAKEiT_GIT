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
    public static class PlvChevaletA5meubleUtils
    {
        /// Fonction permetant de génerer le document PDF pour l impression format reglette carré
        /// 18/07/2020
        /// Mehdi SRIDI
        /// <param name="chevalet"></param>
        /// <param name="format"></param>
        /// <param name="magasinId"></param>
        /// <param name="dateQuery"></param>
        /// <returns></returns>
        static public string getHtmlA5(TickitDataChevalet chevalet, String format, int magasinId, DateTime dateQuery)
        {
            string baseUrlFonts = "http://ean.habitat.fr/TAKEIT/webfonts/";
            string baseUrlImages = "http://ean.habitat.fr/TAKEIT/A5/images/";

            string prefixFilaire = "filaire_A5_";

            int RangeId = Managers.ChevaletManager.getRangeIDOfChevalet(chevalet);

            List<string> listPlus = new List<string>();
            listPlus = DAO.RangeDao.getPlusByRangeId(RangeId, magasinId);

            bool isbarATissu = DAO.RangeDao.isRangeBarAtissu(RangeId);

            List<T_Produit_A4> lista = getTableauProduitsFromChevalet(chevalet, magasinId, RangeId, dateQuery);

            T_Produit_A4 pro1 = lista[0];
            T_Produit_A4 pro2 = lista[1];
            T_Produit_A4 pro3 = lista[2];
            T_Produit_A4 pro4 = lista[3];
            T_Produit_A4 pro5 = lista[4];
            T_Produit_A4 pro6 = lista[5];
            //T_Produit_A4 pro7 = lista[6];
            //T_Produit_A4 pro8 = lista[7];

            string texteDur1 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A5_TEXTE_DUR_1", magasinId);
            string texteDur2 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A5_TEXTE_DUR_2", magasinId);
            string texteDur3 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A5_TEXTE_DUR_3", magasinId);
            string texteDur4 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A5_TEXTE_DUR_4", magasinId);

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

            string comp1 = DAO.RangeDao.getDescriptionCompositionByRangeId(RangeId, magasinId, 1);
            string comp2 = DAO.RangeDao.getDescriptionCompositionByRangeId(RangeId, magasinId, 2);


            if (madeIn != "")
            {
                imageMadeIn = "<object data=\"" + baseUrlImages + "Flag_" + madeIn + ".png\" width=\"75\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\"><img src=\"" + baseUrlImages + "blanc.png\" width=\"75\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\"/> </object>";
            }


                string codeHtml = "";
                codeHtml = codeHtml + "<!DOCTYPE html>";
                codeHtml = codeHtml + "<html lang=\"fr\">";
                codeHtml = codeHtml + "	<head>";
                codeHtml = codeHtml + "		<meta charset=\"UTF-8\">";
                codeHtml = codeHtml + "		<title>Habitat</title>";
                codeHtml = codeHtml + "		<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">";
                codeHtml = codeHtml + "		<style>";
                codeHtml = codeHtml + "			.divReglette";
                codeHtml = codeHtml + "			{";
                codeHtml = codeHtml + "				grid-area: auto;";
                codeHtml = codeHtml + "				width: 650px;";
                codeHtml = codeHtml + "				height: 400px;";
                codeHtml = codeHtml + "				margin: 0px;";
                codeHtml = codeHtml + "				padding: 0px;";
                codeHtml = codeHtml + "				background-repeat: no-repeat;";
                codeHtml = codeHtml + "				background-size: cover;";
                codeHtml = codeHtml + "			}";
                codeHtml = codeHtml + "";
                codeHtml = codeHtml + "			.promo_rose";
                codeHtml = codeHtml + "			{";
                codeHtml = codeHtml + "				background-image: url(" + baseUrlImages + "pastille_rose.png);";
                codeHtml = codeHtml + "				background-repeat: no-repeat;";
                codeHtml = codeHtml + "				background-origin: border-box;";
                codeHtml = codeHtml + "				padding-top: 18px;";
                codeHtml = codeHtml + "				padding-right: 11px;";
                codeHtml = codeHtml + "				background-size: 80%;";
                codeHtml = codeHtml + "			}";
                codeHtml = codeHtml + "";
                //Cillia 12/05/22 (pastille bleue pour les promo avec la carte habitat)

                codeHtml = codeHtml + "	.promo_bleue {";
                codeHtml = codeHtml + "		background-image: url(" + baseUrlImages + "pastille_bleue.png);";
                codeHtml = codeHtml + "		background-repeat: no-repeat;";
                codeHtml = codeHtml + "		background-origin: border-box;";
                codeHtml = codeHtml + "		padding-top: 18px;";
                codeHtml = codeHtml + "		padding-right: 11px;";
                codeHtml = codeHtml + "		background-size: 80%;";
                codeHtml = codeHtml + "	}";
                codeHtml = codeHtml + "	";
               //
                codeHtml = codeHtml + "			#flag_EU";
                codeHtml = codeHtml + "			{";
                codeHtml = codeHtml + "				background-image: url(" + baseUrlImages + "flag_" + madeIn + ".jpg);";
                codeHtml = codeHtml + "				background-repeat: no-repeat;";
                codeHtml = codeHtml + "				background-origin: border-box;";
                codeHtml = codeHtml + "				background-size: 90%;";
                codeHtml = codeHtml + "			}";
                codeHtml = codeHtml + "";
                codeHtml = codeHtml + "			.filet";
                codeHtml = codeHtml + "			{";
                codeHtml = codeHtml + "				background-image: url(" + baseUrlImages + "filet.jpg);";
                codeHtml = codeHtml + "				background-repeat: no-repeat;";
                codeHtml = codeHtml + "				background-size: contain;";
                codeHtml = codeHtml + "			}";
                codeHtml = codeHtml + "";
                codeHtml = codeHtml + "			html, body";
                codeHtml = codeHtml + "			{";
                codeHtml = codeHtml + "				height: 100%;";
                codeHtml = codeHtml + "			}";
                codeHtml = codeHtml + "";
                codeHtml = codeHtml + "			.contenu";
                codeHtml = codeHtml + "			{";
                codeHtml = codeHtml + "				min-height: 100%;";
                codeHtml = codeHtml + "				position: relative;";
                codeHtml = codeHtml + "			}";
                codeHtml = codeHtml + "";
                codeHtml = codeHtml + "			footer";
                codeHtml = codeHtml + "			{";
                codeHtml = codeHtml + "				height: 1px;";
                codeHtml = codeHtml + "				/* à titre d'exemple */";
                codeHtml = codeHtml + "				position: absolute;";
                codeHtml = codeHtml + "				bottom: 0px;";
                codeHtml = codeHtml + "			}";
                codeHtml = codeHtml + "		</style>";
                codeHtml = codeHtml + "		<style type=\"text/css\">";
                codeHtml = codeHtml + "			@font-face";
                codeHtml = codeHtml + "			{";
                codeHtml = codeHtml + "				font-family: 'dinhabbold';";
                codeHtml = codeHtml + "				src: url('" + baseUrlFonts + "DINHabBd.ttf') format('truetype');";
                codeHtml = codeHtml + "			}";
                codeHtml = codeHtml + "";
                codeHtml = codeHtml + "			@font-face";
                codeHtml = codeHtml + "			{";
                codeHtml = codeHtml + "				font-family: 'DINHabRg';";
                codeHtml = codeHtml + "				src: url('" + baseUrlFonts + "/DINHabRg.ttf') format('truetype');";
                codeHtml = codeHtml + "			}";
                codeHtml = codeHtml + "		</style>";
                codeHtml = codeHtml + "	</head>";
                codeHtml = codeHtml + "	<body>";
                codeHtml = codeHtml + "		<table  width=\"2000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "			<tr>";
                codeHtml = codeHtml + "				<td align=\"center\" style=\"padding: 10px 0 0 20px\" >";
                codeHtml = codeHtml + "					<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"center\">";
                codeHtml = codeHtml + "								<table width=\"100\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" class=\"full\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"center\" padding=\"120px 0 0 0\">";
                codeHtml = codeHtml + "											<table align=\"center\" width=\"150\" heigth=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td align=\"center\" padding=\"0 0 0 0\">";
                codeHtml = codeHtml + "														<img src=\"" + baseUrlImages + "logo.png\" width=\"75\" border=\"0\" alt=\"habitat\" style=\"display:block height:auto\" class=\"full\">";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "								<table width=\"300\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" class=\"full\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"left\">";
                codeHtml = codeHtml + "											<table align=\"left\" width=\"500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" class=\"deviceWidth\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td width=\"500\" height=\"34\" colspan=\"3\" valign=\"bottom\" style=\"text-align: left; font-family: dinhabbold; font-size: 62px; text-transform: uppercase\">";
                codeHtml = codeHtml + "														 "+ chevalet.rangeChevalet+" ";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"left\">";
                codeHtml = codeHtml + "											<table align=\"left\" width=\"900\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"deviceWidth\">";
                codeHtml = codeHtml + "												<tr>";
                string descriptionRange = DAO.RangeDao.getDescriptionPlusByRangeID(RangeId, magasinId);
                
                codeHtml = codeHtml + "													<td height=\"50\" style=\"text-align: left; font-family: DINHabrg; font-size: 50px; color: #000000; \">";
                codeHtml = codeHtml + "														"+ descriptionRange +" ";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"center\" style=\"padding: 20px 0 20px 0\">";
                codeHtml = codeHtml + "											<table align=\"left\" width=\"1000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"deviceWidth\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td class=\"filet\" width=\"1%\" rowspan=\"4\"></td>";
                codeHtml = codeHtml + "													<td width=\"3%\" rowspan=\"4\"></td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td width=\"600\" style=\"text-align: left; font-family: DINHabRg; font-size: 38px;\">";
                codeHtml = codeHtml + "														 &nbsp;" + plus1 + "  &nbsp;";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td width=\"600\" style=\"text-align: left; font-family: DINHabRg; font-size: 38px;\">";
                codeHtml = codeHtml + "														 &nbsp;" + plus2 + " ";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td width=\"600\" style=\"text-align: left; font-family: DINHabRg; font-size: 38px;\">";
                codeHtml = codeHtml + "														 &nbsp;" + plus3 + " ";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "								<table width=\"250\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"center\" style=\"padding: 40px 0 0 100px\">";
                codeHtml = codeHtml + "											<table align=\"center\" width=\"200\" heigth=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" valign=\"top\">";
                codeHtml = codeHtml + "												<tr>";
                
                
                string typePastille = "";
                
                 //cillia 12/05/22
                //string typeTarifCbr="HABHFR";
               // chevalet.typeTarifCbr = "";

             if ((pro1.typeTarifCbr == "HABHFR") && (pro2.typeTarifCbr =="" || pro2.typeTarifCbr == "HABHFR") && (pro3.typeTarifCbr == "" || pro3.typeTarifCbr == "HABHFR")
                    && (pro4.typeTarifCbr =="" || pro4.typeTarifCbr == "HABHFR") && (pro5.typeTarifCbr == "" || pro5.typeTarifCbr == "HABHFR")
                    && (pro6.typeTarifCbr == "" || pro6.typeTarifCbr == "HABHFR") && (chevalet.typePrix == ApplicationConsts.typePrix_promo))
                //if ((pro1.typeTarifCbr == "HABHFR") && (pro2.typeTarifCbr == "" || pro2.typeTarifCbr == "HABHFR") && (chevalet.typePrix == ApplicationConsts.typePrix_promo))
                { typePastille = ApplicationConsts.typePastillePromoHab; }

                else if (chevalet.typePrix == ApplicationConsts.typePrix_demarqueLocale || chevalet.typePrix == ApplicationConsts.typePrix_promo) { typePastille = ApplicationConsts.typePastillePromoReglette; }
                else if (chevalet.typePrix == ApplicationConsts.typePrix_solde) { typePastille = ApplicationConsts.typePastilleSoldeReglette; }
                
                string pourcentagetexte = "";
                if (chevalet.pourcentageReduction != null)
                {
                    pourcentagetexte = "-" + chevalet.pourcentageReduction + "%";
                }
                
                
                codeHtml = codeHtml + "													<td width=\"200\" height=\"200\" align=\"center\" valign=\"top\" class=\"" + typePastille + "\" style=\"text-align: center; font-family: dinhabbold; font-size: 40pt; color: #FFFFFF; line-height: 120px;\">";
                codeHtml = codeHtml + "														" + pourcentagetexte + " &nbsp;&nbsp;";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<! -- ligne 2 -- >";
                codeHtml = codeHtml + "					<!-- module 4 colonnes -->";
                codeHtml = codeHtml + "					<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"left\" style=\"padding: 0 0 0 100px\">";
                codeHtml = codeHtml + "								<table>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 1 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + "                                                                 <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro1.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0px 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro1.Sku + ".png" +"\"  width=\"250\" height=\"295\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																		" + pro1.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
                if (pro1.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro1.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro1.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
            else {
                codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                codeHtml = codeHtml + "																		 " + pro1.prixGauche + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                codeHtml = codeHtml + "																		" + pro1.prixDroite + "";
                codeHtml = codeHtml + "																	</td>";
            }

                codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                codeHtml = codeHtml + "																		 "+ pro1.Nombre_colis +" &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																		" + pro1.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		 " + pro1.Dimenions + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro1.DimensionsCouchage + " &nbsp;";																
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 1  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 2 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + "                                                                 <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro2.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0px 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro2.Sku + ".png" + "\"  width=\"200\" height=\"295\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																		" + pro2.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
                if (pro2.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro2.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro2.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                else
                {
                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                    codeHtml = codeHtml + "																		 " + pro2.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		 " + pro2.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                codeHtml = codeHtml + "																		"+ pro2.Nombre_colis +" &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																	" + pro2.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		" + pro2.Dimenions + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro2.DimensionsCouchage + " &nbsp;";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 2  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<!--/module 4 colonnes -->";
                codeHtml = codeHtml + "					<! --/ ligne 2 -- >";
                codeHtml = codeHtml + "					<! -- ligne 2 -- >";
                codeHtml = codeHtml + "					<!-- module 4 colonnes -->";
                codeHtml = codeHtml + "					<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"left\" style=\"padding: 0 0 0 100px\">";
                codeHtml = codeHtml + "								<table>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 1 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + "                                                                 <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro3.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0px 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro3.Sku + ".png" + "\"  width=\"200\" height=\"200\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																		" + pro3.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
                if (pro3.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro3.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro3.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                else
                {
                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                    codeHtml = codeHtml + "																		 " + pro3.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		 " + pro3.prixDroite + " ";
                    codeHtml = codeHtml + "																	</td>";
                }
                codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                codeHtml = codeHtml + "																		"+ pro3.Nombre_colis +" &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																		" + pro3.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		" + pro3.Dimenions + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro3.DimensionsCouchage + " &nbsp;";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 1  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 2 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + "                                                                 <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro4.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0px 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro4.Sku + ".png" + "\"  width=\"200\" height=\"200\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																		" + pro4.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
                if (pro4.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro4.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro4.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                else
                {
                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                    codeHtml = codeHtml + "																		 " + pro4.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		 " + pro4.prixDroite + " ";
                    codeHtml = codeHtml + "																	</td>";
                }
                codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                codeHtml = codeHtml + "																	"+ pro4.Nombre_colis +" &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																		" + pro4.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		" + pro4.Dimenions + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro4.DimensionsCouchage + " &nbsp;";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 2  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<!--/module 4 colonnes -->";
                codeHtml = codeHtml + "					<!-- module 4 colonnes -->";
                codeHtml = codeHtml + "					<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"left\" style=\"padding: 0 0 0 100px\">";
                codeHtml = codeHtml + "								<table>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 1 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + "	                                                                    <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro5.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0px 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro5.Sku + ".png" + "\"  width=\"300\" height=\"295\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																		" + pro5.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
                if (pro5.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro5.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro5.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                else
                {
                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                    codeHtml = codeHtml + "																		 " + pro5.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		 " + pro5.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                codeHtml = codeHtml + "																		"+ pro5.Nombre_colis +" &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																		" + pro5.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		 " + pro5.Dimenions + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro5.DimensionsCouchage + " &nbsp;";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 1  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 2 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + " 																<object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro6.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0px 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro6.Sku + ".png" + "\"  width=\"300\" height=\"295\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																		" + pro6.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
                  if (pro6.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro6.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro6.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
            else {
                codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                codeHtml = codeHtml + "																		 " + pro6.prixGauche + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                codeHtml = codeHtml + "																		 " + pro6.prixDroite + " ";
                codeHtml = codeHtml + "																	</td>";
                  }
                codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                codeHtml = codeHtml + "																		"+ pro6.Nombre_colis +" &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																		" + pro6.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		" + pro6.Dimenions + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro6.DimensionsCouchage + " &nbsp;";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 2  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<!--/module 4 colonnes -->";
                codeHtml = codeHtml + "					<! --/ ligne 2 -- >";
                codeHtml = codeHtml + "					<table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td style=\"font-size: 1px; padding-bottom: 2px\"></td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<table width=\"1500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" style=\"padding-top: 2px;\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"center\">";
                codeHtml = codeHtml + "								<tr>";
                codeHtml = codeHtml + "									<td align=\"center\" style=\"padding: 0px 0 0 100px\">";
                codeHtml = codeHtml + "										<table align=\"left\" width=\"450\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td width=\"450\" style=\"text-align: left; font-family: DINHabbold; font-size: 38px; text-align: justify; padding: 0 0 0 0\">";
                codeHtml = codeHtml + "													  " + texteDur1 + " ";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "									</td>";
                codeHtml = codeHtml + "								</tr>";
                codeHtml = codeHtml + "								<tr>";
                codeHtml = codeHtml + "									<td align=\"center\" style=\"padding: 0 0 0 100px\">";
                codeHtml = codeHtml + "										<table align=\"left\" width=\"1300\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td width=\"1300\" style=\"vertical-align: top;text-align: left; font-family: DINHabRg; font-size: 22px; text-align: justify; padding: 0 0\">";
                codeHtml = codeHtml + "													<br>";
                codeHtml = codeHtml + "													" + comp1 +""+comp2+  "</td> ";
                //codeHtml = codeHtml + "											   <td width=\"1300\" style=\"vertical-align: top;text-align: left; font-family: DINHabRg; font-size: 22px; text-align: justify; padding: 0 20px\">";
                //codeHtml = codeHtml + "												<br>" + comp2 + "</td>";
                //codeHtml = codeHtml + "													 Fixe existe en plusieurs versions, tissu A Ancio : composé de 77% de polyester, 17% d’acrylique, 6% de coton. ";
                //codeHtml = codeHtml + "													<strong>Structure :</strong>";
                //codeHtml = codeHtml + "													 En bois de pin, panneaux de particules et contreplaqué. ";
                //codeHtml = codeHtml + "													<strong>Coussin de l’assise :</strong>";
                //codeHtml = codeHtml + "													 Mousse haute résilience (densité de 30 et 35 kg/m3). ";
                //codeHtml = codeHtml + "													<strong>Coussin du dossier :</strong>";
                //codeHtml = codeHtml + "													 Coussin du dossier : 70% de mousse, 30% de plume de canard.";
                //codeHtml = codeHtml + "													<strong>Assise :</strong>";
                //codeHtml = codeHtml + "													 Suspension de l’assise à ressorts zig-zag. H. de l’assise 37 P. de l’assise 62 ";
                //codeHtml = codeHtml + "													<strong>Dossier :</strong>";
                //codeHtml = codeHtml + "													 Composé de sangles élastiques entrecroisées. ";
                //codeHtml = codeHtml + "													<strong>Pieds :</strong>";
                //codeHtml = codeHtml + "													 En acier brossé d’une hauteur de 18 cm. ";
                //codeHtml = codeHtml + "													<strong>Conseil d’entretien :</strong>";
                //codeHtml = codeHtml + "													 Nettoyage professionnel recommandé. ";
                //codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "									</td>";
                codeHtml = codeHtml + "								</tr>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<div class=\"footer\">";
                codeHtml = codeHtml + "						<table align=\"left\" width=\"1500\" valign=\"top\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "							<tr>";
                codeHtml = codeHtml + "								<td style=\"padding:5px 0 0 0\">";
                codeHtml = codeHtml + "									<!-- -- trait -- -->";
                codeHtml = codeHtml + "									<table align=\"left\" width=\"1500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "										<tr>";
                codeHtml = codeHtml + "											<td align=\"center\" style=\"padding:5px 0 0 0\" width=\"400\">";
                codeHtml = codeHtml + "												<hr width=\"400\">";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "											<td height=\"10\" valign=\"bottom\" style=\"text-align: center; font-family: dinhabbold; font-size: 18px;\" width=\"250\">";
                codeHtml = codeHtml + "												 " + texteDur2 + " ";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "											<td align=\"center\" style=\"padding:5px 0 0 0\" width=\"400\">";
                codeHtml = codeHtml + "												<hr width=\"400\">";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "										</tr>";
                codeHtml = codeHtml + "									</table>";
                codeHtml = codeHtml + "									<! --/ trait -- >";
                codeHtml = codeHtml + "									<table align=\"left\" width=\"1500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" >";
                codeHtml = codeHtml + "										<tr>";
                codeHtml = codeHtml + "											<td width=\"75\" align=\"center\" style=\"padding:0 0 0 40px\">";
                codeHtml = codeHtml + "												" + imageMadeIn +" ";
                            //<img src=\"img/drapeau_format_reglette2.png\" width=\"75\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\">";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "											<td width=\"700\" align=\"right\" style=\"text-align: right; font-family: DINHabRg; font-size: 20px; padding: 0px 0 0px 0px\">";
                codeHtml = codeHtml + "												" + texteDur3 + " ";
                codeHtml = codeHtml + "												<br>";
                codeHtml = codeHtml + "												<strong>" + texteDur4 + " </strong>";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "											<td width=\"55\" align=\"left\">";
                codeHtml = codeHtml + "												<img src=\"" + baseUrlImages + "Camion_livraison.png\" width=\"80\" border=\"0\" alt=\"camion\" title=\"camion livraison\" style=\"display:block; padding: 0px 0 0px 10px\">";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "										</tr>";
                codeHtml = codeHtml + "									</table>";
                codeHtml = codeHtml + "									<!--/ wrapper -->";
                codeHtml = codeHtml + "								</td>";
                codeHtml = codeHtml + "							</tr>";
                codeHtml = codeHtml + "						</table>";
                codeHtml = codeHtml + "					</div>";
                codeHtml = codeHtml + "				</td>";
                codeHtml = codeHtml + "				<td align=\"center\" style=\"padding: 10px 0 0 20px\" >";
                codeHtml = codeHtml + "					<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"center\">";
                codeHtml = codeHtml + "								<table width=\"100\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" class=\"full\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"center\" padding=\"120px 0 0 0\">";
                codeHtml = codeHtml + "											<table align=\"center\" width=\"150\" heigth=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td align=\"center\" padding=\"0 0 0 0\">";
                codeHtml = codeHtml + "														<img src=\"" + baseUrlImages + "logo.png\" width=\"75\" border=\"0\" alt=\"habitat\" style=\"display:block height:auto\" class=\"full\">";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "								<table width=\"300\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" class=\"full\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"left\">";
                codeHtml = codeHtml + "											<table align=\"left\" width=\"500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" class=\"deviceWidth\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td height=\"34\" colspan=\"3\" valign=\"bottom\" style=\"text-align: left; font-family: dinhabbold; font-size: 62px; text-transform: uppercase\">";
                codeHtml = codeHtml + "														" + chevalet.rangeChevalet + " ";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"left\">";
                codeHtml = codeHtml + "											<table align=\"left\" width=\"900\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"deviceWidth\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td height=\"50\" style=\"text-align: left; font-family: DINHabrg; font-size: 50px; color: #000000; \">";
                codeHtml = codeHtml + "														" + descriptionRange + " ";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"center\" style=\"padding: 20px 0 20px 0\">";
                codeHtml = codeHtml + "											<table align=\"left\" width=\"1000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"deviceWidth\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td class=\"filet\" width=\"1%\" rowspan=\"4\"></td>";
                codeHtml = codeHtml + "													<td width=\"3%\" rowspan=\"4\"></td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td width=\"600\" style=\"text-align: left; font-family: DINHabRg; font-size: 38px;\">";
                codeHtml = codeHtml + "														  &nbsp;" + plus1 + "  &nbsp;";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td width=\"600\" style=\"text-align: left; font-family: DINHabRg; font-size: 38px;\">";
                codeHtml = codeHtml + "														  &nbsp;" + plus2 + " ";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td width=\"600\" style=\"text-align: left; font-family: DINHabRg; font-size: 38px;\">";
                codeHtml = codeHtml + "														  &nbsp;" + plus3 + " ";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "								<table width=\"250\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td align=\"center\" style=\"padding: 40px 0 0 100px\">";
                codeHtml = codeHtml + "											<table align=\"center\" width=\"200\" heigth=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" valign=\"top\">";
                codeHtml = codeHtml + "												<tr>";
                codeHtml = codeHtml + "													<td width=\"200\" height=\"200\" align=\"center\" valign=\"top\" class=\"" + typePastille + "\" style=\"text-align: center; font-family: dinhabbold; font-size: 40pt; color: #FFFFFF; line-height: 120px;\">      ";
                codeHtml = codeHtml + "														" + pourcentagetexte + " &nbsp;&nbsp;";
                codeHtml = codeHtml + "													</td>";
                codeHtml = codeHtml + "												</tr>";
                codeHtml = codeHtml + "											</table>";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<! -- ligne 2 -- >";
                codeHtml = codeHtml + "					<!-- module 4 colonnes -->";
                codeHtml = codeHtml + "					<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"left\" style=\"padding: 0 0 0 100px\">";
                codeHtml = codeHtml + "								<table>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 1 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + "                                                                 <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro1.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0px 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro1.Sku + ".png" + "\"  width=\"250\" height=\"295\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																		" + pro1.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
                if (pro1.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro1.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro1.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                else
                {

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                    codeHtml = codeHtml + "																		 " + pro1.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		 " + pro1.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                codeHtml = codeHtml + "																		"+ pro1.Nombre_colis +" &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																		" + pro1.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		" + pro1.Dimenions + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro1.DimensionsCouchage + " &nbsp;";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 1  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 2 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + "                                                                 <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro2.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0px 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro2.Sku + ".png" +"\"  width=\"200\" height=\"295\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																		" + pro2.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
                if (pro2.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro2.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro2.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                else
                {
                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                    codeHtml = codeHtml + "																		 " + pro2.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		 " + pro2.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                codeHtml = codeHtml + "																		"+ pro2.Nombre_colis +" &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																		" + pro2.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		" + pro2.Dimenions + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro2.DimensionsCouchage + " &nbsp;";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 2  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<!--/module 4 colonnes -->";
                codeHtml = codeHtml + "					<! --/ ligne 2 -- >";
                codeHtml = codeHtml + "					<! -- ligne 2 -- >";
                codeHtml = codeHtml + "					<!-- module 4 colonnes -->";
                codeHtml = codeHtml + "					<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"left\" style=\"padding: 0 0 0 100px\">";
                codeHtml = codeHtml + "								<table>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 1 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + "                                                                 <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro3.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0px 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro3.Sku + ".png" + "\"  width=\"200\" height=\"200\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																		" + pro3.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
                															
                if (pro3.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro3.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro3.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                else
                {
                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                    codeHtml = codeHtml + "																		 " + pro3.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</td>";
                    
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		 " + pro3.prixDroite + " ";
                    codeHtml = codeHtml + "																	</td>";
                }
                    codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                    codeHtml = codeHtml + "																		" + pro3.Nombre_colis + " &nbsp;";
                    codeHtml = codeHtml + "																	</td>";
                
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																		" + pro3.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		" + pro3.Dimenions + " &nbsp;"; 
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro3.DimensionsCouchage + " &nbsp;";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 1  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 2 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + "                                                                 <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro4.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0px 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro4.Sku + ".png" +"\"  width=\"200\" height=\"200\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																	" + pro4.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
              
            if (pro4.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro4.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro4.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                else
                {
                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                    codeHtml = codeHtml + "																		 " + pro4.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</td>";
                    
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		 " + pro4.prixDroite + " ";
                    codeHtml = codeHtml + "																	</td>";
                }
                codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                codeHtml = codeHtml + "																		"+ pro4.Nombre_colis +" &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																		" + pro4.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";

                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		" + pro4.Dimenions + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro4.DimensionsCouchage + " &nbsp;";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 2  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<!--/module 4 colonnes -->";
                codeHtml = codeHtml + "					<!-- module 4 colonnes -->";
                codeHtml = codeHtml + "					<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"left\" style=\"padding: 0 0 0 100px\">";
                codeHtml = codeHtml + "								<table>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 1 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + "                                                                 <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro5.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0px 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro5.Sku + ".png" +"\"  width=\"300\" height=\"295\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																	" + pro5.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
                if (pro5.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro5.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro5.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                else
                {
                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                    codeHtml = codeHtml + "																		 " + pro5.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		 " + pro5.prixDroite + " ";
                    codeHtml = codeHtml + "																	</td>";
                }
                codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                codeHtml = codeHtml + "																		"+ pro5.Nombre_colis +" &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																		" + pro5.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		" + pro5.Dimenions + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro5.DimensionsCouchage + " &nbsp;";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 1  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "										<td>";
                codeHtml = codeHtml + "											<!-- colonne 2 -->";
                codeHtml = codeHtml + "											<div class=\"divReglette\">";
                codeHtml = codeHtml + "												<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
                codeHtml = codeHtml + "													<tr>";
                codeHtml = codeHtml + "														<td>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"200\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td align=\"left\">";
                codeHtml = codeHtml + "                                                                 <object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro6.Sku + ".png" + "\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 0 0\" type=\"image/png\">";
                codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"600\" height=\"225\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
                codeHtml = codeHtml + " 																</object>";
                //codeHtml = codeHtml + "																		<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro6.Sku + ".png" + "\"  width=\"300\" height=\"295\" border=\"0\" alt=\"montino\" style=\"display:block; height:auto\">";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"600\" height=\"30\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 34px; padding: 0 0; line-height: 2\">";
                codeHtml = codeHtml + "																		" + pro6.Variation + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"35\">";
                codeHtml = codeHtml + "																<tr>";
                if (pro6.typeTarifCbr == "HABHFR")
                {
                    codeHtml = codeHtml + "               <tr> <td style=\"text-align: left; font-family: DINHabbold; font-size: 20px;  padding: 0 0px\"><font color=\"#318CE7\">" + "Prix Habitant " + "</font></td></tr>";

                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\"><font color=\"#318CE7\">";
                    codeHtml = codeHtml + "																		 " + pro6.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</font></td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		" + pro6.prixDroite + "";
                    codeHtml = codeHtml + "																	</td>";
                }
                else
                {
                    codeHtml = codeHtml + "																	<td width=\"150\" valign=\"bottom\" style=\"text-align: left; font-family: DINHabbold; font-size: 55px; padding:0 0 0 0; line-height:65px;\">";
                    codeHtml = codeHtml + "																		 " + pro6.prixGauche + " &nbsp;";
                    codeHtml = codeHtml + "																	</td>";
                    codeHtml = codeHtml + "																	<td width=\"100\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 32px; text-decoration: line-through; text-align: left; line-height:30px;\">";
                    codeHtml = codeHtml + "																		 " + pro6.prixDroite + " ";
                    codeHtml = codeHtml + "																	</td>";
                }
                codeHtml = codeHtml + "																	<td width=\"50\" height=\"22\" valign=\"bottom\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0 0; line-height:30px\">";
                codeHtml = codeHtml + "																		"+ pro6.Nombre_colis +" &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table width=\"500\" height=\"18\">";
                codeHtml = codeHtml + "																<tr>";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabRg; font-size: 14px; padding: 0 0; line-height: 0\" >";
                codeHtml = codeHtml + "																		" + pro6.EcoPart + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "															<table>";
                codeHtml = codeHtml + "																<tr width=\"400\" height=\"20\">";
                codeHtml = codeHtml + "																	<td style=\"text-align: left; font-family: DINHabbold; font-size: 28px; padding: 0 0 0 0; line-height:28px\">";
                codeHtml = codeHtml + "																		<br>";
                codeHtml = codeHtml + "																		" + pro6.Dimenions + " &nbsp;";
                codeHtml = codeHtml + "																	</td>";
                codeHtml = codeHtml + "																</tr>";
                codeHtml = codeHtml + "															</table>";
                codeHtml = codeHtml + "                                                         <table width=\"400\" height=\"18\">";
                codeHtml = codeHtml + "															<tr>";
                codeHtml = codeHtml + "																<td style=\"text-align: left; font-family: DINHabRg; font-size: 25px; padding: 0 0 0 0; \"> " + pro6.DimensionsCouchage + " &nbsp;";
                codeHtml = codeHtml + "															</tr>";
                codeHtml = codeHtml + "														</table>";
                codeHtml = codeHtml + "														</td>";
                codeHtml = codeHtml + "													</tr>";
                codeHtml = codeHtml + "												</table>";
                codeHtml = codeHtml + "											</div>";
                codeHtml = codeHtml + "											<!--/ colonne 2  -->";
                codeHtml = codeHtml + "										</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<!--/module 4 colonnes -->";
                codeHtml = codeHtml + "					<! --/ ligne 2 -- >";
                codeHtml = codeHtml + "					<table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td style=\"font-size: 1px; padding-bottom: 2px\"></td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<table width=\"1500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" style=\"padding-top: 2px;\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td align=\"center\">";
                codeHtml = codeHtml + "								<tr>";
                codeHtml = codeHtml + "									<td align=\"center\" style=\"padding: 2px 0 0 100px\">";
                codeHtml = codeHtml + "										<table align=\"left\" width=\"450\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td width=\"450\" style=\"text-align: left; font-family: DINHabbold; font-size: 38px; text-align: justify; padding: 0 0 0 0\">";
                codeHtml = codeHtml + "													  " + texteDur1 + " ";
                codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "									</td>";
                codeHtml = codeHtml + "								</tr>";
                codeHtml = codeHtml + "								<tr>";
                codeHtml = codeHtml + "									<td align=\"center\" style=\"padding: 0 0 0 100px\">";
                codeHtml = codeHtml + "										<table align=\"left\" width=\"1300\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
                codeHtml = codeHtml + "											<tr>";
                codeHtml = codeHtml + "												<td width=\"1300\" style=\"vertical-align: top;text-align: left; font-family: DINHabRg; font-size: 22px; text-align: justify; padding: 0 0\">";
                codeHtml = codeHtml + "													<br>";
                codeHtml = codeHtml + "												" + comp1+""+comp2 +" </td> ";            
                //codeHtml = codeHtml + "											   <td width=\"\" style=\"vertical-align: top;text-align: left; font-family: DINHabRg; font-size: 22px; text-align: justify; padding: 0 20px\">";
                //codeHtml = codeHtml + "												<br>" + comp2 + "</td>";
                //codeHtml = codeHtml + "													 Fixe existe en plusieurs versions, tissu A Ancio : composé de 77% de polyester, 17% d’acrylique, 6% de coton. ";
                //codeHtml = codeHtml + "													<strong>Structure :</strong>";
                //codeHtml = codeHtml + "													 En bois de pin, panneaux de particules et contreplaqué. ";
                //codeHtml = codeHtml + "													<strong>Coussin de l’assise :</strong>";
                //codeHtml = codeHtml + "													 Mousse haute résilience (densité de 30 et 35 kg/m3). ";
                //codeHtml = codeHtml + "													<strong>Coussin du dossier :</strong>";
                //codeHtml = codeHtml + "													 Coussin du dossier : 70% de mousse, 30% de plume de canard.";
                //codeHtml = codeHtml + "													<strong>Assise :</strong>";
                //codeHtml = codeHtml + "													 Suspension de l’assise à ressorts zig-zag. H. de l’assise 37 P. de l’assise 62 ";
                //codeHtml = codeHtml + "													<strong>Dossier :</strong>";
                //codeHtml = codeHtml + "													 Composé de sangles élastiques entrecroisées. ";
                //codeHtml = codeHtml + "													<strong>Pieds :</strong>";
                //codeHtml = codeHtml + "													 En acier brossé d’une hauteur de 18 cm. ";
                //codeHtml = codeHtml + "													<strong>Conseil d’entretien :</strong>";
                //codeHtml = codeHtml + "													 Nettoyage professionnel recommandé. ";
                //codeHtml = codeHtml + "												</td>";
                codeHtml = codeHtml + "											</tr>";
                codeHtml = codeHtml + "										</table>";
                codeHtml = codeHtml + "									</td>";
                codeHtml = codeHtml + "								</tr>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "					<div class=\"footer\">";
                codeHtml = codeHtml + "						<table align=\"left\" width=\"1500\" valign=\"top\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "							<tr>";
                codeHtml = codeHtml + "								<td style=\"padding:5px 0 0 0\">";
                codeHtml = codeHtml + "									<!-- -- trait -- -->";
                codeHtml = codeHtml + "									<table align=\"left\" width=\"1500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
                codeHtml = codeHtml + "										<tr>";
                codeHtml = codeHtml + "											<td align=\"center\" style=\"padding:5px 0 0 0\" width=\"400\">";
                codeHtml = codeHtml + "												<hr width=\"400\">";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "											<td height=\"10\" valign=\"bottom\" style=\"text-align: center; font-family: dinhabbold; font-size: 18px;\" width=\"250\">";
                codeHtml = codeHtml + "												" + texteDur2 + " ";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "											<td align=\"center\" style=\"padding:5px 0 0 0\" width=\"400\">";
                codeHtml = codeHtml + "												<hr width=\"400\">";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "										</tr>";
                codeHtml = codeHtml + "									</table>";
                codeHtml = codeHtml + "									<! --/ trait -- >";
                codeHtml = codeHtml + "									<table align=\"left\" width=\"1500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" >";
                codeHtml = codeHtml + "										<tr>";
                codeHtml = codeHtml + "											<td width=\"75\" align=\"center\" style=\"padding:0 0 0 40px\">";
                codeHtml = codeHtml + "												" + imageMadeIn +" ";
                    //<img src=\"img/drapeau_format_reglette2.png\" width=\"75\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\">";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "											<td width=\"700\" align=\"right\" style=\"text-align: right; font-family: DINHabRg; font-size: 20px; padding: 0px 0 0px 0px\">";
                codeHtml = codeHtml + "											" + texteDur3 + " ";
                codeHtml = codeHtml + "												<br>";
                codeHtml = codeHtml + "												<strong>" + texteDur4 + " </strong>";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "											<td width=\"55\" align=\"left\">";
                codeHtml = codeHtml + "												<img src=\"" + baseUrlImages + "Camion_livraison.png\" width=\"80\" border=\"0\" alt=\"camion\" title=\"camion livraison\" style=\"display:block; padding: 0px 0 0px 10px\">";
                codeHtml = codeHtml + "											</td>";
                codeHtml = codeHtml + "										</tr>";
                codeHtml = codeHtml + "									</table>";
                codeHtml = codeHtml + "									<!--/ wrapper -->";
                codeHtml = codeHtml + "								</td>";
                codeHtml = codeHtml + "							</tr>";
                codeHtml = codeHtml + "						</table>";
                codeHtml = codeHtml + "					</div>";
                codeHtml = codeHtml + "				</td>";
                codeHtml = codeHtml + "			</tr>";
                codeHtml = codeHtml + "		</table>";
                codeHtml = codeHtml + "	</body>";
                codeHtml = codeHtml + "</html>";

          
            return codeHtml;
        }

       

        static public List<T_Produit_A4> getTableauProduitsFromChevalet(TickitDataChevalet chevalet, int magasinId, int RangeId, DateTime dateQuery)
        {
            List<T_Produit_A4> listeProduitsA4 = new List<T_Produit_A4>();

            T_Produit_A4 pro1 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro2 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro3 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro4 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro5 = Models.T_Produit_A4.initializeProduit();
            T_Produit_A4 pro6 = Models.T_Produit_A4.initializeProduit();
           // T_Produit_A4 pro7 = Models.T_Produit_A4.initializeProduit();
            //T_Produit_A4 pro8 = Models.T_Produit_A4.initializeProduit();

            listeProduitsA4.Add(pro1);
            listeProduitsA4.Add(pro2);
            listeProduitsA4.Add(pro3);
            listeProduitsA4.Add(pro4);
            listeProduitsA4.Add(pro5);
            listeProduitsA4.Add(pro6);
           // listeProduitsA4.Add(pro7);
           // listeProduitsA4.Add(pro8);

            string texte_colis = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A5_TEXTE_DUR_6", magasinId);

            string APartirDe = "";
            if (DAO.RangeDao.isRangeBarAtissu(RangeId))
            {
                APartirDe = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A5_TEXTE_DUR_5", magasinId);
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

                /*string Nombre_colis = "";
                //if (data.Nombre_colis != "0")
                //{
                 //   Nombre_colis = data.Nombre_colis + " colis ";
                }
                else
                {
                    Nombre_colis = "";
                }
                */
               string typeTarifCbr = "";
                T_Prix prix= DAO.PrixDao.getPrixBySkuAndDate(data.sku, magasinId, dateQuery);

                data.typeTarifCbr = prix.TypeTarifCbr;

                typeTarifCbr = data.typeTarifCbr;


              

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
                listeProduitsA4[i].typeTarifCbr = data.typeTarifCbr;

                if (data.Nombre_colis != null && data.Nombre_colis != "" && data.Nombre_colis != "0")
                {
                    listeProduitsA4[i].Nombre_colis = data.Nombre_colis + texte_colis;
                }
                if (data.Nombre_colis == "0")
                {
                    listeProduitsA4[i].Nombre_colis = "1" + texte_colis;
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