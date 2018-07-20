namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModifierStates
    {
        public static ModifierStates Empty { get; } = new ModifierStates();

        public bool Control { get; set; }

        public bool Shift { get; set; }

        public bool Alt { get; set; }

        public bool CapsLock { get; set; }
    }
}