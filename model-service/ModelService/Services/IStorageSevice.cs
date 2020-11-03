namespace ModelService.Services
{
    public interface IStorageSevice
    {
        public byte[] GetBlobFile(string filePath);
    }
}