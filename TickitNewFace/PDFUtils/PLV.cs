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
using TickitNewFace.Managers;

namespace TickitNewFace.PDFUtils
{
    public static class PLV
    {
        /// <summary>
        /// generation plv de gamme format A4 avec opt ou sans opt (les tailles evoluent en fonction du nombre d'option, du nombre de produit et la taille de la DGCCRF)
        /// </summary>
        /// <param name="Gamme"></param>
        /// <param name="dateQuery"></param>
        /// <param name="magasinId"></param>
        /// <returns></returns>
        static public MemoryStream GeneratePLVPdf2(T_Presentation_Gamme_PLV Gamme, DateTime dateQuery, int magasinId)
        {

            //taille text
            tailleTexteGammePlv Taille = new tailleTexteGammePlv();
            List<int> dimentionLG = new List<int>();
            List<float> dimentionIP = new List<float>();
            dimentionIP.Add(300 - 30 * Gamme.options.Count - 10 * (float)Gamme.dgccrfGamme.Length/130);
            List<int> dimentionLG2 = new List<int>();
            Taille.nomGamme = 35;
            Taille.sousTitreDeGamme = 15;
            Taille.logoAuteur = dimentionLG;
            Taille.DGCCRF = 11;
            Taille.imageProduit = dimentionIP;
            Taille.variation = 10;
            Taille.description = 8;
            Taille.option = 13;
            Taille.prix = 20;
            Taille.prixPermanent = 13;
            Taille.lastPrix = 18;
            Taille.ecopart = 5;
            Taille.logoAuteur2 = dimentionLG2;
            Taille.reduct = 50;
            int indiceTailleImage = 0;

//parametre pdf
            string codeMonnaie = DAO.LangueDao.getCodeMonnaieByMagasinId(magasinId);
            string PLV_REGLETTE_TAILLE_PRIX = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_REGLETTE_TAILLE_PRIX", magasinId);
            MemoryStream ms = new MemoryStream();
            //            Document pdfDoc = new Document(PageSize.A4, 20, 20, 20, 20);//A4 marge + format portrait
            Document pdfDoc = new Document(new RectangleReadOnly(842, 595), 40, 40, 20, 20);//A4 marge + format paysage
            //            Document pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0); //format
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms); //ecriture
            pdfDoc.Open();//ouverture
            //parametre/initialisation tableau final
            PdfPTable tablefinal = new PdfPTable(1);//declaration table final contenant les reglettes
            tablefinal.WidthPercentage = 100; // Table size is set to 100% of the page
            tablefinal.DefaultCell.Padding = 0; //marge dans la cellule
            //tablefinal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablefinal.DefaultCell.BorderWidth = 0; // pas de bord

            PdfPCell Cellnomsousgammespace = new PdfPCell(new Phrase(" ")); //espacement
            PdfPCell Cellnomsousgammespace2 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabRg, 8, Font.NORMAL)));
            PdfPCell Cellnomsousgammespace3 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 14, Font.NORMAL)));
            Cellnomsousgammespace.Border = 0;
            Cellnomsousgammespace2.Border = 0;
            Cellnomsousgammespace3.Border = 0;
//En Tete
            PdfPTable tableEnTete = new PdfPTable(3);
            tablefinal.DefaultCell.BorderWidth = 0;
            tableEnTete.DefaultCell.BorderWidth = 0;
    //titre
            PdfPTable tabletitre = new PdfPTable(1);
            PdfPCell CellTwoR311 = new PdfPCell(new Phrase(Gamme.nomGamme, new Font(Utils.PoliceUtils.DINHabBd, Taille.nomGamme, Font.NORMAL)));
            CellTwoR311.Border = 0;
            tabletitre.AddCell(CellTwoR311);
            PdfPCell CellTwo = new PdfPCell(new Phrase(Gamme.sousTitreGamme, new Font(Utils.PoliceUtils.DINHabRg, Taille.sousTitreDeGamme, Font.NORMAL)));
            CellTwo.Border = 0;
            tabletitre.AddCell(CellTwo);
            tabletitre.DefaultCell.BorderWidth = 0;
            tableEnTete.AddCell(tabletitre);


    //reduct?
            T_Prix prixProduitreduct = Managers.FicheProduitManager.getPrixBySkuAndDate(Gamme.sousGammes[0].skus[0], magasinId, dateQuery);
            decimal? pourcentageReductionreduct = 0;
            if (prixProduitreduct.Type_promo != ApplicationConsts.typePrix_permanent)
            {
                T_Prix prixProduitReference = Managers.FicheProduitManager.getPrixPermanentPrecedent(prixProduitreduct);
                pourcentageReductionreduct = 100 - (((prixProduitreduct.Prix_produit - prixProduitreduct.Eco_mobilier) * 100) / prixProduitReference.Prix_produit);

                // Arrondit un pourcentage très proche de 0 (chiffre après virgule < 0.05)
                pourcentageReductionreduct = Utils.SpecificMathUtils.getRoundDecimal(pourcentageReductionreduct);
            }
            PdfPCell CellTwoR312;
            if (prixProduitreduct.Type_promo != ApplicationConsts.typePrix_permanent) { CellTwoR312 = new PdfPCell(new Phrase("- " + pourcentageReductionreduct + "% *", new Font(Utils.PoliceUtils.DINHabRg, Taille.reduct, Font.NORMAL))); }
            else { CellTwoR312 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabRg, Taille.reduct, Font.NORMAL))); }
            //PdfPCell CellTwoR312 = new PdfPCell(new Phrase("reduc1"));
            CellTwoR312.Border = 0;
            tableEnTete.AddCell(CellTwoR312);
            /*PdfPCell CellTwoR313 = new PdfPCell(new Phrase("reduc2"));
            CellTwoR313.Border = 0;
            tableEnTete.AddCell(CellTwoR313);*/
    //logo HDS?
            string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            string pathImgFr = "Content//themes//image//" + "LogoHDS.PNG";
            if (Gamme.logoGamme == 1)
            {
                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(applicationPath + pathImgFr);
                //iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(new Uri("http://2997fr-mssql04/product/Content/themes/image/LogoHDS.PNG"));
                jpg.ScaleAbsolute(77f, 56f);
                PdfPCell CellTwoR314 = new PdfPCell(jpg);
                CellTwoR314.Border = 0;
                CellTwoR314.HorizontalAlignment = Element.ALIGN_RIGHT;
                tableEnTete.AddCell(CellTwoR314);
            }
            else {
                PdfPCell Cellvide7 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 5, Font.NORMAL)));
                Cellvide7.Border = 0;
                tableEnTete.AddCell(Cellvide7);
            
            }
            tablefinal.AddCell(tableEnTete);
            PdfPCell Cellespa = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 10, Font.NORMAL)));
            Cellespa.Border = 0;
            
//corps
            PdfPTable tableCorps = new PdfPTable(1);
            tableCorps.DefaultCell.BorderWidth = 0;
    //DGCCRF
            PdfPCell CellDGCCRF = new PdfPCell(new Phrase(Gamme.dgccrfGamme, new Font(Utils.PoliceUtils.DINHabRg, Taille.DGCCRF, Font.NORMAL)));
            CellDGCCRF.Border = 0;
            tableCorps.AddCell(CellDGCCRF);
            tableCorps.AddCell(Cellespa);
    //PRODUIT
            //table de taille nombre de sous + 1 si opt
            int nbopt = Gamme.options.Count;
            int nbSGamme = Gamme.sousGammes.Count;
            PdfPTable tableProduit1 = new PdfPTable(2);
            tableProduit1.DefaultCell.BorderWidth = 0;
            float[] sglTblHdWidths1 = new float[2];
            sglTblHdWidths1[0] = 200f;
            sglTblHdWidths1[1] = 1f;
            
            PdfPTable tableProduit = new PdfPTable(nbSGamme + 1);
            tableProduit.DefaultCell.BorderWidth = 0;
            float[] sglTblHdWidths = new float[nbSGamme + 1]; //tailles colonnes
            if (Gamme.sousGammes.Count != 1 && Gamme.options.Count == 1 && (Gamme.options[0] == "" || Gamme.options[0] == " "))//pas option
            {
                sglTblHdWidths[0] = 1f;
            }else{
                sglTblHdWidths[0] = 80f;
            }
            if (Gamme.sousGammes.Count == 1 && Gamme.options.Count == 1 && (Gamme.options[0] == "" || Gamme.options[0] == " "))//pas option
            {
                sglTblHdWidths[0] = 1f;
                sglTblHdWidths1[1] = 1f;
            }
            for (var i = 1; i < nbSGamme + 1; i++)
            {
                sglTblHdWidths[i] = 400f / nbSGamme;
            }
            tableProduit.SetWidths(sglTblHdWidths); // Set the column widths on table creation. Unlike HTML cells cannot be sized.
            tableProduit1.SetWidths(sglTblHdWidths1);
            //remplissage table produit
            for (var j = 0; j < nbopt + 2; j++)
            {
                for (var i = 0; i < nbSGamme + 1; i++)
                {
                    if (i != 0)
                    {
                        if (j > 1)
                        {
                            //sku produit i-1 j-1
                            //PdfPCell Cellproduit = new PdfPCell(new Phrase(Gamme.sousGammes[i - 1].skus[j - 2], new Font(Utils.PoliceUtils.DINHabBd, 15, Font.NORMAL)));
                           // Cellproduit.Border = 0;
                            //tableProduit.AddCell(Cellproduit);
                            
                            
                            if (Gamme.sousGammes[i - 1].skus[j - 2] == " ") {
                                PdfPCell Cellproduit = new PdfPCell(new Phrase(Gamme.sousGammes[i - 1].skus[j - 2], new Font(Utils.PoliceUtils.DINHabBd, 15, Font.NORMAL)));
                                Cellproduit.Border = 0;
                                tableProduit.AddCell(Cellproduit);
                            } else {
                                TickitDataProduit ticket = Managers.TickitDataManager.getTickitDataPourChevalet(Gamme.sousGammes[i - 1].skus[j - 2], magasinId, dateQuery, null, null); // on reutilise fct existante ducoup pas besoin des 2 derniers parametre ici

                                //creation des phrase
                                PdfPTable tablePage1 = new PdfPTable(1);
                                Phrase phraseprixReference = new Phrase(ticket.prixPermanent, new Font(Utils.PoliceUtils.DINHabRg, Taille.prixPermanent, Font.STRIKETHRU));
                                Phrase espaceur = new Phrase("   ", new Font(Utils.PoliceUtils.DINHabRg, 5, Font.NORMAL));
                                Phrase phraseprix = new Phrase(ticket.prix, new Font(Utils.PoliceUtils.DINHabBd, Taille.prix, Font.NORMAL));
                                Phrase phraseTaxe = new Phrase(ticket.Taxe_eco, new Font(Utils.PoliceUtils.DINHabRg, Taille.ecopart, Font.NORMAL));

                                // MSRIDI 14122015
                                Phrase phrasePrixSoldePrecedent = new Phrase(ticket.prixSoldePrecedent, new Font(Utils.PoliceUtils.DINHabRg, Taille.lastPrix, Font.STRIKETHRU));
                                //prix
                                PdfPCell cellPrix = new PdfPCell(new Phrase());
                                cellPrix.Phrase.Add(phraseprixReference);
                                cellPrix.Phrase.Add(espaceur);

                                // MSRIDI 14122015
                                if (ticket.prixSoldePrecedent != "")
                                {
                                    cellPrix.Phrase.Add(phrasePrixSoldePrecedent);
                                    cellPrix.Phrase.Add(espaceur);
                                }

                                cellPrix.Phrase.Add(phraseprix);
                                //cellPrix.VerticalAlignment = Element.ALIGN_BOTTOM;
                                cellPrix.HorizontalAlignment = Element.ALIGN_LEFT;
                                cellPrix.Border = 0;
                                tablePage1.AddCell(cellPrix);

                                //taxe eco
                                PdfPCell cellTaxeEco = new PdfPCell(new Phrase());
                                cellTaxeEco.Phrase.Add(phraseTaxe);
                                //cellTaxeEco.VerticalAlignment = Element.ALIGN_BOTTOM;
                                cellTaxeEco.HorizontalAlignment = Element.ALIGN_LEFT;
                                cellTaxeEco.Border = 0;
                                //cellTaxeEco.Colspan = 2;
                                tablePage1.AddCell(cellTaxeEco);
                                tableProduit.AddCell(tablePage1);
                         
                            } 
                          
                        }else {
                            //description ss gamme produit i-1
                            //PdfPCell Celldescription = new PdfPCell(new Phrase(Gamme.sousGammes[i - 1].nomSGamme, new Font(Utils.PoliceUtils.DINHabBd, 15, Font.NORMAL)));
                            //Celldescription.Border = 0;
                            //tableProduit.AddCell(Celldescription);
                            
                            //creation des phrase
                            PdfPTable tablePage = new PdfPTable(1);
                            Phrase phraseItem = new Phrase(Gamme.sousGammes[i - 1].nomSGamme, new Font(Utils.PoliceUtils.DINHabRg, Taille.variation, Font.NORMAL));
                            Phrase phraseDesc = new Phrase(Gamme.sousGammes[i - 1].descSGamme, new Font(Utils.PoliceUtils.DINHabRg, Taille.description, Font.NORMAL));
                            Phrase espaceur = new Phrase("   ", new Font(Utils.PoliceUtils.DINHabRg, 5, Font.NORMAL));

                            //Image
                            //applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                            //pathImgFr = "Content//themes//image//" + "habitat" + ".png";
                            //iTextSharp.text.Image jpg3 = iTextSharp.text.Image.GetInstance(applicationPath + pathImgFr);
                            //iTextSharp.text.Image jpg3 = iTextSharp.text.Image.GetInstance(new Uri("http://2997fr-mssql04/product/Content/themes/image/habitat.png"));
                            if (j == 0)
                            {
                                int g = 0;
                                int reussi = 0;
                                while (g < Gamme.sousGammes[i - 1].skus.Count && reussi == 0)// on parcourt tous les skus de la sous gamme pour trouver une image a la sous gamme
                                {
                                    while (Gamme.sousGammes[i - 1].skus[g] == " " && g < Gamme.sousGammes[i - 1].skus.Count-1)
                                    {
                                        g++;//si pas de sku associé a  l option on prend le suivant
                                    }
                                    try
                                    {
                                        iTextSharp.text.Image jpg3 = iTextSharp.text.Image.GetInstance(new Uri("http://ean.habitat.fr/PLV_TAKEIT/" + Gamme.sousGammes[i - 1].skus[g] + ".png"));
                                        //jpg3.ScaleAbsolute(130f, 100f);
                                        //jpg3.WidthPercentage = 20;
                                        PdfPCell cellImage = new PdfPCell(jpg3);
                                        //cellImage.VerticalAlignment = Element.ALIGN_BOTTOM;
                                        cellImage.HorizontalAlignment = Element.ALIGN_LEFT;
                                        cellImage.Border = 0;
                                        //tablePage.AddCell(cellImage);

                                        var paragraph = new Paragraph();
                                        var rapportimage = jpg3.Width / jpg3.Height;
                                        var Largeur = Taille.imageProduit[indiceTailleImage];
                                        //if (rapportimage < 1.7)//probleme car image avec largeur trop grand qui peu faire deborder de la page
                                        {
                                            jpg3.ScaleAbsolute(Largeur * rapportimage, Largeur);// ducoup taille assignée (proportion gardée)
                                        }
                                        var chunk = new Chunk(jpg3, 0, 0, true);
                                        paragraph.Add(chunk);
                                        PdfPCell celli = new PdfPCell();
                                        celli.Border = 0;
                                        celli.AddElement(paragraph);
                                        tablePage.AddCell(celli);

                                        reussi++;
                                    }
                                    catch (Exception)
                                    {
                                        g++;//si pas d image existante on ragarde le suivant 
                                    }
                                }

                                if (reussi == 0)// si pas d image trouvée
                                {
                                    applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                                    pathImgFr = "Content//themes//image//" + "habitat" + ".png";
                                    iTextSharp.text.Image jpg3 = iTextSharp.text.Image.GetInstance(applicationPath + pathImgFr);//image logo habitat
                                    jpg3.ScaleAbsolute(100f, 100f);
                                    PdfPCell cellImage = new PdfPCell(jpg3);
                                    //cellImage.VerticalAlignment = Element.ALIGN_BOTTOM;
                                    cellImage.HorizontalAlignment = Element.ALIGN_LEFT;
                                    cellImage.Border = 0;
                                    //tablePage.AddCell(cellImage);

                                    var paragraph = new Paragraph();
                                    var rapportimage = jpg3.Width / jpg3.Height;
                                    var Largeur = Taille.imageProduit[indiceTailleImage];
                                    //if (rapportimage < 1.7)//probleme car image avec largeur trop grand qui peu faire deborder de la page
                                    {
                                        jpg3.ScaleAbsolute(Largeur * rapportimage, Largeur);// ducoup taille assignée (proportion gardée)
                                    }
                                    var chunk = new Chunk(jpg3, 0, 0, true);
                                    paragraph.Add(chunk);
                                    PdfPCell celli = new PdfPCell();
                                    celli.Border = 0;
                                    celli.AddElement(paragraph);
                                    tablePage.AddCell(celli);
                                }
                            }
                            else
                            {
                                //variation
                                PdfPCell cellItems = new PdfPCell(phraseItem);
                                //cellItems.VerticalAlignment = Element.ALIGN_BOTTOM;
                                cellItems.HorizontalAlignment = Element.ALIGN_LEFT;
                                cellItems.Border = 0;
                                tablePage.AddCell(cellItems);

                                //desc
                                PdfPCell cellDesc = new PdfPCell(phraseDesc);
                                //cellDesc.VerticalAlignment = Element.ALIGN_BOTTOM;
                                cellDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                                cellDesc.Border = 0;
                                tablePage.AddCell(cellDesc);
                            }
                            tableProduit.AddCell(tablePage);
                            
                        }
                    }else{
                        if (j > 1)
                        {
                            //opt sku produit j-1
                            PdfPCell Cellopt = new PdfPCell(new Phrase(Gamme.options[j - 2], new Font(Utils.PoliceUtils.DINHabBd, Taille.option, Font.NORMAL)));
                            Cellopt.Border = 0;
                            Cellopt.Padding = 10;
                            Cellopt.VerticalAlignment = Element.ALIGN_CENTER;
                            tableProduit.AddCell(Cellopt);

                        }
                        else
                        {
                            //vide
                            PdfPCell Cellvide = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 5, Font.NORMAL)));
                            Cellvide.Border = 0;
                            tableProduit.AddCell(Cellvide);
                        }
                    }

                }
            
            }
            tableProduit1.AddCell(tableProduit);
            PdfPCell Cellvide1 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 5, Font.NORMAL)));
            Cellvide1.Border = 0;
            tableProduit1.AddCell(Cellvide1);
            tableCorps.AddCell(tableProduit1);
            PdfPCell tableCorps2 = new PdfPCell(tableCorps);
            tableCorps2.Border = 0;
            tableCorps2.MinimumHeight = 410;//largeur fixe table

            tablefinal.AddCell(tableCorps2);
//Bas de page
           /* PdfPCell Cellespa2 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 20, Font.NORMAL)));
            Cellespa2.Border = Rectangle.TOP_BORDER;
            tablefinal.AddCell(Cellespa2);
            */
            if (prixProduitreduct.Type_promo != ApplicationConsts.typePrix_permanent)
            {
                PdfPCell Cellespa2 = new PdfPCell(new Phrase("* La remise s’applique sur le prix hors éco-mobilier", new Font(Utils.PoliceUtils.DINHabRg, 10, Font.NORMAL)));
                Cellespa2.Border = Rectangle.TOP_BORDER;
                tablefinal.AddCell(Cellespa2);
            }
            else
            {
                PdfPCell Cellespa2 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabRg, 10, Font.NORMAL)));
                Cellespa2.Border = Rectangle.TOP_BORDER;
                tablefinal.AddCell(Cellespa2);
            }

            PdfPTable tableBasdepage = new PdfPTable(4);
            tableBasdepage.DefaultCell.Padding = 10;
            PdfPCell CellTwoR321 = new PdfPCell(new Phrase(""));
            CellTwoR321.Border = 0;
            tableBasdepage.AddCell(CellTwoR321);
            PdfPCell CellTwoR322 = new PdfPCell(new Phrase(""));
            CellTwoR322.Border = 0;
            tableBasdepage.AddCell(CellTwoR322);
            PdfPCell CellTwoR323 = new PdfPCell(new Phrase(""));
            CellTwoR323.Border = 0;
            tableBasdepage.AddCell(CellTwoR323);

            applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            pathImgFr = "Content//themes//image//" + "habitat" + ".png";
            iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(applicationPath + pathImgFr);
            jpg2.ScaleAbsolute(120f, 27f);
            PdfPCell CellTwoR324 = new PdfPCell(jpg2);
            CellTwoR324.Border = 0;
            CellTwoR324.HorizontalAlignment = Element.ALIGN_RIGHT;
            tableBasdepage.AddCell(CellTwoR324);

            tablefinal.AddCell(tableBasdepage);
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
        /// generation plv de gamme format A5 recto verso (les tailles evoluent en fonction du nombre d'option, du nombre de produit et la taille de la DGCCRF)
        /// </summary>
        /// <param name="Gamme"></param>
        /// <param name="dateQuery"></param>
        /// <param name="magasinId"></param>
        /// <returns></returns>
        static public MemoryStream GeneratePLVPdf3(T_Presentation_Gamme_PLV Gamme, DateTime dateQuery, int magasinId)
        {
//taille text
            tailleTexteGammePlv Taille = new tailleTexteGammePlv();
            List<int> dimentionLG = new List<int>();
            List<float> dimentionIP = new List<float>();
            dimentionIP.Add(260 - 10 * (float)Gamme.dgccrfGamme.Length / 130);
            dimentionIP.Add(110 - 5 * (float)Gamme.dgccrfGamme.Length / 130);
            dimentionIP.Add(60 - 3.3f * (float)Gamme.dgccrfGamme.Length / 130);
            List<int> dimentionLG2 = new List<int>();
            Taille.nomGamme = 35;
            Taille.sousTitreDeGamme = 15;
            Taille.logoAuteur = dimentionLG;
            Taille.DGCCRF = 11;
            Taille.imageProduit = dimentionIP;
            Taille.variation = 10;
            Taille.description = 8;
            Taille.option = 13;
            Taille.prix = 20;
            Taille.prixPermanent = 13;
            Taille.lastPrix = 18;
            Taille.ecopart = 5;
            Taille.logoAuteur2 = dimentionLG2;
            Taille.reduct = 18;

//parametre pdf
            string codeMonnaie = DAO.LangueDao.getCodeMonnaieByMagasinId(magasinId);
            string PLV_REGLETTE_TAILLE_PRIX = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_REGLETTE_TAILLE_PRIX", magasinId);
            MemoryStream ms = new MemoryStream();
            //            Document pdfDoc = new Document(PageSize.A4, 20, 20, 20, 20);//A4 marge + format portrait
            Document pdfDoc = new Document(new RectangleReadOnly(842, 595), 40, 40, 20, 20);//A4 marge + format paysage
            //            Document pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0); //format
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms); //ecriture
            pdfDoc.Open();//ouverture
            //parametre/initialisation tableau final RV
            PdfPTable tablefinalRV = new PdfPTable(3);//declaration table final RV c
            tablefinalRV.WidthPercentage = 100; // Table size is set to 100% of the page
            tablefinalRV.DefaultCell.Padding = 0; //marge dans la cellule
            //tablefinal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablefinalRV.DefaultCell.BorderWidth = 0; // pas de bord

            float[] sglTblHdWidths = new float[3]; //tailles colonnes
            sglTblHdWidths[0] = 100f;
            sglTblHdWidths[1] = 10f;
            sglTblHdWidths[2] = 100f;
            tablefinalRV.SetWidths(sglTblHdWidths);


//parametre/initialisation tableau final
            PdfPTable tablefinal = new PdfPTable(1);//declaration table final 
            tablefinal.WidthPercentage = 100; // Table size is set to 100% of the page
            tablefinal.DefaultCell.Padding = 0; //marge dans la cellule
            //tablefinal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablefinal.DefaultCell.BorderWidth = 0; // pas de bord

            PdfPCell Cellnomsousgammespace = new PdfPCell(new Phrase(" ")); //espacement
            PdfPCell Cellnomsousgammespace2 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabRg, 8, Font.NORMAL)));
            PdfPCell Cellnomsousgammespace3 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 14, Font.NORMAL)));
            Cellnomsousgammespace.Border = 0;
            Cellnomsousgammespace2.Border = 0;
            Cellnomsousgammespace3.Border = 0;
//En Tete
            PdfPTable tableEnTete = new PdfPTable(3);
            float[] sglTblHdWidths2 = new float[3]; //tailles colonnes
            sglTblHdWidths2[0] = 150f;
            sglTblHdWidths2[1] = 40f;
            sglTblHdWidths2[2] = 40f;
            tableEnTete.SetWidths(sglTblHdWidths2);

            tablefinal.DefaultCell.BorderWidth = 0;
            tableEnTete.DefaultCell.BorderWidth = 0;
        //titre
            PdfPTable tabletitre = new PdfPTable(1);
            string nom = "";
            for(int v =0; v<= Math.Min(10,Gamme.nomGamme.Length-1); v++)
                nom += Gamme.nomGamme[v];
            PdfPCell CellTwoR311 = new PdfPCell(new Phrase(nom, new Font(Utils.PoliceUtils.DINHabBd, Taille.nomGamme, Font.NORMAL)));
            CellTwoR311.Border = 0;
            tabletitre.AddCell(CellTwoR311);
            PdfPCell CellTwo = new PdfPCell(new Phrase(Gamme.sousTitreGamme, new Font(Utils.PoliceUtils.DINHabRg, Taille.sousTitreDeGamme, Font.NORMAL)));
            CellTwo.Border = 0;
            tabletitre.AddCell(CellTwo);
            tabletitre.DefaultCell.BorderWidth = 0;
            tableEnTete.AddCell(tabletitre);

        //reduct ?
            T_Prix prixProduitreduct = Managers.FicheProduitManager.getPrixBySkuAndDate(Gamme.sousGammes[0].skus[0], magasinId, dateQuery);
            decimal? pourcentageReductionreduct = 0;
            if (prixProduitreduct.Type_promo != ApplicationConsts.typePrix_permanent)
            {
                T_Prix prixProduitReference = Managers.FicheProduitManager.getPrixPermanentPrecedent(prixProduitreduct);
                pourcentageReductionreduct = 100 - (((prixProduitreduct.Prix_produit - prixProduitreduct.Eco_mobilier) * 100) / prixProduitReference.Prix_produit);

                // Arrondit un pourcentage très proche de 0 (chiffre après virgule < 0.05)
                pourcentageReductionreduct = Utils.SpecificMathUtils.getRoundDecimal(pourcentageReductionreduct);
            }
            PdfPCell CellTwoR312;
            if (prixProduitreduct.Type_promo != ApplicationConsts.typePrix_permanent) { CellTwoR312 = new PdfPCell(new Phrase("- " + pourcentageReductionreduct + "% *", new Font(Utils.PoliceUtils.DINHabRg, Taille.reduct, Font.NORMAL))); }
            else { CellTwoR312 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabRg, Taille.reduct, Font.NORMAL))); }
            //PdfPCell CellTwoR312 = new PdfPCell(new Phrase("- " + pourcentageReduction + "%"));
            CellTwoR312.Border = 0;
            tableEnTete.AddCell(CellTwoR312);
           /* PdfPCell CellTwoR313 = new PdfPCell(new Phrase("reduc2"));
            CellTwoR313.Border = 0;
            tableEnTete.AddCell(CellTwoR313);*/
        //logo HDS?
            string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            string pathImgFr = "Content//themes//image//" + "LogoHDS.PNG";
            if (Gamme.logoGamme == 1)
            {
                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(applicationPath + pathImgFr);
                //iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(new Uri("http://2997fr-mssql04/product/Content/themes/image/LogoHDS.PNG"));
                jpg.ScaleAbsolute(77f, 56f);
                PdfPCell CellTwoR314 = new PdfPCell(jpg);
                CellTwoR314.Border = 0;
                CellTwoR314.HorizontalAlignment = Element.ALIGN_RIGHT;
                tableEnTete.AddCell(CellTwoR314);
            }
            else
            {
                PdfPCell Cellvide7 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 5, Font.NORMAL)));
                Cellvide7.Border = 0;
                tableEnTete.AddCell(Cellvide7);

            }
            tablefinal.AddCell(tableEnTete);
//corps
            PdfPTable tableCorps = new PdfPTable(1);
            tableCorps.DefaultCell.BorderWidth = 0;
        //DGCCRF
            PdfPCell CellDGCCRF = new PdfPCell(new Phrase(Gamme.dgccrfGamme, new Font(Utils.PoliceUtils.DINHabRg, Taille.DGCCRF, Font.NORMAL)));
            CellDGCCRF.Border = 0;
            tableCorps.AddCell(CellDGCCRF);
            PdfPCell Cellespa = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 10, Font.NORMAL)));
            Cellespa.Border = 0;
            if (Gamme.sousGammes.Count > 9 || Gamme.sousGammes.Count < 7)
                tableCorps.AddCell(Cellespa);
        //PRODUIT
            //table de taille nombre de sous + 1 si opt
            int tailleT = 0;
            int nbProduit = Gamme.sousGammes.Count;
            int indiceTailleImage = 0;
            float coef = 1.0F; 
            if (nbProduit > 2)
                coef = 0.6F;
            if (nbProduit == 1)
                tailleT = 1;
            if (nbProduit > 1 && nbProduit < 5)
                tailleT = 2;

            if (nbProduit > 4 && nbProduit < 10)
            {
                tailleT = 3;

            } if (nbProduit > 9 && nbProduit < 17)
            {
                tailleT = 4;
            }
            if (nbProduit > 2 && nbProduit < 7)          
            {
                indiceTailleImage = 1;
            }
            if (nbProduit > 6 && nbProduit < 13)
            {
                indiceTailleImage = 2;
            }
            Taille.prix = (int) Math.Round((float)Taille.prix*coef);
            Taille.prixPermanent = (int)Math.Round((float)Taille.prixPermanent * coef); 
            Taille.lastPrix = (int)Math.Round((float)Taille.lastPrix * coef);
            Taille.variation = (int)Math.Round((float)Taille.variation * (coef+0.1F));
            Taille.description = (int)Math.Round((float)Taille.description * (coef+0.1F));

            nbProduit = nbProduit + tailleT - (nbProduit % tailleT);//nbProduit devient nombre de case dans le tableau
            PdfPTable tableProduit = new PdfPTable(tailleT);
            tableProduit.DefaultCell.BorderWidth = 0;
            int rang =0;
            for (int i = 0; i < nbProduit*2; i++)
            {
                if (i % (tailleT*2) == tailleT && rang != 0)
                    rang -= tailleT;
                if (Gamme.sousGammes.Count<=rang)
                {
                    //vide
                    PdfPCell Cellvide = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 15, Font.NORMAL)));
                    Cellvide.Border = 0;
                    tableProduit.AddCell(Cellvide);
                }
                else
                {
                    
                    //produit i
                    TickitDataProduit ticket = Managers.TickitDataManager.getTickitDataPourChevalet(Gamme.sousGammes[rang].skus[0], magasinId, dateQuery, null, null); // on reutilise fct existante ducoup pas besoin des 2 derniers parametre ici
                    //TickitDataProduit ticket = TickitDataManager.getTickitData(Gamme.sousGammes[i].skus[0], magasinId, "", dateQuery);//on recupere toutes les données pour afficher le prix...
                    T_Prix prixProduit = Managers.FicheProduitManager.getPrixBySkuAndDate(Gamme.sousGammes[rang].skus[0], magasinId, dateQuery);
                    if (prixProduit.Type_promo != ApplicationConsts.typePrix_permanent)//a voir
                    {
                        T_Prix prixProduitReference = Managers.FicheProduitManager.getPrixPermanentPrecedent(prixProduit);
                        decimal? pourcentageReduction = 100 - (((prixProduit.Prix_produit - prixProduit.Eco_mobilier) * 100) / prixProduitReference.Prix_produit);

                        // Arrondit un pourcentage très proche de 0 (chiffre après virgule < 0.05)
                        pourcentageReduction = Utils.SpecificMathUtils.getRoundDecimal(pourcentageReduction);
                    }



                    PdfPCell Cellproduit = new PdfPCell(new Phrase(Gamme.sousGammes[rang].skus[0], new Font(Utils.PoliceUtils.DINHabBd, 15, Font.NORMAL)));
                    Cellproduit.Border = 0;
                    //tableProduit.AddCell(Cellproduit);

                    //creation des phrase
                    PdfPTable tablePage1 = new PdfPTable(1);
                    Phrase phraseItem = new Phrase(ticket.variation, new Font(Utils.PoliceUtils.DINHabRg, Taille.variation, Font.NORMAL));
                    //Phrase phraseDesc = new Phrase("description", new Font(Utils.PoliceUtils.DINHabRg, 10, Font.NORMAL));
                    Phrase phraseprixReference = new Phrase(ticket.prixPermanent, new Font(Utils.PoliceUtils.DINHabRg, Taille.prixPermanent, Font.STRIKETHRU));
                    Phrase espaceur = new Phrase("   ", new Font(Utils.PoliceUtils.DINHabRg, 5, Font.NORMAL));
                    Phrase phraseprix = new Phrase(ticket.prix, new Font(Utils.PoliceUtils.DINHabBd, Taille.prix, Font.NORMAL));
                    Phrase phraseTaxe = new Phrase(ticket.Taxe_eco, new Font(Utils.PoliceUtils.DINHabRg, Taille.ecopart, Font.NORMAL));

                    //phrase desc
                    Phrase phraseDesc = new Phrase();
                    T_Produit produit = Managers.FicheProduitManager.getProduitBySku(Gamme.sousGammes[rang].skus[0], magasinId);
                    Phrase separateurDesc = new Phrase(" x ", new Font(Utils.PoliceUtils.DINHabRg, Taille.description, Font.NORMAL));//a mettre entre les args
                    Phrase phraselargeur = new Phrase("l." + produit.Largeur, new Font(Utils.PoliceUtils.DINHabRg, Taille.description, Font.NORMAL));
                    Phrase phraselongueur = new Phrase("L." + produit.Longueur, new Font(Utils.PoliceUtils.DINHabRg, Taille.description, Font.NORMAL));
                    Phrase phrasehauteur = new Phrase("H." + produit.Hauteur, new Font(Utils.PoliceUtils.DINHabRg, Taille.description, Font.NORMAL));
                    Phrase phraseprofondeur = new Phrase("P." + produit.Profondeur, new Font(Utils.PoliceUtils.DINHabRg, Taille.description, Font.NORMAL));
                    Phrase phrasediametre = new Phrase("D." + produit.Diametre, new Font(Utils.PoliceUtils.DINHabRg, Taille.description, Font.NORMAL));
                    Phrase phrasecolis = new Phrase("colis : " + produit.Nombre_colis, new Font(Utils.PoliceUtils.DINHabRg, Taille.description, Font.NORMAL));
                    
                    int sepatest = 0;

                    if (produit.Largeur != null) {
                        phraseDesc.Add(phraselargeur);
                        sepatest++;
                    }

                    if (produit.Longueur != null)
                    {
                        if (sepatest != 0) {
                            phraseDesc.Add(separateurDesc);
                        }
                        phraseDesc.Add(phraselongueur);
                        sepatest++;
                    }

                    if (produit.Hauteur != null)
                    {
                        if (sepatest != 0)
                        {
                            phraseDesc.Add(separateurDesc);
                        }
                        phraseDesc.Add(phrasehauteur);
                        sepatest++;
                    }

                    if (produit.Profondeur != null)
                    {
                        if (sepatest != 0)
                        {
                            phraseDesc.Add(separateurDesc);
                        }
                        phraseDesc.Add(phraseprofondeur);
                        sepatest++;
                    }

                    if (produit.Diametre != null)
                    {
                        if (sepatest != 0)
                        {
                            phraseDesc.Add(separateurDesc);
                        }
                        phraseDesc.Add(phrasediametre);
                        sepatest++;
                    }

                    if (produit.Nombre_colis != null)
                    {
                        if (sepatest != 0)
                        {
                            phraseDesc.Add(espaceur);
                        }
                        phraseDesc.Add(phrasecolis);
                        sepatest++;
                    }


                    // MSRIDI 14122015
                    Phrase phrasePrixSoldePrecedent = new Phrase(ticket.prixSoldePrecedent, new Font(Utils.PoliceUtils.DINHabRg, Taille.lastPrix, Font.STRIKETHRU));
                    if (i % (tailleT * 2) >= 0 && i % (tailleT * 2) < tailleT)
                    {
                        try
                        {
                            iTextSharp.text.Image jpg3 = iTextSharp.text.Image.GetInstance(new Uri("http://ean.habitat.fr/PLV_TAKEIT/" + Gamme.sousGammes[rang].skus[0] + ".png"));
                            //jpg3.ScaleAbsolute(130f, 100f);
                            PdfPCell cellImage = new PdfPCell(jpg3);
                            //cellImage.VerticalAlignment = Element.ALIGN_BOTTOM;
                            cellImage.HorizontalAlignment = Element.ALIGN_LEFT;
                            cellImage.Border = 0;
                            //tablePage1.AddCell(cellImage);
                            var paragraph = new Paragraph();
                            var rapportimage = jpg3.Width / jpg3.Height;
                            var Largeur= Taille.imageProduit[indiceTailleImage];
                            //if (rapportimage < 1.7)//probleme car image avec largeur trop grand qui peu faire deborder de la page
                            { 
                                jpg3.ScaleAbsolute(Largeur * rapportimage, Largeur);// ducoup taille assignée (proportion gardée)
                            }
                            var chunk = new Chunk(jpg3, 0, 0, true);
                            paragraph.Add(chunk);
                            PdfPCell celli = new PdfPCell();
                            celli.Border = 0;
                            celli.AddElement(paragraph);
                            tablePage1.AddCell(celli);
                        }
                        catch (Exception)
                        {
                            applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                            pathImgFr = "Content//themes//image//" + "habitat" + ".png";
                            iTextSharp.text.Image jpg3 = iTextSharp.text.Image.GetInstance(applicationPath + pathImgFr);
                            //jpg3.ScaleAbsolute(100f, 100f);
                            PdfPCell cellImage = new PdfPCell(jpg3);
                            //cellImage.VerticalAlignment = Element.ALIGN_BOTTOM;
                            cellImage.HorizontalAlignment = Element.ALIGN_LEFT;
                            cellImage.Border = 0;
                            //tablePage1.AddCell(cellImage);
                            var paragraph = new Paragraph();
                            var rapportimage = jpg3.Width / jpg3.Height;
                            var Largeur = Taille.imageProduit[indiceTailleImage];
                            //if (rapportimage < 1.7)//probleme car image avec largeur trop grand qui peu faire deborder de la page
                            {
                                jpg3.ScaleAbsolute(Largeur * rapportimage, Largeur);// ducoup taille assignée (proportion gardée)
                            }
                            var chunk = new Chunk(jpg3, 0, 0, true);
                            paragraph.Add(chunk);
                            PdfPCell celli = new PdfPCell();
                            celli.Border = 0;
                            celli.AddElement(paragraph);
                            tablePage1.AddCell(celli);
                        }

                    }
                    else
                    {
                        //variation
                        PdfPCell cellItems = new PdfPCell(phraseItem);
                        cellItems.VerticalAlignment = Element.ALIGN_BOTTOM;
                        cellItems.HorizontalAlignment = Element.ALIGN_LEFT;
                        cellItems.Border = 0;
                        tablePage1.AddCell(cellItems);

                        //desc
                        PdfPCell cellDesc = new PdfPCell(phraseDesc);
                        cellDesc.VerticalAlignment = Element.ALIGN_BOTTOM;
                        cellDesc.HorizontalAlignment = Element.ALIGN_LEFT;
                        cellDesc.Border = 0;
                        tablePage1.AddCell(cellDesc);

                        //prix
                        PdfPCell cellPrix = new PdfPCell(new Phrase());
                        cellPrix.Phrase.Add(phraseprixReference);
                        cellPrix.Phrase.Add(espaceur);

                        // MSRIDI 14122015
                        if (ticket.prixSoldePrecedent != "")
                        {
                            cellPrix.Phrase.Add(phrasePrixSoldePrecedent);
                            cellPrix.Phrase.Add(espaceur);
                        }

                        cellPrix.Phrase.Add(phraseprix);
                        cellPrix.VerticalAlignment = Element.ALIGN_BOTTOM;
                        cellPrix.HorizontalAlignment = Element.ALIGN_LEFT;
                        cellPrix.Border = 0;
                        tablePage1.AddCell(cellPrix);

                        //taxe eco
                        PdfPCell cellTaxeEco = new PdfPCell(new Phrase());
                        cellTaxeEco.Phrase.Add(phraseTaxe);
                        cellTaxeEco.VerticalAlignment = Element.ALIGN_BOTTOM;
                        cellTaxeEco.HorizontalAlignment = Element.ALIGN_LEFT;
                        cellTaxeEco.Border = 0;
                        cellTaxeEco.Colspan = 2;
                        tablePage1.AddCell(cellTaxeEco);

                        tablePage1.AddCell(spacer(10));
                    }
                    tableProduit.AddCell(tablePage1);
                }
                rang++;
            
            }
            tableCorps.AddCell(tableProduit);
            PdfPCell tableCorps2 = new PdfPCell(tableCorps);
            tableCorps2.Border = 0;
            tableCorps2.MinimumHeight = 410;//largeur fixe table

            tablefinal.AddCell(tableCorps2);
//Bas de page
            if (prixProduitreduct.Type_promo != ApplicationConsts.typePrix_permanent)
            {
                PdfPCell Cellespa2 = new PdfPCell(new Phrase("* La remise s’applique sur le prix hors éco-mobilier", new Font(Utils.PoliceUtils.DINHabRg, 10, Font.NORMAL)));
                Cellespa2.Border = Rectangle.TOP_BORDER;
                tablefinal.AddCell(Cellespa2);
            }
            else
            {
                PdfPCell Cellespa2 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 10, Font.NORMAL)));
                Cellespa2.Border = Rectangle.TOP_BORDER;
                tablefinal.AddCell(Cellespa2);
            }

            PdfPTable tableBasdepage = new PdfPTable(4);
            tableBasdepage.DefaultCell.Padding = 10;
            PdfPCell CellTwoR321 = new PdfPCell(new Phrase(""));
            CellTwoR321.Border = 0;
            tableBasdepage.AddCell(CellTwoR321);
            PdfPCell CellTwoR322 = new PdfPCell(new Phrase(""));
            CellTwoR322.Border = 0;
            tableBasdepage.AddCell(CellTwoR322);
            PdfPCell CellTwoR323 = new PdfPCell(new Phrase(""));
            CellTwoR323.Border = 0;
            tableBasdepage.AddCell(CellTwoR323);

            applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            pathImgFr = "Content//themes//image//" + "habitat" + ".png";
            iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(applicationPath + pathImgFr);
            jpg2.ScaleAbsolute(120f, 27f);
            PdfPCell CellTwoR324 = new PdfPCell(jpg2);
            CellTwoR324.Border = 0;
            CellTwoR324.HorizontalAlignment = Element.ALIGN_RIGHT;
            tableBasdepage.AddCell(CellTwoR324);

            tablefinal.AddCell(tableBasdepage);

            tablefinalRV.AddCell(tablefinal);//page de gauche
            CellTwoR323.Border = 0;
            tablefinalRV.AddCell(CellTwoR323);//separation au milieu
            tablefinalRV.AddCell(tablefinal);//page droite (copy)
//fermeture pdf
            pdfDoc.Add(tablefinalRV);
            pdfDoc.Close();
            writer.Dispose();
//return
            MemoryStream msToReturn = new MemoryStream();
            msToReturn.Write(ms.ToArray(), 0, ms.ToArray().Length);
            msToReturn.Position = 0;
            return msToReturn;


        }

        /// Fonction permetant de génerer le document PDF pour PLV (version A4 bloc separé si option de produit differente)
        /// 21/07/16
        /// ALOUI Driss
        /// <param name="chevalet"></param>
        /// <param name="format"></param>
        /// <param name="magasinId"></param>
        /// <param name="dateQuery"></param>
        /// <returns></returns>
        static public MemoryStream GeneratePLVPdf(List<T_Gamme_PLV> Gammes, List<T_Sous_Gamme_PLV> SousGammes, DateTime dateQuery, int magasinId)
        {           
//parametre pdf
            string codeMonnaie = DAO.LangueDao.getCodeMonnaieByMagasinId(magasinId);
            string PLV_REGLETTE_TAILLE_PRIX = DAO.ConfigurationBisDao.getValeurByCleAndMagasinId("PLV_REGLETTE_TAILLE_PRIX", magasinId);
            MemoryStream ms = new MemoryStream();
//            Document pdfDoc = new Document(PageSize.A4, 20, 20, 20, 20);//A4 marge + format portrait
            Document pdfDoc = new Document(new RectangleReadOnly(842, 595), 40, 40, 20, 20);//A4 marge + format paysage
//            Document pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0); //format
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms); //ecriture
            pdfDoc.Open();//ouverture
//parametre/initialisation tableau final
            PdfPTable tablefinal = new PdfPTable(1);//declaration table final contenant les reglettes
            tablefinal.WidthPercentage = 100; // Table size is set to 100% of the page
            /*
            float[] sglTblHdWidths = new float[3]; //tailles colonnes
            sglTblHdWidths[0] = 100f;
            sglTblHdWidths[1] = 10f;
            sglTblHdWidths[2] = 100f;

            tablefinal.SetWidths(sglTblHdWidths); // Set the column widths on table creation. Unlike HTML cells cannot be sized.
       */     tablefinal.DefaultCell.Padding = 0; //marge dans la cellule
            //tablefinal.HorizontalAlignment = Element.ALIGN_LEFT;
            tablefinal.DefaultCell.BorderWidth = 0; // pas de bord
            

            PdfPCell Cellnomsousgammespace = new PdfPCell(new Phrase(" ")); //espacement
            PdfPCell Cellnomsousgammespace2 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabRg, 8, Font.NORMAL)));
            PdfPCell Cellnomsousgammespace3 = new PdfPCell(new Phrase(" ", new Font(Utils.PoliceUtils.DINHabBd, 14, Font.NORMAL)));
            Cellnomsousgammespace.Border = 0;
            Cellnomsousgammespace2.Border = 0;
            Cellnomsousgammespace3.Border = 0;
//En Tete
            PdfPTable tableEnTete = new PdfPTable(4);
            tablefinal.DefaultCell.BorderWidth = 0; 
            PdfPCell CellTwoR311 = new PdfPCell(new Phrase(Gammes[0].nomGamme, new Font(Utils.PoliceUtils.DINHabBd, 35, Font.NORMAL)));
            CellTwoR311.Border = 0;
            tableEnTete.AddCell(CellTwoR311);
            PdfPCell CellTwoR312 = new PdfPCell(new Phrase("reduc1"));
            CellTwoR312.Border = 0;
            tableEnTete.AddCell(CellTwoR312);
            PdfPCell CellTwoR313 = new PdfPCell(new Phrase("reduc2"));
            CellTwoR313.Border = 0;
            tableEnTete.AddCell(CellTwoR313);
//logo HDS?
            string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            string pathImgFr = "Content//themes//image//" + "LogoHDS.PNG";
            if (Gammes[0].logoGamme==1)
            {  
                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(applicationPath + pathImgFr);
                //iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(new Uri("http://2997fr-mssql04/product/Content/themes/image/LogoHDS.PNG"));
                jpg.ScaleAbsolute(77f, 56f);
                PdfPCell CellTwoR314 = new PdfPCell(jpg);
                CellTwoR314.Border = 0;
                CellTwoR314.HorizontalAlignment = Element.ALIGN_RIGHT;
                tableEnTete.AddCell(CellTwoR314);
            }
            tablefinal.AddCell(tableEnTete);
//Corps
            PdfPTable tableCorps = new PdfPTable(1);
            tableCorps.DefaultCell.BorderWidth = 0;
    //DGCCRF
            PdfPCell CellDGCCRF = new PdfPCell(new Phrase(Gammes[0].dgccrfGamme, new Font(Utils.PoliceUtils.DINHabRg, 11, Font.NORMAL)));
            CellDGCCRF.Border = 0;
            tableCorps.AddCell(CellDGCCRF);
    //PRODUIT
            string[] NomsousGammes = Gammes[0].sousGammes.Split(';');
            int nbSousgamme = NomsousGammes.Length;
            //opt diferrente?
            List<List<T_Sous_Gamme_PLV>> LSG = new List<List<T_Sous_Gamme_PLV>>();
            int cpto = 0;
            foreach (var item in SousGammes)
            {
                cpto = 0;
                for(var i = 0;i<LSG.Count;i++)
                {
                    for (var j = 0; j<LSG[i].Count; j++)
                    {
                        if (LSG[i][j].options == item.options && cpto == 0)
                        {
                            LSG[i].Add(item);
                            cpto = 1;
                        }
                    }
                }
                if (cpto == 0)
                {
                    List < T_Sous_Gamme_PLV > SG = new List<T_Sous_Gamme_PLV>();
                    SG.Add(item);
                    LSG.Add(SG);
                }
            }
            int nbOptionDif = LSG.Count;

            PdfPTable tableProduit = new PdfPTable(nbOptionDif);
            tableProduit.DefaultCell.BorderWidth = 0;
            
            float[] sglTblHdWidths = new float[nbOptionDif]; //tailles colonnes
            for( var i = 0; i< nbOptionDif; i++){
                sglTblHdWidths[i] = (LSG[i].Count+1)*100f; 
            }
            tableProduit.SetWidths(sglTblHdWidths); // Set the column widths on table creation. Unlike HTML cells cannot be sized.

            for (var i = 0; i < nbOptionDif; i++)
            {
                PdfPTable tableSousGamme = new PdfPTable(LSG[i].Count+1);
                tableSousGamme.DefaultCell.BorderWidth = 0;

                float[] sglTblHdWidths2 = new float[LSG[i].Count+1]; //tailles colonnes
               // if (nbOptionDif) { }
                sglTblHdWidths2[0] = 50f;
                for( var k = 1; k< LSG[i].Count+1; k++){
                    sglTblHdWidths2[k] = 100f; 
                }
                tableSousGamme.SetWidths(sglTblHdWidths2); // Set the column widths on table creation. Unlike HTML cells cannot be sized.

                PdfPCell Cell11 = new PdfPCell(new Phrase(" "));
                Cell11.Border = 0;
                tableSousGamme.AddCell(Cell11);
                //image
                foreach (var item in LSG[i])
                {

                    applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                    pathImgFr = "Content//themes//image//" + "LogoHDS.PNG";
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(applicationPath + pathImgFr);
                    //iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(new Uri("http://2997fr-mssql04/product/Content/themes/image/LogoHDS.PNG"));
                    jpg.ScaleAbsolute(77f, 56f);
                    PdfPCell Cellnomsousgamme = new PdfPCell(jpg);
                    Cellnomsousgamme.Border = 0;
                    tableSousGamme.AddCell(Cellnomsousgamme); 
                }
                
                tableSousGamme.AddCell(Cell11);
                foreach (var item in LSG[i])
                {
                    PdfPCell Cellnomsousgamme = new PdfPCell(new Phrase("image " + item.skus.Split(',', ';')[0], new Font(Utils.PoliceUtils.DINHabRg, 8, Font.NORMAL)));
                    Cellnomsousgamme.Border = 0;
                    tableSousGamme.AddCell(Cellnomsousgamme);
                }
                //nomdesousgamme
                tableSousGamme.AddCell(Cell11);
                foreach (var item in LSG[i])
                {
                    PdfPCell Cellnomsousgamme = new PdfPCell(new Phrase(item.nomSGamme, new Font(Utils.PoliceUtils.DINHabBd, 16, Font.NORMAL)));
                    Cellnomsousgamme.Border = 0;
                    tableSousGamme.AddCell(Cellnomsousgamme);
                }
                tableSousGamme.AddCell(Cell11);
                foreach (var item in LSG[i])
                {
                    PdfPCell Celldescsousgamme = new PdfPCell(new Phrase(item.descSGamme, new Font(Utils.PoliceUtils.DINHabRg, 8, Font.NORMAL)));
                    Celldescsousgamme.Border = 0;
                    tableSousGamme.AddCell(Celldescsousgamme);
                }
                //prixoption
                for (var j=0; j<LSG[i].Count+1; j++)
                {
                    PdfPTable tableprix = new PdfPTable(1);
                    tableprix.DefaultCell.BorderWidth = 0;
                    string[] sku = LSG[i][0].options.Split(';', ',');//on init avec n importe quoi
                    if (j != 0)
                    {
                         sku = LSG[i][j-1].skus.Split(';', ',');// on le remplit avec sku de la sous game sauf pour la premiere colonne qui la colonne pour les desc des opts
                    }
                    string[] opt = LSG[i][0].options.Split(';', ',');
                    int nbopt = opt.Length;
                    for (var h = 0; h < nbopt; h++)
                    {
                        if (j == 0)//colonne libele opt
                        {
                            PdfPCell Cellnomsousgamme = new PdfPCell(new Phrase(opt[h]));
                            Cellnomsousgamme.Border = 0;
                            tableprix.AddCell(Cellnomsousgamme);
                            tableprix.AddCell(Cellnomsousgammespace2);//espacement
                            tableprix.AddCell(Cellnomsousgammespace2);//espacement
                            tableprix.AddCell(Cellnomsousgammespace3);//espacement
                        }
                        else
                        {
                            TickitDataProduit ticket = TickitDataManager.getTickitData(sku[h], magasinId, "", dateQuery);//on recupere toutes les données pour afficher le prix...
                            PdfPCell Cellnomsousgamme = new PdfPCell(new Phrase(ticket.prixPermanent + codeMonnaie, new Font(Utils.PoliceUtils.DINHabBd, 14, Font.NORMAL)));
                            Cellnomsousgamme.Border = 0;
                            tableprix.AddCell(Cellnomsousgamme);
                            PdfPCell CellnomsousgammeT = new PdfPCell(new Phrase("dont " + ticket.Taxe_eco + " d'éco-part", new Font(Utils.PoliceUtils.DINHabRg, 8, Font.NORMAL)));
                            CellnomsousgammeT.Border = 0;
                            tableprix.AddCell(CellnomsousgammeT);
                            PdfPCell CellnomsousgammeR = new PdfPCell(new Phrase(ticket.sku, new Font(Utils.PoliceUtils.DINHabRg, 8, Font.NORMAL)));
                            CellnomsousgammeR.Border = 0;
                            tableprix.AddCell(CellnomsousgammeR);
                            tableprix.AddCell(Cellnomsousgammespace);//espacement
                        }
                    }
                    tableSousGamme.AddCell(tableprix);
                }
                tableProduit.AddCell(tableSousGamme);

            }
            tableCorps.AddCell(tableProduit);

            PdfPCell tableCorps2 = new PdfPCell(tableCorps);
            tableCorps2.Border = 0;
            tableCorps2.MinimumHeight = 455;//largeur fixe table

            tablefinal.AddCell(tableCorps2);
//Bas de page
            PdfPTable tableBasdepage = new PdfPTable(4);
            tableBasdepage.DefaultCell.Padding = 10;
            PdfPCell CellTwoR321 = new PdfPCell(new Phrase(""));
            CellTwoR321.Border = Rectangle.TOP_BORDER;
            tableBasdepage.AddCell(CellTwoR321);
            PdfPCell CellTwoR322 = new PdfPCell(new Phrase(""));
            CellTwoR322.Border = Rectangle.TOP_BORDER;
            tableBasdepage.AddCell(CellTwoR322);
            PdfPCell CellTwoR323 = new PdfPCell(new Phrase(""));
            CellTwoR323.Border = Rectangle.TOP_BORDER;
            tableBasdepage.AddCell(CellTwoR323);

            applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            pathImgFr = "Content//themes//image//" + "habitat" + ".png";
            iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(applicationPath + pathImgFr);
            //iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(new Uri("http://2997fr-mssql04/product/Content/themes/image/habitat.png"));
            jpg2.ScaleAbsolute(120f, 27f);
            PdfPCell CellTwoR324 = new PdfPCell(jpg2);
            CellTwoR324.Border = Rectangle.TOP_BORDER;
            CellTwoR324.HorizontalAlignment = Element.ALIGN_RIGHT;

            tableBasdepage.AddCell(CellTwoR324);

            tablefinal.AddCell(tableBasdepage);
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