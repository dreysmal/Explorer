using System;

namespace Explorer
{
    public struct MyFiledata
    {
        public string Name;
        public IntPtr HLarge;
        public IntPtr HSmall;
        public string DateLastModified;
        public string Length;

        public MyFiledata(string name, IntPtr hLarge, IntPtr hSmall, string dateLastModified, string length)
        {
            Name = name;
            HLarge = hLarge;
            HSmall = hSmall;
            DateLastModified = dateLastModified;
            Length = length;
        }
    }
}
