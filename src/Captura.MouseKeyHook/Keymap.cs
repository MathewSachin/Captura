namespace Captura.Models
{
    class Keymap
    {
        // 0 to 9
        public char[] Numbers { get; } =
        {
            '0',
            '1', '2', '3',
            '4', '5', '6',
            '7', '8', '9'
        };

        // Shift + (0 to 9)
        public char[] SpecialDPad { get; } =
        {
            ')',
            '!', '@', '#',
            '$', '%', '^',
            '&', '*', '('
        };

        // a to z
        public char[] Lowercase { get; } =
        {
            'a', 'b', 'c', 'd',
            'e', 'f', 'g', 'h',
            'i', 'j', 'k', 'l',
            'm', 'n', 'o', 'p',
            'q', 'r', 's', 't',
            'u', 'v', 'w',
            'x', 'y', 'z'
        };

        // A to Z
        public char[] Uppercase { get; } =
        {
            'A', 'B', 'C', 'D',
            'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L',
            'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T',
            'U', 'V', 'W',
            'X', 'Y', 'Z'
        };

        public string Control { get; } = "Ctrl";

        public string Shift { get; } = nameof(Shift);

        public string Alt { get; } = nameof(Alt);

        public string Windows { get; } = "Win";
    }
}