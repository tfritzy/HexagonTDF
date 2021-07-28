using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private static TrebuchetHealthbar trebuchetHealthbar;
    public static TrebuchetHealthbar TrebuchetHealthbar
    {
        get
        {
            if (trebuchetHealthbar == null)
            {
                trebuchetHealthbar = Canvas.Find("TrebuchetHealthbar").GetComponent<TrebuchetHealthbar>();
            }

            return trebuchetHealthbar;
        }
    }

    private static InMemPrefabs prefabs;
    public static InMemPrefabs Prefabs
    {
        get
        {
            if (prefabs == null)
            {
                prefabs = GameObject.Find("Prefabs").GetComponent<InMemPrefabs>();
            }

            return prefabs;
        }
    }

    private static SelectTowerMenu selectTowerMenu;
    public static SelectTowerMenu SelectTowerMenu
    {
        get
        {
            if (selectTowerMenu == null)
            {
                selectTowerMenu = GameObject.Instantiate(Managers.prefabs.SelectTowerMenu, Canvas).GetComponent<SelectTowerMenu>();
                selectTowerMenu.Disable();
            }

            return selectTowerMenu;
        }
    }
}
