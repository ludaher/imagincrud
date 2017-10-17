using PdfSharp.Drawing;
using PdfSharp.Charting;
using PdfSharp.Pdf;
using PdfSharp.SharpZipLib;
using PdfSharp.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using System.IO;

namespace ImaginCrud.Util
{
    public static class PdfConverter
    {
        public static void ConvertTiffToPdf(byte[] source, string destinaton)
        {
            try
            {
                using (var ms = new MemoryStream(source))
                {
                    Image MyImage = Image.FromStream(ms);
                    PdfDocument doc = new PdfDocument();
                    for (int PageIndex = 0; PageIndex < MyImage.GetFrameCount(FrameDimension.Page); PageIndex++)
                    {
                        MyImage.SelectActiveFrame(FrameDimension.Page, PageIndex);
                        XImage img = XImage.FromGdiPlusImage(MyImage);
                        var page = new PdfPage();
                        if (img.PixelWidth > img.PixelHeight)
                        {
                            page.Orientation = PageOrientation.Landscape;

                        }
                        else
                        {
                            page.Orientation = PageOrientation.Portrait;
                        }
                        doc.Pages.Add(page);
                        XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[PageIndex]);
                        xgr.DrawImage(img, 0, 0);
                    }
                    doc.Save(destinaton);
                    doc.Close();
                    MyImage.Dispose();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error convirtiendo archivo a pdf: {0}", ex.ToString());
            }
        }

        public static MemoryStream ConvertTiffToPdf(byte[] source)
        {
            MemoryStream streamToReturn = new MemoryStream() ;
            try
            {
                using (var ms = new MemoryStream(source))
                {
                    Image MyImage = Image.FromStream(ms);
                    PdfDocument doc = new PdfDocument();
                    for (int PageIndex = 0; PageIndex < MyImage.GetFrameCount(FrameDimension.Page); PageIndex++)
                    {
                        MyImage.SelectActiveFrame(FrameDimension.Page, PageIndex);
                        XImage img = XImage.FromGdiPlusImage(MyImage);
                        var page = new PdfPage();
                        if (img.PixelWidth > img.PixelHeight)
                        {
                            page.Orientation = PageOrientation.Landscape;
                        }
                        else
                        {
                            page.Orientation = PageOrientation.Portrait;
                        }
                        doc.Pages.Add(page);
                        XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[PageIndex]);
                        xgr.DrawImage(img, 0, 0);
                    }
                    doc.Save(streamToReturn, false);
                    doc.Close();
                    MyImage.Dispose();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error convirtiendo archivo a pdf: {0}", ex.ToString());
            }
            return streamToReturn;
        }

        public static byte[] GetPdfFromFile(System.Web.HttpPostedFileBase file)
        {
            byte[] data;
            using (Stream inputStream = file.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }
            var extension = Path.GetExtension(file.FileName);
            switch (extension.ToLower())
            {
                case ".tiff":
                case ".tif":
                    using (var stream = ConvertTiffToPdf(data))
                    {
                        data = stream.ToArray();
                    }
                    break;
                default:
                    break;
            }
            return data;
        }
    }
}
