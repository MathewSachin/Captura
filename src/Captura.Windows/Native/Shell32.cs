using System;
using System.Runtime.InteropServices;

namespace Captura.Native
{
    static class Shell32
    {
        const string DllName = "shell32.dll";

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation(ref ShFileOpStruct FileOp);

        public static int FileOperation(string Path, FileOperationType OperationType, FileOperationFlags Flags)
        {
            try
            {
                var fs = new ShFileOpStruct
                {
                    Func = OperationType,
                    From = Path + '\0' + '\0',
                    Flags = Flags
                };

                return SHFileOperation(ref fs);
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}