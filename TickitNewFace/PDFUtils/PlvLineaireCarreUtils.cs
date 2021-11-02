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

namespace TickitNewFace.PDFUtils
{
    public static class PlvLineaireCarreUtils
    {
        /// Fonction permetant de génerer le document PDF pour l impression format reglette carré
        /// 18/07/2020
        /// Mehdi SRIDI
        /// <param name="chevalet"></param>
        /// <param name="format"></param>
        /// <param name="magasinId"></param>
        /// <param name="dateQuery"></param>
        /// <returns></returns>
        static public MemoryStream GenerateChevaletLineairePdf(TickitDataChevalet chevalet, String format, int magasinId, DateTime dateQuery)
        {
            //parametre pdf
            format = ApplicationConsts.format_A5_recto_verso;
            string codeMonnaie = DAO.LangueDao.getCodeMonnaieByMagasinId(magasinId);
            string PLV_REGLETTE_TAILLE_PRIX = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_REGLETTE_TAILLE_PRIX", magasinId);
            MemoryStream ms = new MemoryStream();
            //            Document pdfDoc = new Document(PageSize.A4, -30, -20, 0, 0);
            Document pdfDoc = new Document(PageSize.A4, 20, 20, 20, 20); //format

            //            Document pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0); //format
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms); //ecriture
            pdfDoc.Open();//ouverture
            //parametre/initialisation tableau final
            PdfPTable tablefinal = new PdfPTable(10);//declaration table final contenant les reglettes
            //            tablefinal.WidthPercentage = 94; // Table size is set to 100% of the page
            tablefinal.WidthPercentage = 100; // Table size is set to 100% of the page
            //tablefinal.HorizontalAlignment = 0; //Left aLign
            //tablefinal.SpacingAfter = 10;
            float[] sglTblHdWidths = new float[10]; //tailles colonnes
            sglTblHdWidths[0] = 5.5f;
            sglTblHdWidths[1] = 16f;
            sglTblHdWidths[2] = 1f;
            sglTblHdWidths[3] = 16f;
            sglTblHdWidths[4] = 1f;
            sglTblHdWidths[5] = 16f;
            sglTblHdWidths[6] = 1f;
            sglTblHdWidths[7] = 16f;
            sglTblHdWidths[8] = 1f;
            sglTblHdWidths[9] = 5.5f;
            tablefinal.SetWidths(sglTblHdWidths); // Set the column widths on table creation. Unlike HTML cells cannot be sized.
            // tablefinal.DefaultCell.Padding = 9; //marge dans la cellule
            tablefinal.DefaultCell.Padding = 0; //marge dans la cellule
            // tablefinal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablefinal.DefaultCell.BorderWidth = 0; // pas de bord
            // tablefinal.DefaultCell.Border = Rectangle.NO_BORDER; // pas de bord 2
            float[] columnWidths = new float[] { 80f, 20f };//dim colonne tabeau local (reglette elle meme)

            Font fontSeparateurVertical = new Font(Utils.PoliceUtils.DINHabHl, 4, Font.NORMAL);
            fontSeparateurVertical.Color = BaseColor.LIGHT_GRAY;

            Font fontSeparateurHorizontal = new Font(Utils.PoliceUtils.DINHabHl, 4, Font.NORMAL);
            fontSeparateurHorizontal.Color = BaseColor.LIGHT_GRAY;

            Phrase phraseSeparateur = new Phrase("-", fontSeparateurHorizontal);
            Phrase phraseSeparateur2 = new Phrase("-", fontSeparateurHorizontal);
            //construction du document

            int current_case=0;

            foreach (TickitDataProduit data in chevalet.produitsData)
            {
                if (current_case == 0)
                {

                    PdfPTable regletteGauche = new PdfPTable(1); //tableau local (reglette elle meme)
                    regletteGauche.HorizontalAlignment = Element.ALIGN_CENTER;
                    regletteGauche.KeepTogether = true;
                    regletteGauche.WidthPercentage = 100;

                    // Ajout du trait vertical.
                    Phrase phraseSeparateurGauche = new Phrase("|", fontSeparateurVertical);
                    PdfPCell cellSeparateurGauche = new PdfPCell(phraseSeparateurGauche);
                    cellSeparateurGauche.VerticalAlignment = Element.ALIGN_TOP;
                    cellSeparateurGauche.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellSeparateurGauche.Border = 0;
                    regletteGauche.AddCell(cellSeparateurGauche);
                    tablefinal.AddCell(regletteGauche);

                    current_case++;
                }
                
                PdfPTable reglettecourante = new PdfPTable(2); //tableau local (reglette elle meme)
                reglettecourante.SetWidths(columnWidths);
                reglettecourante.HorizontalAlignment = Element.ALIGN_LEFT;
                reglettecourante.KeepTogether = true;
                reglettecourante.WidthPercentage = 100;
                //separation
                //-- gauche
                
                // Ajout des bordures à gauche quand le produit est à gauche.
                PdfPCell cellSeparateur1 = new PdfPCell(phraseSeparateur);
                cellSeparateur1.VerticalAlignment = Element.ALIGN_TOP;
                cellSeparateur1.HorizontalAlignment = Element.ALIGN_LEFT;
                cellSeparateur1.Border = 0;
                reglettecourante.AddCell(cellSeparateur1);

                PdfPCell cellSeparateur2 = new PdfPCell(phraseSeparateur2);
                cellSeparateur2.VerticalAlignment = Element.ALIGN_TOP;
                cellSeparateur2.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellSeparateur2.Border = 0;
                reglettecourante.AddCell(cellSeparateur2);

                //DAO fonction fesant appel à la base de donnée
                string variation = DAO.VariationDao.getVariationBySku(data.sku, magasinId) == null ? "" : DAO.VariationDao.getVariationBySku(data.sku, magasinId).VariationName;
                String descReglette = DAO.Description_RegletteDao.getDescRegletteBySku(data.sku, magasinId) == null ? "" : DAO.Description_RegletteDao.getDescRegletteBySku(data.sku, magasinId).Description;
                T_Prix prixObj = DAO.PrixDao.getPrixBySkuAndDate(data.sku, magasinId, dateQuery);

                decimal prix = prixObj.Prix_produit;
                int posPoint = prix.ToString().IndexOf(".");
                if (posPoint == -1)
                {
                    posPoint = prix.ToString().IndexOf(",");
                }
                string prixAvantVirgule = prix.ToString().Substring(0, posPoint);
                string prixApresVirgule = prix.ToString().Substring(posPoint, prix.ToString().Length - posPoint);
                if (prixApresVirgule == ".00")
                {
                    prixApresVirgule = "";
                }
                //declaration/remplissage phrases
                // MSRIDI : Ajout règle de gestion par rapport  au nombre de caractères.
                variation = variation.Length < 15 ? variation : variation.Substring(0, 15);

                Phrase phraseVariation = new Phrase(variation, new Font(Utils.PoliceUtils.DINHabBd, 10, Font.NORMAL));
                Phrase phraseRangeName = new Phrase(Utils.StringUtils.convertStringMajusjToMinus(data.range), new Font(Utils.PoliceUtils.DINHabRg, 10, Font.NORMAL));
                
                // descReglette
                // EXP : Duvet de canard français 90gr.220x240cm français
                // MSRIDI : Ajout règle de gestion par rapport  au nombre de caractères.
                descReglette = descReglette.Length < 44 ? descReglette : descReglette.Substring(0, 44);
                                
                Phrase phraseDescReglette = new Phrase(descReglette, new Font(Utils.PoliceUtils.DINHabRg, 7, Font.NORMAL));
                Phrase phraseSku = new Phrase(data.sku, new Font(Utils.PoliceUtils.DINHabRg, 5, Font.NORMAL));
                Phrase phrasePrix = new Phrase(data.prix, new Font(Utils.PoliceUtils.DINHabBd, 13, Font.NORMAL));
                //Phrase phrasePrix = new Phrase(prixAvantVirgule + prixApresVirgule + "€", new Font(Utils.PoliceUtils.DINHabBd, int.Parse(PLV_REGLETTE_TAILLE_PRIX), Font.NORMAL));
                Phrase phraseEcoForReglette = new Phrase(data.Taxe_eco, new Font(Utils.PoliceUtils.DINHabRg, 5, Font.NORMAL));
                
                Phrase phrasePrixPermanent ;

                if(data.prixPermanent != "" && data.prixPermanent != null)
                    phrasePrixPermanent = new Phrase(data.prixPermanent, new Font(Utils.PoliceUtils.DINHabRg, 9, Font.STRIKETHRU));
                else
                    phrasePrixPermanent = new Phrase("             ", new Font(Utils.PoliceUtils.DINHabRg, 9, Font.NORMAL));

                Phrase phraseEspacePrix = new Phrase(" ", new Font(Utils.PoliceUtils.DINHabRg, 10, Font.NORMAL));
                Phrase phrasePrixSoldePrecedent = new Phrase(data.prixSoldePrecedent, new Font(Utils.PoliceUtils.DINHabRg, 10, Font.STRIKETHRU));
                Phrase fusion = new Phrase();

                fusion.Add(phraseSku);
                fusion.Add(" ");

                fusion.Add(phrasePrixPermanent);
                fusion.Add(phraseEspacePrix);
                // MSRIDI 14122015
                
                fusion.Add(phrasePrix);
                Phrase phrasePourcentageReduction = new Phrase("", new Font(Utils.PoliceUtils.DINHabHl, 15, Font.NORMAL));

                //declaration/remplissage/parametre Cell
                PdfPCell cellRangeLibelleRecto = new PdfPCell(phraseVariation);
                cellRangeLibelleRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellRangeLibelleRecto.Colspan = 2;
                cellRangeLibelleRecto.Border = 0;
                reglettecourante.AddCell(cellRangeLibelleRecto);
                
                //range
                PdfPCell cellRangeNameRecto = new PdfPCell(phraseRangeName);
                cellRangeNameRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellRangeNameRecto.Border = 0;
                reglettecourante.AddCell(cellRangeNameRecto);

                if (data.prixPermanent != null && data.prixPermanent != "")
                    phrasePourcentageReduction = new Phrase("-" + getFormattedDecimalToStringWithoutNeedlessPoint(data.pourcentage, "%"), new Font(Utils.PoliceUtils.DINHabHl, 11, Font.NORMAL));
                else
                    phrasePourcentageReduction = new Phrase(" ", new Font(Utils.PoliceUtils.DINHabHl, 11, Font.NORMAL));
                
                reglettecourante.AddCell(spacer(3));
                
                //Description reglette
                PdfPCell cellDescReglette = new PdfPCell(phraseDescReglette);
                cellDescReglette.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellDescReglette.HorizontalAlignment = Element.ALIGN_LEFT;
                cellDescReglette.Border = 0;

                if (descReglette == "" || descReglette == null)
                {
                    cellDescReglette.Phrase.Add(Environment.NewLine);
                    // cellDescReglette.Phrase.Add(Environment.NewLine);
                }

                if (descReglette.Length < 22)
                {
                    cellDescReglette.Phrase.Add(Environment.NewLine);
                }

                reglettecourante.AddCell(cellDescReglette);
                reglettecourante.AddCell(spacer(1));

                //PourcentageReduction
                PdfPCell cellPourcentageReduction = new PdfPCell(phrasePourcentageReduction);
                cellPourcentageReduction.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellPourcentageReduction.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPourcentageReduction.Border = 0;
                cellPourcentageReduction.Colspan = 2;
                reglettecourante.AddCell(cellPourcentageReduction);
               
                //prix et ku
                PdfPCell cellPrixSku = new PdfPCell(fusion);
                cellPrixSku.VerticalAlignment = Element.ALIGN_TOP;
                cellPrixSku.HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL;
                cellPrixSku.Border = 0;
                cellPrixSku.Colspan = 2;
                reglettecourante.AddCell(cellPrixSku);

                PdfPCell cellEcopart = new PdfPCell(phraseEcoForReglette);
                cellEcopart.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellEcopart.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellEcopart.Border = 0;
                cellEcopart.Colspan = 2;
                cellEcopart.Phrase.Add(Environment.NewLine);
                cellEcopart.Phrase.Add(Environment.NewLine);
                cellEcopart.Phrase.Add(Environment.NewLine);
                cellEcopart.Phrase.Add(Environment.NewLine);
                cellEcopart.Phrase.Add(Environment.NewLine);
                cellEcopart.Phrase.Add(Environment.NewLine);
                reglettecourante.AddCell(cellEcopart);

                //ajout de la reglette au tableau final
                tablefinal.AddCell(reglettecourante);
                current_case++;

                PdfPTable reglettecourante2 = new PdfPTable(1); //tableau local (reglette elle meme)
                reglettecourante2.HorizontalAlignment = Element.ALIGN_CENTER;
                reglettecourante2.KeepTogether = true;
                reglettecourante2.WidthPercentage = 100;
                
                // Ajout du trait vertical.
                Phrase phraseSeparateur3 = new Phrase("|", fontSeparateurVertical);
                PdfPCell cellSeparateur3 = new PdfPCell(phraseSeparateur3);
                cellSeparateur3.VerticalAlignment = Element.ALIGN_TOP;
                cellSeparateur3.HorizontalAlignment = Element.ALIGN_CENTER;
                cellSeparateur3.Border = 0;
                reglettecourante2.AddCell(cellSeparateur3);
                tablefinal.AddCell(reglettecourante2);
                current_case++;

                if (current_case == 9)
                {
                    PdfPTable regletteDroite = new PdfPTable(2);
                    regletteDroite.SetWidths(columnWidths);
                    PdfPCell CellTwoR311 = new PdfPCell(new Phrase(" "));
                    CellTwoR311.Border = 0;
                    regletteDroite.AddCell(CellTwoR311);
                    regletteDroite.HorizontalAlignment = Element.ALIGN_CENTER;
                    regletteDroite.KeepTogether = true;
                    regletteDroite.WidthPercentage = 100;
                    tablefinal.AddCell(regletteDroite);
                    current_case = 0;
                }
            }
            tablefinal.HorizontalAlignment = Element.ALIGN_CENTER;
            
            // Si le nombre de produits n'est pas multiple de 4 : ajouter des cases fictives vides pour que le PFD se génère correctement.
            double nb_produits_float = (double)chevalet.produitsData.Count / 4;
            double nb_produits_int = chevalet.produitsData.Count / 4;
            double nb_iterations ;
            if (nb_produits_float - nb_produits_int == 0) nb_iterations = 0;
            else nb_iterations = 4 - ((double)nb_produits_float - (double)nb_produits_int) * 4;

            int iteration=0;
            while (iteration < nb_iterations)
            {
                    PdfPTable reglettecourante = new PdfPTable(2);
                    reglettecourante.SetWidths(columnWidths);
                    PdfPCell CellTwoR311 = new PdfPCell(new Phrase(" "));
                    CellTwoR311.Border = 0;
                    reglettecourante.AddCell(CellTwoR311);
                    reglettecourante.HorizontalAlignment = Element.ALIGN_CENTER;
                    reglettecourante.KeepTogether = true;
                    reglettecourante.WidthPercentage = 100;
                    tablefinal.AddCell(reglettecourante);
                    current_case++;

                    PdfPTable reglettecourante2 = new PdfPTable(2);
                    reglettecourante2.SetWidths(columnWidths);
                    PdfPCell CellTwoR3113 = new PdfPCell(new Phrase(" "));
                    CellTwoR3113.Border = 0;
                    reglettecourante2.AddCell(CellTwoR3113);
                    reglettecourante2.HorizontalAlignment = Element.ALIGN_CENTER;
                    reglettecourante2.KeepTogether = true;
                    reglettecourante2.WidthPercentage = 100;
                    tablefinal.AddCell(reglettecourante2);
                    iteration = iteration + 1;
                    current_case++;
            }

            if (current_case == 9)
            {
                PdfPTable regletteDoite = new PdfPTable(2);
                regletteDoite.SetWidths(columnWidths);
                PdfPCell CellTwoR311 = new PdfPCell(new Phrase(" "));
                CellTwoR311.Border = 0;
                regletteDoite.AddCell(CellTwoR311);
                regletteDoite.HorizontalAlignment = Element.ALIGN_CENTER;
                regletteDoite.KeepTogether = true;
                regletteDoite.WidthPercentage = 100;
                tablefinal.AddCell(regletteDoite);
                current_case=0;
            }

            // Ajout des traits en bas de la page.
            // trait vertical
            PdfPTable regletteTraitsVerticaux = new PdfPTable(1); //tableau local (reglette elle meme)
            regletteTraitsVerticaux.HorizontalAlignment = Element.ALIGN_CENTER;
            regletteTraitsVerticaux.KeepTogether = true;
            regletteTraitsVerticaux.WidthPercentage = 100;
            Phrase phraseSeparateurGaucheBIS = new Phrase("|", fontSeparateurVertical);
            PdfPCell cellSeparateurGaucheBIS = new PdfPCell(phraseSeparateurGaucheBIS);
            cellSeparateurGaucheBIS.VerticalAlignment = Element.ALIGN_TOP;
            cellSeparateurGaucheBIS.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellSeparateurGaucheBIS.Border = 0;
            regletteTraitsVerticaux.AddCell(cellSeparateurGaucheBIS);

            // traits horizontaux
            PdfPTable regletteTraitsHorizontaux = new PdfPTable(2); //tableau local (reglette elle meme)
            regletteTraitsHorizontaux.SetWidths(columnWidths);
            regletteTraitsHorizontaux.HorizontalAlignment = Element.ALIGN_LEFT;
            regletteTraitsHorizontaux.KeepTogether = true;
            regletteTraitsHorizontaux.WidthPercentage = 100;
            //-- gauche
            PdfPCell cellSeparateur1BIS = new PdfPCell(phraseSeparateur);
            cellSeparateur1BIS.VerticalAlignment = Element.ALIGN_TOP;
            cellSeparateur1BIS.HorizontalAlignment = Element.ALIGN_LEFT;
            cellSeparateur1BIS.Border = 0;
            regletteTraitsHorizontaux.AddCell(cellSeparateur1BIS);
            //-- droite
            PdfPCell cellSeparateur2BIS = new PdfPCell(phraseSeparateur2);
            cellSeparateur2BIS.VerticalAlignment = Element.ALIGN_TOP;
            cellSeparateur2BIS.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellSeparateur2BIS.Border = 0;
            regletteTraitsHorizontaux.AddCell(cellSeparateur2BIS);

            PdfPCell cellDroite = new PdfPCell();
            cellDroite.Border=0;
            
            tablefinal.AddCell(regletteTraitsVerticaux);
            tablefinal.AddCell(regletteTraitsHorizontaux);
            tablefinal.AddCell(regletteTraitsVerticaux);
            tablefinal.AddCell(regletteTraitsHorizontaux);
            tablefinal.AddCell(regletteTraitsVerticaux);
            tablefinal.AddCell(regletteTraitsHorizontaux);
            tablefinal.AddCell(regletteTraitsVerticaux);
            tablefinal.AddCell(regletteTraitsHorizontaux);
            tablefinal.AddCell(regletteTraitsVerticaux);
            tablefinal.AddCell(cellDroite);


            //fermeture pdf
            pdfDoc.Add(tablefinal);
            pdfDoc.Close();
            writer.Dispose();
            //return
            MemoryStream msToReturn = new MemoryStream();
            msToReturn.Write(ms.ToArray(), 0, ms.ToArray().Length);
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
