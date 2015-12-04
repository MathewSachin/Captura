using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Captura
{
    public sealed class FolderBrowserDialog : CommonDialog
    {
        #region Native
        delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [Flags]
        enum FolderBrowserOptions
        {
            None = 0,
            FolderOnly = 0x0001,
            FindComputer = 0x0002,
            ShowStatusText = 0x0004,
            ReturnAncestors = 0x0008,
            ShowEditBox = 0x0010,
            ValidateResult = 0x0020,
            UseNewStyle = 0x0040,
            UseNewStyleWithEditBox = (UseNewStyle | ShowEditBox),
            AllowUrls = 0x0080,
            ShowUsageHint = 0x0100,
            HideNewFolderButton = 0x0200,
            GetShortcuts = 0x0400,
            BrowseComputers = 0x1000,
            BrowsePrinters = 0x2000,
            BrowseFiles = 0x4000,
            BrowseShares = 0x8000
        }

        [SecurityCritical, DllImport("shell32")]
        static extern int SHGetFolderLocation(IntPtr hwndOwner, Int32 nFolder, IntPtr hToken, uint dwReserved,
            out IntPtr ppidl);

        [SecurityCritical, DllImport("shell32")]
        static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lbpi);

        [SecurityCritical, DllImport("shell32", CharSet = CharSet.Auto)]
        static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

        [StructLayout(LayoutKind.Sequential)]
        struct BROWSEINFO
        {
            /// <summary>
            ///     Handle to the owner window for the dialog box.
            /// </summary>
            public IntPtr HwndOwner;

            /// <summary>
            ///     Pointer to an item identifier list (PIDL) specifying the
            ///     location of the root folder from which to start browsing.
            /// </summary>
            public IntPtr Root;

            /// <summary>
            ///     Address of a buffer to receive the display name of the
            ///     folder selected by the user.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string DisplayName;

            /// <summary>
            ///     Address of a null-terminated string that is displayed
            ///     above the tree view control in the dialog box.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string Title;

            /// <summary>
            ///     Flags specifying the options for the dialog box.
            /// </summary>
            public uint Flags;

            /// <summary>
            ///     Address of an application-defined function that the
            ///     dialog box calls when an event occurs.
            /// </summary>
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc Callback;

            /// <summary>
            ///     Application-defined value that the dialog box passes to
            ///     the callback function
            /// </summary>
            public int LParam;

            /// <summary>
            ///     Variable to receive the image associated with the selected folder.
            /// </summary>
            public int Image;
        }

        #region Malloc
        [ComImport, SuppressUnmanagedCodeSecurity, Guid("00000002-0000-0000-c000-000000000046"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IMalloc
        {
            [PreserveSig]
            IntPtr Alloc(int cb);

            [PreserveSig]
            IntPtr Realloc(IntPtr pv, int cb);

            [PreserveSig]
            void Free(IntPtr pv);

            [PreserveSig]
            int GetSize(IntPtr pv);

            [PreserveSig]
            int DidAlloc(IntPtr pv);

            [PreserveSig]
            void HeapMinimize();
        }

        [SecurityCritical]
        static IMalloc GetSHMalloc()
        {
            IMalloc[] ppMalloc = new IMalloc[1];
            SHGetMalloc(ppMalloc);
            return ppMalloc[0];
        }

        [SecurityCritical, DllImport("shell32")]
        static extern int SHGetMalloc([Out, MarshalAs(UnmanagedType.LPArray)] IMalloc[] ppMalloc);
        #endregion
        #endregion

        [SecuritySafeCritical]
        FolderBrowserOptions _dialogOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowserDialog" /> class.
        /// </summary>
        [SecurityCritical]
        public FolderBrowserDialog() { Init(); }

        void Init()
        {
            Title = string.Empty;

            // default options
            _dialogOptions = FolderBrowserOptions.ShowEditBox
                             | FolderBrowserOptions.UseNewStyle
                             | FolderBrowserOptions.BrowseShares
                             | FolderBrowserOptions.ShowStatusText
                             | FolderBrowserOptions.ValidateResult;
        }

        /// <summary>
        /// Resets the properties of a common dialog to their default values.
        /// </summary>
        [SecuritySafeCritical]
        public override void Reset()
        {
            new FileIOPermission(PermissionState.Unrestricted).Demand();

            Init();
        }

        /// <summary>
        /// Displays the folder browser dialog.
        /// </summary>
        /// <param name="hwndOwner">Handle to the window that owns the dialog box.</param>
        /// <returns>
        /// If the user clicks the OK button of the dialog that is displayed, true is returned; otherwise, false.
        /// </returns>
        [SecuritySafeCritical]
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            bool result = false;

            IntPtr pidlRoot = IntPtr.Zero,
                pszPath = IntPtr.Zero,
                pidlSelected = IntPtr.Zero;

            SelectedPath = string.Empty;

            try
            {
                SHGetFolderLocation(hwndOwner, (int)Environment.SpecialFolder.Desktop, IntPtr.Zero, 0, out pidlRoot);

                var browseInfo = new BROWSEINFO
                {
                    HwndOwner = hwndOwner,
                    Root = pidlRoot,
                    DisplayName = new string(' ', 256),
                    Title = Title,
                    Flags = (uint)_dialogOptions,
                    LParam = 0,
                    Callback = HookProc
                };

                // Show dialog
                pidlSelected = SHBrowseForFolder(ref browseInfo);

                if (pidlSelected != IntPtr.Zero)
                {
                    result = true;

                    pszPath = Marshal.AllocHGlobal(260 * Marshal.SystemDefaultCharSize);
                    SHGetPathFromIDList(pidlSelected, pszPath);

                    SelectedPath = Marshal.PtrToStringAuto(pszPath);
                }
            }
            finally // release all unmanaged resources
            {
                IMalloc malloc = GetSHMalloc();

                if (pidlRoot != IntPtr.Zero) malloc.Free(pidlRoot);

                if (pidlSelected != IntPtr.Zero) malloc.Free(pidlSelected);

                if (pszPath != IntPtr.Zero) Marshal.FreeHGlobal(pszPath);

                Marshal.ReleaseComObject(malloc);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string SelectedPath { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }
    }
}