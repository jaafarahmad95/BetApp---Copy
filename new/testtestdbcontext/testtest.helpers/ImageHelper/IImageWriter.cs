using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace testtest.Helpers
{
    public interface IImageWriter
    {
        Task<string> UploadImage(IFormFile file);
        Task<string> UploadImage(byte[] file);
        Task<string> WriteFileByte(byte[] file);
        Task<bool> DeleteFile(string location);
    }
}
