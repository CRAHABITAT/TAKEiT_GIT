using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Web.Mvc;
using TickitNewFace.Models;
using TickitNewFace.Const;
using System.Linq;

namespace TickitNewFace.PDFUtils
{
    public static class PlvChevaletUtils
    {
        /// <summary>
        /// Permet de génerer le documentFinal PDF correspondant au type de PLV permanente
        /// </summary>
        /// <param name="chevalet"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        static public MemoryStream GenerateChevaletDeGammePdf(TickitDataChevalet chevalet, String format, int langageId)
        {
            // passage de la division range.
            List<string> divisionsProduits = new List<string>();
            string divisionRange = "";
            
            foreach (TickitDataProduit produit in chevalet.produitsData)
            {
                divisionsProduits.Add(produit.division);
            }

            var distinctDivisions = (from div in divisionsProduits select div).Distinct();
            List<string> ls = distinctDivisions.ToList();

            if (ls.Count == 1)
            {
                divisionRange = ls[0];
            }

            // Obligatoire pour ne pas déplacer le reste de l'étiquette si le libellé de la gamme est vide.
            string libelleRange = DAO.RangeDao.getLibelleByRangeName(chevalet.rangeChevalet, langageId, divisionRange);
            if (libelleRange == "" || libelleRange == null)
            {
                // garder au moins un caractère
                libelleRange = " ";
            }

            Phrase phraselibelleRange = new Phrase(libelleRange, new Font(Utils.PoliceUtils.DINHabBd, 35, Font.NORMAL));
            Phrase phraseRangeName = new Phrase(Utils.StringUtils.convertStringMajusjToMinus(chevalet.rangeChevalet), new Font(Utils.PoliceUtils.DINHabRg, 35, Font.NORMAL));

            Phrase phrasePlusRange = new Phrase(Managers.TickitDataManager.getPlusOfRange(chevalet.rangeChevalet, langageId, divisionRange), new Font(Utils.PoliceUtils.DINHabRg, 20, Font.NORMAL));

            Phrase phraseInfoEntete = new Phrase(Resources.Langue.LesInfos, new Font(Utils.PoliceUtils.DINHabHl, 35, Font.NORMAL));
            Phrase phrasePourcentageReduction = new Phrase("-" + getFormattedDecimalToStringWithoutNeedlessPoint(chevalet.pourcentageReduction.ToString(), "%"), new Font(Utils.PoliceUtils.DINHabHl, 60, Font.NORMAL));

            MemoryStream ms = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4, -30, -30, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);

            pdfDoc.Open();
            float[] columnWidths = new float[] { 50f, 50f };

            ////////////////////////////////////////first page////////////////////////////////////////

            PdfPTable tablePage1 = new PdfPTable(2);
            tablePage1.SetWidths(columnWidths);

            tablePage1.AddCell(spacer(120));

            // specifique solde
            if (chevalet.typePrix == ApplicationConsts.typePrix_promo || chevalet.typePrix == ApplicationConsts.typePrix_solde) { tablePage1.AddCell(spacer(75)); }

            PdfPCell cellRangeLibelleRecto = new PdfPCell(phraselibelleRange);
            cellRangeLibelleRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellRangeLibelleRecto.Colspan = 2;
            cellRangeLibelleRecto.Border = 0;
            tablePage1.AddCell(cellRangeLibelleRecto);

            tablePage1.AddCell(spacer(5));

            PdfPCell cellRangeNameRecto = new PdfPCell(phraseRangeName);
            cellRangeNameRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellRangeNameRecto.Colspan = 2;
            cellRangeNameRecto.Border = 0;
            tablePage1.AddCell(cellRangeNameRecto);

            // spécifique solde
            if (chevalet.typePrix == ApplicationConsts.typePrix_promo || chevalet.typePrix == ApplicationConsts.typePrix_solde) { tablePage1.AddCell(spacer(35)); }
            else { tablePage1.AddCell(spacer(110)); }

            // spécifique solde
            PdfPCell cellEnteteInfo = new PdfPCell(phraseInfoEntete);
            cellEnteteInfo.Border = 0;
            cellEnteteInfo.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellEnteteInfo.HorizontalAlignment = Element.ALIGN_LEFT;

            // spécifique solde
            PdfPCell cellPourcentageReduction = new PdfPCell(phrasePourcentageReduction);
            //cellPourcentageReduction.Phrase.Add(phraseSymbolePourcentage);
            cellPourcentageReduction.Border = 0;
            cellPourcentageReduction.VerticalAlignment = Element.ALIGN_TOP;
            cellPourcentageReduction.HorizontalAlignment = Element.ALIGN_RIGHT;

            // spécifique solde
            if (chevalet.typePrix == ApplicationConsts.typePrix_promo || chevalet.typePrix == ApplicationConsts.typePrix_solde)
            {
                tablePage1.AddCell(cellEnteteInfo);
                tablePage1.AddCell(cellPourcentageReduction);
                tablePage1.AddCell(spacer(10));
            }

            PdfPCell cellPlusGamme = new PdfPCell(phrasePlusRange);
            cellPlusGamme.VerticalAlignment = Element.ALIGN_TOP;
            cellPlusGamme.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            cellPlusGamme.Colspan = 2;
            cellPlusGamme.Border = 0;
            tablePage1.AddCell(cellPlusGamme);

            tablePage1.AddCell(spacer(20));

            List<TickitDataProduit> listeChevaletPage1 = new List<TickitDataProduit>();
            List<TickitDataProduit> listeChevaletPage2 = new List<TickitDataProduit>();
            if (chevalet.produitsData.Count > 7)
            {
                for (int i = 0; i < 7; i++)
                {
                    listeChevaletPage1.Add(chevalet.produitsData[i]);
                }

                for (int i = 7; i < chevalet.produitsData.Count; i++)
                {
                    listeChevaletPage2.Add(chevalet.produitsData[i]);
                }
            }
            else
            {
                for (int i = 0; i < chevalet.produitsData.Count; i++)
                {
                    listeChevaletPage1.Add(chevalet.produitsData[i]);
                }
            }

            foreach (TickitDataProduit item in listeChevaletPage1)
            {
                Phrase phraseItem = new Phrase(item.variation, new Font(Utils.PoliceUtils.DINHabRg, 18, Font.NORMAL));
                Phrase phraseprixReference = new Phrase(item.prixPermanent, new Font(Utils.PoliceUtils.DINHabRg, 18, Font.STRIKETHRU));
                Phrase espaceur = new Phrase("   ", new Font(Utils.PoliceUtils.DINHabRg, 22, Font.NORMAL));
                Phrase phraseprix = new Phrase(item.prix, new Font(Utils.PoliceUtils.DINHabBd, 26, Font.NORMAL));
                Phrase phraseTaxe = new Phrase(item.Taxe_eco, new Font(Utils.PoliceUtils.DINHabRg, 10, Font.NORMAL));

                // MSRIDI 14122015
                Phrase phrasePrixSoldePrecedent = new Phrase(item.prixSoldePrecedent, new Font(Utils.PoliceUtils.DINHabRg, 18, Font.STRIKETHRU));

                PdfPCell cellItems = new PdfPCell(phraseItem);
                cellItems.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellItems.HorizontalAlignment = Element.ALIGN_LEFT;
                cellItems.Border = 0;
                tablePage1.AddCell(cellItems);
                
                PdfPCell cellPrix = new PdfPCell(new Phrase());
                cellPrix.Phrase.Add(phraseprixReference);
                cellPrix.Phrase.Add(espaceur);

                // MSRIDI 14122015
                if (item.prixSoldePrecedent != "") 
                {
                    cellPrix.Phrase.Add(phrasePrixSoldePrecedent);
                    cellPrix.Phrase.Add(espaceur);
                }

                cellPrix.Phrase.Add(phraseprix);
                cellPrix.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellPrix.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPrix.Border = 0;
                tablePage1.AddCell(cellPrix);

                PdfPCell cellTaxeEco = new PdfPCell(new Phrase());
                cellTaxeEco.Phrase.Add(phraseTaxe);
                cellTaxeEco.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellTaxeEco.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellTaxeEco.Border = 0;
                cellTaxeEco.Colspan = 2;
                tablePage1.AddCell(cellTaxeEco);

                tablePage1.AddCell(spacer(10));
            }

            pdfDoc.Add(tablePage1);

            ////////////////////////////////////////second page////////////////////////////////////////

            pdfDoc.NewPage();

            PdfPTable tablePage2 = new PdfPTable(2);
            tablePage2.SetWidths(columnWidths);

            // spécifique solde
            if (chevalet.typePrix == ApplicationConsts.typePrix_promo || chevalet.typePrix == ApplicationConsts.typePrix_solde) { tablePage2.AddCell(spacer(75)); }

            tablePage2.AddCell(spacer(120));
            tablePage2.AddCell(cellRangeLibelleRecto);
            tablePage2.AddCell(spacer(5));
            tablePage2.AddCell(cellRangeNameRecto);

            // spécifique solde
            if (chevalet.typePrix == ApplicationConsts.typePrix_promo || chevalet.typePrix == ApplicationConsts.typePrix_solde) { tablePage2.AddCell(spacer(35)); }
            else { tablePage2.AddCell(spacer(110)); }

            // spécifique solde
            if (chevalet.typePrix == ApplicationConsts.typePrix_promo || chevalet.typePrix == ApplicationConsts.typePrix_solde)
            {
                tablePage2.AddCell(cellEnteteInfo);
                tablePage2.AddCell(cellPourcentageReduction);
                tablePage2.AddCell(spacer(10));
            }

            tablePage2.AddCell(cellPlusGamme);
            tablePage2.AddCell(spacer(20));

            foreach (TickitDataProduit item in listeChevaletPage2)
            {
                Phrase phraseItem = new Phrase(item.variation, new Font(Utils.PoliceUtils.DINHabRg, 18, Font.NORMAL));
                Phrase phraseprixReference = new Phrase(item.prixPermanent, new Font(Utils.PoliceUtils.DINHabRg, 18, Font.STRIKETHRU));
                Phrase espaceur = new Phrase("   ", new Font(Utils.PoliceUtils.DINHabRg, 22, Font.NORMAL));
                Phrase phraseprix = new Phrase(item.prix, new Font(Utils.PoliceUtils.DINHabBd, 26, Font.NORMAL));
                Phrase phraseTaxe = new Phrase(item.Taxe_eco, new Font(Utils.PoliceUtils.DINHabRg, 10, Font.NORMAL));

                // MSRIDI 14122015
                Phrase phrasePrixSoldePrecedent = new Phrase(item.prixSoldePrecedent, new Font(Utils.PoliceUtils.DINHabRg, 18, Font.STRIKETHRU));

                PdfPCell cellItems = new PdfPCell(phraseItem);
                cellItems.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellItems.HorizontalAlignment = Element.ALIGN_LEFT;
                cellItems.Border = 0;
                tablePage2.AddCell(cellItems);

                PdfPCell cellPrix = new PdfPCell(new Phrase());
                cellPrix.Phrase.Add(phraseprixReference);
                cellPrix.Phrase.Add(espaceur);

                // MSRIDI 14122015
                if (item.prixSoldePrecedent != "")
                {
                    cellPrix.Phrase.Add(phrasePrixSoldePrecedent);
                    cellPrix.Phrase.Add(espaceur);
                }

                cellPrix.Phrase.Add(phraseprix);
                cellPrix.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellPrix.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPrix.Border = 0;
                tablePage2.AddCell(cellPrix);

                PdfPCell cellTaxeEco = new PdfPCell(new Phrase());
                cellTaxeEco.Phrase.Add(phraseTaxe);
                cellTaxeEco.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellTaxeEco.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellTaxeEco.Border = 0;
                cellTaxeEco.Colspan = 2;
                tablePage2.AddCell(cellTaxeEco);

                tablePage2.AddCell(spacer(10));
            }

            pdfDoc.Add(tablePage2);

            pdfDoc.Close();
            writer.Dispose();

            ////////////////////////////////////////Document Final////////////////////////////////////////

            byte[] file = ms.ToArray();
            MemoryStream msReadDocOrigine = new MemoryStream();
            msReadDocOrigine.Write(file, 0, file.Length);
            msReadDocOrigine.Position = 0;
            PdfReader readerDocOrigine = new PdfReader(msReadDocOrigine);

            MemoryStream msDocFinal = new MemoryStream();
            Document documentFinal = new Document();

            int nombreColonnes = 0;
            int nombreTickets = 0;

            if (format == ApplicationConsts.format_A5_recto_verso)
            {
                documentFinal = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);
                nombreColonnes = 2;
                nombreTickets = 1;
            }
            if (format == ApplicationConsts.format_A6_recto_verso)
            {
                documentFinal = new Document(PageSize.A4, 0, 0, 0, 0);
                nombreColonnes = 2;
                nombreTickets = 2;
            }

            if (format == ApplicationConsts.format_A7_recto_verso)
            {
                documentFinal = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);
                nombreColonnes = 4;
                nombreTickets = 4;
            }

            PdfWriter writerDocFinal = PdfWriter.GetInstance(documentFinal, msDocFinal);
            documentFinal.Open();
            PdfImportedPage page;

            PdfPTable tableFinale = new PdfPTable(nombreColonnes);
            tableFinale.DefaultCell.Border = Rectangle.NO_BORDER;
            tableFinale.WidthPercentage = 100;

            for (int j = 0; j < nombreTickets; j++)
            {
                for (int i = 1; i <= readerDocOrigine.NumberOfPages; i++)
                {
                    page = writerDocFinal.GetImportedPage(readerDocOrigine, i);

                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(page);
                    image.Border = 0;
                    image.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    tableFinale.AddCell(image);
                }
            }

            documentFinal.Add(tableFinale);
            documentFinal.Close();

            MemoryStream msToReturn = new MemoryStream();
            msToReturn.Write(msDocFinal.ToArray(), 0, msDocFinal.ToArray().Length);
            msToReturn.Position = 0;

            return msToReturn;
        }

        /// <summary>
        /// Permet de génerer le documentFinal PDF correspondant au type de PLV permanante
        /// </summary>
        /// <param name="chevalet"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        static public MemoryStream GenerateChevaletVitrinePdf(TickitDataChevalet chevalet, String format, int langageId)
        {
            MemoryStream ms = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4, -30, -20, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);

            pdfDoc.Open();
            float[] columnWidths = new float[] { 75f, 25f };

            ////////////////////////////////////////first page////////////////////////////////////////

            PdfPTable tablePage1 = new PdfPTable(2);

            tablePage1.SetWidths(columnWidths);

            tablePage1.AddCell(spacer(153));

            List<TickitDataProduit> listeChevaletPage1 = new List<TickitDataProduit>();
            List<TickitDataProduit> listeChevaletPage2 = new List<TickitDataProduit>();

            for (int i = 0; i < chevalet.produitsData.Count; i++)
            {
                listeChevaletPage1.Add(chevalet.produitsData[i]);
                listeChevaletPage2.Add(chevalet.produitsData[i]);
            }

            foreach (TickitDataProduit item in listeChevaletPage1)
            {
                Phrase phraseItem = new Phrase(item.variation + " " + Utils.StringUtils.convertStringMajusjToMinus(item.range), new Font(Utils.PoliceUtils.DINHabRg, 20, Font.NORMAL));
                Phrase phraseprix = new Phrase(getFormattedDecimalToStringWithoutNeedlessPoint(item.prix, "€"), new Font(Utils.PoliceUtils.DINHabBd, 20, Font.NORMAL));

                PdfPCell cellItems = new PdfPCell(phraseItem);
                cellItems.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellItems.HorizontalAlignment = Element.ALIGN_LEFT;
                cellItems.Border = 0;
                tablePage1.AddCell(cellItems);

                PdfPCell cellPrix = new PdfPCell(phraseprix);
                cellPrix.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellPrix.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPrix.Border = 0;
                tablePage1.AddCell(cellPrix);

                tablePage1.AddCell(spacer(13));
            }

            pdfDoc.Add(tablePage1);

            ////////////////////////////////////////second page////////////////////////////////////////

            pdfDoc.NewPage();

            // la deuxième page est la copie de la première.
            pdfDoc.Add(tablePage1);

            pdfDoc.Close();
            writer.Dispose();

            ////////////////////////////////////////Document Final////////////////////////////////////////

            byte[] file = ms.ToArray();
            MemoryStream msReadDocOrigine = new MemoryStream();
            msReadDocOrigine.Write(file, 0, file.Length);
            msReadDocOrigine.Position = 0;
            PdfReader readerDocOrigine = new PdfReader(msReadDocOrigine);

            MemoryStream msDocFinal = new MemoryStream();
            Document documentFinal = new Document();

            int nombreColonnes = 0;
            int nombreTickets = 0;

            if (format == ApplicationConsts.format_A5_recto_verso)
            {
                documentFinal = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);
                nombreColonnes = 2;
                nombreTickets = 1;
            }
            if (format == ApplicationConsts.format_A6_recto_verso)
            {
                documentFinal = new Document(PageSize.A4, 0, 0, 0, 0);
                nombreColonnes = 2;
                nombreTickets = 2;
            }

            if (format == ApplicationConsts.format_A7_recto_verso)
            {
                documentFinal = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);
                nombreColonnes = 4;
                nombreTickets = 4;
            }

            PdfWriter writerDocFinal = PdfWriter.GetInstance(documentFinal, msDocFinal);
            documentFinal.Open();
            PdfImportedPage page;

            PdfPTable tableFinale = new PdfPTable(nombreColonnes);
            tableFinale.DefaultCell.Border = Rectangle.NO_BORDER;
            tableFinale.WidthPercentage = 100;

            for (int j = 0; j < nombreTickets; j++)
            {
                for (int i = 1; i <= readerDocOrigine.NumberOfPages; i++)
                {
                    page = writerDocFinal.GetImportedPage(readerDocOrigine, i);

                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(page);
                    image.Border = 0;
                    image.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    tableFinale.AddCell(image);
                }
            }

            documentFinal.Add(tableFinale);
            documentFinal.Close();

            MemoryStream msToReturn = new MemoryStream();
            msToReturn.Write(msDocFinal.ToArray(), 0, msDocFinal.ToArray().Length);
            msToReturn.Position = 0;

            return msToReturn;
        }

        /// <summary>
        /// retourne une cellule espace de hauteur passée en paramètre.
        /// </summary>
        /// <param name="Hauteur"></param>
        /// <returns></returns>
        private static PdfPCell spacer(int Height)
        {
            PdfPCell cellSpacer = new PdfPCell(new Phrase());
            cellSpacer.Colspan = 2;
            cellSpacer.FixedHeight = Height;
            cellSpacer.Border = 0;
            return cellSpacer;
        }

        /// <summary>
        /// Cette methode permet d'enlever le ".0**0" dans le cas d'un décimal entier.
        /// </summary>
        /// <param name="decimalString"></param>
        /// <param name="symbole"></param>
        /// <returns></returns>
        private static string getFormattedDecimalToStringWithoutNeedlessPoint(string decimalString, string symbole)
        {

            if (decimalString == null || decimalString == "") { return ""; }
            if (symbole == "%") { decimalString = decimalString + "%"; }

            int posPoint = decimalString.ToString().IndexOf(".");
            if (posPoint == -1)
            {
                posPoint = decimalString.ToString().IndexOf(",");
            }

            if (posPoint == -1)
            {
                return decimalString;
            }

            string stringAvantVirgule = decimalString.ToString().Substring(0, posPoint);
            string stringApresVirgule = decimalString.ToString().Substring(posPoint, decimalString.ToString().Length - posPoint);

            if ((stringApresVirgule == ".0" + symbole) || (stringApresVirgule == ".00" + symbole) || (stringApresVirgule == ".000" + symbole))
            {
                stringAvantVirgule = stringAvantVirgule + symbole;
                stringApresVirgule = "";
            }

            return stringAvantVirgule + stringApresVirgule;
        }
    }
}