using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Explorer
{
    public partial class Form1 : Form, IExplorer
    {
        private TreeNode _treeNode;
        private readonly ImageList _driveImages = new ImageList();
        private ImageList _viewImagesLarge;
        private ImageList _viewImagesSmall;
        bool IsLastButtonNavigBack = false;
        bool IsLastButtonNavigUp = false;
        private readonly ColumnHeader _column = new ColumnHeader();
        private readonly ColumnHeader _column1 = new ColumnHeader();
        private readonly ColumnHeader _column2 = new ColumnHeader();

        List<string> paths = new List<string>(); //для навигации вперед назад
        List<string> pathsUp = new List<string>();
        int navigation_index = 0;

        public Dictionary<string, string> Drives { get; set; }
        public List<string> DirectoriesNames { get; set; }
        public List<MyFiledata> MyFiledatas { get; set; }

        public void Initialization()
        {
            InitTree();
        }

        public event EventHandler GetDrives;
        public event MyVoidStringDelegate GetPaths;
        public event MyVoidStringDelegate FillListWithDirectories;
        public event MyVoidStringDelegate FillListWithFiles;

        public Form1()
        {
            InitializeComponent();
            _driveImages.ColorDepth = ColorDepth.Depth32Bit;
            _column.Text = @"Name";
            _column.Width = 300;
            _column1.Text = @"Date Modified";
            _column1.Width = 100;
            _column2.Text = @"Size B";
            _column2.Width = 100;

            listView1.Columns.AddRange(new ColumnHeader[] { _column, _column1, _column2 });

            _driveImages.Images.Add(Bitmap.FromFile(@"../../icons/floppy.png"));
            _driveImages.Images.Add(Bitmap.FromFile(@"../../icons/HDD.png"));
            _driveImages.Images.Add(Bitmap.FromFile(@"../../icons/Folder.png"));
            treeView1.ImageList = _driveImages;
        }
        private void InitTree()
        {
            GetDrives?.Invoke(this, EventArgs.Empty);
            foreach (var drive in Drives)
            {
                try//картинки на диски
                {
                    _treeNode = new TreeNode();
                    _treeNode = treeView1.Nodes.Add(drive.Key);

                    switch (drive.Value)
                    {
                        case "DriveType.Fixed":
                        case "DriveType.Network":
                            _treeNode.ImageIndex = 1;
                            _treeNode.SelectedImageIndex = 1;
                            break;
                        case "DriveType.Removable":
                        case "DriveType.CDRom":
                            _treeNode.ImageIndex = 0;
                            _treeNode.SelectedImageIndex = 0;
                            break;
                    }
                    GetPaths?.Invoke(drive.Key);
                    foreach (var item in DirectoriesNames)
                    {
                        TreeNode node = new TreeNode(item)
                        {
                            ImageIndex = 2,
                            SelectedImageIndex = 2
                        };
                        _treeNode.Nodes.Add(node);
                    }
                }
                catch { }
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)  //Заполняем +1 уровень перед раскрытием ветки
        {
            try
            {
                foreach (TreeNode subnode in e.Node.Nodes)
                {
                    while (true)
                    {
                        try
                        {
                            GetPaths?.Invoke(subnode.FullPath);
                            while (true)
                            {
                                try
                                {
                                    bool flag = false;
                                    foreach (var item in DirectoriesNames)
                                    {
                                        if (subnode.Nodes.Count == 0)
                                            flag = true;
                                        if (flag)
                                        {
                                            var node = new TreeNode(item)
                                            {
                                                ImageIndex = 2,
                                                SelectedImageIndex = 2
                                            };
                                            subnode.Nodes.Add(node);
                                        }
                                    }
                                }
                                catch { continue; }
                                break;
                            }
                        }
                        catch
                        {
                            // ignored
                        }

                        break;
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //сохранение пути для навигации вперед назад
            if (paths.Count == 0)
            {
                paths.Add(e.Node.FullPath);
                navigation_index++;
            }
            else
            {
                if (IsLastButtonNavigBack)
                {
                    paths[navigation_index - 1] = e.Node.FullPath;
                    IsLastButtonNavigBack = false;
                    IsLastButtonNavigUp = false;
                    toolStripButton2.Enabled = false;
                }
                else
                {
                    if (!e.Node.FullPath.Equals(paths[navigation_index - 1]))
                    {
                        paths.Add(e.Node.FullPath);
                        toolStripButton1.Enabled = true;
                        toolStripButton3.Enabled = true;
                        navigation_index++;
                    }
                }
            }

            FillListView(e.Node.FullPath);
        }

        private void FillListView(string path)
        {
            listView1.Items.Clear();
            _viewImagesSmall = new ImageList();
            _viewImagesLarge = new ImageList();
            _viewImagesSmall.ImageSize = new Size(16, 16);
            _viewImagesLarge.ImageSize = new Size(32, 32);

            listView1.SmallImageList = _viewImagesSmall;
            listView1.LargeImageList = _viewImagesLarge;
            _viewImagesSmall.ColorDepth = ColorDepth.Depth32Bit;
            _viewImagesLarge.ColorDepth = ColorDepth.Depth32Bit;

            var counter = 0;

            try
            {
                FillListWithDirectories?.Invoke(path);

                foreach (MyFiledata variable in MyFiledatas)
                {
                    try
                    {
                        Icon iconLarge = Icon.FromHandle(variable.HLarge);
                        _viewImagesLarge.Images.Add(iconLarge);
                        Icon iconSmall = Icon.FromHandle(variable.HSmall);
                        _viewImagesSmall.Images.Add(iconSmall);
                        listView1.Items.Add(new ListViewItem(new string[] {variable.Name, variable.DateLastModified}, counter++));
                    }
                    catch { }
                }

                FillListWithFiles?.Invoke(path);

                foreach (MyFiledata variable in MyFiledatas)
                {
                    try
                    {
                        Icon iconLarge = Icon.FromHandle(variable.HLarge);
                        _viewImagesLarge.Images.Add(iconLarge);
                        Icon iconSmall = Icon.FromHandle(variable.HSmall);
                        _viewImagesSmall.Images.Add(iconSmall);
                        listView1.Items.Add(new ListViewItem(new string[] { variable.Name, variable.DateLastModified, variable.Length }, counter++));
                    }
                    catch { }
                }
            }
            catch
            {
                MessageBox.Show(@"Нет доступа");
            }
        }

                                         ///////////////////////
                                        ////////Кнопки/////////
                                       ///////////////////////

        private void toolStripButton1_Click(object sender, EventArgs e)//назад
        {
            try
            {
                FillListView(paths[navigation_index-- - 1 - 1]);
                if (navigation_index <= 1)
                    toolStripButton1.Enabled = false;
                toolStripButton2.Enabled = true;
                IsLastButtonNavigBack = true;
            }
            catch { }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)//вперед
        {
            try
            {
                if (paths.Count > navigation_index)
                {
                    FillListView(paths[navigation_index]);
                    navigation_index++;
                    toolStripButton1.Enabled = true;
                }
                if (paths.Count == navigation_index)
                    toolStripButton2.Enabled = false;
                IsLastButtonNavigBack = false;
                IsLastButtonNavigUp = false;
            }
            catch { }
        }

        private void toolStripButton3_Click(object sender, EventArgs e) //вверх
        {
            try
            {
                string substring = paths[navigation_index - 1]
                    .Substring(0, paths[navigation_index - 1].LastIndexOf('\\'));
                if (substring.Length > 3)
                {
                    FillListView(substring);
                    paths.Add(substring);
                    navigation_index++;
                    IsLastButtonNavigUp = true;
                }
                else
                {
                    FillListView(substring);

                    if (!substring.Equals(paths[navigation_index - 1]))
                    {
                        paths.Add(substring);
                        toolStripButton3.Enabled = false;
                        IsLastButtonNavigUp = true;
                        navigation_index++;
                    }
                    else
                    {
                        toolStripButton3.Enabled = false;
                        IsLastButtonNavigUp = true;
                    }
                }
            }
            catch { }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            listView1.Items.Clear();
            paths.Clear();
            navigation_index = 0;
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = false;
            toolStripButton3.Enabled = false;
            IsLastButtonNavigBack = false;
            IsLastButtonNavigUp = false;

            InitTree();
        }

        private void списокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.List;
        }

        private void таблицаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Details;
        }

        private void плиткаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Tile;
        }

        private void большиеЗначкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.LargeIcon;
        }
    }
}



