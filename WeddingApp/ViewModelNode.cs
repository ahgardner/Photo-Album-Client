using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace WeddingApp
{
    /// <summary>
    /// View model for the nodes of the directory tree
    /// </summary>
    public class ViewModelNode : INotifyPropertyChanged
    {
        public const string TYPE_FOLDER = "Folder";
        public const string TYPE_FILE = "File";

        #region data
        private string mKey;
        private string mName;
        private string mNodeType;
        private ObservableCollection<ViewModelNode> mChildren;
        private string mFilePath;
        private bool mIsSelected;
        private bool mIsExpanded;
        private ViewModelNode mParent;
        private ViewModel mViewModel;
        #endregion

        public ViewModelNode(string key, string name, string type, ViewModelNode parent, ViewModel viewModel)
        {
            mKey = key;
            mName = name;
            mNodeType = type;
            mChildren = null;
            mFilePath = null;
            mIsSelected = false;
            mIsExpanded = false;
            mParent = parent;
            mViewModel = viewModel;
        }

        public string Key
        {
            get { return mKey; }
        }

        public string Name
        {
            get { return mName; }
            set
            {
                if (value != mName)
                {
                    mName = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        public string NodeType
        {
            get { return mNodeType; }
        }

        public ViewModelNode Parent
        {
            get { return mParent; }
        }

        public ObservableCollection<ViewModelNode> Children
        {
            get { return mChildren; }
            set
            {
                if (value != mChildren)
                {
                    mChildren = value;
                    this.OnPropertyChanged("Children");
                }
            }
        }

        public string FilePath
        {
            get { return mFilePath; }
            set
            {
                if (value != mFilePath)
                {
                    mFilePath = value;
                    this.OnPropertyChanged("FilePath");
                }
            }
        }

        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                if (value != mIsSelected)
                {
                    if (mNodeType.Equals(TYPE_FOLDER) && (mChildren == null))
                    {
                        ImageInfo[] images = Control.GetImagesForFolder(mName);
                        if (images.Length > 0)
                        {
                            ObservableCollection<ViewModelNode> children
                                = new ObservableCollection<ViewModelNode>();
                            foreach (ImageInfo image in images) //TODO: alias support
                            {
                                string key = image.key;
                                string name = image.name;
                                if (string.IsNullOrEmpty(name))
                                    name = key;
                                children.Add(new ViewModelNode(key, name, TYPE_FILE, this, mViewModel));
                            }
                            Children = children;
                        }
                    }
                    if (mNodeType.Equals(TYPE_FILE))
                    {
                        if (mFilePath == null)
                        {
                            FilePath = Control.GetFile(mParent.mName, mKey);
                        }
                        Control.DisplayFile(mFilePath);
                    }
                    mIsSelected = value;
                    this.OnPropertyChanged("IsSelected");
                    if (mIsSelected)
                    {
                        mViewModel.SelectedNode = this;
                    }
                    else
                    {
                        mViewModel.SelectedNode = null;
                    }
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set
            {
                if (value != mIsExpanded)
                {
                    mIsExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
