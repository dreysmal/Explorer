using System;
using System.Runtime.InteropServices;

namespace Explorer
{
    public class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        public const int SHGFI_ICON = 0x000000100;
        public const int SHGFI_SMALLICON = 0x000000001;
        public const int SHGFI_LARGEICON = 0x000000000;
        public const int SHGFI_DISPLAYNAME = 0x000000200;
        public const int SHGFI_TYPENAME = 0x000000400;
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
    }
}
