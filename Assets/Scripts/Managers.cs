using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Managers
{
    private static Camera camera;
    public static Camera Camera
    {
        get
        {
            if (camera == null)
            {
                camera = Camera.main;
            }

            return camera;
        }
    }

    private static CameraControl cameraControl;
    public static CameraControl CameraControl
    {
        get
        {
            if (cameraControl == null)
            {
                cameraControl = Camera.GetComponent<CameraControl>();
            }
            return cameraControl;
        }
    }

    private static BoardManager boardManager;
    public static BoardManager Board
    {
        get
        {
            if (boardManager == null)
            {
                boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
            }

            return boardManager;
        }
    }

    private static ResourceStore resourceStore;
    public static ResourceStore ResourceStore
    {
        get
        {
            if (resourceStore == null)
            {
                resourceStore = GameObject.Find("ResourceStore").GetComponent<ResourceStore>();
            }
            return resourceStore;
        }
    }

    private static Builder builder;
    public static Builder Builder
    {
        get
        {
            if (builder == null)
            {
                builder = GameObject.Find("Builder").GetComponent<Builder>();
            }
            return builder;
        }
    }

    private static Transform canvas;
    public static Transform Canvas
    {
        get
        {
            if (canvas == null)
            {
                canvas = GameObject.Find("Canvas").transform;
            }
            return canvas;
        }
    }

    private static EnemySpawner enemySpawner;
    public static EnemySpawner EnemySpawner
    {
        get
        {
            if (enemySpawner == null)
            {
                enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
            }
            return enemySpawner;
        }
    }
}
