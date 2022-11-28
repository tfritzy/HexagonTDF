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

    private static LoadingMenu loadingMenu;
    public static LoadingMenu LoadingMenu
    {
        get
        {
            if (loadingMenu == null)
            {
                loadingMenu = GameObject.Find("LoadingMenu").GetComponent<LoadingMenu>();
            }

            return loadingMenu;
        }
    }

    private static Canvas loadingCanvas;
    public static Canvas LoadingCanvas
    {
        get
        {
            if (loadingCanvas == null)
            {
                loadingCanvas = GameObject.Find("LoadingCanvas").GetComponent<Canvas>();
            }

            return loadingCanvas;
        }
    }

    private static OverworldManager overworldManager;
    public static OverworldManager OverworldManager
    {
        get
        {
            if (overworldManager == null)
            {
                overworldManager = GameObject.Find("OverworldManager").GetComponent<OverworldManager>();
            }

            return overworldManager;
        }
    }

    private static InputManager _inputManager;
    public static InputManager InputManager
    {
        get
        {
            if (_inputManager == null)
            {
                _inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
            }

            return _inputManager;
        }
    }

    private static UI _ui;
    public static UI UI
    {
        get
        {
            if (_ui == null)
            {
                _ui = GameObject.Find("UI").GetComponent<UI>();
            }

            return _ui;
        }
    }
}
