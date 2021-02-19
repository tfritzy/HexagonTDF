using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Contract
{
    public class Map
    {
        [JsonProperty]
        public HexagonType[,] TypeMap;
    }
}

