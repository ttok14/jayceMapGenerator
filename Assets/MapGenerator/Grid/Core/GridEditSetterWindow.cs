using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GridEditSetterWindow : GridEditSetter
{
    public GridEditHandler generatorGUI;

    public RectTransform optionWindow;

    public InputField worldOriPosXInput;
    public InputField worldOriPosYInput;

    public InputField cellWidthInput;
    public InputField cellHeightInput;

    float worldOriPosX, worldOriPosY;
    float cellWidth, cellHeight;

    public Button createButton;

    public System.Func<GridEditSetter, bool> onCreate;

    public UnityEvent onOpen, onClose;

    // Use this for initialization
    void Start()
    {
        if (generatorGUI == null)
            generatorGUI = FindObjectOfType<GridEditHandler>();

        worldOriPosXInput.onValueChanged.AddListener(OnChangeWorldOriPosX);
        worldOriPosYInput.onValueChanged.AddListener(OnChangeWorldOriPosY);
        cellWidthInput.onValueChanged.AddListener(OnChangeCellWidth);
        cellHeightInput.onValueChanged.AddListener(OnChangeCellHeight);

        onCreate = generatorGUI.SetGrid;

        SetActiveWindow(false);
    }

    public bool IsWindowShowing()
    {
        return optionWindow.gameObject.activeInHierarchy;
    }

    public void OpenWindow()
    {
        SetActiveWindow(true);
    }

    public void CloseWindow()
    {
        SetActiveWindow(false);
    }

    public void SetActiveWindow(bool active)
    {
        optionWindow.gameObject.SetActive(active);

        if (active)
        {
            if (onOpen != null)
            {
                onOpen.Invoke();
            }
        }
        else
        {
            if (onClose != null)
            {
                onClose.Invoke();
            }
        }
    }

    void ParseAndSet(string str, ref float source)
    {
        float parsed;

        if (float.TryParse(str, out parsed))
        {
            source = parsed;
        }
    }
    public void OnChangeWorldOriPosX(string str)
    {
        ParseAndSet(str, ref worldOriPosX);
    }
    public void OnChangeWorldOriPosY(string str)
    {
        ParseAndSet(str, ref worldOriPosY);
    }
    public void OnChangeCellWidth(string str)
    {
        ParseAndSet(str, ref cellWidth);
    }
    public void OnChangeCellHeight(string str)
    {
        ParseAndSet(str, ref cellHeight);
    }
    public void OnClickCreateButton()
    {
        if (onCreate(this) == false)
        {
            Debug.LogError("Could not create Grid, Please Check inputValues");
        }
        else
        {
            SetActiveWindow(false);
        }
    }

    public override void SetGrid(float worldOriPosX, float worldOriPosY, float cellWidth, float cellHeight)
    {
        this.worldOriPosX = worldOriPosX;
        this.worldOriPosY = worldOriPosY;
        this.cellWidth = cellWidth;
        this.cellHeight = cellHeight;

        onCreate(this);
    }

    public override float WorldOriginX()
    {
        return worldOriPosX;
    }

    public override float WorldOriginY()
    {
        return worldOriPosY;
    }

    public override float CellWidth()
    {
        return cellWidth;
    }

    public override float CellHeight()
    {
        return cellHeight;
    }
}
