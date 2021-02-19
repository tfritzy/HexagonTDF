using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor : MonoBehaviour
{
    public HexagonType SelectedType;

    public void Save()
    {
        Contract.Map map = new Contract.Map();
        map.TypeMap = Managers.BoardManager.GetTypeMap();

        string json = JsonConvert.SerializeObject(map);
        string fileName = Application.dataPath + "/Resources/Maps/" + Guid.NewGuid().ToString("N") + ".json";
        Debug.Log(fileName);
        System.IO.File.WriteAllText(fileName, json);
    }
}
