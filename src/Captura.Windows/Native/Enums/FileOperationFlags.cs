using System;
// ReSharper disable UnusedMember.Global

namespace Captura.Native
{
    [Flags]
    enum FileOperationFlags
    {
        MultiDestFiles = 0x1,

        ConfirmMouse = 0x2,
        
        /// <summary>
        /// Don't create progress/report
        /// </summary>
        Silent = 0x4,

        RenameOnCollission = 0x8,
        
        /// <summary>
        /// Don't prompt the user.
        /// </summary>
        NoConfirmation = 0x10,
        
        /// <summary>
        /// Fill in <see cref="ShFileOpStruct.NameMappings"/>.
        /// Must be freed using SHFreeNameMappings
        /// </summary>
        WantMappingHandle = 0x20,

        AllowUndo = 0x40,
        
        /// <summary>
        /// On *.*, do only files
        /// </summary>
        FilesOnly = 0x80,
        
        /// <summary>
        /// Don't show names of files
        /// </summary>
        SimpleProgress = 0x100,
        
        /// <summary>
        /// Don't confirm making any needed dirs
        /// </summary>
        NoConfirmMkdir = 0x200,
        
        /// <summary>
        /// Don't put up error UI
        /// </summary>
        // ReSharper disable once InconsistentNaming
        NoErrorUI = 0x400,
        
        /// <summary>
        /// Dont copy NT file Security Attributes
        /// </summary>
        NoCopySecurityAttribs = 0x800,
        
        /// <summary>
        /// Don't recurse into directories.
        /// </summary>
        NoRecursion = 0x1000,
        
        /// <summary>
        /// Don't operate on connected elements.
        /// </summary>
        NoConnectedElements = 0x2000,
        
        /// <summary>
        /// During delete operation, 
        /// warn if nuking instead of recycling (partially overrides <see cref="NoConfirmation"/>)
        /// </summary>
        WantNukeWarning = 0x4000,
        
        /// <summary>
        /// Treat reparse points as objects, not containers
        /// </summary>
        NoRecurseReparse = 0x8000
    }
}