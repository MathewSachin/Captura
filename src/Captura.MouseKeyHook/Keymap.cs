// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Keymap
    {
        // 0 to 9
        public string[] Numbers { get; set; } =
        {
            "0",
            "1", "2", "3",
            "4", "5", "6",
            "7", "8", "9"
        };

        // Shift + (0 to 9)
        public string[] SpecialDPad { get; set; } =
        {
            ")",
            "!", "@", "#",
            "$", "%", "^",
            "&", "*", "("
        };

        // a to z
        public string[] Lowercase { get; set; } =
        {
            "a", "b", "c", "d",
            "e", "f", "g", "h",
            "i", "j", "k", "l",
            "m", "n", "o", "p",
            "q", "r", "s", "t",
            "u", "v", "w",
            "x", "y", "z"
        };

        // A to Z
        public string[] Uppercase { get; set; } =
        {
            "A", "B", "C", "D",
            "E", "F", "G", "H",
            "I", "J", "K", "L",
            "M", "N", "O", "P",
            "Q", "R", "S", "T",
            "U", "V", "W",
            "X", "Y", "Z"
        };

        public string Control { get; set; } = "Ctrl";

        public string Shift { get; set; } = nameof(Shift);

        public string Alt { get; set; } = nameof(Alt);

        public string Windows { get; set; } = "Win";
    }
}