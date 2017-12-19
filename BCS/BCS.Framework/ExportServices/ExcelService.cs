﻿using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BCS.Framework.ExportServices
{
    /// <summary>
    /// Class to help to build an Excel workbook
    /// </summary>
    /// <remarks>
    /// The following code was written by Mike Wendelius and can be found on Code Project at:
    /// http://www.codeproject.com/Articles/371203/Creating-basic-Excel-workbook-with-Open-XML
    /// </remarks>
    public static partial class Excel
    {
        /// <summary>
        /// Creates the workbook
        /// </summary>
        /// <returns>Spreadsheet created</returns>
        public static SpreadsheetDocument CreateWorkbook(string fileName)
        {
            SpreadsheetDocument spreadSheet = null;
            SharedStringTablePart sharedStringTablePart;
            WorkbookStylesPart workbookStylesPart;

            try
            {
                // Create the Excel workbook
                spreadSheet = SpreadsheetDocument.Create(fileName, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook, false);

                // Create the parts and the corresponding objects

                // Workbook
                spreadSheet.AddWorkbookPart();
                spreadSheet.WorkbookPart.Workbook = new Workbook();
                spreadSheet.WorkbookPart.Workbook.Save();

                // Shared string table
                sharedStringTablePart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
                sharedStringTablePart.SharedStringTable = new SharedStringTable();
                sharedStringTablePart.SharedStringTable.Save();

                // Sheets collection
                spreadSheet.WorkbookPart.Workbook.Sheets = new Sheets();
                spreadSheet.WorkbookPart.Workbook.Save();

                // Stylesheet
                workbookStylesPart = spreadSheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                workbookStylesPart.Stylesheet = new Stylesheet();
                workbookStylesPart.Stylesheet.Save();
            }
            catch (System.Exception exception)
            {
                throw exception;
            }


            return spreadSheet;
        }

        /// <summary>
        /// Adds a new worksheet to the workbook
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="name">Name of the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool AddWorksheet(SpreadsheetDocument spreadsheet, string name)
        {
            Sheets sheets = spreadsheet.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            Sheet sheet;
            WorksheetPart worksheetPart;

            // Add the worksheetpart
            worksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());
            worksheetPart.Worksheet.Save();

            // Add the sheet and make relation to workbook
            sheet = new Sheet()
            {
                Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = (uint)(spreadsheet.WorkbookPart.Workbook.Sheets.Count() + 1),
                Name = name
            };
            sheets.Append(sheet);
            spreadsheet.WorkbookPart.Workbook.Save();

            return true;
        }

        /// <summary>
        /// Adds the basic styles to the workbook
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <returns>True if succesful</returns>
        public static bool AddBasicStyles(SpreadsheetDocument spreadsheet)
        {
            Stylesheet stylesheet = spreadsheet.WorkbookPart.WorkbookStylesPart.Stylesheet;

            // Numbering formats (x:numFmts)
            stylesheet.InsertAt<NumberingFormats>(new NumberingFormats(), 0);

            // Currency
            stylesheet.GetFirstChild<NumberingFormats>().InsertAt<NumberingFormat>(
               new NumberingFormat()
               {
                   NumberFormatId = 164,
                   FormatCode = "#,##0.00"
                   + "\\ \"" + System.Globalization.CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol + "\""
               }, 0);
            stylesheet.GetFirstChild<NumberingFormats>().InsertAt<NumberingFormat>(
               new NumberingFormat()
               {
                   NumberFormatId = 165,
                   FormatCode = "#,##0.00"
               }, 0);
            // Fonts (x:fonts)
            stylesheet.InsertAt<Fonts>(new Fonts(), 1);
            stylesheet.GetFirstChild<Fonts>().InsertAt<Font>(
               new Font()
               {
                   FontSize = new FontSize()
                   {
                       Val = 11
                   },
                   FontName = new FontName()
                   {
                       Val = "Calibri"
                   }
               }, 0);

            // Fills (x:fills)
            stylesheet.InsertAt<Fills>(new Fills(), 2);
            stylesheet.GetFirstChild<Fills>().InsertAt<Fill>(
               new Fill()
               {
                   PatternFill = new PatternFill()
                   {
                       PatternType = new DocumentFormat.OpenXml.EnumValue<PatternValues>()
                       {
                           Value = PatternValues.None
                       }
                   }
               }, 0);
            stylesheet.GetFirstChild<Fills>().InsertAt<Fill>(
               new Fill()
               {
                   PatternFill = new PatternFill()
                   {
                       PatternType = new DocumentFormat.OpenXml.EnumValue<PatternValues>()
                       {
                           Value = PatternValues.Gray125
                       }
                   }
               }, 1);

            // Borders (x:borders)
            stylesheet.InsertAt<Borders>(new Borders(), 3);
            stylesheet.GetFirstChild<Borders>().InsertAt<Border>(
               new Border()
               {
                   LeftBorder = new LeftBorder(),
                   RightBorder = new RightBorder(),
                   TopBorder = new TopBorder(),
                   BottomBorder = new BottomBorder(),
                   DiagonalBorder = new DiagonalBorder()
               }, 0);

            // Cell style formats (x:CellStyleXfs)
            stylesheet.InsertAt<CellStyleFormats>(new CellStyleFormats(), 4);
            stylesheet.GetFirstChild<CellStyleFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   NumberFormatId = 0,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0
               }, 0);

            // Cell formats (x:CellXfs)
            stylesheet.InsertAt<CellFormats>(new CellFormats(), 5);

            // General text
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   FormatId = 0,
                   NumberFormatId = 0
               }, 0);

            // Date
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   ApplyNumberFormat = true,
                   FormatId = 0,
                   NumberFormatId = 22,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0
               },
                  1);

            // Currency
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   ApplyNumberFormat = true,
                   FormatId = 0,
                   NumberFormatId = 164,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0,
                   Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right, Vertical = VerticalAlignmentValues.Center },
                   ApplyAlignment = true
               }, 2);                      

            // Percentage
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   ApplyNumberFormat = true,
                   FormatId = 0,
                   NumberFormatId = 10,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0
               },
                  3);

            stylesheet.Save();

            return true;
        }

        /// <summary>
        /// Adds a list of strings to the shared strings table.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet</param>
        /// <param name="stringList">Strings to add</param>
        /// <returns></returns>
        public static bool AddSharedStrings(SpreadsheetDocument spreadsheet, System.Collections.Generic.List<string> stringList)
        {
            foreach (string item in stringList)
            {
                Excel.AddSharedString(spreadsheet, item, false);
            }
            spreadsheet.WorkbookPart.SharedStringTablePart.SharedStringTable.Save();

            return true;
        }

        /// <summary>
        /// Add a single string to shared strings table.
        /// Shared string table is created if it doesn't exist.
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="stringItem">string to add</param>
        /// <param name="save">Save the shared string table</param>
        /// <returns></returns>
        public static bool AddSharedString(SpreadsheetDocument spreadsheet, string stringItem, bool save = true)
        {
            SharedStringTable sharedStringTable = spreadsheet.WorkbookPart.SharedStringTablePart.SharedStringTable;

            if (0 == sharedStringTable.Where(item => item.InnerText == stringItem).Count())
            {
                sharedStringTable.AppendChild(
                   new SharedStringItem(
                      new Text(stringItem)));

                // Save the changes
                if (save)
                {
                    sharedStringTable.Save();
                }
            }

            return true;
        }
        /// <summary>
        /// Returns the index of a shared string.
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="stringItem">String to search for</param>
        /// <returns>Index of a shared string. -1 if not found</returns>
        public static int IndexOfSharedString(SpreadsheetDocument spreadsheet, string stringItem)
        {
            SharedStringTable sharedStringTable = spreadsheet.WorkbookPart.SharedStringTablePart.SharedStringTable;
            bool found = false;
            int index = 0;

            foreach (SharedStringItem sharedString in sharedStringTable.Elements<SharedStringItem>())
            {
                if (sharedString.InnerText == stringItem)
                {
                    found = true;
                    break; ;
                }
                index++;
            }

            return found ? index : -1;
        }

        /// <summary>
        /// Converts a column number to column name (i.e. A, B, C..., AA, AB...)
        /// </summary>
        /// <param name="columnIndex">Index of the column</param>
        /// <returns>Column name</returns>
        public static string ColumnNameFromIndex(uint columnIndex)
        {
            uint remainder;
            string columnName = "";

            while (columnIndex > 0)
            {
                remainder = (columnIndex - 1) % 26;
                columnName = System.Convert.ToChar(65 + remainder).ToString() + columnName;
                columnIndex = (uint)((columnIndex - remainder) / 26);
            }

            return columnName;
        }

        public static bool SetColumnHeadingValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, string stringValue, bool useSharedString, bool save = true)
        {
            string columnValue = stringValue;
            CellValues cellValueType;

            // Add the shared string if necessary
            if (useSharedString)
            {
                if (Excel.IndexOfSharedString(spreadsheet, stringValue) == -1)
                {
                    Excel.AddSharedString(spreadsheet, stringValue, true);
                }
                columnValue = Excel.IndexOfSharedString(spreadsheet, stringValue).ToString();
                cellValueType = CellValues.SharedString;
            }
            else
            {
                cellValueType = CellValues.String;
            }

            return SetCellValue(spreadsheet, worksheet, columnIndex, 1, cellValueType, columnValue, 4, save);
        }

        /// <summary>
        /// Sets a column heading to a cell
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="stringValue">String value to set</param>
        /// <param name="useSharedString">Use shared strings? If true and the string isn't found in shared strings, it will be added</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetColumnHeadingValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, string stringValue, bool useSharedString, bool save = true)
        {
            string columnValue = stringValue;
            CellValues cellValueType;

            // Add the shared string if necessary
            if (useSharedString)
            {
                if (Excel.IndexOfSharedString(spreadsheet, stringValue) == -1)
                {
                    Excel.AddSharedString(spreadsheet, stringValue, true);
                }
                columnValue = Excel.IndexOfSharedString(spreadsheet, stringValue).ToString();
                cellValueType = CellValues.SharedString;
            }
            else
            {
                cellValueType = CellValues.String;
            }

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, cellValueType, columnValue, 4, save);
        }

        /// <summary>
        /// Sets a string value to a cell
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="stringValue">String value to set</param>
        /// <param name="useSharedString">Use shared strings? If true and the string isn't found in shared strings, it will be added</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, string stringValue, bool useSharedString, bool save = true, uint? styleIndex = null)
        {
            string columnValue = stringValue;
            CellValues cellValueType;

            // Add the shared string if necessary
            if (useSharedString)
            {
                if (Excel.IndexOfSharedString(spreadsheet, stringValue) == -1)
                {
                    Excel.AddSharedString(spreadsheet, stringValue, true);
                }
                columnValue = Excel.IndexOfSharedString(spreadsheet, stringValue).ToString();
                cellValueType = CellValues.SharedString;
            }
            else
            {
                cellValueType = CellValues.String;
            }

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, cellValueType, columnValue, styleIndex, save);
        }

        /// <summary>
        /// Sets a cell value with a date
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="datetimeValue">DateTime value</param>
        /// <param name="styleIndex">Style to use</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, System.DateTime datetimeValue, uint? styleIndex, bool save = true)
        {
#if EN_US_CULTURE
         string columnValue = datetimeValue.ToOADate().ToString();
#else
            string columnValue = datetimeValue.ToOADate().ToString().Replace(",", ".");
#endif

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, CellValues.Date, columnValue, styleIndex, save);
        }

        /// <summary>
        /// Sets a cell value with double number
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="percentValue"></param>
        /// <param name="styleIndex">Style to use</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, string percentValue, uint? styleIndex, bool save = true)
        {
#if EN_US_CULTURE
         string columnValue = doubleValue.ToString();
#else
            string columnValue = percentValue.Replace(",", ".");
#endif

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, CellValues.Number, columnValue, styleIndex, save);
        }

        /// <summary>
        /// Sets a cell value with double number
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="doubleValue">Double value</param>
        /// <param name="styleIndex">Style to use</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, double doubleValue, uint? styleIndex, bool save = true)
        {
#if EN_US_CULTURE
         string columnValue = doubleValue.ToString();
#else
            string columnValue = doubleValue.ToString().Replace(",", ".");
#endif

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, CellValues.Number, columnValue, styleIndex, save);
        }

        /// <summary>
        /// Sets a cell value with boolean value
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="boolValue">Boolean value</param>
        /// <param name="styleIndex">Style to use</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, bool boolValue, uint? styleIndex, bool save = true)
        {
            string columnValue = boolValue ? "1" : "0";

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, CellValues.Boolean, columnValue, styleIndex, save);
        }

        /// <summary>
        /// Sets the column width
        /// </summary>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="width">Width to set</param>
        /// <returns>True if succesful</returns>
        public static bool SetColumnWidth(Worksheet worksheet, int columnIndex, int width)
        {
            Columns columns;
            Column column;

            // Get the column collection exists
            columns = worksheet.Elements<Columns>().FirstOrDefault();
            if (columns == null)
            {
                return false;
            }
            // Get the column
            column = columns.Elements<Column>().Where(item => item.Min == columnIndex).FirstOrDefault();
            if (columns == null)
            {
                return false;
            }
            column.Width = width;
            column.CustomWidth = true;

            worksheet.Save();

            return true;
        }

        /// <summary>
        /// Sets a cell value. The row and the cell are created if they do not exist. If the cell exists, the contents of the cell is overwritten
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="valueType">Type of the value</param>
        /// <param name="value">The actual value</param>
        /// <param name="styleIndex">Index of the style to use. Null if no style is to be defined</param>
        /// <param name="save">Save the worksheet?</param>
        /// <returns>True if succesful</returns>
        private static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, CellValues valueType, string value, uint? styleIndex, bool save = true)
        {
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            Row row;
            Row previousRow = null;
            Cell cell;
            Cell previousCell = null;
            Columns columns;
            Column previousColumn = null;
            string cellAddress = Excel.ColumnNameFromIndex(columnIndex) + rowIndex;

            // Check if the row exists, create if necessary
            if (sheetData.Elements<Row>().Where(item => item.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(item => item.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                //sheetData.Append(row);
                for (uint counter = rowIndex - 1; counter > 0; counter--)
                {
                    previousRow = sheetData.Elements<Row>().Where(item => item.RowIndex == counter).FirstOrDefault();
                    if (previousRow != null)
                    {
                        break;
                    }
                }
                sheetData.InsertAfter(row, previousRow);
            }

            // Check if the cell exists, create if necessary
            if (row.Elements<Cell>().Where(item => item.CellReference.Value == cellAddress).Count() > 0)
            {
                cell = row.Elements<Cell>().Where(item => item.CellReference.Value == cellAddress).First();
            }
            else
            {
                // Find the previous existing cell in the row
                for (uint counter = columnIndex - 1; counter > 0; counter--)
                {
                    previousCell = row.Elements<Cell>().Where(item => item.CellReference.Value == Excel.ColumnNameFromIndex(counter) + rowIndex).FirstOrDefault();
                    if (previousCell != null)
                    {
                        break;
                    }
                }
                cell = new Cell() { CellReference = cellAddress };
                row.InsertAfter(cell, previousCell);
            }

            // Check if the column collection exists
            columns = worksheet.Elements<Columns>().FirstOrDefault();
            if (columns == null)
            {
                columns = worksheet.InsertAt(new Columns(), 0);
            }
            // Check if the column exists
            if (columns.Elements<Column>().Where(item => item.Min == columnIndex).Count() == 0)
            {
                // Find the previous existing column in the columns
                for (uint counter = columnIndex - 1; counter > 0; counter--)
                {
                    previousColumn = columns.Elements<Column>().Where(item => item.Min == counter).FirstOrDefault();
                    if (previousColumn != null)
                    {
                        break;
                    }
                }
                columns.InsertAfter(
                   new Column()
                   {
                       Min = columnIndex,
                       Max = columnIndex,
                       CustomWidth = true,
                       Width = 9
                   }, previousColumn);
            }

            // Add the value
            cell.CellValue = new CellValue(value);
            if (styleIndex != null)
            {
                cell.StyleIndex = styleIndex;
            }
            if (valueType != CellValues.Date)
            {
                cell.DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(valueType);
            }

            if (save)
            {
                worksheet.Save();
            }

            return true;
        }

        /// <summary>
        /// Adds a predefined style from the given xml
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="xml">Style definition as xml</param>
        /// <returns>True if succesful</returns>
        public static bool AddPredefinedStyles(SpreadsheetDocument spreadsheet, string xml)
        {
            spreadsheet.WorkbookPart.WorkbookStylesPart.Stylesheet.InnerXml = xml;
            spreadsheet.WorkbookPart.WorkbookStylesPart.Stylesheet.Save();

            return true;
        }
    }

    /// <summary>
    /// Class to help to build an Excel workbook
    /// </summary>
    /// <remarks>
    /// The following code was written by John DeVight, reference here:
    /// http://www.kendoui.com/blogs/teamblog/posts/13-03-12/exporting_the_kendo_ui_grid_data_to_excel.aspx
    /// </remarks>
    public static partial class Excel
    {
        /// <summary>
        /// Creates the workbook in memory.
        /// </summary>
        /// <returns>Spreadsheet created</returns>
        public static SpreadsheetDocument CreateWorkbook(Stream stream)
        {
            SpreadsheetDocument spreadSheet = null;
            SharedStringTablePart sharedStringTablePart;
            WorkbookStylesPart workbookStylesPart;

            // Create the Excel workbook
            spreadSheet = SpreadsheetDocument.Create(stream, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook, false);

            // Create the parts and the corresponding objects

            // Workbook
            spreadSheet.AddWorkbookPart();
            spreadSheet.WorkbookPart.Workbook = new Workbook();
            spreadSheet.WorkbookPart.Workbook.Save();

            // Shared string table
            sharedStringTablePart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
            sharedStringTablePart.SharedStringTable = new SharedStringTable();
            sharedStringTablePart.SharedStringTable.Save();

            // Sheets collection
            spreadSheet.WorkbookPart.Workbook.Sheets = new Sheets();
            spreadSheet.WorkbookPart.Workbook.Save();

            // Stylesheet
            workbookStylesPart = spreadSheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            workbookStylesPart.Stylesheet = new Stylesheet();
            workbookStylesPart.Stylesheet.Save();

            return spreadSheet;
        }

        /// <summary>
        /// Adds additional styles to the workbook
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <returns>True if succesful</returns>
        public static bool AddAdditionalStyles(SpreadsheetDocument spreadsheet)
        {
            Stylesheet stylesheet = spreadsheet.WorkbookPart.WorkbookStylesPart.Stylesheet;

            // Additional Font for Column Heder.
            stylesheet.GetFirstChild<Fonts>().InsertAt<Font>(
               new Font()
               {
                   FontSize = new FontSize()
                   {
                       Val = 12
                   },
                   FontName = new FontName()
                   {
                       Val = "Calibri"
                   },
                   Bold = new Bold()
                   {
                       Val = true
                   }
               }, 1);

            // Additional Fill for Column Header.
            stylesheet.GetFirstChild<Fills>().InsertAt<Fill>(
               new Fill()
               {
                   PatternFill = new PatternFill()
                   {
                       PatternType = new DocumentFormat.OpenXml.EnumValue<PatternValues>()
                       {
                           Value = PatternValues.Solid
                       },
                       BackgroundColor = new BackgroundColor
                       {
                           Indexed = 64U
                       },
                       ForegroundColor = new ForegroundColor
                       {
                           Rgb = "F2F2F2"
                       }
                   }
               }, 2);

            // Column Header
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   FormatId = 0,
                   NumberFormatId = 0,
                   FontId = 1,
                   FillId = 2,
                   ApplyFill = true,
                   ApplyFont = true
               }, 4);

            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
                new CellFormat()
                {
                    FormatId = 0,
                    NumberFormatId = 0,
                    FontId = 1,
                    ApplyFont = true
                }, 5);

            // Currency bold
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   ApplyNumberFormat = true,
                   FormatId = 0,
                   NumberFormatId = 164,
                   FontId = 1,
                   ApplyFont = true,
                   Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right, Vertical = VerticalAlignmentValues.Center },
                   ApplyAlignment = true
               }, 6);

            //double
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
              new CellFormat()
              {
                  ApplyNumberFormat = true,
                  FormatId = 0,
                  NumberFormatId = 165,
                  FontId = 0,
                  FillId = 0,
                  BorderId = 0,
                  Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right, Vertical = VerticalAlignmentValues.Center },
                  ApplyAlignment = true
              }, 7);

            //double
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
              new CellFormat()
              {
                  ApplyNumberFormat = true,
                  FormatId = 0,
                  NumberFormatId = 165,
                  FontId = 1,
                  ApplyFont = true,
                  Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right, Vertical = VerticalAlignmentValues.Center },
                  ApplyAlignment = true
              }, 8);

            stylesheet.Save();

            return true;
        }

        /// <summary>
        /// Freeze the top row of the Excel Grid.  (Doesn't work yet)
        /// </summary>
        /// <remarks>
        /// http://justgeeks.blogspot.com/2012/02/freeze-top-row.html
        /// </remarks>
        /// <param name="spreadsheet"></param>
        public static void FreezeTopRow(SpreadsheetDocument spreadsheet)
        {
            WorkbookPart workbookPart = spreadsheet.WorkbookPart;
            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
            SheetViews sheetviews = worksheetPart.Worksheet.GetFirstChild<SheetViews>();
            SheetView sheetview = sheetviews.GetFirstChild<SheetView>();
            Selection selection = sheetview.GetFirstChild<Selection>();
            Pane pane = new Pane()
            {
                VerticalSplit = 1D,
                TopLeftCell = "A2",
                ActivePane = PaneValues.BottomLeft,
                State = PaneStateValues.Frozen
            };
            sheetview.InsertBefore(pane, selection);
            selection.Pane = PaneValues.BottomLeft;
        }
    }
}
