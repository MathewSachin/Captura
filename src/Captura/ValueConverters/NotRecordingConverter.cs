using Captura.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Captura
{
    public class NotRecordingConverter : OneWayConverter
    {
        public override object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (Value is RecorderState state)
                return state == RecorderState.NotRecording;

            return Binding.DoNothing;
        }
    }
}