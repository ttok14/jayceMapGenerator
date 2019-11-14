using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectEditGenerator : MapObjectEditGeneratorBase<MapObjectSingleInstance>
{
    public GameObject prefab; 

    GridEditCameraHandler camHandler;
    GridEditSetterWindow gridWindow;
    GridEditHandler gridSystem;

    private void Awake()
    {
        gridSystem = FindObjectOfType<GridEditHandler>();
        gridWindow = FindObjectOfType<GridEditSetterWindow>();
        camHandler = FindObjectOfType<GridEditCameraHandler>();

        Setup(gridSystem);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(gridWindow.IsWindowShowing())
            {
                gridWindow.CloseWindow();
            }
            else
            {
                gridWindow.OpenWindow();
            }
        }

        if (gridSystem.IsGridShowing())
        {
            if (Input.GetMouseButtonDown(0))
            {
                /*Generate(new MapObjectAttribute(10), 0, prefab, transform, (created, go) =>
                {
                    created.gameObject = go;
                    go.name = "maman";
                    go.transform.position = FindObjectOfType<GridEditHandler>().GetCellWorldPos(Input.mousePosition);
                });*/
            }
        }
    }
}
