using Captura.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Captura
{
    public class StateToRecordButtonGeometryConverter : OneWayConverter
    {
        public override object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var app = Application.Current;

            if (Value is RecorderState state)
                return state == RecorderState.NotRecording
                    ? app.Resources["IconRecord"]
                    : app.Resources["IconStop"];

            return Binding.DoNothing;
        }
    }
}