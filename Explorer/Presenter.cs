using System;

namespace Explorer
{
    class Presenter
    {
        private readonly IExplorer _iexplorer;
        readonly Model _model = new Model();
        public Presenter(IExplorer iexplorer)
        {
            this._iexplorer = iexplorer;
            iexplorer.GetDrives += _GetDrives;
            iexplorer.GetPaths += GetPaths;
            iexplorer.FillListWithDirectories += fillList_with_Directories;
            iexplorer.FillListWithFiles += fillList_with_Files;
            iexplorer.Initialization();
        }

        public void _GetDrives(object sender, EventArgs e)
        {
            _iexplorer.Drives = _model.get_drives();
        }
        public void GetPaths(string path)
        {
            _iexplorer.DirectoriesNames = _model.get_pathes(path);
        }

        public void fillList_with_Directories(string path)
        {
            _iexplorer.MyFiledatas = _model._FillList_with_Directories(path);
        }
        public void fillList_with_Files(string path)
        {
            _iexplorer.MyFiledatas = _model._FillList_with_Files(path);
        }
    }
}
