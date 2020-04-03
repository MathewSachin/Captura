using System;
using System.Drawing;

namespace Captura.MouseKeyHook.Steps
{
    abstract class KeyModifiedStep : IRecordStep
    {
        readonly KeystrokesSettings _keystrokesSettings;
        readonly ModifierStates _modifierStates;
        readonly KeymapViewModel _keymap;

        public KeyModifiedStep(KeystrokesSettings Settings,
            KeymapViewModel Keymap)
        {
            _keymap = Keymap;
            _keystrokesSettings = Settings;
            _modifierStates = ModifierStates.GetCurrent();
        }

        public virtual void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform)
        {
            KeyStep.DrawString(Editor, _modifierStates.ToString(_keymap), _keystrokesSettings);
        }

        public virtual bool Merge(IRecordStep NextStep)
        {
            switch (NextStep)
            {
                case KeyStep keyStep:
                    if (_modifierStates.Control && keyStep.Text == _keymap.Control)
                        return true;

                    if (_modifierStates.Shift && keyStep.Text == _keymap.Shift)
                        return true;

                    if (_modifierStates.Alt && keyStep.Text == _keymap.Alt)
                        return true;
                    break;
            }

            return false;
        }
    }
}
