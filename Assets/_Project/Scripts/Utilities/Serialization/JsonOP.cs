using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonOP
{
    public static void SerializeData<T>(T data, string path)
    {
        var dataPath = Path.Combine(Application.persistentDataPath, path);

        using var writer = new StreamWriter(dataPath);
        using var jsonWriter = new JsonTextWriter(writer);
        var jsonSerializer = JsonSerializer.Create(JsonSerializerSettings);
        jsonSerializer.Serialize(jsonWriter, data);
    }
    

    public static void DeserializeSo(string path, ScriptableObject scriptableObject)
    {
        var dataPath = Path.Combine(Application.persistentDataPath, path);
        if (File.Exists(dataPath))
        {
            using var reader = new StreamReader(dataPath);
            using var jsonReader = new JsonTextReader(reader);

            JsonConvert.PopulateObject(reader.ReadToEnd(), scriptableObject);
        }
    }

    public static void RemoveData(string path)
    {
        var dataPath = Path.Combine(Application.persistentDataPath, path);
        if (File.Exists(dataPath)) File.Delete(dataPath);
    }

    private static readonly JsonSerializerSettings JsonSerializerSettings =
        new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
        };
}