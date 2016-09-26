using System.IO;
using System.Windows;

namespace WeddingApp
{
    /// <summary>
    /// Control module, which mediates between the view model and the app logic
    /// (which pretty much equals calling the server).
    /// </summary>
    public class Control
    {
        private const string TEMP_FOLDER = "weddingtempfiles";
        private static ViewModel vm;
        private static bool mHasUI;

        public static bool HasUI { get { return mHasUI; } }

        internal static ViewModel Init(bool hasUI=true)
        {
            // Set up temp directory for images
            InitTempPath();

            // Note whether there is a UI
            mHasUI = hasUI;

            // Initialize server accessor
            ServiceAccess.Init();

            // Initialize view model and return it to the App
            vm = new ViewModel(ServiceAccess.GetFolders());
            return vm;
        }

        internal static ImageInfo[] GetImagesForFolder(string folder)
        {
            return ServiceAccess.GetImageList(folder);
        }

        internal static string GetFile (string folder, string item)
        {
            string fileName = Path.GetRandomFileName();
            string path = Path.Combine(Path.GetTempPath(), TEMP_FOLDER, fileName);
            ServiceAccess.DownloadFile(folder, item, path);
            return path;
        }

        internal static void DisplayFile (string path)
        {
            vm.ImageFile = path;
        }

        internal static string RenamePicture (string folder, string item)
        {
            RenamePictureParms parms = new RenamePictureParms();
            RenamePictureDialog.run(ref parms);
            string alias = parms.NewName;
            if (string.IsNullOrWhiteSpace(alias))
            {
                return null;
            }
            ServiceAccess.SetAlias(folder, item, alias);
            return alias;
        }

        internal static void ShowError (string message)
        {
            if (mHasUI) MessageBox.Show(message);
        }

        private static void InitTempPath()
        {
            string path = Path.Combine(Path.GetTempPath(), TEMP_FOLDER);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }
    }
}
