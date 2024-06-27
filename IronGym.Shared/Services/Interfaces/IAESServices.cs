namespace IronGym.Shared.Services.Interfaces
{
    public interface IAESService
    {
        string EncryptAES(string plainText);
        string DecryptAES(string cipherText);
    }

}
