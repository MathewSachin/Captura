namespace Captura
{
    public partial class FileNameFormatWindow
    {
        public FileNameFormatWindow()
        {
            InitializeComponent();
        }

        public FileNameFormatGroup[] FormatGroups { get; } =
        {
            new FileNameFormatGroup("Year", new []
            {
                new FileNameFormatItem("%yyyy%", "Year (2018)"), 
                new FileNameFormatItem("%yy%", "Year (18)") 
            }),
            new FileNameFormatGroup("Month", new []
            {
                new FileNameFormatItem("%MMMM%", "Month (September)"), 
                new FileNameFormatItem("%MMM%", "Month (Sep)"), 
                new FileNameFormatItem("%MM%", "Month (09)")
            }),
            new FileNameFormatGroup("Date", new []
            {
                new FileNameFormatItem("%dd%", "Date (22)"), 
                new FileNameFormatItem("%ddd%", "Day (Tue)"), 
                new FileNameFormatItem("%dddd%", "Day (Tuesday)") 
            }),
            new FileNameFormatGroup("Time", new []
            {
                new FileNameFormatItem("%HH%", "Hours (24 hr) (20)"), 
                new FileNameFormatItem("%hh%", "Hours (12 hr) (08)"), 
                new FileNameFormatItem("%mm%", "Minutes (58)"), 
                new FileNameFormatItem("%ss%", "Seconds (54)"), 
                new FileNameFormatItem("%tt%", "AM / PM"),
                new FileNameFormatItem("%zzz%", "Time Zone (+05:30)")
            }) 
        };
    }

    public class FileNameFormatGroup
    {
        public FileNameFormatGroup(string Name, FileNameFormatItem[] Formats)
        {
            this.Name = Name;
            this.Formats = Formats;
        }

        public string Name { get; }

        public FileNameFormatItem[] Formats { get; }
    }

    public class FileNameFormatItem
    {
        public FileNameFormatItem(string Format, string Description)
        {
            this.Format = Format;
            this.Description = Description;
        }

        public string Format { get; }

        public string Description { get; }
    }
}
