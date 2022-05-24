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
    public static class PlvChevaletA4Utils
    {
        /// Fonction permetant de génerer le document PDF pour l impression format reglette carré
        /// 18/07/2020
        /// Mehdi SRIDI
        /// <param name="chevalet"></param>
        /// <param name="format"></param>
        /// <param name="magasinId"></param>
        /// <param name="dateQuery"></param>
        /// <returns></returns>
        static public string getHtmlA4(TickitDataChevalet chevalet, String format, int magasinId, DateTime dateQuery)
        {
            string baseUrlFonts = "http://ean.habitat.fr/TAKEIT/webfonts/";
            string baseUrlImages = "http://ean.habitat.fr/TAKEIT/A4/images/";

            string prefixFilaire = "filaire_A4_";

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
            T_Produit_A4 pro7 = lista[6];
            T_Produit_A4 pro8 = lista[7];

            string texteDur1 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A4_TEXTE_DUR_1", magasinId);
            string texteDur2 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A4_TEXTE_DUR_2", magasinId);
            string texteDur3 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A4_TEXTE_DUR_3", magasinId);
            string texteDur4 = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A4_TEXTE_DUR_4", magasinId);

            string imageMadeIn = "";
            string madeIn = DAO.ProduitDao.getMadeInByRangeId(RangeId);

            string codeHtml = "";
            codeHtml = codeHtml + "<html lang=\"fr\">";
            codeHtml = codeHtml + "";
            codeHtml = codeHtml + "<head>";
            codeHtml = codeHtml + "	<meta charset=\"UTF-8\">";
            codeHtml = codeHtml + "	<title>Habitat</title>";
            codeHtml = codeHtml + "	<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">";
            codeHtml = codeHtml + "	<style>";

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
        //Cillia 

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
            codeHtml = codeHtml + "	</style>";
            codeHtml = codeHtml + "	<style type=\"text/css\">";
            codeHtml = codeHtml + "	@font-face {";
            codeHtml = codeHtml + "		font-family: 'dinhabbold';";
            codeHtml = codeHtml + "		src: url('" + baseUrlFonts + "DINHabBd.ttf') format('truetype');";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	";
            codeHtml = codeHtml + "	@font-face {";
            codeHtml = codeHtml + "		font-family: 'DINHabRg';";
            codeHtml = codeHtml + "		src: url('" + baseUrlFonts + "/DINHabRg.ttf') format('truetype');";
            codeHtml = codeHtml + "	}";
            codeHtml = codeHtml + "	</style>";
            codeHtml = codeHtml + "</head>";
            codeHtml = codeHtml + "<table style=\"padding: 20px 0 0 20px\" align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "	<tr>";
            codeHtml = codeHtml + "		<td align=\"center\">";
            codeHtml = codeHtml + "			<table width=\"100\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" class=\"full\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"center\" padding=\"120px 0 0 0\">";
            codeHtml = codeHtml + "						<table align=\"center\" width=\"150\" heigth=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "							<tr>";
            codeHtml = codeHtml + "								<td align=\"center\" padding=\"0 0 0 100px\"> <img src=\"" + baseUrlImages + "logo.png\" width=\"100\" border=\"0\" alt=\"habitat\" style=\"display:block height:auto\" class=\"full\"> </td>";
            codeHtml = codeHtml + "							</tr>";
            codeHtml = codeHtml + "						</table>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "			<table width=\"300\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" class=\"full\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"left\">";
            codeHtml = codeHtml + "						<table align=\"left\" width=\"500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" class=\"deviceWidth\">";
            codeHtml = codeHtml + "							<tr>";
            codeHtml = codeHtml + "								<td height=\"34\" colspan=\"3\" valign=\"bottom\" style=\"text-align: left; font-family: dinhabbold; font-size: 55px; text-transform: uppercase\">" + chevalet.rangeChevalet + "</td>";
            codeHtml = codeHtml + "							</tr>";
            codeHtml = codeHtml + "						</table>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"left\">";
            codeHtml = codeHtml + "						<table align=\"left\" width=\"500\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"deviceWidth\">";
            codeHtml = codeHtml + "							<tr>";

            string descriptionRange = DAO.RangeDao.getDescriptionPlusByRangeID(RangeId, magasinId);


            if (isbarATissu)
            {
                codeHtml = codeHtml + "								<td height=\"50\" style=\"text-align: left; font-family: DINHabbold; font-size: 28px; background-color: #000000; color: #FFFFFF; padding: 0 0 0 20px \">" + descriptionRange + "</td>";
            }
            else
            {
                codeHtml = codeHtml + "								<td height=\"50\" style=\"text-align: left; font-family: DINHabbold; font-size: 28px; background-color: #FFFFFF; color: #000000; padding: 0 0 0 20px \">" + descriptionRange + "</td>";
            }


            codeHtml = codeHtml + "							</tr>";
            codeHtml = codeHtml + "						</table>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"center\" style=\"padding: 20px 0 20px 0\">";
            codeHtml = codeHtml + "						<table align=\"left\" width=\"600\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"deviceWidth\">";
            codeHtml = codeHtml + "							<tr>";
            codeHtml = codeHtml + "								<td class=\"filet\" width=\"1%\" rowspan=\"4\"> </td>";
            codeHtml = codeHtml + "								<td width=\"3%\" rowspan=\"4\"></td>";
            codeHtml = codeHtml + "							</tr>";


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


            codeHtml = codeHtml + "							<tr>";
            codeHtml = codeHtml + "								<td width=\"600\" style=\"text-align: left; font-family: DINHabRg; font-size: 28px;\"> " + plus1 + " &nbsp;</td>";
            codeHtml = codeHtml + "							</tr>";
            codeHtml = codeHtml + "							<tr>";
            codeHtml = codeHtml + "								<td width=\"600\" style=\"text-align: left; font-family: DINHabRg; font-size: 28px;\"> " + plus2 + " &nbsp;</td>";
            codeHtml = codeHtml + "							</tr>";
            codeHtml = codeHtml + "							<tr>";
            codeHtml = codeHtml + "								<td width=\"600\" style=\"text-align: left; font-family: DINHabRg; font-size: 28px;\"> " + plus3 + " &nbsp;</td>";
            codeHtml = codeHtml + "							</tr>";
            codeHtml = codeHtml + "						</table>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "			<table width=\"250\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"center\">";
            codeHtml = codeHtml + "						<table align=\"center\" width=\"200\" heigth=\"200\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" valign=\"top\">";
            codeHtml = codeHtml + "							<tr>";

            string typePastille = "";

            if ((pro1.typeTarifCbr == "HABHFR") && (pro2.typeTarifCbr == "" || pro2.typeTarifCbr == "HABHFR") && (pro3.typeTarifCbr == "" || pro3.typeTarifCbr == "HABHFR")
                   && (pro4.typeTarifCbr == "" || pro4.typeTarifCbr == "HABHFR") && (pro5.typeTarifCbr == "" || pro5.typeTarifCbr == "HABHFR")
                   && (pro6.typeTarifCbr == "" || pro6.typeTarifCbr == "HABHFR") && (chevalet.typePrix == ApplicationConsts.typePrix_promo))
            //if ((pro1.typeTarifCbr == "HABHFR") && (pro2.typeTarifCbr == "" || pro2.typeTarifCbr == "HABHFR") && (chevalet.typePrix == ApplicationConsts.typePrix_promo))
            { typePastille = ApplicationConsts.typePastillePromoHab; }


            else if (chevalet.typePrix == ApplicationConsts.typePrix_demarqueLocale || chevalet.typePrix == ApplicationConsts.typePrix_promo) typePastille = ApplicationConsts.typePastillePromoReglette;
            else if (chevalet.typePrix == ApplicationConsts.typePrix_solde) typePastille = ApplicationConsts.typePastilleSoldeReglette;

            string pourcentagetexte = "";
            if (chevalet.pourcentageReduction != null)
            {
                pourcentagetexte = "-" + chevalet.pourcentageReduction + "%";
            }

            codeHtml = codeHtml + "								<td width=\"200\" height=\"200\" align=\"center\" valign=\"top\" class=\"" + typePastille + "\" style=\"text-align: center; font-family: dinhabbold; font-size: 40pt; color: #FFFFFF; line-height: 120px;\">" + pourcentagetexte + " &nbsp;&nbsp;</td>";
            codeHtml = codeHtml + "							</tr>";
            codeHtml = codeHtml + "						</table>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "			<table width=\"450\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\" style=\"padding-top: 20px;\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"center\">";
            codeHtml = codeHtml + "						<tr>";
            codeHtml = codeHtml + "							<td align=\"center\">";
            codeHtml = codeHtml + "								<table align=\"left\" width=\"450\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "									<tr>";
            codeHtml = codeHtml + "										<td width=\"450\" style=\"text-align: left; font-family: DINHabRg; font-size: 28px; text-align: justify; padding: 0 20px\"> " + texteDur1 + "</td>";
            codeHtml = codeHtml + "									</tr>";
            codeHtml = codeHtml + "								</table>";
            codeHtml = codeHtml + "							</td>";
            codeHtml = codeHtml + "						</tr>";
            codeHtml = codeHtml + "						<tr>";
            codeHtml = codeHtml + "							<td align=\"center\">";
            codeHtml = codeHtml + "								<table align=\"left\" width=\"900\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\">";
            codeHtml = codeHtml + "									<tr>";

            string comp1 = DAO.RangeDao.getDescriptionCompositionByRangeId(RangeId, magasinId, 1);
            string comp2 = DAO.RangeDao.getDescriptionCompositionByRangeId(RangeId, magasinId, 2);

            codeHtml = codeHtml + "										<td width=\"450\" style=\"vertical-align: top;text-align: left; font-family: DINHabRg; font-size: 18px; text-align: justify; padding: 0 20px\">";
            codeHtml = codeHtml + "											<br>" + comp1 + "</td>";
            codeHtml = codeHtml + "										<td width=\"450\" style=\"vertical-align: top;text-align: left; font-family: DINHabRg; font-size: 18px; text-align: justify; padding: 0 10px\">";
            codeHtml = codeHtml + "											<br>" + comp2 + "</td>";
            codeHtml = codeHtml + "									</tr>";
            codeHtml = codeHtml + "								</table>";
            codeHtml = codeHtml + "							</td>";
            codeHtml = codeHtml + "						</tr>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "		</td>";
            codeHtml = codeHtml + "	</tr>";
            codeHtml = codeHtml + "</table>";
            codeHtml = codeHtml + "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#ffffff\" style=\"height: 910px;\">";
            codeHtml = codeHtml + "	<tr><td align=\"center\" style=\"padding: 20px 0 0 0\" ></td></tr>";
            codeHtml = codeHtml + "	<tr style=\"height: 425px;\">";
            codeHtml = codeHtml + "		<td align=\"center\" style=\"padding: 0 0 0 150px\">";
            codeHtml = codeHtml + "			<table width=\"430\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"left\" width=\"350\" height=\"195\">";
            codeHtml = codeHtml + " 					<object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro1.Sku + ".png" + "\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 10px 0\" type=\"image/png\">";
            codeHtml = codeHtml + "							<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
            codeHtml = codeHtml + " 					</object>";
            codeHtml = codeHtml + " 				</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 22px; padding: 0 20px\">" + pro1.Variation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro1.Orientation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro1.APartirDe + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\">";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabbold; font-size: 35px; padding: 0 20px\">" + pro1.prixGauche + "</span>";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabRg; text-decoration: line-through; font-size:25px\">" + pro1.prixDroite + "</span>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro1.EcoPart + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 20px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro1.Dimenions + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px; \"> " + pro1.DimensionsDeplie + "";
            codeHtml = codeHtml + "						<br>" + pro1.DimensionsCouchage + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "			<table width=\"430\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"left\" width=\"350\" height=\"195\">";
            codeHtml = codeHtml + " 					<object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro2.Sku + ".png" + "\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 10px 0\" type=\"image/png\">";
            codeHtml = codeHtml + "							<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
            codeHtml = codeHtml + " 					</object>";
            codeHtml = codeHtml + " 				</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 22px; padding: 0 20px\">" + pro2.Variation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro2.Orientation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro2.APartirDe + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\">";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabbold; font-size: 35px; padding: 0 20px\">" + pro2.prixGauche + "</span>";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabRg; text-decoration: line-through; font-size:25px\">" + pro2.prixDroite + "</span>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro2.EcoPart + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 20px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro2.Dimenions + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px; \"> " + pro2.DimensionsDeplie + "";
            codeHtml = codeHtml + "						<br>" + pro2.DimensionsCouchage + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "			<table width=\"430\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"left\" width=\"350\" height=\"195\">";
            codeHtml = codeHtml + " 					<object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro3.Sku + ".png" + "\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 10px 0\" type=\"image/png\">";
            codeHtml = codeHtml + "							<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
            codeHtml = codeHtml + " 					</object>";
            codeHtml = codeHtml + " 				</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 22px; padding: 0 20px\">" + pro3.Variation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro3.Orientation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro3.APartirDe + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\">";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabbold; font-size: 35px; padding: 0 20px\">" + pro3.prixGauche + "</span>";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabRg; text-decoration: line-through; font-size:25px\">" + pro3.prixDroite + "</span>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro3.EcoPart + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 20px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro3.Dimenions + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px; \"> " + pro3.DimensionsDeplie + "";
            codeHtml = codeHtml + "						<br>" + pro3.DimensionsCouchage + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "			<table width=\"430\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"left\" width=\"350\" height=\"195\">";
            codeHtml = codeHtml + " 					<object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro4.Sku + ".png" + "\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 10px 0\" type=\"image/png\">";
            codeHtml = codeHtml + "							<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
            codeHtml = codeHtml + " 					</object>";
            codeHtml = codeHtml + " 				</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 22px; padding: 0 20px\">" + pro4.Variation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro4.Orientation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro4.APartirDe + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\">";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabbold; font-size: 35px; padding: 0 20px\">" + pro4.prixGauche + "</span>";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabRg; text-decoration: line-through; font-size:25px\">" + pro4.prixDroite + "</span>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro4.EcoPart + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 20px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro4.Dimenions + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px; \"> " + pro4.DimensionsDeplie + "";
            codeHtml = codeHtml + "						<br>" + pro4.DimensionsCouchage + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "		</td>";
            codeHtml = codeHtml + "	</tr>";
            codeHtml = codeHtml + "	<tr><td align=\"center\" style=\"padding: 30px 0 0 0\" ></td></tr>";
            codeHtml = codeHtml + "	<tr style=\"height: 425px;\">";
            codeHtml = codeHtml + "		<td align=\"center\" style=\"padding: 0 0 0 150px\">";
            codeHtml = codeHtml + "			<table width=\"430\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"left\" width=\"350\" height=\"195\">";
            codeHtml = codeHtml + " 					<object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro5.Sku + ".png" + "\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 10px 0\" type=\"image/png\">";
            codeHtml = codeHtml + "							<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
            codeHtml = codeHtml + " 					</object>";
            codeHtml = codeHtml + " 				</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 22px; padding: 0 20px\">" + pro5.Variation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro5.Orientation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro5.APartirDe + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\">";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabbold; font-size: 35px; padding: 0 20px\">" + pro5.prixGauche + "</span>";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabRg; text-decoration: line-through; font-size:25px\">" + pro5.prixDroite + "</span>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro5.EcoPart + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 20px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro5.Dimenions + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px; \"> " + pro5.DimensionsDeplie + "";
            codeHtml = codeHtml + "						<br>" + pro5.DimensionsCouchage + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "			<table width=\"430\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"left\" width=\"350\" height=\"195\">";
            codeHtml = codeHtml + " 					<object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro6.Sku + ".png" + "\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 10px 0\" type=\"image/png\">";
            codeHtml = codeHtml + "							<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
            codeHtml = codeHtml + " 					</object>";
            codeHtml = codeHtml + " 				</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 22px; padding: 0 20px\">" + pro6.Variation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro6.Orientation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro6.APartirDe + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\">";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabbold; font-size: 35px; padding: 0 20px\">" + pro6.prixGauche + "</span>";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabRg; text-decoration: line-through; font-size:25px\">" + pro6.prixDroite + "</span>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro6.EcoPart + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 20px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro6.Dimenions + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px; \"> " + pro6.DimensionsDeplie + "";
            codeHtml = codeHtml + "						<br>" + pro6.DimensionsCouchage + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "			<table width=\"430\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"left\" width=\"350\" height=\"195\">";
            codeHtml = codeHtml + " 					<object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro7.Sku + ".png" + "\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 10px 0\" type=\"image/png\">";
            codeHtml = codeHtml + "							<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
            codeHtml = codeHtml + " 					</object>";
            codeHtml = codeHtml + " 				</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 22px; padding: 0 20px\">" + pro7.Variation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro7.Orientation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro7.APartirDe + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\">";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabbold; font-size: 35px; padding: 0 20px\">" + pro7.prixGauche + "</span>";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabRg; text-decoration: line-through; font-size:25px\">" + pro7.prixDroite + "</span>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro7.EcoPart + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 20px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro7.Dimenions + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px; \"> " + pro7.DimensionsDeplie + "";
            codeHtml = codeHtml + "						<br>" + pro7.DimensionsCouchage + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "			<table width=\"430\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\" align=\"left\">";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td align=\"left\" width=\"350\" height=\"195\">";
            codeHtml = codeHtml + " 					<object data=\"" + baseUrlImages + "filaires/" + prefixFilaire + pro8.Sku + ".png" + "\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto; padding: 0 0 10px 0\" type=\"image/png\">";
            codeHtml = codeHtml + "							<img src=\"" + baseUrlImages + "filaires/" + "/blanc.png\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\"> ";
            codeHtml = codeHtml + " 					</object>";
            codeHtml = codeHtml + " 				</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 22px; padding: 0 20px\">" + pro8.Variation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro8.Orientation + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro8.APartirDe + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\">";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabbold; font-size: 35px; padding: 0 20px\">" + pro8.prixGauche + "</span>";
            codeHtml = codeHtml + "						<span style=\"text-align: left; font-family: DINHabRg; text-decoration: line-through; font-size:25px\">" + pro8.prixDroite + "</span>";
            codeHtml = codeHtml + "					</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 12px; padding: 0 20px\">" + pro8.EcoPart + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabbold; font-size: 20px; padding: 0 20px\">";
            codeHtml = codeHtml + "						<br>" + pro8.Dimenions + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "				<tr>";
            codeHtml = codeHtml + "					<td width=\"400\" style=\"text-align: left; font-family: DINHabRg; font-size: 18px; padding: 0 20px; \"> " + pro8.DimensionsDeplie + "";
            codeHtml = codeHtml + "						<br>" + pro8.DimensionsCouchage + "</td>";
            codeHtml = codeHtml + "				</tr>";
            codeHtml = codeHtml + "			</table>";
            codeHtml = codeHtml + "		</td>";
            codeHtml = codeHtml + "	</tr>";
            codeHtml = codeHtml + "</table>";
            codeHtml = codeHtml + "<table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
            codeHtml = codeHtml + "	<tr>";
            codeHtml = codeHtml + "		<td style=\"font-size: 1px; padding-bottom: 50px\"> </td>";
            codeHtml = codeHtml + "	</tr>";
            codeHtml = codeHtml + "</table>";
            codeHtml = codeHtml + "<table align=\"left\" width=\"2000\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
            codeHtml = codeHtml + "	<tr>";
            codeHtml = codeHtml + "		<td align=\"center\" style=\"padding:0 0 0 150px\">";
            codeHtml = codeHtml + "			<hr width=\"600\"> </td>";
            codeHtml = codeHtml + "		<td height=\"30\" valign=\"bottom\" style=\"text-align: center; font-family: dinhabbold; font-size: 28px;\">" + texteDur2 + "</td>";
            codeHtml = codeHtml + "		<td align=\"center\" style=\"padding:0 60px 0 0\">";
            codeHtml = codeHtml + "			<hr width=\"600\"> </td>";
            codeHtml = codeHtml + "	</tr>";
            codeHtml = codeHtml + "</table>";
            codeHtml = codeHtml + "<table align=\"left\" width=\"1930\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"ffffff\">";
            codeHtml = codeHtml + "	<tr>";

            if (madeIn != "")
            {
                imageMadeIn = "<object data=\"" + baseUrlImages + "Flag_" + madeIn + ".png\" width=\"95\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\"><img src=\"" + baseUrlImages + "filaires/blanc.png\" width=\"95\" border=\"0\" alt=\"drapeau\" title=\"flag\" style=\"display:block\"> </object>";
            }

            codeHtml = codeHtml + "		<td width=\"100\" align=\"left\" style=\"padding:0px 0 0px 180px\">" + imageMadeIn + "</td>";
            codeHtml = codeHtml + "		<td width=\"200\" align=\"right\" style=\"text-align: right; font-family: DINHabRg; font-size: 18px; padding: 0px 0 0px 900px\">" + texteDur3;
            codeHtml = codeHtml + "			<br><strong>" + texteDur4 + "</strong> </td>";
            codeHtml = codeHtml + "		<td width=\"100\" align=\"right\"><img src=\"" + baseUrlImages + "Camion_livraison.png\" width=\"100\" border=\"0\" alt=\"camion\" title=\"camion livraison\" style=\"display:block\"> </td>";
            codeHtml = codeHtml + "	</tr>";
            codeHtml = codeHtml + "</table>";
            codeHtml = codeHtml + "";
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

            string APartirDe = "";
            if (DAO.RangeDao.isRangeBarAtissu(RangeId)  )
            {
                APartirDe = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A4_TEXTE_DUR_5", magasinId);
            }

//cillia
            if (DAO.RangeDao.isRangeTissu_Cuir_A_partir(RangeId))

            {
                APartirDe = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_A4_TEXTE_DUR_5", magasinId);
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

                //Cillia 

                string typeTarifCbr = "";
                T_Prix prix = DAO.PrixDao.getPrixBySkuAndDate(data.sku, magasinId, dateQuery);

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


                i++;
            }

            return listeProduitsA4;
        }

        /// <summary>
        /// renvoie le contenu html de l'image à insérer si elle existe.
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="baseUrlImages"></param>
        /// <returns></returns>
        public static string getUrlImageFilaire(string sku, string baseUrlImages, string prefixFilaire)
        {
            string urlImage = "";
            bool imageExists = false;
            imageExists = StringUtils.URLExists(baseUrlImages + "filaires/" + prefixFilaire + sku + ".png");
            if (imageExists == true)
            {
                urlImage = "<img src=\"" + baseUrlImages + "filaires/" + prefixFilaire + sku + ".png\" width=\"350\" height=\"195\" border=\"0\" alt=\"montino\" style=\"display:block height:auto\">";
            }
            return urlImage;
        }

        //Cillia

      /*  public static List<TickitDataChevalet> MergeChevalet(TickitDataChevalet chevalet)
        {
            List<TickitDataChevalet> chevalets = new List<TickitDataChevalet>();

                TickitDataChevalet chevaletCurrentPage = new TickitDataChevalet();
                chevaletCurrentPage.originePanier = chevalet.originePanier;
                chevaletCurrentPage.pourcentageReduction = chevalet.pourcentageReduction;
                chevaletCurrentPage.rangeChevalet = chevalet.rangeChevalet;
                chevaletCurrentPage.typePrix = chevalet.typePrix;
               // chevaletCurrentPage.formatImpressionEtiquettesSimples = chevalet.formatImpressionEtiquettesSimples;
                chevaletCurrentPage.produitsData = new List<TickitDataProduit>();


                chevalets.Add(chevaletCurrentPage);
            

            return chevalets;
        }*/

        /// <summary>
        /// Renvoie le modèle de convertisseur html.
        /// </summary>
        /// <returns></returns>
        public static HtmlToPdf getHtmlToPdfModel()
        {
            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set browser width
            //htmlToPdfConverter.BrowserWidth = 595;

            // set browser height if specified, otherwise use the default
            htmlToPdfConverter.BrowserHeight = 1400;

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