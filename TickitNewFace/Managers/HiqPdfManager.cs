using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HiQPdf;
using System.Web.Mvc;
using System.IO;
using System.Text;
using iTextSharp.text;


namespace TickitNewFace.Managers
{
    public static class HiqPdfManager 
    {
        /// <summary>
        /// Convertit un code HTML en PDF.
        /// </summary>
        /// <param name="htmlCode">code HTML</param>
        /// <param name="suffix">suffixe du fichier de sortie</param>
        /// <param name="prefix">préfixe du fichier de sortie</param>
        public static void ConvertToPdf(HtmlToPdf htmlToPdfConverter, string htmlCode, string suffix, string prefix, string url)
        {
            // convert HTML to PDF
            byte[] pdfBuffer = null;

            htmlToPdfConverter.SerialNumber = Const.ApplicationConsts.HiqPdfSerialNumber;

            if (url != "")
            pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(url);
            else
            pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlCode, "");

            File.WriteAllBytes(Const.ApplicationConsts.dossierTraitementPdf + prefix + Const.ApplicationConsts.SessionID + "_" + suffix + ".pdf", pdfBuffer);
        }

        public static ActionResult mergePdf(List<string> listePathPdfFiles, string prefixFileName, string FileDownloadName)
        {
            // create an empty document which will become the final document after merge
            PdfDocument resultDocument = new PdfDocument();
            List<PdfDocument> listDocs = new List<PdfDocument>();

            resultDocument.SerialNumber = Const.ApplicationConsts.HiqPdfSerialNumber;

            foreach (string path in listePathPdfFiles)
            {
                PdfDocument currentDoc = PdfDocument.FromFile(path);
                resultDocument.AddDocument(currentDoc);
                listDocs.Add(currentDoc);
            }

            string pdfFile = Const.ApplicationConsts.dossierTraitementPdf + prefixFileName + Const.ApplicationConsts.SessionID + "_final.pdf" + ".pdf";
            resultDocument.WriteToFile(pdfFile);
            

            byte[] pdfBuffer = File.ReadAllBytes(pdfFile);
            
            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = FileDownloadName;

            resultDocument.Close();
            File.Delete(pdfFile);

            // reproduire la boucle car sinon il est impossible d'avoir un fichier de sortie en bonne et due forme.
            foreach (PdfDocument doc in listDocs)
            {

                doc.Close();
            }

            foreach (string path in listePathPdfFiles)
            {
                File.Delete(path);
            }


            return fileResult;
        }


        private static PdfPageSize GetSelectedPageSize()
        {
            return PdfPageSize.A4;
        }
    }
}