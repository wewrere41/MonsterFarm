using UnityEditor;
using UnityEngine;


public class RBFolderLocator
{
    private static readonly string RBFolderKey = $"Borodar.RainbowFolders.HomeFolder.{GetProjectName()}";

    private static readonly string RBFolderValue =
        @"Assets\3rd Party\StaticAssets\EditorTools\RainbowAssets\RainbowFolders";

    [InitializeOnLoadMethod]
    private static void CheckAndSetFolder()
    {
        if (CheckTargetLocation() is false)
        {
            SetTargetLocation();
        }
    }

    private static bool CheckTargetLocation()
    {
        return EditorPrefs.HasKey(RBFolderKey) && EditorPrefs.GetString(RBFolderKey) == RBFolderValue;
    }

    private static string GetProjectName()
    {
        var s = Application.dataPath.Split('/');
        var p = s[s.Length - 2];
        return p;
    }

    private static void SetTargetLocation()
    {
        EditorPrefs.SetString(RBFolderKey, RBFolderValue);
    }
}