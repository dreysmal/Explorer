using System;
using System.Collections.Generic;
namespace Explorer
{
    public delegate void MyVoidStringDelegate(string strng);
    interface IExplorer
    {
        Dictionary<string, string> Drives { get; set; }
        List<string> DirectoriesNames { get; set; }
        List<MyFiledata> MyFiledatas { get; set; }
        void Initialization();

        event EventHandler GetDrives;
        event MyVoidStringDelegate GetPaths;
        event MyVoidStringDelegate FillListWithDirectories;
        event MyVoidStringDelegate FillListWithFiles;
    }
}
