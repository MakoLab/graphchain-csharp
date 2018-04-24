namespace elephant.core.service.cryptography
{
    public interface IHashCalculator
    {
        int HashSize { get; }
        string CalculateHash(string input);
        byte[] CalculateHashAsBytes(string input);

        string CombineOrdered(string[] tup);

        string CombineUnordered(string[] tup);
    }
}
