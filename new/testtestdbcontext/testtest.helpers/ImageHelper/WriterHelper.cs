using System;
using System.Linq;
using System.Text;

namespace testtest.Helpers
{
    public class WriterHelper
    {
        public enum ImageFormat
        {
            bmp,
            jpeg,
            gif,
            tiff,
            png,
            unknown
        }
        public enum DocumentFormat
        {
            doc,
            docx,
            pdf,
            unknown
        }
        public static ImageFormat GetImageFormat(byte[] bytes)
        {
            var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
            var gif = Encoding.ASCII.GetBytes("GIF");    // GIF
            var png = new byte[] { 137, 80, 78, 71 };              // PNG
            var tiff = new byte[] { 73, 73, 42 };                  // TIFF
            var tiff2 = new byte[] { 77, 77, 42 };                 // TIFF
            var jpeg = new byte[] { 255, 216, 255, 224 };          // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 };         // jpeg canon

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return ImageFormat.bmp;

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return ImageFormat.gif;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return ImageFormat.png;

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                return ImageFormat.tiff;

            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return ImageFormat.tiff;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return ImageFormat.jpeg;

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return ImageFormat.jpeg;

            return ImageFormat.unknown;
        }
        public static DocumentFormat GetDocumentFormat(byte[] bytes)
        {

            var DOC =new byte[] { 208, 207, 17, 224, 161, 177, 26, 225 };
            var DOCX = new byte[] { 80, 75, 3, 4 };
            var PDF = new byte[] { 37, 80, 68, 70, 45, 49, 46 };

            if (DOC.SequenceEqual(bytes.Take(DOC.Length)))
                return DocumentFormat.doc;

            if (DOCX.SequenceEqual(bytes.Take(DOCX.Length)))
                return DocumentFormat.docx;

            if (PDF.SequenceEqual(bytes.Take(PDF.Length)))
                return DocumentFormat.pdf;           

            return DocumentFormat.unknown;
        }

        public static string GetFileExtension(byte[] bytes)
        {
            var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
            var gif = Encoding.ASCII.GetBytes("GIF");    // GIF
            var png = new byte[] { 137, 80, 78, 71 };              // PNG
            var tiff = new byte[] { 73, 73, 42 };                  // TIFF
            var tiff2 = new byte[] { 77, 77, 42 };                 // TIFF
            var jpeg = new byte[] { 255, 216, 255, 224 };          // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 };         // jpeg canon
            var DOC = new byte[] { 208, 207, 17, 224, 161, 177, 26, 225 };
            var DOCX = new byte[] { 80, 75, 3, 4 };
            var PDF = new byte[] { 37, 80, 68, 70, 45, 49, 46 };

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return ".bmp";

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return ".gif";

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return ".png";

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                return ".tiff";

            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return ".tiff";

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return ".jpg";

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return ".jpeg";

            if (DOC.SequenceEqual(bytes.Take(DOC.Length)))
                return ".doc";

            if (DOCX.SequenceEqual(bytes.Take(DOCX.Length)))
                return ".docx";

            if (PDF.SequenceEqual(bytes.Take(PDF.Length)))
                return ".pdf";

            return "unknown";
        }

    }
}
