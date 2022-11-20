using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceUtility : MonoBehaviour
{
    public static void GetInterfaces<T>(out List<T> resultList, GameObject objectToSearch) where T : class
    {
        MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
        resultList = new List<T>();
        foreach (MonoBehaviour mb in list)
        {
            if (mb is T)
            {
                //found one
                resultList.Add((T)((System.Object)mb));
            }
        }
    }

    public static bool TryGetInterface<T>(out T result, GameObject gameObject) where T : class
    {
        MonoBehaviour[] list = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour mb in list)
        {
            if (mb is T)
            {
                result = (T)((System.Object)mb);
                return true;
            }
        }

        result = null;
        return false;
    }
}
