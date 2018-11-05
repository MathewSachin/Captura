using System;
using System.Runtime.InteropServices;

namespace Captura.Native
{
    public static class Shell32
    {
        const string DllName = "shell32.dll";

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation(ref ShFileOpStruct FileOp);

        public static bool FileOperation(string Path, FileOperationType OperationType, FileOperationFlags Flags)
        {
            try
            {
                var fs = new ShFileOpStruct
                {
                    Func = OperationType,
                    From = Path + '\0' + '\0',
                    Flags = Flags
                };

                SHFileOperation(ref fs);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}