using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;

namespace Captura.Webcam
{
    /// <summary>
    ///  Represents a DirectShow filter (e.g. video capture device, compression codec).
    /// </summary>
    class Filter : IComparable
    {
        /// <summary> Human-readable name of the filter </summary>
        public string Name { get; }

        /// <summary> Unique string referencing this filter. This string can be used to recreate this filter. </summary>
        public string MonikerString { get; }

        /// <summary> Create a new filter from its moniker </summary>
        public Filter(IMoniker Moniker)
        {
            Name = GetName(Moniker);
            MonikerString = GetMonikerString(Moniker);
        }

        /// <summary> Retrieve the a moniker's display name (i.e. it's unique string) </summary>
        static string GetMonikerString(IMoniker Moniker)
        {
            Moniker.GetDisplayName(null, null, out var s);
            return s;
        }

        /// <summary> Retrieve the human-readable name of the filter </summary>
        static string GetName(IMoniker Moniker)
        {
            object bagObj = null;

            try
            {
                var bagId = typeof(IPropertyBag).GUID;
                Moniker.BindToStorage(null, null, ref bagId, out bagObj);
                var bag = (IPropertyBag)bagObj;
                var hr = bag.Read("FriendlyName", out var val, null);

                if (hr != 0)
                    Marshal.ThrowExceptionForHR(hr);

                var ret = val as string;

                if (string.IsNullOrEmpty(ret))
                    throw new NotImplementedException("Device FriendlyName");
                return ret;
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                if (bagObj != null)
                    Marshal.ReleaseComObject(bagObj);
            }
        }

        /// <summary>
        ///  Compares the current instance with another object of the same type.
        /// </summary>
        public int CompareTo(object Obj)
        {
            if (Obj == null)
                return 1;

            var f = (Filter)Obj;

            return string.Compare(Name, f.Name, StringComparison.Ordinal);
        }

        public static IEnumerable<Filter> VideoInputDevices
        {
            get
            {
                object comObj = null;
                IEnumMoniker enumMon = null;
                var mon = new IMoniker[1];

                try
                {
                    // Get the system device enumerator
                    comObj = new CreateDevEnum();
                    var enumDev = (ICreateDevEnum)comObj;

                    var category = FilterCategory.VideoInputDevice;

                    // Create an enumerator to find filters in category
                    var hr = enumDev.CreateClassEnumerator(category, out enumMon, 0);
                    if (hr != 0)
                        yield break;

                    // Loop through the enumerator
                    do
                    {
                        // Next filter
                        hr = enumMon.Next(1, mon, IntPtr.Zero);

                        if (hr != 0 || mon[0] == null)
                            break;

                        // Add the filter
                        yield return new Filter(mon[0]);

                        // Release resources
                        Marshal.ReleaseComObject(mon[0]);
                        mon[0] = null;
                    } while (true);
                }
                finally
                {
                    if (mon[0] != null)
                        Marshal.ReleaseComObject(mon[0]);

                    mon[0] = null;

                    if (enumMon != null)
                        Marshal.ReleaseComObject(enumMon);

                    if (comObj != null)
                        Marshal.ReleaseComObject(comObj);
                }
            }
        }
    }
}
