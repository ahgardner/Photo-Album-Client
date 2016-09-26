namespace WeddingApp
{
    /// <summary>
    /// Note: to test with internal classes, add to AssemblyInfo.cs:
    /// [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("WeddingUnitTest")]
    /// </summary>
    internal class RenamePictureParms
    {
        internal string NewName;
    }

    /// <summary>
    /// Pattern for a dialog that can run with or without UI.
    /// This could be a separate form in more complex situations,
    /// in which case the pattern could be the following method.
    /// Both this and the definition of MyDialogParms could reside in MyDiaglog.cs.
    /// 
    /// internal static void run(ref MyDialogParms parms)
    /// {
    ///     if (Control.HasUI)
    ///     {
    ///         MyDialog window = new MyDialog();
    ///         window.mParms = parms;
    ///         window.ShowDialog();
    ///         parms = window.mParms;
    ///     }
    ///     else
    ///     {
    ///         parms = (MyDialogParms)TestData.Fill(parms);
    ///     }
    /// }
    /// 
    /// </summary>
    internal class RenamePictureDialog
    {
        internal static void run(ref RenamePictureParms parms)
        {
            if (Control.HasUI)
            {
                parms.NewName = Microsoft.VisualBasic.Interaction.InputBox("Enter alias");
            }
            else
            {
                parms = (RenamePictureParms)TestData.Fill(parms);
            }
        }
    }
}
