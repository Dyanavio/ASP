namespace ASP.Services.Storage
{
    public interface IStorageService
    {
        public abstract byte[] GetItemBytes(string itemName);
        public abstract string TryGetMimeType(string itemName);
        public abstract string SaveItem(IFormFile formFile);
        public abstract Task<string> SaveItemAsync(IFormFile formFile);
    }
}
