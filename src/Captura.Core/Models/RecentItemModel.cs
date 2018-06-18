// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Captura.Models
{
    public class RecentItemModel
    {
        public string FilePath { get; set; }

        public RecentItemType ItemType { get; set; }

        public string DeleteHash { get; set; }

        public RecentItemModel(string FilePath, RecentItemType ItemType, string DeleteHash)
        {
            this.FilePath = FilePath;
            this.ItemType = ItemType;
            this.DeleteHash = DeleteHash;
        }

        // Default constructor required by Settings
        public RecentItemModel() { }
    }
}