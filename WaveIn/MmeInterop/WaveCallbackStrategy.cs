using System;
using System.Collections.Generic;
using System.Text;

namespace NWaveIn
{
    /// <summary>
    /// Wave Callback Strategy
    /// </summary>
    public enum WaveCallbackStrategy
    {
        /// <summary>
        /// Use a function
        /// </summary>
        FunctionCallback,
        /// <summary>
        /// Create a new window (should only be done if on GUI thread)
        /// </summary>
        NewWindow,
        /// <summary>
        /// Use an existing window handle
        /// </summary>
        ExistingWindow,
        /// <summary>
        /// Use an event handle
        /// </summary>
        Event,
    }
}
