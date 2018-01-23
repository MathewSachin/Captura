using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

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

        /// <summary> Create a new filter from its moniker string. </summary>
        public Filter(string MonikerString)
        {
            Name = GetName(MonikerString);
            this.MonikerString = MonikerString;
        }

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
                object val = "";
                var hr = bag.Read("FriendlyName", ref val, IntPtr.Zero);

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

        /// <summary> Get a moniker's human-readable name based on a moniker string. </summary>
        static string GetName(string monikerString)
        {
            IMoniker parser = null;
            IMoniker moniker = null;

            try
            {
                parser = GetAnyMoniker();
                parser.ParseDisplayName(null, null, monikerString, out int _, out moniker);
                return GetName(parser);
            }
            finally
            {
                if (parser != null)
                    Marshal.ReleaseComObject(parser);

                if (moniker != null)
                    Marshal.ReleaseComObject(moniker);
            }
        }

        /// <summary>
        ///  This method gets a IMoniker object.
        /// 
        ///  HACK: The only way to create a IMoniker from a moniker 
        ///  string is to use IMoniker.ParseDisplayName(). So I 
        ///  need ANY IMoniker object so that I can call 
        ///  ParseDisplayName(). Does anyone have a better solution?
        /// 
        ///  This assumes there is at least one video compressor filter
        ///  installed on the system.
        /// </summary>
        static IMoniker GetAnyMoniker()
        {
            var category = Uuid.FilterCategory.VideoCompressorCategory;
            object comObj = null;
            IEnumMoniker enumMon = null;
            var mon = new IMoniker[1];

            try
            {
                // Get the system device enumerator
                var srvType = Type.GetTypeFromCLSID(Uuid.Clsid.SystemDeviceEnum);
                if (srvType == null)
                    throw new NotImplementedException("System Device Enumerator");
                comObj = Activator.CreateInstance(srvType);
                var enumDev = (ICreateDevEnum)comObj;

                //Create an enumerator to find filters in category
                var hr = enumDev.CreateClassEnumerator(ref category, out enumMon, 0);
                if (hr != 0)
                    throw new NotSupportedException("No devices of the category");

                // Get first filter
                hr = enumMon.Next(1, mon, IntPtr.Zero);
                if (hr != 0)
                    mon[0] = null;

                return mon[0];
            }
            finally
            {
                if (enumMon != null)
                    Marshal.ReleaseComObject(enumMon);

                if (comObj != null)
                    Marshal.ReleaseComObject(comObj);
            }
        }

        /// <summary>
        ///  Compares the current instance with another object of 
        ///  the same type.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            var f = (Filter)obj;

            return string.Compare(Name, f.Name, StringComparison.Ordinal);
        }
    }
}
