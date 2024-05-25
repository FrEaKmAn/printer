using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class Printer
    {
        private string _printerName;
        private int[] _paperSize;
        private int[] _margins;
        private string _printerResolution;

        public Printer(string printerName, int[] paperSize, int[] margins, string printerResolution)
        {
            _printerName = printerName;
            _paperSize = paperSize;
            _margins = margins;
            _printerResolution = printerResolution;
        }

        public bool Print(string data)
        {
            var bytes = Convert.FromBase64String(data);
            using (var stream = new MemoryStream(bytes))
            {
                if (stream == null)
                {
                    return false;
                }

                // create the printer settings for our printer
                var printerSettings = new PrinterSettings
                {
                    PrinterName = _printerName,
                    Copies = 1,
                    Duplex = Duplex.Simplex // single sided printing
                };

                // https://github.com/pvginkel/PdfiumViewer/issues/27

                var paperSize = new PaperSize("label", _paperSize[0], _paperSize[1]);
                paperSize.RawKind = (int)PaperKind.Custom;

                // create our page settings for the paper size selected
                var pageSettings = new PageSettings(printerSettings)
                {
                    Margins = new Margins(this._margins[0], this._margins[1], this._margins[2], this._margins[3]),
                };

                if (this._printerResolution != null)
                {
                    var printerResolution = new PrinterResolution();
                    switch (this._printerResolution)
                    {
                        case "MEDIUM":
                            printerResolution.Kind = PrinterResolutionKind.Medium;
                            break;
                        case "HIGH":
                            printerResolution.Kind = PrinterResolutionKind.High;
                            break;
                        case "LOW":
                            printerResolution.Kind = PrinterResolutionKind.Low;
                            break;
                        case "CUSTOM":
                            printerResolution.Kind = PrinterResolutionKind.Custom;
                            printerResolution.X = 600;
                            printerResolution.Y = 600;
                            break;
                    }

                    pageSettings.PrinterResolution = printerResolution;
                }

                // pageSettings.PaperSize = paperSize;
                pageSettings.PaperSize.RawKind = (int)PaperKind.A4;

                // now print the PDF document
                using (var document = PdfiumViewer.PdfDocument.Load(stream))
                {
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        printDocument.PrinterSettings = printerSettings;
                        printDocument.DefaultPageSettings = pageSettings;
                        printDocument.PrintController = new StandardPrintController();
                        printDocument.Print();
                    }
                }

                return true;
            }
        }
    }
}
