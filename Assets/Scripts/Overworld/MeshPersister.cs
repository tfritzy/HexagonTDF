using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class MeshPersister
{
    public static void CacheMesh(string folder, string fileName, Mesh mesh)
    {
        string path = Path.Combine(Application.persistentDataPath, folder);
        Directory.CreateDirectory(path);
        byte[] bytes = MeshSerializer.WriteMesh(mesh, true);
        File.WriteAllBytes(Path.Combine(path, fileName), bytes);
    }

    public static bool TryGetCacheMesh(string url, out UnityEngine.Mesh mesh)
    {
        string path = Path.Combine(Application.persistentDataPath, url);
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            mesh = MeshSerializer.ReadMesh(bytes);
            return true;
        }

        mesh = null;
        return false;
    }

    public static void CacheData<T>(string folder, string fileName, T dataClass)
    {
        string path = Path.Combine(Application.persistentDataPath, folder);
        Directory.CreateDirectory(path);
        string data = JsonConvert.SerializeObject(dataClass);
        File.WriteAllText(Path.Combine(path, fileName), data);
    }

    public static bool TryGetDataFromCache<T>(string url, out T data)
    {
        string path = Path.Combine(Application.persistentDataPath, url);
        if (File.Exists(path))
        {
            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                data = (T)serializer.Deserialize(file, typeof(T));
                return true;
            }
        }

        data = default(T);
        return false;
    }
}