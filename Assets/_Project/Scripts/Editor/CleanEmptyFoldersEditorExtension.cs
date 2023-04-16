using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CleanEmptyFoldersEditorExtension : EditorWindow
{
    private static string deletedFolders;

    [MenuItem("Tools/Clean Empty Folders")]
    private static void Cleanup()
    {
        deletedFolders = string.Empty;

        var directoryInfo = new DirectoryInfo(Application.dataPath);
        foreach (var subDirectory in directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories))
        {
            if (subDirectory.Exists)
            {
                ScanDirectory(subDirectory);
            }
        }

        Debug.Log("Deleted Folders:\n" + (deletedFolders.Length > 0 ? deletedFolders : "NONE"));
    }

    private static void ScanDirectory(DirectoryInfo subDirectory)
    {
        Debug.Log("Scanning Directory: " + subDirectory.FullName);

        var filesInSubDirectory = subDirectory.GetFiles("*.*", SearchOption.AllDirectories);

        if (filesInSubDirectory.Length == 0 || filesInSubDirectory.All(t => t.FullName.EndsWith(".meta")))
        {
            deletedFolders += subDirectory.FullName + "\n";
            subDirectory.Delete(true);
            var subDirectoryMeta = subDirectory.FullName + ".meta";
            if (File.Exists(subDirectoryMeta))
            {
                File.Delete(subDirectoryMeta);
            }
            
        }
    }
}