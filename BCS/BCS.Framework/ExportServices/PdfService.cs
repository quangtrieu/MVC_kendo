using System.IO;
using System.Web.DynamicData;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BCS.Framework.ExportServices
{
    public partial class Pdf : HttpServerUtilityBase
    {

        // First, create our fonts... (For more on working w/fonts in iTextSharp, see: http://www.mikesdotnetting.com/Article/81/iTextSharp-Working-with-Fonts
        public static Font titleFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
        public static Font subTitle = FontFactory.GetFont("Arial", 9, Font.NORMAL);
        //public static Font subTitleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
        //public static Font boldTableFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
        public static Font endingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
        public static Font bodyFont = FontFactory.GetFont("Arial", 11, Font.NORMAL);

        /// <summary>
        /// InitDocument
        /// </summary>
        /// <param name="pageSize">A4</param>
        /// <param name="isLandscape"></param>
        /// <param name="marginLeft"></param>
        /// <param name="marginRight"></param>
        /// <param name="marginTop"></param>
        /// <param name="marginButtom"></param>
        /// <returns></returns>
        public static Document InitDocument(Rectangle pageSize, bool isLandscape = false, float ? marginLeft = 35, float ? marginRight = 35, float ? marginTop = 35, float ? marginButtom = 35)
        {
            // Create a Document object
            Document document;
            // Set Landscape document
            if (isLandscape)
            {
                document = new Document(pageSize, 20, 20, 35, 35);
                document.SetPageSize(PageSize.A4.Rotate());
            }
            else
            {
                document = new Document(pageSize, 20, 20, 35, 35);
            }
                            

            return document;
        }

        /// <summary>
        /// InitHeaderDocument
        /// </summary>
        /// <param name="document"></param>
        /// <param name="header">Add the header to document</param>
        /// <param name="subHeader">Add the sub header to document</param>
        /// <param name="titlePdf">Add Title to the PDF file at the top</param>
        public static void InitHeaderDocument(Document document, string header, string subHeader, string titlePdf)
        {
            try
            {
                //Add Title {titlePdf} to the PDF file at the top
                var title = new PdfPTable(1);
                title.AddCell(new PdfPCell(new Phrase(titlePdf, new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(153, 51, 0))))
                {
                    Colspan = 1,
                    Border = 0,
                    PaddingTop = -16,
                    PaddingBottom = 8,
                    BorderWidthBottom = 2,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });

                // Init table percentage 100%
                title.WidthPercentage = 100;

                document.Add(title);
                //document.Add(new Paragraph(3, Chunk.NEWLINE) { Alignment = 1 });

                // Add the {header} to document
                document.Add(new Paragraph(header, titleFont));

                // Now add the {sub header} to document
                document.Add(new Paragraph(subHeader, subTitle));

                //document.Add(Chunk.NEWLINE);
                document.Add(new Paragraph(5, Chunk.NEWLINE) { Alignment = 1 });
            }
            catch (Exception ex)
            {
            }
        }

        public static void InitHeaderDocument(Document document, string header, string subHeader)
        {
            try
            {
                // Add the {header} to document
                document.Add(new Paragraph(header, titleFont));

                // Now add the {sub header} to document
                document.Add(new Paragraph(subHeader, bodyFont));

                //document.Add(Chunk.NEWLINE);
                document.Add(new Paragraph(16, Chunk.NEWLINE) { Alignment = 1 });

            }
            catch (Exception ex)
            {
            }
        }

        public static void InitLogoDocument(Document document, Image logo)
        {
            try
            {
                //Re-ScaleAbsolute
                if (document.PageSize == PageSize.A4.Rotate())
                {
                    logo.ScaleAbsolute(185, 40);
                }
                else
                {
                    logo.ScaleAbsolute(160, 30);
                }
               
                // Set SetAbsolutePosition Img document
                logo.SetAbsolutePosition(document.Right - (logo.ScaledWidth), document.Top - document.TopMargin);
                
                // Add logo to document
                document.Add(logo);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Method to add single cell to the header
        /// </summary>
        /// <param name="tableLayout"></param>
        /// <param name="cellText"></param>
        /// <param name="align">Element.ALIGN_CENTER</param>
        /// <param name="colspan"></param>
        public static void AddCellToHeader(PdfPTable tableLayout, string cellText, int colspan, int align = Element.ALIGN_CENTER)
        {
            tableLayout.AddCell(
                new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, BaseColor.WHITE)))
                {
                    HorizontalAlignment = align,
                    Colspan = colspan,
                    Padding = 5,
                    BackgroundColor = new BaseColor(0, 51, 102),
                });
        }

        /// <summary>
        /// Method to add single cell to the header
        /// </summary>
        /// <param name="tableLayout"></param>
        /// <param name="cellText"></param>
        /// <param name="align">Element.ALIGN_CENTER</param>
        public static void AddCellToHeader(PdfPTable tableLayout, string cellText, int align = Element.ALIGN_CENTER, int fontSize = 8, int style = 1)
        {
            tableLayout.AddCell(
                new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, fontSize, style, BaseColor.WHITE)))
                {
                    HorizontalAlignment = align,
                    Padding = 5,
                    BackgroundColor = new BaseColor(0, 51, 102),
                });
        }

        

        /// <summary>
        ///  Method to add single cell to the body
        ///  public Font(Font.FontFamily family, float size, int style, BaseColor color);
        /// </summary>
        /// <param name="tableLayout"></param>
        /// <param name="cellText"></param>
        /// <param name="fontSytle"></param>
        /// <param name="align">example Element.ALIGN_CENTER</param>
        public static void AddCellToBody(PdfPTable tableLayout, string cellText, int fontSytle = Font.NORMAL, int align = Element.ALIGN_CENTER, int fontSize = 8)
        {
            //public Font(Font.FontFamily family, float size, int style, BaseColor color);
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, fontSize, fontSytle, BaseColor.DARK_GRAY)))
            {
                HorizontalAlignment = align,
                Padding = 5,
                BackgroundColor = BaseColor.WHITE
            });
        }

        public static void AddCellToBodyBorderNone(PdfPTable tableLayout, string cellText, int fontSytle = Font.NORMAL, int align = Element.ALIGN_CENTER)
        {
            //public Font(Font.FontFamily family, float size, int style, BaseColor color);
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, fontSytle, BaseColor.DARK_GRAY)))
            {
                HorizontalAlignment = align,
                Padding = 5,
                Border = 0,
                BackgroundColor = BaseColor.WHITE
            });
        }

        public static void AddCellToBody(PdfPTable tableLayout, string cellText, BaseColor baseColor, int fontSytle = Font.NORMAL, int align = Element.ALIGN_CENTER)
        {
            //public Font(Font.FontFamily family, float size, int style, BaseColor color);
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, fontSytle, baseColor)))
            {
                HorizontalAlignment = align,
                Padding = 5,
                BackgroundColor = BaseColor.WHITE
            });
        }

        public static void AddCellToBodyBorderNone(PdfPTable tableLayout, string cellText, BaseColor baseColor, int fontSytle = Font.NORMAL, int align = Element.ALIGN_CENTER)
        {
            //public Font(Font.FontFamily family, float size, int style, BaseColor color);
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, fontSytle, baseColor)))
            {
                HorizontalAlignment = align,
                Padding = 5,
                Border= 0,
                BackgroundColor = BaseColor.WHITE
            });
        }

        public static void AddCellToBodyBorderRighNone(PdfPTable tableLayout, string cellText, int fontSytle = Font.NORMAL, int align = Element.ALIGN_CENTER)
        {
            //public Font(Font.FontFamily family, float size, int style, BaseColor color);
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, fontSytle, BaseColor.DARK_GRAY)))
            {
                HorizontalAlignment = align,
                Padding = 5,
                BackgroundColor = BaseColor.WHITE,
                BorderWidthRight = 0

            });
        }

        /// <summary>
        ///  Method to add single cell to the body
        ///  public Font(Font.FontFamily family, float size, int style, BaseColor color);
        /// </summary>
        /// <param name="tableLayout"></param>
        /// <param name="pdfTableBody"></param>
        public static void AddCellToBody(PdfPTable tableLayout, PdfPTable pdfTableBody)
        {
            //public Font(Font.FontFamily family, float size, int style, BaseColor color);
            tableLayout.AddCell(new PdfPTable(pdfTableBody){HorizontalAlignment = Element.ALIGN_RIGHT});
        }


        public static byte[] AddPageNumber(Document document, byte[] bytes,string text)
        {
            float x = document.Right;
            float y = document.Bottom + document.RightMargin - document.BottomMargin;

            //Font blackFont = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);
            using (var stream = new MemoryStream())
            {
                var reader = new PdfReader(bytes);
                using (var stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_LEFT, new Phrase("Copyright © 2015 TheRestaurantExpert.com. Generated by Smart System Pro All Rights Reserved.", subTitle), document.Left, y, 0);
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page " + i, subTitle), x, y, 0);

                        if (i>1)
                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_LEFT, new Phrase(text, subTitle), document.Left, document.PageSize.Height - 30, 0);
                    }
                }
                bytes = stream.ToArray();
            }
            return bytes;
        }

        public static byte[] AddPageNumber(Document document, byte[] bytes)
        {
            float x = document.Right;
            float y = document.Bottom + document.RightMargin - document.BottomMargin;

            //Font blackFont = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);
            using (var stream = new MemoryStream())
            {
                var reader = new PdfReader(bytes);
                using (var stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_LEFT, new Phrase("Copyright © 2015 TheRestaurantExpert.com. Generated by Smart System Pro All Rights Reserved.", subTitle), document.Left, y, 0);
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page " + i, titleFont), x, y, 0);
                    }
                }
                bytes = stream.ToArray();
            }
            return bytes;
        }

        public static byte[] AddPageNumber(byte[] bytes, float x = 568f, float y = 15f)
        {
            Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
            using (var stream = new MemoryStream())
            {
                var reader = new PdfReader(bytes);
                using (var stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Copyright © 2015 TheRestaurantExpert.com. All Rights Reserved."), 390f, y, 0);
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page " + i, blackFont), x, y, 0);
                    }
                }
                bytes = stream.ToArray();
            }
            return bytes;
        }

    }

    public partial class Pdf
    {

        
    }
}
