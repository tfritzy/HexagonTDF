using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Managers
{
    private static Editor editor;
    public static Editor Editor
    {
        get
        {
            if (editor == null)
            {
                editor = GameObject.Find("Editor").GetComponent<Editor>();
            }

            return editor;
        }
    }

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

    private static BoardManager boardManager;
    public static BoardManager BoardManager
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
}
