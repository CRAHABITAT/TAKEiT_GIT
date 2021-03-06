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
    public static class PlvLineaireUtils
    {
        /// Fonction permetant de génerer le document PDF pour l impression format reglette
        /// 08/07/16
        /// ALOUI Driss
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
            PdfPTable tablefinal = new PdfPTable(3);//declaration table final contenant les reglettes
//            tablefinal.WidthPercentage = 94; // Table size is set to 100% of the page
            tablefinal.WidthPercentage = 100; // Table size is set to 100% of the page
            //tablefinal.HorizontalAlignment = 0; //Left aLign
            //tablefinal.SpacingAfter = 10;
            float[] sglTblHdWidths = new float[3]; //tailles colonnes
            sglTblHdWidths[0] = 100f;
            sglTblHdWidths[1] = 10f;
            sglTblHdWidths[2] = 100f;
            tablefinal.SetWidths(sglTblHdWidths); // Set the column widths on table creation. Unlike HTML cells cannot be sized.
//            tablefinal.DefaultCell.Padding = 9; //marge dans la cellule
            tablefinal.DefaultCell.Padding = 0; //marge dans la cellule
            //tablefinal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablefinal.DefaultCell.BorderWidth = 0; // pas de bord
            //tablefinal.DefaultCell.Border = Rectangle.NO_BORDER; // pas de bord 2
            float[] columnWidths = new float[] { 60f, 40f };//dim colonne tabeau local (reglette elle meme)
            Phrase phraseSeparateur = new Phrase("--", new Font(Utils.PoliceUtils.DINHabBd, 10, Font.NORMAL));
            Phrase phraseSeparateur2 = new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 10, Font.NORMAL));
//construction du document
            int cpt = 0; // compteur de case (different du nombre de produit dans le chevalet ! )
            foreach (TickitDataProduit data in chevalet.produitsData)
            {
                PdfPTable reglettecourante = new PdfPTable(2); //tableau local (reglette elle meme)
                reglettecourante.SetWidths(columnWidths);
                reglettecourante.HorizontalAlignment = Element.ALIGN_LEFT;
                reglettecourante.KeepTogether = true;
                reglettecourante.WidthPercentage = 100;
//separation
                //-- gauche
                if (cpt % 3 == 0)
                {
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
                }
                //case milieu
                if (cpt % 3 == 1)
                {
                    PdfPTable reglettecourante2 = new PdfPTable(2);
                    reglettecourante2.SetWidths(columnWidths);
                    PdfPCell CellTwoR311 = new PdfPCell(new Phrase(" "));
                    CellTwoR311.Border = 0;
                    reglettecourante2.AddCell(CellTwoR311);
                    reglettecourante2.HorizontalAlignment = Element.ALIGN_CENTER;
                    reglettecourante2.KeepTogether = true;
                    reglettecourante2.WidthPercentage = 100;
                    tablefinal.AddCell(reglettecourante2);
                //-- droit
                    PdfPCell cellSeparateur1 = new PdfPCell(phraseSeparateur2);
                    cellSeparateur1.VerticalAlignment = Element.ALIGN_TOP;
                    cellSeparateur1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellSeparateur1.Border = 0;
                    reglettecourante.AddCell(cellSeparateur1);
                    PdfPCell cellSeparateur2 = new PdfPCell(phraseSeparateur);
                    cellSeparateur2.VerticalAlignment = Element.ALIGN_TOP;
                    cellSeparateur2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellSeparateur2.Border = 0;
                    reglettecourante.AddCell(cellSeparateur2);
                    cpt++;//case suivante
                }
//DAO fonction fesant appel à la base de donnée
                string variation = DAO.VariationDao.getVariationBySku(data.sku, magasinId) == null ? "" : DAO.VariationDao.getVariationBySku(data.sku, magasinId).VariationName;
                string descReglette = DAO.Description_RegletteDao.getDescRegletteBySku(data.sku, magasinId) == null ? "" : DAO.Description_RegletteDao.getDescRegletteBySku(data.sku, magasinId).Description;
                T_Prix prixObj = DAO.PrixDao.getPrixBySkuAndDate(data.sku, magasinId, dateQuery);

                // MSRIDI 20042016 a supprimer dès fin de la promo.
                if (prixObj.TypeTarifCbr == "HABHFR")
                {
                    descReglette = descReglette + " * Prix avec carte Habitant.";
                }
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
                Phrase phraseVariation = new Phrase(variation, new Font(Utils.PoliceUtils.DINHabBd, 15, Font.NORMAL));
                Phrase phrasePrixRond = new Phrase("PRIX ROND", new Font(Utils.PoliceUtils.DINHabBd, 15, Font.NORMAL));

                Phrase phraseRangeName = new Phrase(Utils.StringUtils.convertStringMajusjToMinus(data.range), new Font(Utils.PoliceUtils.DINHabRg, 15, Font.NORMAL));
                Phrase phraseDescReglette = new Phrase(descReglette, new Font(Utils.PoliceUtils.DINHabRg, 7, Font.NORMAL));
                Phrase phraseSku = new Phrase(data.sku, new Font(Utils.PoliceUtils.DINHabBd, 7, Font.NORMAL));
                Phrase phrasePrix = new Phrase(data.prix, new Font(Utils.PoliceUtils.DINHabBd, int.Parse(PLV_REGLETTE_TAILLE_PRIX) / 2, Font.NORMAL));
                //Phrase phrasePrix = new Phrase(prixAvantVirgule + prixApresVirgule + "€", new Font(Utils.PoliceUtils.DINHabBd, int.Parse(PLV_REGLETTE_TAILLE_PRIX), Font.NORMAL));
                Phrase phraseEcoForReglette = new Phrase(data.Taxe_eco, new Font(Utils.PoliceUtils.DINHabBd, 7, Font.NORMAL));

                // MSRIDI 11102016
                if (prixObj.TypeTarifCbr == "HABHFR")
                {
                    phrasePrix = new Phrase("Prix Habitant " + data.prix, new Font(Utils.PoliceUtils.DINHabBd, int.Parse(PLV_REGLETTE_TAILLE_PRIX) / 2, Font.NORMAL));
                }

                Phrase phrasePrixPermanentStandard = new Phrase("Prix standard " , new Font(Utils.PoliceUtils.DINHabRg, 10, Font.NORMAL));
                Phrase phrasePrixPermanent = new Phrase(data.prixPermanent, new Font(Utils.PoliceUtils.DINHabRg, 10, Font.STRIKETHRU));
                Phrase phraseEspacePrix = new Phrase("   ", new Font(Utils.PoliceUtils.DINHabRg, 10, Font.NORMAL));
                Phrase phrasePrixSoldePrecedent = new Phrase(data.prixSoldePrecedent, new Font(Utils.PoliceUtils.DINHabRg, 10, Font.STRIKETHRU));

                Phrase fusion = new Phrase();
                
                // MSRIDI 11102016
                if (prixObj.TypeTarifCbr == "HABHFR")
                {
                    fusion.Add(phrasePrixPermanentStandard);
                }

                fusion.Add(phrasePrixPermanent);
                fusion.Add(phraseEspacePrix);
                // MSRIDI 14122015
                if (data.prixSoldePrecedent != "")
                {
                    fusion.Add(phrasePrixSoldePrecedent);
                    fusion.Add("     ");
                }
                fusion.Add(phrasePrix);
                Phrase phrasePourcentageReduction = new Phrase("", new Font(Utils.PoliceUtils.DINHabHl, 15, Font.NORMAL));

                //declaration/remplissage/parametre Cell
                //nom
                PdfPCell cellRangeLibelleRecto = new PdfPCell(phraseVariation);
                cellRangeLibelleRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellRangeLibelleRecto.Colspan = 2;
                cellRangeLibelleRecto.Border = 0;
                reglettecourante.AddCell(cellRangeLibelleRecto);

                /*
                // Ajout prix rond dans les réglettes. Demande de Chloé 01062018.
                if (magasinId == 4 && (data.sku == "990537" || data.sku == "812516" || data.sku == "990550" || data.sku == "990551" || data.sku == "812517" || data.sku == "990552" || data.sku == "990555" || data.sku == "812520" || data.sku == "812521" || data.sku == "916785" || data.sku == "951578" || data.sku == "990539" || data.sku == "912123" || data.sku == "812600" || data.sku == "953573" || data.sku == "990553" || data.sku == "990554" || data.sku == "993004" || data.sku == "812518" || data.sku == "812519" || data.sku == "973115" || data.sku == "801132" || data.sku == "801135" || data.sku == "912931" || data.sku == "969424" || data.sku == "807217" || data.sku == "807065" || data.sku == "807068" || data.sku == "807071" || data.sku == "811429" || data.sku == "811430" || data.sku == "811431" || data.sku == "909030" || data.sku == "805850" || data.sku == "803919" || data.sku == "803920" || data.sku == "803921" || data.sku == "803922" || data.sku == "803891" || data.sku == "807183" || data.sku == "807184" || data.sku == "807210" || data.sku == "905637" || data.sku == "905638" || data.sku == "807153" || data.sku == "915077" || data.sku == "915078" || data.sku == "808428" || data.sku == "808429" || data.sku == "801242" || data.sku == "801244" || data.sku == "915889" || data.sku == "803943" || data.sku == "803944" || data.sku == "803945" || data.sku == "803925" || data.sku == "803926" || data.sku == "803927" || data.sku == "803889" || data.sku == "803946" || data.sku == "803928" || data.sku == "994271" || data.sku == "994300" || data.sku == "994301" || data.sku == "994302" || data.sku == "994303" || data.sku == "994304" || data.sku == "805490" || data.sku == "805491" || data.sku == "805492" || data.sku == "805493" || data.sku == "807212" || data.sku == "807213" || data.sku == "807214" || data.sku == "914935" || data.sku == "917806" || data.sku == "912109" || data.sku == "912110" || data.sku == "915985" || data.sku == "800262" || data.sku == "912108" || data.sku == "912111" || data.sku == "912286" || data.sku == "917801" || data.sku == "903299" || data.sku == "915663" || data.sku == "915664" || data.sku == "915665" || data.sku == "915666" || data.sku == "914880" || data.sku == "807215" || data.sku == "913629" || data.sku == "913592" || data.sku == "913593" || data.sku == "913594" || data.sku == "913625" || data.sku == "913626" || data.sku == "913627" || data.sku == "916531" || data.sku == "917475" || data.sku == "917477" || data.sku == "917478" || data.sku == "917480" || data.sku == "917415" || data.sku == "915420" || data.sku == "915421" || data.sku == "915422" || data.sku == "915425" || data.sku == "917472" || data.sku == "917473" || data.sku == "917476" || data.sku == "917479" || data.sku == "801139" || data.sku == "803833" || data.sku == "805518" || data.sku == "807180" || data.sku == "969924" || data.sku == "981425" || data.sku == "801156" || data.sku == "906152" || data.sku == "957506" || data.sku == "957509" || data.sku == "970367" || data.sku == "915335" || data.sku == "915336" || data.sku == "912306" || data.sku == "912307" || data.sku == "801261" || data.sku == "801264" || data.sku == "801265" || data.sku == "801266" || data.sku == "807712" || data.sku == "807713" || data.sku == "807714" || data.sku == "994936" || data.sku == "994937" || data.sku == "805409" || data.sku == "805410" || data.sku == "810552" || data.sku == "810553" || data.sku == "812417" || data.sku == "812418" || data.sku == "807685" || data.sku == "807697" || data.sku == "807698" || data.sku == "807699" || data.sku == "807615" || data.sku == "807616" || data.sku == "807617" || data.sku == "805146" || data.sku == "805147" || data.sku == "805148" || data.sku == "807985" || data.sku == "814023" || data.sku == "814024" || data.sku == "814025" || data.sku == "814026" || data.sku == "807618" || data.sku == "807619" || data.sku == "807620" || data.sku == "801896" || data.sku == "803754" || data.sku == "805152" || data.sku == "805153" || data.sku == "805154" || data.sku == "814031" || data.sku == "814032" || data.sku == "814033" || data.sku == "915451" || data.sku == "902681" || data.sku == "807621" || data.sku == "807622" || data.sku == "807623" || data.sku == "805149" || data.sku == "805150" || data.sku == "805151" || data.sku == "811778" || data.sku == "814027" || data.sku == "814028" || data.sku == "814029" || data.sku == "814030" || data.sku == "811750" || data.sku == "811751" || data.sku == "807808" || data.sku == "807809" || data.sku == "807810" || data.sku == "806263" || data.sku == "806264" || data.sku == "807756" || data.sku == "807757" || data.sku == "807758" || data.sku == "807759" || data.sku == "806265" || data.sku == "907575" || data.sku == "910093" || data.sku == "910160" || data.sku == "912521" || data.sku == "914623" || data.sku == "916276" || data.sku == "968194" || data.sku == "987588" || data.sku == "914680" || data.sku == "807786" || data.sku == "807793" || data.sku == "807858" || data.sku == "912053" || data.sku == "912330" || data.sku == "917080" || data.sku == "972978" || data.sku == "917079" || data.sku == "805484" || data.sku == "972979" || data.sku == "972982" || data.sku == "972981" || data.sku == "968533" || data.sku == "968534" || data.sku == "994465" || data.sku == "994466"))
                {
                    cellRangeLibelleRecto.Colspan = 1;
                }
                cellRangeLibelleRecto.Border = 0;
                reglettecourante.AddCell(cellRangeLibelleRecto);

                // Ajout prix rond dans les réglettes. Demande de Chloé 01062018.
                if (magasinId == 4 && (data.sku == "990537" || data.sku == "812516" || data.sku == "990550" || data.sku == "990551" || data.sku == "812517" || data.sku == "990552" || data.sku == "990555" || data.sku == "812520" || data.sku == "812521" || data.sku == "916785" || data.sku == "951578" || data.sku == "990539" || data.sku == "912123" || data.sku == "812600" || data.sku == "953573" || data.sku == "990553" || data.sku == "990554" || data.sku == "993004" || data.sku == "812518" || data.sku == "812519" || data.sku == "973115" || data.sku == "801132" || data.sku == "801135" || data.sku == "912931" || data.sku == "969424" || data.sku == "807217" || data.sku == "807065" || data.sku == "807068" || data.sku == "807071" || data.sku == "811429" || data.sku == "811430" || data.sku == "811431" || data.sku == "909030" || data.sku == "805850" || data.sku == "803919" || data.sku == "803920" || data.sku == "803921" || data.sku == "803922" || data.sku == "803891" || data.sku == "807183" || data.sku == "807184" || data.sku == "807210" || data.sku == "905637" || data.sku == "905638" || data.sku == "807153" || data.sku == "915077" || data.sku == "915078" || data.sku == "808428" || data.sku == "808429" || data.sku == "801242" || data.sku == "801244" || data.sku == "915889" || data.sku == "803943" || data.sku == "803944" || data.sku == "803945" || data.sku == "803925" || data.sku == "803926" || data.sku == "803927" || data.sku == "803889" || data.sku == "803946" || data.sku == "803928" || data.sku == "994271" || data.sku == "994300" || data.sku == "994301" || data.sku == "994302" || data.sku == "994303" || data.sku == "994304" || data.sku == "805490" || data.sku == "805491" || data.sku == "805492" || data.sku == "805493" || data.sku == "807212" || data.sku == "807213" || data.sku == "807214" || data.sku == "914935" || data.sku == "917806" || data.sku == "912109" || data.sku == "912110" || data.sku == "915985" || data.sku == "800262" || data.sku == "912108" || data.sku == "912111" || data.sku == "912286" || data.sku == "917801" || data.sku == "903299" || data.sku == "915663" || data.sku == "915664" || data.sku == "915665" || data.sku == "915666" || data.sku == "914880" || data.sku == "807215" || data.sku == "913629" || data.sku == "913592" || data.sku == "913593" || data.sku == "913594" || data.sku == "913625" || data.sku == "913626" || data.sku == "913627" || data.sku == "916531" || data.sku == "917475" || data.sku == "917477" || data.sku == "917478" || data.sku == "917480" || data.sku == "917415" || data.sku == "915420" || data.sku == "915421" || data.sku == "915422" || data.sku == "915425" || data.sku == "917472" || data.sku == "917473" || data.sku == "917476" || data.sku == "917479" || data.sku == "801139" || data.sku == "803833" || data.sku == "805518" || data.sku == "807180" || data.sku == "969924" || data.sku == "981425" || data.sku == "801156" || data.sku == "906152" || data.sku == "957506" || data.sku == "957509" || data.sku == "970367" || data.sku == "915335" || data.sku == "915336" || data.sku == "912306" || data.sku == "912307" || data.sku == "801261" || data.sku == "801264" || data.sku == "801265" || data.sku == "801266" || data.sku == "807712" || data.sku == "807713" || data.sku == "807714" || data.sku == "994936" || data.sku == "994937" || data.sku == "805409" || data.sku == "805410" || data.sku == "810552" || data.sku == "810553" || data.sku == "812417" || data.sku == "812418" || data.sku == "807685" || data.sku == "807697" || data.sku == "807698" || data.sku == "807699" || data.sku == "807615" || data.sku == "807616" || data.sku == "807617" || data.sku == "805146" || data.sku == "805147" || data.sku == "805148" || data.sku == "807985" || data.sku == "814023" || data.sku == "814024" || data.sku == "814025" || data.sku == "814026" || data.sku == "807618" || data.sku == "807619" || data.sku == "807620" || data.sku == "801896" || data.sku == "803754" || data.sku == "805152" || data.sku == "805153" || data.sku == "805154" || data.sku == "814031" || data.sku == "814032" || data.sku == "814033" || data.sku == "915451" || data.sku == "902681" || data.sku == "807621" || data.sku == "807622" || data.sku == "807623" || data.sku == "805149" || data.sku == "805150" || data.sku == "805151" || data.sku == "811778" || data.sku == "814027" || data.sku == "814028" || data.sku == "814029" || data.sku == "814030" || data.sku == "811750" || data.sku == "811751" || data.sku == "807808" || data.sku == "807809" || data.sku == "807810" || data.sku == "806263" || data.sku == "806264" || data.sku == "807756" || data.sku == "807757" || data.sku == "807758" || data.sku == "807759" || data.sku == "806265" || data.sku == "907575" || data.sku == "910093" || data.sku == "910160" || data.sku == "912521" || data.sku == "914623" || data.sku == "916276" || data.sku == "968194" || data.sku == "987588" || data.sku == "914680" || data.sku == "807786" || data.sku == "807793" || data.sku == "807858" || data.sku == "912053" || data.sku == "912330" || data.sku == "917080" || data.sku == "972978" || data.sku == "917079" || data.sku == "805484" || data.sku == "972979" || data.sku == "972982" || data.sku == "972981" || data.sku == "968533" || data.sku == "968534" || data.sku == "994465" || data.sku == "994466"))
                {
                    PdfPCell cellRangePrixRond = new PdfPCell(phrasePrixRond);
                    cellRangePrixRond.VerticalAlignment = Element.ALIGN_BOTTOM;
                    cellRangeLibelleRecto.Colspan = 1;
                    cellRangePrixRond.Border = 0;
                    cellRangePrixRond.HorizontalAlignment = Element.ALIGN_RIGHT;
                    reglettecourante.AddCell(cellRangePrixRond);
                }
                */

                //range
                PdfPCell cellRangeNameRecto = new PdfPCell(phraseRangeName);
                cellRangeNameRecto.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellRangeNameRecto.Border = 0;
                reglettecourante.AddCell(cellRangeNameRecto);

                if (data.prixPermanent != null)
                {
                    phrasePourcentageReduction = new Phrase("-" + getFormattedDecimalToStringWithoutNeedlessPoint(data.pourcentage, "%"), new Font(Utils.PoliceUtils.DINHabHl, 11, Font.NORMAL));
                }
                //PourcentageReduction
                PdfPCell cellPourcentageReduction = new PdfPCell(phrasePourcentageReduction);
                cellPourcentageReduction.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellPourcentageReduction.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPourcentageReduction.Border = 0;
                reglettecourante.AddCell(cellPourcentageReduction);
                //espace
                reglettecourante.AddCell(spacer(3));
                //DescRegletteAndSku
                PdfPCell cellDescRegletteAndSku = new PdfPCell(phraseDescReglette);
                cellDescRegletteAndSku.VerticalAlignment = Element.ALIGN_BOTTOM;
                cellDescRegletteAndSku.Phrase.Add(Environment.NewLine);
                cellDescRegletteAndSku.Phrase.Add(Environment.NewLine);
                cellDescRegletteAndSku.Phrase.Add(phraseSku);
                cellDescRegletteAndSku.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                cellDescRegletteAndSku.Border = 0;
                reglettecourante.AddCell(cellDescRegletteAndSku);
                //prix
                PdfPCell cellPrix = new PdfPCell(fusion);
                cellPrix.VerticalAlignment = Element.ALIGN_TOP;
                cellPrix.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPrix.Border = 0;
                cellPrix.Phrase.Add(Environment.NewLine);  // ADD IM 20183112 - Pour ajouter l'éco-part (eco-mobilier) au format reglette
                cellPrix.Phrase.Add(Environment.NewLine);  // ADD IM 20183112 - Pour ajouter l'éco-part (eco-mobilier) au format reglette
                cellPrix.Phrase.Add(phraseEcoForReglette); // ADD IM 20183112 - Pour ajouter l'éco-part (eco-mobilier) au format reglette
                reglettecourante.AddCell(cellPrix);

//ajout de la reglette au tableau final
                tablefinal.AddCell(reglettecourante); 
                cpt++;//case suivante
            }
            tablefinal.HorizontalAlignment = Element.ALIGN_CENTER;
// Si nb d'etiquette impaire (gestion de la derniere case toute seule)
            if(chevalet.produitsData.Count%2==1)
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
                PdfPTable reglettecourante2 = new PdfPTable(2);
                reglettecourante.SetWidths(columnWidths);
                PdfPCell CellTwoR3113 = new PdfPCell(new Phrase(" "));
                CellTwoR3113.Border = 0;
                reglettecourante2.AddCell(CellTwoR3113);
                reglettecourante2.HorizontalAlignment = Element.ALIGN_CENTER;
                reglettecourante2.KeepTogether = true;
                reglettecourante2.WidthPercentage = 100;
                tablefinal.AddCell(reglettecourante2);
            }           
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
