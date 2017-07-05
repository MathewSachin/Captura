using System;

namespace Screna
{
    /// <summary>
    /// End Event Args.
    /// </summary>
    public class EndEventArgs : EventArgs
    {
        /// <summary>
        /// Contains the Exception that occured... null if no error occured.
        /// </summary>
        public Exception Error { get; }

        /// <summary>
        /// Creates a new instance of <see cref="EndEventArgs"/>.
        /// </summary>
        /// <param name="Error">The <see cref="Exception"/> that occured.</param>
        public EndEventArgs(Exception Error)
        {
            this.Error = Error;
        }
    }
}