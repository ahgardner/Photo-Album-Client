using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace WeddingApp
{
    /// <summary>
    /// Application view model, with mappings for the directory tree and view panel.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        #region data
        private ObservableCollection<ViewModelNode> mAppNodes = null;
        private string mImageFile = null;
        private ViewModelNode mSelectedNode = null;

        ICommand mRenamePictureCommand = null;
        #endregion

        public ViewModel (string[] initialFolders)
        {
            mAppNodes = new ObservableCollection<ViewModelNode>();
            foreach (string name in initialFolders)
            {
                mAppNodes.Add(new ViewModelNode(name, name, ViewModelNode.TYPE_FOLDER, null, this));
                // TODO: alias support
            }
            mRenamePictureCommand = new RelayCommand(RenameItemExecute, RenameItemCanExecute);
        }

        public ObservableCollection<ViewModelNode> AppNodes
        {
            get { return mAppNodes; }
            set
            {
                if (value != mAppNodes)
                {
                    mAppNodes = value;
                    this.OnPropertyChanged("AppNodes");
                }
            }
        }

        public ViewModelNode SelectedNode
        {
            get { return mSelectedNode; }
            set
            {
                if (value != mSelectedNode)
                {
                    mSelectedNode = value;
                    this.OnPropertyChanged("SelectedItem");
                    if (value != null)
                    {
                       // Controller.NodeSelectedEvent(mSelectedNode.DesignObject);
                    }
                }
            }
        }

        public string ImageFile
        {
            get { return mImageFile; }
            set
            {
                if (value != mImageFile)
                {
                    mImageFile = value;
                    this.OnPropertyChanged("ImageFile");
                }
            }
        }

        #region Rename command
        /// <summary>
        /// Rename a node
        /// </summary>
        public ICommand RenamePictureCommand
        {
            get { return mRenamePictureCommand; }
        }

        public void RenameItemExecute()
        {
            ViewModelNode node = mSelectedNode;    // Action is on selected item
            string alias = Control.RenamePicture(node.Parent.Key, node.Key);
            if (!string.IsNullOrWhiteSpace(alias))
            {
                node.Name = alias;
            }
        }

        public bool RenameItemCanExecute()
        {
            return (mSelectedNode != null && mSelectedNode.NodeType == ViewModelNode.TYPE_FILE);
        }

        #endregion

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
