using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
namespace Explorer
{
    internal class Model
    {
        public List<MyFiledata> _FillList_with_Directories(string path)
        {
            var sh = new Win32.SHFILEINFO();
            var myFiledatas = new List<MyFiledata>();
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (var dir in dirs)
                {
                    string dirMod;
                    if (dir[2].Equals('\\') && dir[3].Equals('\\')) // непонятный момент...! почему-то путь e.Node.FullPath выглядит типа С:\\\\new_folder\\folder , т.е. после диска идет лишний слеш и изза этого структура SH не заполняется. Пришлось удалять его, может есть другой путь или что-то неверно
                    {
                        dirMod = dir;
                        var dirModStart = dir;
                        dirMod = dirMod.Substring(3);
                        dirModStart = dirModStart.Substring(0, 2);
                        dirMod = dirModStart + dirMod;
                    }
                    else
                    {
                        dirMod = dir;
                    }

                    try
                    {
                        Win32.SHGetFileInfo(dirMod, 0, ref sh, (uint) Marshal.SizeOf(sh),
                            Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON | Win32.SHGFI_DISPLAYNAME);
                        var hLargeIcon = sh.hIcon;
                        Win32.SHGetFileInfo(dirMod, 0, ref sh, (uint) Marshal.SizeOf(sh),
                            Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON | Win32.SHGFI_DISPLAYNAME);
                        myFiledatas.Add(new MyFiledata(sh.szDisplayName, hLargeIcon, sh.hIcon,
                            (new DirectoryInfo(dirMod).LastWriteTime).ToShortDateString(), null));
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch
            {
                throw;}
            return myFiledatas;
        }

        public List<MyFiledata> _FillList_with_Files(string path)
        {
            Win32.SHFILEINFO sh = new Win32.SHFILEINFO();
            List<MyFiledata> myFiledatas = new List<MyFiledata>();
            try
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    string dirMod;
                    if (file[2].Equals('\\') && file[3].Equals('\\')) // непонятный момент...! почему-то путь e.Node.FullPath выглядит типа С:\\\\new_folder\\folder , т.е. после диска идет лишний слеш и изза этого структура SH не заполняется. Пришлось удалять его, может есть другой путь или что-то неверно
                    {
                        dirMod = file;
                        var dirModStart = file;
                        dirMod = dirMod.Substring(3);
                        dirModStart = dirModStart.Substring(0, 2);
                        dirMod = dirModStart + dirMod;

                    }
                    else
                    {
                        dirMod = file;
                    }

                    try
                    {
                        Win32.SHGetFileInfo(dirMod, 0, ref sh, (uint) Marshal.SizeOf(sh),
                                            Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON | Win32.SHGFI_DISPLAYNAME);
                        IntPtr hLargeIcon = sh.hIcon;
                        Win32.SHGetFileInfo(dirMod, 0, ref sh, (uint) Marshal.SizeOf(sh),
                                            Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON | Win32.SHGFI_DISPLAYNAME);

                        myFiledatas.Add(new MyFiledata(sh.szDisplayName, hLargeIcon, sh.hIcon,
                                       (new FileInfo(dirMod).LastWriteTime).ToShortDateString(),
                                       (new FileInfo(dirMod).Length).ToString()));

                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            return myFiledatas;
        }

        public List<string> get_pathes(string strng)
        {
            var directory = new DirectoryInfo(strng);
            var subdir = directory.GetDirectories();
            var dirNames = new List<string>();
            foreach (var item in subdir)
            {
                dirNames.Add(item.Name);
            }
            return dirNames;
        }

        public Dictionary<string, string> get_drives()
        {
            var drives = new Dictionary<string, string>();
            foreach (var drive in Directory.GetLogicalDrives())
            {
                drives.Add(drive, new DriveInfo(drive).DriveType.ToString());
            }
            return drives;
        }
    }
}
