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

namespace TickitNewFace.PDFUtils
{
    public static class PlvLineaire653Utils
    {
        /// Fonction permetant de génerer le document PDF pour l impression format reglette carré
        /// 18/07/2020
        /// Mehdi SRIDI
        /// <param name="chevalet"></param>
        /// <param name="format"></param>
        /// <param name="magasinId"></param>
        /// <param name="dateQuery"></param>
        /// <returns></returns>
        static public string getHtmlLineaire653(TickitDataChevalet chevalet, String format, int magasinId, DateTime dateQuery)
        {
            string baseUrlFonts = "http://ean.habitat.fr/TAKEIT/webfonts/";
            string baseUrlImages = "http://ean.habitat.fr/TAKEIT/REGLETTE/images/";

            string typePastille = "";
            if (chevalet.typePrix == ApplicationConsts.typePrix_demarqueLocale || chevalet.typePrix == ApplicationConsts.typePrix_promo) typePastille = ApplicationConsts.typePastillePromoReglette;
            if (chevalet.typePrix == ApplicationConsts.typePrix_solde) typePastille = ApplicationConsts.typePastilleSoldeReglette;

            string codeHtml = "";
            codeHtml = codeHtml + "<html>";
            codeHtml = codeHtml + "<head>";
            codeHtml = codeHtml + "<meta charset=\"UTF-8\">";
            codeHtml = codeHtml + "<title>REGLETTE</title>";
            codeHtml = codeHtml + "	<style>";
            codeHtml = codeHtml + "		.divReglette {";
            codeHtml = codeHtml + "			grid-area: auto;";
            codeHtml = codeHtml + "			width: 65mm;";
            codeHtml = codeHtml + "			height: 30mm;";
            codeHtml = codeHtml + "			margin: 0px;";
            codeHtml = codeHtml + "			padding: 0px;";
            codeHtml = codeHtml + "         background-image: url(" + baseUrlImages + "corps.jpg);";
            codeHtml = codeHtml + "			background-repeat: no-repeat;";
            codeHtml = codeHtml + "			background-size: cover;";
            codeHtml = codeHtml + "		}		";
            codeHtml = codeHtml + "				";

            codeHtml = codeHtml + "		.divRegletteRight {";
            codeHtml = codeHtml + "			grid-area: auto;";
            codeHtml = codeHtml + "			width: 0.5mm;";
            codeHtml = codeHtml + "			height: 30mm;";
            codeHtml = codeHtml + "			margin: 0px;";
            codeHtml = codeHtml + "			padding: 0px;";
            codeHtml = codeHtml + "			background-image: url(" + baseUrlImages + "corps_right.jpg);";
            codeHtml = codeHtml + "			background-repeat: no-repeat;";
            codeHtml = codeHtml + "			background-size: cover;";
            codeHtml = codeHtml + "		}";


            codeHtml = codeHtml + "		.promo_rose {";
            codeHtml = codeHtml + "         background-image: url(" + baseUrlImages + "pastille_rose.jpg);";
            codeHtml = codeHtml + "			background-repeat: no-repeat;";
            codeHtml = codeHtml + "			background-origin: border-box;";
            codeHtml = codeHtml + "			padding-top: 18px;";
            codeHtml = codeHtml + "			padding-right: 11px;";
            codeHtml = codeHtml + "			background-size: 80%;";
            codeHtml = codeHtml + "		}";
            codeHtml = codeHtml + "		";
            codeHtml = codeHtml + "		.promo_rouge {";
            codeHtml = codeHtml + "         background-image: url(" + baseUrlImages + "pastille_rouge.jpg);";
            codeHtml = codeHtml + "			background-repeat: no-repeat;";
            codeHtml = codeHtml + "			background-origin: border-box;";
            codeHtml = codeHtml + "			padding-top: 18px;";
            codeHtml = codeHtml + "			padding-right: 11px;";
            codeHtml = codeHtml + "			background-size: 80%;";
            codeHtml = codeHtml + "		}";
            codeHtml = codeHtml + "		";
            codeHtml = codeHtml + "";

            List<string> madeInList = DAO.ProduitDao.getListeMadeIn();
            foreach (string mdae_in in madeInList)
            {
                codeHtml = codeHtml + "		#flag_" + mdae_in + " {";
                codeHtml = codeHtml + "         background-image: url(" + baseUrlImages + "flag_" + mdae_in + ".jpg);";
                codeHtml = codeHtml + "			background-repeat: no-repeat;";
                codeHtml = codeHtml + "			background-origin: border-box;";
                codeHtml = codeHtml + "			background-size: 90%;";
                codeHtml = codeHtml + "		}";
            }


          
            codeHtml = codeHtml + "				";
            codeHtml = codeHtml + "		.filet {";
            codeHtml = codeHtml + "         background-image: url(" + baseUrlImages + "filet.jpg);";
            codeHtml = codeHtml + "			background-repeat: no-repeat;";
            codeHtml = codeHtml + "			background-size: contain;";
            codeHtml = codeHtml + "		}";
            codeHtml = codeHtml + "    </style>";
            codeHtml = codeHtml + "	<style type=\"text/css\">";
            codeHtml = codeHtml + "		@font-face {font-family: 'dinhabbold'; src: url('" + baseUrlFonts + "DINHabBd.ttf') format('truetype');}";
            codeHtml = codeHtml + "		@font-face {font-family: 'DINHabRg';src: url('" + baseUrlFonts + "DINHabRg.ttf') format('truetype');}";
            codeHtml = codeHtml + "	</style>";
            codeHtml = codeHtml + "</head>";
            codeHtml = codeHtml + "<body>";
            codeHtml = codeHtml + "	<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
            codeHtml = codeHtml + "		<tr>";

            int cpt = 0;
            foreach (TickitDataProduit data in chevalet.produitsData)
            {
                int val = cpt % 3;

                if (cpt % 3 == 0)
                {
                    codeHtml = codeHtml + "		<tr>";
                }

                codeHtml = codeHtml + "			<td style=\"padding: 0 0 22px 5px\">";
                codeHtml = codeHtml + "				<div class=\"divReglette\">";
                codeHtml = codeHtml + "					<table width=\"245\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td width=\"43\" height=\"79\" valign=\"top\"></td>";
                codeHtml = codeHtml + "							<td width=\"150\" height=\"79\" valign=\"top\">";
                codeHtml = codeHtml + "								<table width=\"150\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td height=\"34\" colspan=\"3\" valign=\"bottom\" style=\"text-align: left; font-family: dinhabbold; font-size: 16px;\">" + data.range + "</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td height=\"15\" colspan=\"3\" valign=\"top\" style=\"text-align: left; font-family: DINHabRg; font-size: 9px;\">" + data.variation + "</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td class=\"filet\" width=\"1%\" rowspan=\"3\"> </td>";
                codeHtml = codeHtml + "										<td width=\"3%\" rowspan=\"3\"></td>";

                List<T_Description_Plus> listePlus = DAO.Description_PlusDao.getListePlusBySku(data.sku, magasinId);

                string plus1 = "";
                string plus2 = "";
                string plus3 = "";

                if (listePlus.Count == 1)
                {
                    plus1 = listePlus[0].Plus;
                }
                if (listePlus.Count == 2)
                {
                    plus1 = listePlus[0].Plus;
                    plus2 = listePlus[1].Plus;

                }
                if (listePlus.Count == 3)
                {
                    plus1 = listePlus[0].Plus;
                    plus2 = listePlus[1].Plus;
                    plus3 = listePlus[2].Plus;
                }

                codeHtml = codeHtml + "										<td width=\"96%\" height=\"10\" style=\"text-align: left; font-family: DINHabRg; font-size: 9px;\">" + plus1 + "</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td width=\"98%\" height=\"10\" style=\"text-align: left; font-family: DINHabRg; font-size: 9px;\">" + plus2 + "</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td width=\"98%\" height=\"10\" style=\"text-align: left; font-family: DINHabRg; font-size: 9px;\">" + plus3 + "</td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";

                string pourcentagetexte = "";
                string widthPromo = "70";
                if (data.pourcentage != null)
                {
                    pourcentagetexte = "-" + data.pourcentage + "%";
                    widthPromo = "52";
                }

                codeHtml = codeHtml + "							<td width=\"" + widthPromo + "\" height=\"79\" valign=\"top\" class=\"" + typePastille + "\" style=\"text-align: center; font-family: dinhabbold; font-size: 12pt; color: #FFFFFF; line-height: 35px;\">" + pourcentagetexte + "</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "						<tr>";
                codeHtml = codeHtml + "							<td height=\"34\" colspan=\"3\">";
                codeHtml = codeHtml + "								<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td width=\"34\" height=\"33\" rowspan=\"2\" valign=\"top\"></td>";
                codeHtml = codeHtml + "										<td width=\"39\" height=\"22\" style=\"font-family: DINHabRg; font-size: 11px; line-height:7px; text-align: left;\">" + data.sku + "</td>";
                codeHtml = codeHtml + "										<td width=\"16\" height=\"20\" rowspan=\"2\" valign=\"bottom\" id=\"flag_" + data.Made_In + "\"></td>";

                string prixDroite = "";
                string prixGauche = "";
                if (data.pourcentage != null)
                {
                    prixGauche = data.prixPermanent;
                    prixDroite = data.prix;
                }
                else
                {
                    prixDroite = data.prix;
                }

                // ajouter par Cillia (ne pas afficher de prix barre quand le type_prix est permanent)

                if (chevalet.typePrix == ApplicationConsts.typePrix_permanent && data.pourcentage != null)
               {
                   prixGauche = "";
                   prixDroite = data.prixPermanent ;

               }

                codeHtml = codeHtml + "										<td width=\"68\" height=\"22\" valign=\"bottom\" style=\"font-family: DINHabRg; font-size: 11px; text-decoration: line-through; text-align: right; line-height:20px;\">" + prixGauche.Replace(".00", "") + "</td>";
                codeHtml = codeHtml + "										<td width=\"72\" height=\"22\" style=\"font-family: dinhabbold; font-size: 16px; text-align: right; line-height:-15px;\"><span style=\"font-family: dinhabbold\">" + prixDroite.Replace(".00", "") + "</span></td>";
                codeHtml = codeHtml + "										<td> </td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "									<tr>";
                codeHtml = codeHtml + "										<td width=\"39\" height=\"13\" valign=\"top\" style=\"font-family: dinhabmedium; font-size: 9px; text-align: left;\"></td>";
                codeHtml = codeHtml + "										<td height=\"13\" valign=\"top\" style=\"font-family: dinhabmedium; font-size: 9px;\"></td>";
                codeHtml = codeHtml + "										<td width=\"72\" height=\"13\" valign=\"top\" style=\"font-family: DINHabRg; font-size: 6px; line-height: 0; text-align: right;\">" + data.Taxe_eco + "</td>";
                codeHtml = codeHtml + "										<td width=\"5\"> </td>";
                codeHtml = codeHtml + "									</tr>";
                codeHtml = codeHtml + "								</table>";
                codeHtml = codeHtml + "							</td>";
                codeHtml = codeHtml + "						</tr>";
                codeHtml = codeHtml + "					</table>";
                codeHtml = codeHtml + "				</div>";
                codeHtml = codeHtml + "			</td>";

                if (cpt % 3 == 2)
                {
                    codeHtml = codeHtml + "		<td style=\"padding: 0 0 22px 0\">";
                    codeHtml = codeHtml + "			<div class=\"divRegletteRight\"> </div>";
                    codeHtml = codeHtml + "		</td>";
                    codeHtml = codeHtml + "		<tr>";
                }
                cpt++;

            }
            codeHtml = codeHtml + "		</tr>";
            codeHtml = codeHtml + "	</table>";
            codeHtml = codeHtml + "</body>";
            codeHtml = codeHtml + "</html>";
            return codeHtml;
        }


        /// <summary>
        /// Cette methode permet d'enlever le ".0**0" dans le cas d'un décimal entier.
        /// </summary>
        /// <param name="decimalString"></param>
        /// <param name="symbole"></param>
        /// <returns></returns>
        public static List<TickitDataChevalet> splitChevalet(TickitDataChevalet chevalet)
        {
            List<TickitDataChevalet> chevalets = new List<TickitDataChevalet>();

            int nbLoops;
            int nbPartieEntiere;
            int nbMaxReglettesNewParPage = 24;

            nbPartieEntiere = chevalet.produitsData.Count % nbMaxReglettesNewParPage;
            nbLoops = chevalet.produitsData.Count / nbMaxReglettesNewParPage;

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

                if (i < nbLoops) nbMaxIterationsProduitsParChevalet = nbMaxReglettesNewParPage; else nbMaxIterationsProduitsParChevalet = nbPartieEntiere;
                if ((i == nbLoops) && (nbPartieEntiere == 0)) nbMaxIterationsProduitsParChevalet = nbMaxReglettesNewParPage;

                for (int i_pro = 0; i_pro < nbMaxIterationsProduitsParChevalet; i_pro++)
                {
                    int pos = ((i - 1) * nbMaxReglettesNewParPage) + i_pro;
                    chevaletCurrentPage.produitsData.Add(chevalet.produitsData[((i - 1) * nbMaxReglettesNewParPage) + i_pro]);
                }
                chevalets.Add(chevaletCurrentPage);
            }

            return chevalets;
        }

        public static HtmlToPdf getHtmlToPdfModel()
        {
            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set a demo serial number
            htmlToPdfConverter.SerialNumber = Const.ApplicationConsts.HiqPdfSerialNumber;

            // set browser width
            htmlToPdfConverter.BrowserWidth = 595;

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