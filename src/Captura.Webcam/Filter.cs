using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;

namespace Captura.Webcam
{
    /// <summary>
    ///  Represents a DirectShow filter (e.g. video capture device, compression codec).
    /// </summary>
    public class Filter : IComparable
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
    }
}
