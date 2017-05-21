namespace Captura.Models
{
    public class RecentItemModel
    {
        public string FilePath { get; set; }

        public RecentItemType ItemType { get; set; }

        public RecentItemModel(string FilePath, RecentItemType ItemType)
        {
            this.FilePath = FilePath;
            this.ItemType = ItemType;
        }

        // Default constructor required by Settings
        public RecentItemModel() { }
    }
}