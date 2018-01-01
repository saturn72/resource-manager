namespace LabManager.Services
{
    public sealed class CacheKeyFormats
    {
        #region Resource

        private const string ResourcePrefix = "resource.";
        public const string ResourceById = ResourcePrefix + "id-{0}";

        #endregion
    }
}
