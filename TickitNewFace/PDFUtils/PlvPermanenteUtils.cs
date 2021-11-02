using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using TickitNewFace.Models;
using TickitNewFace.Const;

namespace TickitNewFace.PDFUtils
{
    public static class PlvPermanenteUtils
    {
        /// <summary>
        /// Permet de génerer le documentFinal PDF correspondant au type de PLV permanante
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        static public MemoryStream GenerateDocumentPlvRectoVerso(List<TickitDataProduit> tickets, String format, int magasinId)
        {
            List<PdfReader> readers = new List<PdfReader>();

            foreach (TickitDataProduit tick in tickets)
            {
                MemoryStream ms = getMSTicketRectoVerso(tick, magasinId);
                byte[] file = ms.ToArray();
                MemoryStream msReadDocOrigine = new MemoryStream();
                msReadDocOrigine.Write(file, 0, file.Length);
                msReadDocOrigine.Position = 0;
                PdfReader readerDocOrigine = new PdfReader(msReadDocOrigine);
                readers.Add(readerDocOrigine);
            }
            
            MemoryStream msDocFinal = new MemoryStream();
            Document documentFinal = new Document();

            int nombreColonnes = 0;
            
            if (format == ApplicationConsts.format_A5_recto_verso)
            {
                documentFinal = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);
                nombreColonnes = 2;
            }

            if (format == ApplicationConsts.format_A6_recto_verso)
            {
                documentFinal = new Document(PageSize.A4, 0, 0, 0, 0);
                nombreColonnes = 2;
            }

            if (format == ApplicationConsts.format_A7_recto_verso)
            {
                documentFinal = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);
                nombreColonnes = 4;
            }

            PdfWriter writerDocFinal = PdfWriter.GetInstance(documentFinal, msDocFinal);
            documentFinal.Open();

            PdfPTable tableFinale = new PdfPTable(nombreColonnes);
            tableFinale.DefaultCell.Border = Rectangle.NO_BORDER;
            tableFinale.WidthPercentage = 100;

            PdfPCell emptyCell = new PdfPCell();
            emptyCell.Border = 0;

            foreach (PdfReader reader in readers)
            {
                PdfImportedPage recto = writerDocFinal.GetImportedPage(reader, 1);
                iTextSharp.text.Image imageRecto = iTextSharp.text.Image.GetInstance(recto);
                tableFinale.AddCell(imageRecto);

                PdfImportedPage verso = writerDocFinal.GetImportedPage(reader, 2);
                iTextSharp.text.Image imageVerso = iTextSharp.text.Image.GetInstance(verso);
                tableFinale.AddCell(imageVerso);
            }

            if ((format == ApplicationConsts.format_A7_recto_verso) && (tickets.Count % 4 != 0))
            {
                for (int i = 0; i < 8 - tickets.Count % 4; i++)
                {
                    tableFinale.AddCell(emptyCell);
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
        /// Renvoie un objet Memory Stream pour un ticket.
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        static public MemoryStream getMSTicketRectoVerso(TickitDataProduit ticket, int magasinId)
        {
            string codeMonnaie = DAO.LangueDao.getCodeMonnaieByMagasinId(magasinId);
            string PLV_PERMANENTE_RECTO_VERSO_TAILLE_PRIX = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_PERMANENTE_RECTO_VERSO_TAILLE_PRIX", magasinId);
            
            Phrase phraseTitre = new Phrase(ticket.variation, new Font(Utils.PoliceUtils.DINHabBd, 40, Font.NORMAL));
            Phrase phraseRange = new Phrase(Utils.StringUtils.convertStringMajusjToMinus(ticket.range), new Font(Utils.PoliceUtils.DINHabRg, 40, Font.NORMAL));
            
            
            Phrase phraseNouvPrixAvantVirgule = new Phrase(ticket.prixSansTaxeAvantVirgule, new Font(Utils.PoliceUtils.DINHabBd, Int16.Parse(PLV_PERMANENTE_RECTO_VERSO_TAILLE_PRIX), Font.NORMAL));
            
            Phrase phraseNouvPrixApresVirgule = new Phrase(ticket.prixSansTaxeApresVirgule, new Font(Utils.PoliceUtils.DINHabBd, 25, Font.NORMAL));
            
            Phrase phraseLivraison = new Phrase(ticket.livraison, new Font(Utils.PoliceUtils.DINHabRg, 15, Font.NORMAL));
            Phrase phraseMontantTaxeEco = new Phrase(ticket.Taxe_eco, new Font(Utils.PoliceUtils.DINHabRg, 20, Font.BOLD));
            Phrase phrasePrixAvecTaxe = new Phrase(ticket.prixAvecTaxe, new Font(Utils.PoliceUtils.DINHabRg, 30, Font.BOLD));
            
            Phrase phraseDontTaxe = new Phrase(ticket.Taxe_eco == "" ? "" : "Dont ", new Font(Utils.PoliceUtils.DINHabRg, 15, Font.NORMAL));
            Phrase phraseTaxeEco = new Phrase(ticket.Taxe_eco == "" ? "" : " d'éco-part", new Font(Utils.PoliceUtils.DINHabRg, 15, Font.NORMAL));
            Phrase phraseTaxeInculuse = new Phrase(ticket.Taxe_eco == "" ? "" : " éco-part incluse", new Font(Utils.PoliceUtils.DINHabRg, 15, Font.NORMAL));
            //Phrase phraseHorsEcoPart = new Phrase("Hors éco-part", new Font(Utils.PoliceUtils.DINHabRg, 15, Font.NORMAL));

            Phrase phrasePlus = new Phrase("", new Font(Utils.PoliceUtils.DINHabRg, 40, Font.NORMAL));
            
            foreach (String elementPlus in ticket.plus)
            {
                phrasePlus.Add(elementPlus);
                phrasePlus.Add(Environment.NewLine);
            }
            
            Phrase phraseDimensions = new Phrase(ticket.dimension, new Font(Utils.PoliceUtils.DINHabRg, 20, Font.NORMAL));
            Phrase phraseAMonterSoiMeme = new Phrase(ticket.aMonterSoiMeme, new Font(Utils.PoliceUtils.DINHabRg, 30, Font.NORMAL));
            Phrase phraseDGCCRF = new Phrase(ticket.DGCCRF, new Font(Utils.PoliceUtils.DINHabRg, 23, Font.NORMAL));
            Phrase phraseSku = new Phrase(ticket.sku, new Font(Utils.PoliceUtils.DINHabLig, 20, Font.NORMAL));

            Phrase phraseNbreColis = new Phrase(ticket.Nombre_colis, new Font(Utils.PoliceUtils.DINHabLig, 20, Font.NORMAL));

            MemoryStream ms = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4, -30, -20, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
            PdfPTable tablePage1 = new PdfPTable(2);
            
            tablePage1.DefaultCell.Border = 0;
            tablePage1.DefaultCell.BorderColor = PatternColor.WHITE;

            pdfDoc.Open();

            float[] columnWidths = new float[] { 55f, 45f };
            tablePage1.SetWidths(columnWidths);

            ////////////////////////////////////////first page////////////////////////////////////////            

            tablePage1.AddCell(spacer(120));

            // cellule titre
            PdfPCell cellTitreRecto = new PdfPCell(phraseTitre);
            cellTitreRecto.Colspan = 2;
            cellTitreRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellTitreRecto.Border = 0;
            tablePage1.AddCell(cellTitreRecto);

            PdfPCell cellRangeRecto = new PdfPCell(phraseRange);
            cellRangeRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellRangeRecto.Colspan = 2;
            cellRangeRecto.Border = 0;
            tablePage1.AddCell(cellRangeRecto);

            // insère un espace vertical
            tablePage1.AddCell(spacer(75));

            PdfPCell cellPriceRecto = new PdfPCell(new Phrase());
            cellPriceRecto.Phrase.Add(phraseNouvPrixAvantVirgule);
            cellPriceRecto.Phrase.Add(phraseNouvPrixApresVirgule);   
            cellPriceRecto.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPriceRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellPriceRecto.Border = 0;
            tablePage1.AddCell(cellPriceRecto);
            
            PdfPCell cellPrixTaxeEtLivraisonRecto = new PdfPCell(new Phrase());
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(phraseDontTaxe);
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(phraseMontantTaxeEco);
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(phraseTaxeEco);
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(Environment.NewLine);
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(phrasePrixAvecTaxe);
            //cellPrixTaxeEtLivraisonRecto.Phrase.Add(phraseTaxeInculuse);
            //cellPrixTaxeEtLivraisonRecto.Phrase.Add(Environment.NewLine);
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(phraseLivraison);
            cellPrixTaxeEtLivraisonRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellPrixTaxeEtLivraisonRecto.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPrixTaxeEtLivraisonRecto.Border = 0;
            tablePage1.AddCell(cellPrixTaxeEtLivraisonRecto);

            tablePage1.AddCell(spacer(120));

            PdfPCell cellPlusRecto = new PdfPCell(phrasePlus);
            cellPlusRecto.Colspan = 2;
            cellPlusRecto.Border = 0;
            cellPlusRecto.MinimumHeight = 180f;
            tablePage1.AddCell(cellPlusRecto);

            // Choix de l'image à insérer dans le document PDF.
            if (ticket.Made_In == ApplicationConsts.made_in_FR || ticket.Made_In == ApplicationConsts.made_in_IT || ticket.Made_In == ApplicationConsts.made_in_UE)
            {
                tablePage1.AddCell(spacer(40));

                string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                string pathImgFr = "Content//themes//image//made_" + ticket.Made_In + ".png";

                PdfPCell cellImage = new PdfPCell(Image.GetInstance(applicationPath + pathImgFr));
                cellImage.Colspan = 2;
                cellImage.Border = 0;
                tablePage1.AddCell(cellImage);
            }

            pdfDoc.Add(tablePage1);

            ////////////////////////////////////////second page////////////////////////////////////////

            pdfDoc.NewPage();

            PdfPTable tablePage2 = new PdfPTable(2);
            tablePage2.WidthPercentage = 80;

            tablePage2.SetWidths(columnWidths);

            tablePage2.AddCell(spacer(370));

            PdfPCell cellTitreVerso = new PdfPCell(phraseTitre);
            cellTitreVerso.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellTitreVerso.Colspan = 2;
            cellTitreVerso.Border = 0;
            tablePage2.AddCell(cellTitreVerso);

            PdfPCell cellRangeVerso = new PdfPCell(phraseRange);
            cellRangeVerso.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellRangeVerso.Colspan = 2;
            cellRangeVerso.Border = 0;
            tablePage2.AddCell(cellRangeVerso);

            tablePage2.AddCell(spacer(50));

            PdfPCell cellDimensionsVerso = new PdfPCell(phraseDimensions);
            cellDimensionsVerso.VerticalAlignment = Element.ALIGN_TOP;
            cellDimensionsVerso.Border = 0;
            tablePage2.AddCell(cellDimensionsVerso);

            PdfPCell cellSkuVerso = new PdfPCell(phraseSku);
            cellSkuVerso.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellSkuVerso.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellSkuVerso.Border = 0;
            tablePage2.AddCell(cellSkuVerso);

            PdfPCell cellAMonterVerso = new PdfPCell(phraseAMonterSoiMeme);
            cellAMonterVerso.VerticalAlignment = Element.ALIGN_TOP;
            cellAMonterVerso.Border = 0;
            tablePage2.AddCell(cellAMonterVerso);

            PdfPCell cellNbreColisVerso = new PdfPCell(phraseNbreColis);
            cellNbreColisVerso.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellNbreColisVerso.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellNbreColisVerso.Border = 0;
            tablePage2.AddCell(cellNbreColisVerso);

            tablePage2.AddCell(spacer(30));

            PdfPCell cellDgccrfVerso = new PdfPCell(phraseDGCCRF);
            cellDgccrfVerso.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellDgccrfVerso.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            cellDgccrfVerso.Border = 0;
            cellDgccrfVerso.Colspan = 2;
            tablePage2.AddCell(cellDgccrfVerso);

            pdfDoc.Add(tablePage2);

            pdfDoc.Close();
            writer.Dispose();

            return ms;
        }





        /// <summary>
        /// Génère un flux PLV de type simple (Recto = Verso).
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        static public MemoryStream GenerateDocumentPlvSimple(List<TickitDataProduit> tickets, String format, int magasinId)
        {
            List<PdfReader> readers = new List<PdfReader>();

            foreach (TickitDataProduit tick in tickets)
            {
                MemoryStream ms = getMSTicketSimple(tick, magasinId);
                byte[] file = ms.ToArray();
                MemoryStream msReadDocOrigine = new MemoryStream();
                msReadDocOrigine.Write(file, 0, file.Length);
                msReadDocOrigine.Position = 0;
                PdfReader readerDocOrigine = new PdfReader(msReadDocOrigine);
                readers.Add(readerDocOrigine);
            }

            MemoryStream msDocFinal = new MemoryStream();
            Document documentFinal = new Document();

            int nombreColonnes = 0;

            if (format == ApplicationConsts.format_A5_simple)
            {
                documentFinal = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);
                nombreColonnes = 2;
            }

            if (format == ApplicationConsts.format_A6_simple)
            {
                documentFinal = new Document(PageSize.A4, 0, 0, 0, 0);
                nombreColonnes = 2;
            }

            if (format == ApplicationConsts.format_A7_simple)
            {
                documentFinal = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);
                nombreColonnes = 4;
            }

            PdfWriter writerDocFinal = PdfWriter.GetInstance(documentFinal, msDocFinal);
            documentFinal.Open();

            PdfPTable tableFinale = new PdfPTable(nombreColonnes);
            tableFinale.DefaultCell.Border = Rectangle.NO_BORDER;
            tableFinale.WidthPercentage = 100;

            PdfPCell emptyCell = new PdfPCell();
            emptyCell.Border = 0;

            foreach (PdfReader reader in readers)
            {
                PdfImportedPage page = writerDocFinal.GetImportedPage(reader, 1);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(page);

                tableFinale.AddCell(image);
            }

            if ((format == ApplicationConsts.format_A5_simple || format == ApplicationConsts.format_A6_simple) && (tickets.Count % 2 != 0))
            {
                tableFinale.AddCell(emptyCell);
            }


            if ((format == ApplicationConsts.format_A7_simple) && (tickets.Count % 4 != 0))
            {
                for (int i = 0; i < 8 - tickets.Count % 4; i++)
                {
                    tableFinale.AddCell(emptyCell);
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
        /// Renvoie un objet Memory Stream pour un ticket.
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        static public MemoryStream getMSTicketSimple(TickitDataProduit ticket, int magasinId)
        {
            string codeMonnaie = DAO.LangueDao.getCodeMonnaieByMagasinId(magasinId);
            string PLV_PERMANENTE_SIMPLE_TAILLE_PRIX = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_PERMANENTE_SIMPLE_TAILLE_PRIX", magasinId);

            Phrase phraseTitre = new Phrase(ticket.variation, new Font(Utils.PoliceUtils.DINHabBd, 35, Font.NORMAL));
            Phrase phraseRange = new Phrase(Utils.StringUtils.convertStringMajusjToMinus(ticket.range), new Font(Utils.PoliceUtils.DINHabRg, 35, Font.NORMAL));
            Phrase phraseNouvPrixAvantVirgule = new Phrase(ticket.prixSansTaxeAvantVirgule, new Font(Utils.PoliceUtils.DINHabBd, Int16.Parse(PLV_PERMANENTE_SIMPLE_TAILLE_PRIX), Font.NORMAL));
            
            Phrase phraseNouvPrixApresVirgule = new Phrase(ticket.prixSansTaxeApresVirgule, new Font(Utils.PoliceUtils.DINHabBd, 25, Font.NORMAL));
            Phrase phrasePrixAvecTaxe = new Phrase(ticket.prixAvecTaxe, new Font(Utils.PoliceUtils.DINHabRg, 40, Font.BOLD));

            Phrase phraseLivraison = new Phrase(ticket.livraison, new Font(Utils.PoliceUtils.DINHabRg, 13, Font.NORMAL));
            Phrase phraseTaxeEco = new Phrase(ticket.Taxe_eco, new Font(Utils.PoliceUtils.DINHabRg, 13, Font.NORMAL));

            Phrase phraseDontTaxe = new Phrase(ticket.Taxe_eco == "" ? "" : "Dont ", new Font(Utils.PoliceUtils.DINHabRg, 15, Font.NORMAL));
            Phrase phraseEcoPart = new Phrase(ticket.Taxe_eco == "" ? "" : " d'éco-part", new Font(Utils.PoliceUtils.DINHabRg, 15, Font.NORMAL));
            Phrase phraseTaxeInculuse = new Phrase(ticket.Taxe_eco == "" ? "" : " éco-part incluse", new Font(Utils.PoliceUtils.DINHabRg, 15, Font.NORMAL));

            Phrase phraseDimensions = new Phrase(ticket.dimension, new Font(Utils.PoliceUtils.DINHabRg, 20, Font.NORMAL));
            Phrase phraseAMonterSoiMeme = new Phrase(ticket.aMonterSoiMeme, new Font(Utils.PoliceUtils.DINHabRg, 30, Font.NORMAL));
            Phrase phraseDGCCRF = new Phrase(ticket.DGCCRF, new Font(Utils.PoliceUtils.DINHabRg, 23, Font.NORMAL));
            Phrase phraseSku = new Phrase(ticket.sku, new Font(Utils.PoliceUtils.DINHabLig, 20, Font.NORMAL));
            
            Phrase phraseNbreColis = new Phrase(ticket.Nombre_colis, new Font(Utils.PoliceUtils.DINHabLig, 20, Font.NORMAL));
            
            MemoryStream ms = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4, -30, -20, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
            PdfPTable tablePage = new PdfPTable(2);

            tablePage.DefaultCell.Border = 0;
            tablePage.DefaultCell.BorderColor = PatternColor.WHITE;
            
            pdfDoc.Open();

            float[] columnWidths = new float[] { 50f, 50f };
            tablePage.SetWidths(columnWidths);

            ////////////////////////////////////////first page////////////////////////////////////////            

            tablePage.AddCell(spacer(135));

            // cellule titre
            PdfPCell cellTitreRecto = new PdfPCell(phraseTitre);
            cellTitreRecto.Colspan = 2;
            cellTitreRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellTitreRecto.Border = 0;
            tablePage.AddCell(cellTitreRecto);
            
            PdfPCell cellRangeRecto = new PdfPCell(phraseRange);
            cellRangeRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellRangeRecto.Colspan = 2;
            cellRangeRecto.Border = 0;
            tablePage.AddCell(cellRangeRecto);

            // insère un espace vertical
            tablePage.AddCell(spacer(60));

            PdfPCell cellPriceRecto = new PdfPCell(new Phrase());
            cellPriceRecto.Phrase.Add(phraseNouvPrixAvantVirgule);
            cellPriceRecto.Phrase.Add(phraseNouvPrixApresVirgule);
            cellPriceRecto.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPriceRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellPriceRecto.Border = 0;
            tablePage.AddCell(cellPriceRecto);

            PdfPCell cellPrixTaxeEtLivraisonRecto = new PdfPCell(new Phrase());
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(phraseDontTaxe);
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(phraseTaxeEco);
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(phraseEcoPart);
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(Environment.NewLine);
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(phrasePrixAvecTaxe);
            // cellPrixTaxeEtLivraisonRecto.Phrase.Add(phraseTaxeInculuse);
            // cellPrixTaxeEtLivraisonRecto.Phrase.Add(Environment.NewLine);
            cellPrixTaxeEtLivraisonRecto.Phrase.Add(phraseLivraison);
            cellPrixTaxeEtLivraisonRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellPrixTaxeEtLivraisonRecto.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPrixTaxeEtLivraisonRecto.Border = 0;
            tablePage.AddCell(cellPrixTaxeEtLivraisonRecto);
            
            tablePage.AddCell(spacer(120));

            PdfPCell cellDimensionsVerso = new PdfPCell(phraseDimensions);
            cellDimensionsVerso.VerticalAlignment = Element.ALIGN_TOP;
            cellDimensionsVerso.Border = 0;
            tablePage.AddCell(cellDimensionsVerso);

            PdfPCell cellSkuVerso = new PdfPCell(phraseSku);
            cellSkuVerso.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellSkuVerso.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellSkuVerso.Border = 0;
            tablePage.AddCell(cellSkuVerso);

            PdfPCell cellAMonterVerso = new PdfPCell(phraseAMonterSoiMeme);
            cellAMonterVerso.VerticalAlignment = Element.ALIGN_TOP;
            cellAMonterVerso.Border = 0;
            tablePage.AddCell(cellAMonterVerso);

            PdfPCell cellNbreColisVerso = new PdfPCell(phraseNbreColis);
            cellNbreColisVerso.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellNbreColisVerso.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellNbreColisVerso.Border = 0;
            tablePage.AddCell(cellNbreColisVerso);

            tablePage.AddCell(spacer(30));

            PdfPCell cellDgccrfVerso = new PdfPCell(phraseDGCCRF);
            cellDgccrfVerso.VerticalAlignment = Element.ALIGN_TOP;
            cellDgccrfVerso.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            cellDgccrfVerso.Border = 0;
            cellDgccrfVerso.Colspan = 2;
            cellDgccrfVerso.MinimumHeight = 100;
            tablePage.AddCell(cellDgccrfVerso);

            if (ticket.Made_In == ApplicationConsts.made_in_FR || ticket.Made_In == ApplicationConsts.made_in_IT || ticket.Made_In == ApplicationConsts.made_in_UE)
            {
                tablePage.AddCell(spacer(30));

                string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                string pathImgFr = "Content//themes//image//made_" + ticket.Made_In + ".png";

                PdfPCell cellImage = new PdfPCell(Image.GetInstance(applicationPath + pathImgFr));
                cellImage.Colspan = 2;
                cellImage.Border = 0;
                tablePage.AddCell(cellImage);
            }

            pdfDoc.Add(tablePage);

            ////////////////////////////////////////second page////////////////////////////////////////

            //pdfDoc.NewPage();
            //pdfDoc.Add(tablePage);

            pdfDoc.Close();
            writer.Dispose();

            return ms;
        }

        /// <summary>
        /// retourne une cellule avec un espaceur de hauteur 
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