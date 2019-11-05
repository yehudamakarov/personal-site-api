using Core.Enums;

namespace Core.Results
{
    public class UpdateTagCountsResult
    {
        public TagCountUpdates UpdateType { get; set; }
        public int AmountChanged { get; set; }
    }
}