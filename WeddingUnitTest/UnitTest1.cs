using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeddingApp;

namespace WeddingUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Exercise view model so that all server calls are made successfully.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            // Start the app. List of albums should be loaded.
            ViewModel vm = Control.Init(false);
            Assert.IsNotNull(vm);
            Assert.AreNotEqual(0, vm.AppNodes.Count);

            // Select an album.  List of images should be loaded.
            ViewModelNode node1 = vm.AppNodes[0];
            node1.IsSelected = true;
            Assert.IsNotNull(node1.Children);

            // Select a picture.  It should be loaded.
            ViewModelNode node2 = node1.Children[0];
            node2.IsSelected = true;
            Assert.IsNotNull(node2.FilePath);

            // Rename the picture
            string saveName = node2.Name;
            const string TEMP_NAME = "George";
            RenamePictureParms parms = new RenamePictureParms();
            parms.NewName = TEMP_NAME;
            TestData.Add(parms);
            vm.RenamePictureCommand.Execute(null);
            Assert.AreEqual(node2.Name, TEMP_NAME);
            // Restore
            parms.NewName = saveName;
            vm.RenamePictureCommand.Execute(null);
            Assert.AreEqual(node2.Name, saveName);
        }
    }
}
