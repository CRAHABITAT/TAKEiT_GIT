using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Web.Mvc;
using TickitNewFace.Models;
using TickitNewFace.Const;
using TickitNewFace.Utils;

namespace TickitNewFace.PDFUtils
{
    public static class PlvLuminaireUtils
    {
        /// <summary>
        /// Permet de génerer le documentFinal PDF correspondant au type de PLV permanente
        /// </summary>
        /// <param name="chevalet"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        static public MemoryStream GenerateLuminairePdf(TickitDataLuminaire luminaire, String format, int langageId, DateTime dateQuery)
        {
            string codeMonnaie = DAO.LangueDao.getCodeMonnaieByMagasinId(langageId);

            T_Description_Dgccrf DGCCRF_dessous = DAO.Description_DgccrfDao.getDgccrfBySku(luminaire.produitDessous.sku, langageId);
            T_Description_Dgccrf DGCCRF_dessus = DAO.Description_DgccrfDao.getDgccrfBySku(luminaire.produitDessus.sku, langageId);

            string range_dessous = luminaire.produitDessous.range;
            string range_dessus = luminaire.produitDessus.range;

            string variation_dessous = luminaire.produitDessous.variation;
            string variation_dessus = luminaire.produitDessus.variation;

            T_Prix prix_dessous = DAO.PrixDao.getPrixBySkuAndDate(luminaire.produitDessous.sku, langageId, dateQuery);
            T_Prix prix_dessus = DAO.PrixDao.getPrixBySkuAndDate(luminaire.produitDessus.sku, langageId, dateQuery);

            T_Produit produit_dessous = DAO.ProduitDao.getProduitBySku(luminaire.produitDessous.sku, langageId);
            T_Produit produit_dessus = DAO.ProduitDao.getProduitBySku(luminaire.produitDessus.sku, langageId);
            
            Phrase phraseVariationDessus = new Phrase(variation_dessus, new Font(Utils.PoliceUtils.DINHabBd, 35, Font.NORMAL));
            Phrase phraseRangeDessus = new Phrase(Utils.StringUtils.convertStringMajusjToMinus(range_dessus), new Font(Utils.PoliceUtils.DINHabRg, 35, Font.NORMAL));
            Phrase phraseDGCCRFDessus = new Phrase(DGCCRF_dessus == null ? "" : DGCCRF_dessus.LegalDescription, new Font(Utils.PoliceUtils.DINHabRg, 20, Font.NORMAL));
            Phrase phrasePrixDessus = new Phrase((langageId == 23 ? StringUtils.getAriaryMonnaieFormat(prix_dessus.Prix_produit) : prix_dessus.Prix_produit.ToString()) + codeMonnaie, new Font(Utils.PoliceUtils.DINHabRg, 25, Font.NORMAL));
            Phrase phraseTaxeDessus = new Phrase(produit_dessus.Eco_part.ToString(), new Font(Utils.PoliceUtils.DINHabRg, 25, Font.NORMAL));
            
            // MSRIDI 25092015 spécifique Allemagne.
            String motRef = "Réf. ";
            if (langageId == 5) motRef = "";
            Phrase phraseReferenceDessus = new Phrase(motRef + produit_dessus.Sku, new Font(Utils.PoliceUtils.DINHabLig, 20, Font.NORMAL));

            Phrase phraseVariationdessous = new Phrase(variation_dessous, new Font(Utils.PoliceUtils.DINHabBd, 35, Font.NORMAL));
            Phrase phraseRangedessous = new Phrase(Utils.StringUtils.convertStringMajusjToMinus(range_dessous), new Font(Utils.PoliceUtils.DINHabRg, 35, Font.NORMAL));
            Phrase phraseDGCCRFdessous = new Phrase(DGCCRF_dessous == null ? "" : DGCCRF_dessous.LegalDescription, new Font(Utils.PoliceUtils.DINHabRg, 20, Font.NORMAL));
            Phrase phrasePrixdessous = new Phrase((langageId == 23 ? StringUtils.getAriaryMonnaieFormat(prix_dessous.Prix_produit) : prix_dessous.Prix_produit.ToString()) + codeMonnaie, new Font(Utils.PoliceUtils.DINHabRg, 25, Font.NORMAL));
            Phrase phraseTaxedessous = new Phrase(produit_dessous.Eco_part.ToString(), new Font(Utils.PoliceUtils.DINHabRg, 25, Font.NORMAL));
            Phrase phraseLivraisondessous = new Phrase("", new Font(Utils.PoliceUtils.DINHabRg, 25, Font.NORMAL));
            Phrase phraseReferencedessous = new Phrase(motRef + produit_dessous.Sku, new Font(Utils.PoliceUtils.DINHabLig, 20, Font.NORMAL));

            decimal prix_total = prix_dessus.Prix_produit + prix_dessous.Prix_produit;

            int posPoint = prix_total.ToString().IndexOf(".");
            if (posPoint == -1)
            {
                posPoint = prix_total.ToString().IndexOf(",");
            }
            String prixTotalAvantVirgule = prix_total.ToString().Substring(0, posPoint);
            String prixTotalApresVirgule = prix_total.ToString().Substring(posPoint, prix_total.ToString().Length - posPoint) + codeMonnaie;

            if ((prixTotalApresVirgule == ".0" + codeMonnaie) || (prixTotalApresVirgule == ".00" + codeMonnaie) || (prixTotalApresVirgule == ".000" + codeMonnaie))
            {
                prixTotalAvantVirgule = prixTotalAvantVirgule + codeMonnaie;
                prixTotalApresVirgule = "";
            }
            
            Phrase phrasePrixTotalAvantVirgule = new Phrase(langageId == 23 ? StringUtils.getAriaryMonnaieFormat(prix_total) + codeMonnaie : prixTotalAvantVirgule, new Font(Utils.PoliceUtils.DINHabBd, 80, Font.NORMAL));
            Phrase phrasePrixTotalApresVirgule = new Phrase(langageId == 23 ? "" : prixTotalApresVirgule, new Font(Utils.PoliceUtils.DINHabBd, 30, Font.NORMAL));

            MemoryStream ms = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4, -30, -20, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
            PdfPTable tablePage1 = new PdfPTable(2);

            tablePage1.DefaultCell.Border = 0;
            tablePage1.DefaultCell.BorderColor = PatternColor.WHITE;

            pdfDoc.Open();

            float[] columnWidths = new float[] { 75f, 25f };
            tablePage1.SetWidths(columnWidths);

            ////////////////////////////////////////first page////////////////////////////////////////

            /////////////////dessus///////////////////////
            tablePage1.AddCell(spacer(90));

            // cellule titre
            PdfPCell cellRectoDessus = new PdfPCell(phraseVariationDessus);
            cellRectoDessus.Colspan = 2;
            cellRectoDessus.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellRectoDessus.Border = 0;
            tablePage1.AddCell(cellRectoDessus);

            PdfPCell cellRangeDessus = new PdfPCell(phraseRangeDessus);
            cellRangeDessus.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellRangeDessus.Colspan = 2;
            cellRangeDessus.Border = 0;
            tablePage1.AddCell(cellRangeDessus);

            // insère un espace vertical
            tablePage1.AddCell(spacer(20));

            PdfPCell cellDgccrDessus = new PdfPCell(phraseDGCCRFDessus);
            cellDgccrDessus.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellDgccrDessus.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            cellDgccrDessus.Border = 0;
            cellDgccrDessus.MinimumHeight = 90f;
            tablePage1.AddCell(cellDgccrDessus);

            PdfPCell cellPrixDessus = new PdfPCell(new Phrase());
            cellPrixDessus.Phrase.Add(phrasePrixDessus);
            cellPrixDessus.Phrase.Add(Environment.NewLine);
            cellPrixDessus.Phrase.Add(phraseTaxeDessus);
            cellPrixDessus.VerticalAlignment = Element.ALIGN_TOP;
            cellPrixDessus.HorizontalAlignment = Element.ALIGN_CENTER;
            cellPrixDessus.Border = 0;
            tablePage1.AddCell(cellPrixDessus);

            tablePage1.AddCell(spacer(10));

            PdfPCell cellReferenceDessus = new PdfPCell(phraseReferenceDessus);
            cellReferenceDessus.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellReferenceDessus.HorizontalAlignment = Element.ALIGN_LEFT;
            cellReferenceDessus.Border = 0;
            cellReferenceDessus.Colspan = 2;
            tablePage1.AddCell(cellReferenceDessus);

            /////////////////dessous///////////////////////
            tablePage1.AddCell(spacer(50));

            // cellule titre
            PdfPCell cellTitreDessous = new PdfPCell(phraseVariationdessous);
            cellTitreDessous.Colspan = 2;
            cellTitreDessous.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellTitreDessous.Border = 0;
            tablePage1.AddCell(cellTitreDessous);

            PdfPCell cellRangeDessous = new PdfPCell(phraseRangedessous);
            cellRangeDessous.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellRangeDessous.Colspan = 2;
            cellRangeDessous.Border = 0;
            tablePage1.AddCell(cellRangeDessous);

            // insère un espace vertical
            tablePage1.AddCell(spacer(20));

            PdfPCell cellDgccrDessous = new PdfPCell(phraseDGCCRFdessous);
            cellDgccrDessous.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellDgccrDessous.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            cellDgccrDessous.Border = 0;
            cellDgccrDessous.MinimumHeight = 90f;
            tablePage1.AddCell(cellDgccrDessous);

            PdfPCell cellPrixDessous = new PdfPCell(new Phrase());
            cellPrixDessous.Phrase.Add(phrasePrixdessous);
            cellPrixDessous.Phrase.Add(Environment.NewLine);
            cellPrixDessous.Phrase.Add(phraseTaxedessous);
            cellPrixDessous.Phrase.Add(Environment.NewLine);
            cellPrixDessous.Phrase.Add(phraseLivraisondessous);
            cellPrixDessous.VerticalAlignment = Element.ALIGN_TOP;
            cellPrixDessous.HorizontalAlignment = Element.ALIGN_CENTER;
            cellPrixDessous.Border = 0;
            tablePage1.AddCell(cellPrixDessous);

            tablePage1.AddCell(spacer(10));

            PdfPCell cellReferenceDessous = new PdfPCell(phraseReferencedessous);
            cellReferenceDessous.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellReferenceDessous.HorizontalAlignment = Element.ALIGN_LEFT;
            cellReferenceDessous.Border = 0;
            cellReferenceDessous.Colspan = 2;
            tablePage1.AddCell(cellReferenceDessous);

            /////////////////Prix total///////////////////////
            tablePage1.AddCell(spacer(50));

            PdfPCell cellPrixTotal = new PdfPCell(phrasePrixTotalAvantVirgule);
            cellPrixTotal.Phrase.Add(phrasePrixTotalApresVirgule);
            cellPrixTotal.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellPrixTotal.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPrixTotal.Border = 0;
            cellPrixTotal.Colspan = 2;
            tablePage1.AddCell(cellPrixTotal);

            pdfDoc.Add(tablePage1);

            ////////////////////////////////////////second page////////////////////////////////////////

            pdfDoc.NewPage();
            pdfDoc.Add(tablePage1);
            pdfDoc.Close();
            writer.Dispose();

            ////////////////////////////////////////Document Final//////////////////////////////////////

            byte[] file = ms.ToArray();
            MemoryStream msReadDocOrigine = new MemoryStream();
            msReadDocOrigine.Write(file, 0, file.Length);
            msReadDocOrigine.Position = 0;
            PdfReader readerDocOrigine = new PdfReader(msReadDocOrigine);

            MemoryStream msDocFinal = new MemoryStream();
            Document documentFinal = new Document();

            int nombreColonnes = 0;
            int nombreTickets = 0;

            if (format == ApplicationConsts.format_A5_simple)
            {
                documentFinal = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);
                nombreColonnes = 2;
                nombreTickets = 1;
            }
            if (format == ApplicationConsts.format_A6_simple)
            {
                documentFinal = new Document(PageSize.A4, 0, 0, 0, 0);
                nombreColonnes = 2;
                nombreTickets = 2;
            }

            if (format == ApplicationConsts.format_A7_simple)
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
    }
}
