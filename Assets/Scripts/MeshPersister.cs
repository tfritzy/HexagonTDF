using System.IO;
using UnityEngine;

public static class MeshPersister
{
    public static void CacheItem(string folder, string fileName, Mesh mesh)
    {
        string path = Path.Combine(Application.persistentDataPath, folder);
        Directory.CreateDirectory(path);
        byte[] bytes = MeshSerializer.WriteMesh(mesh, true);
        File.WriteAllBytes(Path.Combine(path, fileName), bytes);
    }

    public static bool TryGetCacheItem(string url, out UnityEngine.Mesh mesh)
    {
        string path = Path.Combine(Application.persistentDataPath, url);
        if (File.Exists(path) == true)
        {
            byte[] bytes = File.ReadAllBytes(path);
            mesh = MeshSerializer.ReadMesh(bytes);
            return true;
        }

        mesh = null;
        return false;
    }
}