namespace RentalAttireBackend.Application.Clothes.Helper
{
    public static class ClotheCodeGenerator
    {
        public static string Generate(string categoryName, int clotheId)
        {
            var prefix = categoryName.Length >= 3
                ? categoryName[..3].ToUpper()
                : categoryName.ToUpper();

            return $"{prefix}-{clotheId:D4}";
        }
    }
}
