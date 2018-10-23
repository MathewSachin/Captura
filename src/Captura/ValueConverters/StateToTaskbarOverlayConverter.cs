using Captura.Models;
using System;
using System.Globalization;

namespace Captura
{
    public class StateToTaskbarOverlayConverter : OneWayConverter
    {
        public override object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            switch(Value)
            {
                case RecorderState.Recording:
                    return "/Images/record.ico";

                case RecorderState.Paused:
                    return "/Images/pause.ico";

                default:
                    return null;
            }
        }
    }
}