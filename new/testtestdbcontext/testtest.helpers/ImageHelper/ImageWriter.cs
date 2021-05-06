using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace testtest.Helpers
{
    public class ImageWriter : IImageWriter
    {
        public async Task<string> UploadImage(IFormFile file)
        {
            if (CheckIfImageFile(file))
            {
                return await WriteFile(file);
            }
            else if (CheckIfDocumentFile(file))
            {
                return await WriteFile(file);
            }
            return "Invalid";
        }
        public async Task<string> UploadImage(byte[] file)
        {
            if (CheckIfImageByteFile(file))
            {
                return await WriteFileByte(file);
            }
            else if (CheckIfImageByteFile(file))
            {
                return await WriteFileByte(file);
            }
            return "Invalid";
        }

      
        private bool CheckIfImageByteFile(byte[] file)
        {
            return WriterHelper.GetImageFormat(file) != WriterHelper.ImageFormat.unknown;
        }
        private bool CheckIfDocumentByteFile(byte[] document)
        {
            return WriterHelper.GetDocumentFormat(document) != WriterHelper.DocumentFormat.unknown;
        }
        private bool CheckIfImageFile(IFormFile file)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return WriterHelper.GetImageFormat(fileBytes) != WriterHelper.ImageFormat.unknown;
        }
        private bool CheckIfDocumentFile(IFormFile file)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return WriterHelper.GetDocumentFormat(fileBytes) != WriterHelper.DocumentFormat.unknown;
        }

        public async Task<string> WriteFile(IFormFile file)
        {
            string fileName;
            var folder = "images/";
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                fileName = Guid.NewGuid().ToString() + extension;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                if (extension.ToLower() == ".pdf" 
                    || extension.ToLower() == ".doc" 
                    || extension.ToLower() == ".docx")
                {
                     path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\documents", fileName);
                    folder = "documents/";
                }

                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return fileName;
            //return ApiRoutes.Domain+ folder + fileName;
        }
        public async Task<string> WriteFileByte(byte[] file)
        {

            string fileName;
            var folder = "images/";
            try
            {
                var extension = WriterHelper.GetFileExtension(file);
                var stream = new MemoryStream(file);
                fileName = Guid.NewGuid().ToString() + extension;
                //IFormFile ImageFile = new FormFile(stream, 0, file.Length, "ImageFile", fileName);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                if (extension == ".pdf"
                    || extension == ".doc"
                    || extension == ".docx")
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\documents", fileName);
                    folder = "documents/";
                }

                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await bits.WriteAsync(file, 0, file.Length);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return fileName;
            //return ApiRoutes.Domain + folder + fileName;
        }
        public async Task<bool> DeleteFile(string location)
        {
            //Change When Production
            //var splittingName = location.Replace(ApiRoutes.Domain,"").Split('/');
            var splittingName = location.Replace("https://localhost", "").Split('/');
            
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{splittingName[0]}", splittingName[1]);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
