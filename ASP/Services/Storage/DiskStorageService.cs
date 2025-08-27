using Microsoft.AspNetCore.Http;
using System;

namespace ASP.Services.Storage
{
    public class DiskStorageService : IStorageService
    {
        private const string basePath = @"D:/C#/ASP/storage/";
        public byte[] GetItemBytes(string itemName)
        {
            string path = Path.Combine(basePath, itemName);
            if (System.IO.File.Exists(path)) return System.IO.File.ReadAllBytes(path);
            else throw new FileNotFoundException();
        }

        public string TryGetMimeType(string itemName)
        {
            string ext = GetFileExtension(itemName);
            return ext switch 
            {
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".bmp" => "image/bmp",
                _ => throw new ArgumentException($"Unsupported exception {ext}")
            };
        }

        public string SaveItem(IFormFile formFile)
        {
            // Getting the extension
            string extension = GetFileExtension(formFile.FileName);

            // Generating new file name
            string savedName = Guid.NewGuid() + extension;
            string path = Path.Combine(basePath, savedName);

            // Opening stream to save (do not forget to auto-close)
            using Stream stream = new StreamWriter(path).BaseStream;
            formFile.CopyTo(stream);

            return savedName;
        }

        public async Task<string> SaveItemAsync(IFormFile formFile)
        {
            string extension = GetFileExtension(formFile.FileName);

            string savedName = Guid.NewGuid() + extension;
            string path = Path.Combine(basePath, savedName);

            using Stream stream = new StreamWriter(path).BaseStream;
            await formFile.CopyToAsync(stream);

            return savedName;
        }

        private string GetFileExtension(string fileName)
        {
            int dot = fileName.LastIndexOf('.');
            if (dot < 0) throw new ArgumentException("FIle name MUST have an extension");
            return fileName[dot..];
        }
    }
}
