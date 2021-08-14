using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    IslandGenerator generator;
    public GameObject Tile;
    public int Seed;

    private const int NUM_SEGMENTS_SPAWNED = 5;
    private float tileWidth;
    private Camera cam;
    private LinkedList<GameObject> mapSegments;
    private int spawnedMapLowIndex;

    // Start is called before the first frame update
    void Start()
    {
        generator = this.transform.GetComponent<IslandGenerator>();
        tileWidth = Tile.GetComponent<MeshRenderer>().bounds.extents.x * 2;
        cam = Camera.main;
        spawnedMapLowIndex = 0;
        mapSegments = new LinkedList<GameObject>();

        for (int i = 0; i < NUM_SEGMENTS_SPAWNED; i++)
        {
            GameObject segment = createSegment(i);
            formatSegment(i, segment);
            mapSegments.AddLast(segment);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cam.transform.position.z > ((spawnedMapLowIndex + 2) * tileWidth + tileWidth / 2))
        {
            formatSegment(spawnedMapLowIndex + NUM_SEGMENTS_SPAWNED, mapSegments.First.Value);
            GameObject segment = mapSegments.First.Value;
            mapSegments.RemoveFirst();
            mapSegments.AddLast(segment);
            spawnedMapLowIndex += 1;
        }
        if (cam.transform.position.z < (spawnedMapLowIndex * tileWidth + tileWidth / 2))
        {
            formatSegment(spawnedMapLowIndex, mapSegments.First.Value);
            GameObject segment = mapSegments.Last.Value;
            mapSegments.RemoveLast();
            mapSegments.AddFirst(segment);
            spawnedMapLowIndex -= 1;
        }
    }

    private GameObject createSegment(int index)
    {
        return Instantiate(Tile, Vector3.one * 10000, Tile.transform.rotation, this.transform);
    }

    private void formatSegment(int index, GameObject segment)
    {
        segment.name = $"Map Segment {index}";
        segment.transform.position = new Vector3(0, 0, tileWidth * index);
        IslandGenerator.MapPoint[,] map = generator.GetSegment(index, Seed);
        Texture2D texture = generator.GetTextureOfMap(map);
        segment.GetComponent<MeshRenderer>().material.mainTexture = texture;
    }
}
