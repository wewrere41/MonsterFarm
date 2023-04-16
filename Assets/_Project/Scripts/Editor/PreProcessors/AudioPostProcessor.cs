using System.IO;
using UnityEditor;
using UnityEngine;
using AudioImporter = UnityEditor.AudioImporter;

public class AudioPostProcessor : AssetPostprocessor
{
    private void OnPreprocessAudio()
    {
        var audioImporter = (AudioImporter)assetImporter;
        var currentAndroidAudioSettings = audioImporter.GetOverrideSampleSettings("Android");


        var fileInfo = new FileInfo(audioImporter.assetPath);
        var fileSize = GetFileLengthAsMb(fileInfo);


        currentAndroidAudioSettings.loadType = fileSize < 0.3 ? AudioClipLoadType.DecompressOnLoad :
            fileSize < 2 ? AudioClipLoadType.CompressedInMemory : AudioClipLoadType.Streaming;
        audioImporter.SetOverrideSampleSettings("Android", currentAndroidAudioSettings);
    }


    private double GetFileLengthAsMb(FileInfo fileInfo) => (double)fileInfo.Length / 1048576;
    
}