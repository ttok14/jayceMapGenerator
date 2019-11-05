using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class GridEditHandler : MonoBehaviour, IGridEditHandler
{
    const float baseLineThickness = 3;

    public GridEditSetter gridSetter;
    public GridEditCameraHandler cam;

    public RectTransform root;
    RectTransform gridHorizontalRoot;
    RectTransform gridVerticalRoot;

    public Color gridColor = Color.white;

    public Image origIndicator;
    public Image mouseOverIndicator;

    List<Image> horizontalLines;
    List<Image> verticalLines;

    Vector3 origWorldPos;
    Vector3 origScreenPos;
    Vector2 lastUpdatedMousePos;

    float cellScreenWidth, cellScreenHeight;
    float cellScreenHalfWidth, cellScreenHalfHeight;
    float cellWorldWidth, cellWorldHeight;
    float cellWorldHalfWidth, cellWorldHalfHeight;

    bool showGrid;
    bool isMouseWithinScreen;

    GridEditUpdateFlag updateFlags = GridEditUpdateFlag.None;

    void Awake()
    {
        horizontalLines = new List<Image>();
        verticalLines = new List<Image>();

        HandleValidation();
        SetupGridHierarchy();
    }

    private void SetupGridHierarchy()
    {
        gridHorizontalRoot = new GameObject("Horizontal_root").AddComponent<RectTransform>();
        gridVerticalRoot = new GameObject("Vertical_root").AddComponent<RectTransform>();

        gridHorizontalRoot.SetParent(root);
        gridVerticalRoot.SetParent(root);
        gridHorizontalRoot.SetAsFirstSibling();
        gridVerticalRoot.SetAsFirstSibling();

        gridHorizontalRoot.localPosition = Vector3.zero;
        gridVerticalRoot.localPosition = Vector3.zero;
    }

    private void HandleValidation()
    {
        if (gridSetter == null)
        {
            Debug.LogError("GridEdit : No GridEditSetter to get data from, define a class that is derived");
        }

        if (root == null)
        {
            Debug.LogError("GridEdit : No rectTransform found for this package to belong to");
        }

        if (origIndicator == null)
        {
            Debug.LogWarning("GridEdit : No indicator for indicating central position of the grid, will be automatially created");
            origIndicator = new GameObject("centerIndicator").AddComponent<Image>();
            origIndicator.transform.SetParent(root);
            origIndicator.rectTransform.sizeDelta = new Vector2(50, 50);
        }

        if (mouseOverIndicator == null)
        {
            Debug.LogWarning("GridEdit : No mouseHover indicator, will be automatically created");
            mouseOverIndicator = new GameObject("mouseOverIndicator").AddComponent<Image>();
            mouseOverIndicator.transform.SetParent(root);
            mouseOverIndicator.rectTransform.sizeDelta = new Vector2(10, 10);
            mouseOverIndicator.GetComponent<Image>().color = Color.black;
        }

        var eventSystsem = FindObjectOfType<EventSystem>();
        var standaloneInputModule = FindObjectOfType<StandaloneInputModule>();

        if (eventSystsem == null ||
            standaloneInputModule == null)
        {
            Debug.LogWarning("GridEdit : EventSystem Or StandaloneInputModule not Found, will be automatically created");

            var t = new GameObject("EventSystem(createdByGridEditHandler");
            t.AddComponent<EventSystem>();
            t.AddComponent<StandaloneInputModule>();
        }

        origIndicator.gameObject.SetActive(false);
        origIndicator.transform.SetAsFirstSibling();
        mouseOverIndicator.gameObject.SetActive(false);
        mouseOverIndicator.transform.SetAsFirstSibling();
    }

    void Update()
    {
        if (showGrid == false)
        {
            updateFlags = GridEditUpdateFlag.None;
            return;
        }

        if(cam.IsCamDirty())
        {
            var camDirtyFlags = cam.GetCamDirtyFlags();

            if((camDirtyFlags & GridEditCameraDirtyFlag.OrthogonalSizeChange) != 0)
            {
                updateFlags |= GridEditUpdateFlag.Regenerate;
            }
            else if((camDirtyFlags & GridEditCameraDirtyFlag.CameraMove) != 0)
            {
                updateFlags |= GridEditUpdateFlag.Reposition;
            }

            cam.SetCamNotDirty();
        }
        else if((lastUpdatedMousePos.x != Input.mousePosition.x) ||
            (lastUpdatedMousePos.y != Input.mousePosition.y))
        {
            updateFlags |= GridEditUpdateFlag.MouseMove;
        }

        if (updateFlags != GridEditUpdateFlag.None)
        {
            if ((updateFlags & GridEditUpdateFlag.Regenerate) != 0)
            {
                SetGrid(gridSetter);
            }
            else if ((updateFlags & GridEditUpdateFlag.Reposition) != 0)
            {
                RepositionGrid();
            }
            else if ((updateFlags & GridEditUpdateFlag.MouseMove) != 0)
            {
                UpdateMouseOver(Input.mousePosition);
            }

            updateFlags = GridEditUpdateFlag.None;
        }
    }

    private void UpdateMouseOver(Vector2 pos)
    {
        lastUpdatedMousePos = pos;
        mouseOverIndicator.transform.position = GetCenterScreenPosOfTargetPos(pos);
    }

    void UpdateOriIndicator()
    {
        origIndicator.rectTransform.position = cam.WorldToScreen(origWorldPos);
    }

    void UpdateIndicatorsActive()
    {
        origIndicator.gameObject.SetActive(showGrid);
        mouseOverIndicator.gameObject.SetActive(showGrid);
    }

    Vector3 GetCenterScreenPosOfTargetPos(Vector3 targetScreenPos)
    {
        Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(targetScreenPos);
        Vector3 targetWorldCenterPos;

        float xDist = Mathf.Abs(targetWorldPos.x - origWorldPos.x);
        float yDist = Mathf.Abs(targetWorldPos.y - origWorldPos.y);

        int xSignDir = targetWorldPos.x > origWorldPos.x ? 1 : -1;
        int ySignDir = targetWorldPos.y > origWorldPos.y ? 1 : -1;

        targetWorldCenterPos.x = targetWorldPos.x - (xSignDir * (xDist % cellWorldWidth)) + (cellWorldHalfWidth * xSignDir);
        targetWorldCenterPos.y = targetWorldPos.y - (ySignDir * (yDist % cellWorldHeight)) + (cellWorldHalfHeight * ySignDir);
        targetWorldCenterPos.z = 0;

        return Camera.main.WorldToScreenPoint(targetWorldCenterPos);
    }

    public bool SetGrid(GridEditSetter gridSetter)
    {
        ReleaseGrid();

        float width = gridSetter.CellWidth();
        float height = gridSetter.CellHeight();

        if (width <= 0 ||
            height <= 0)
        {
            return false;
        }
        
        showGrid = true;

        origIndicator.gameObject.SetActive(true);
        origWorldPos = new Vector3(gridSetter.WorldOriginX(), gridSetter.WorldOriginY(), 0);
        cellWorldWidth = width;
        cellWorldHeight = height;
        cellWorldHalfWidth = cellWorldWidth * 0.5f;
        cellWorldHalfHeight = cellWorldHeight * 0.5f;
        origScreenPos = cam.WorldToScreen(origWorldPos);
        cellScreenWidth = (cam.WorldToScreen(new Vector3(cellWorldWidth, 0, 0)) -
            cam.WorldToScreen(new Vector3(0, 0, 0))).x;
        cellScreenHeight = (cam.WorldToScreen(new Vector3(0, cellWorldHeight)) -
            cam.WorldToScreen(new Vector3(0, 0, 0))).y;
        cellScreenHalfWidth = cellScreenWidth * 0.5f;
        cellScreenHalfHeight = cellScreenHeight * 0.5f;

        RefreshGridLineCount();
        RepositionGrid();
        UpdateIndicatorsActive();
        UpdateOriIndicator();

        return true;
    }

    void ReleaseGrid()
    {
        showGrid = false;

        foreach (MapGeneratorGUIGridLineDirection dir in Enum.GetValues(typeof(MapGeneratorGUIGridLineDirection)))
        {
            var targetList = GetGridList(dir);

            targetList.ForEach(t => Destroy(t.gameObject));
            targetList.Clear();
        }

        cam.SetCamNotDirty();
        updateFlags = GridEditUpdateFlag.None;

        UpdateIndicatorsActive();
    }

    void RefreshGridLineCount()
    {
        int horizontalRequiredCnt = (int)Mathf.Ceil(Screen.width / cellScreenWidth);
        int verticalRequiredCnt = (int)Mathf.Ceil(Screen.height / cellScreenHeight);

        RefreshGridSingleLineCount(horizontalRequiredCnt, MapGeneratorGUIGridLineDirection.Horizontal);
        RefreshGridSingleLineCount(verticalRequiredCnt, MapGeneratorGUIGridLineDirection.Vertical);
    }

    void RefreshGridSingleLineCount(
        int requiredLineCnt,
        MapGeneratorGUIGridLineDirection dir)
    {
        var targetList = GetGridList(dir);
        int createCnt = requiredLineCnt - targetList.Count;

        for (int i = 0; i < createCnt; i++)
        {
            CreateGridLine(dir);
        }
    }

    void RepositionGrid()
    {
        float leftSidePos = cam.GetCornerWorldPos(RectSide.LEFT);
        float bottomSidePos = cam.GetCornerWorldPos(RectSide.BOTTOM);

        float screenX = 0;

        // horizontal 부터 처리 . 
        if (origWorldPos.x < leftSidePos)
        {
            // Fix me : - 일떄는 ? leftSide pos - orig 저거를 absolute 값으로 해야하지않을까 ? 
            screenX = Camera.main.WorldToScreenPoint(new Vector3(leftSidePos + (cellWorldWidth - ((Mathf.Abs(leftSidePos - origWorldPos.x)) % cellWorldWidth)), 0, 0)).x;
        }
        else
        {
            screenX = Camera.main.WorldToScreenPoint(new Vector3(leftSidePos + (Mathf.Abs(origWorldPos.x - leftSidePos) % cellWorldWidth), 0, 0)).x;
        }

        Vector3 pos = new Vector3(screenX, Screen.height * 0.5f, 0);

        foreach (var item in horizontalLines)
        {
            item.rectTransform.position = pos;
            pos.x += cellScreenWidth;
        }

        float screenY = 0;

        // vertical 처리 
        if (origWorldPos.y < bottomSidePos)
        {
            screenY = Camera.main.WorldToScreenPoint(new Vector3(0, bottomSidePos + (cellWorldHeight - ((Mathf.Abs(bottomSidePos - origWorldPos.y)) % cellWorldHeight)), 0)).y;
        }
        else
        {
            screenY = Camera.main.WorldToScreenPoint(new Vector3(0, bottomSidePos + Mathf.Abs(origWorldPos.y - bottomSidePos) % cellWorldHeight, 0)).y;
        }

        pos = new Vector3(Screen.width * 0.5f, screenY, 0);

        foreach (var item in verticalLines)
        {
            item.rectTransform.position = pos;
            pos.y += cellScreenHeight;
        }

        UpdateMouseOver(Input.mousePosition);
        UpdateOriIndicator();
    }

    Image CreateGridLine(MapGeneratorGUIGridLineDirection direction)
    {
        var targetLineList = GetGridList(direction);
        var result = new GameObject(targetLineList.Count.ToString()).AddComponent<Image>();

        result.transform.SetParent(GetGridRoot(direction));
        result.rectTransform.sizeDelta = new Vector2(direction == MapGeneratorGUIGridLineDirection.Horizontal ? Screen.height : Screen.width, baseLineThickness);
        result.rectTransform.rotation = direction == MapGeneratorGUIGridLineDirection.Horizontal ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;
        result.color = gridColor;

        targetLineList.Add(result);

        return result;
    }

    List<Image> GetGridList(MapGeneratorGUIGridLineDirection dir)
    {
        return dir == MapGeneratorGUIGridLineDirection.Horizontal ? horizontalLines : verticalLines;
    }

    RectTransform GetGridRoot(MapGeneratorGUIGridLineDirection dir)
    {
        return dir == MapGeneratorGUIGridLineDirection.Horizontal ? gridHorizontalRoot : gridVerticalRoot;
    }

    public void SetActiveGUI(bool active)
    {
        root.gameObject.SetActive(active);
    }

    public Vector3 GetCellScreenPos(Vector2 screenPos)
    {
        if (showGrid == false)
            return Vector3.zero;

        return cam.WorldToScreen(GetCellWorldPos(screenPos));
    }

    public Vector3 GetCellWorldPos(Vector2 screenPos)
    {
        if (showGrid == false)
            return Vector3.zero;

        Vector3 targetWorldPos = cam.ScreenToWorld(screenPos);
        Vector3 targetWorldCenterPos;

        float xDist = Mathf.Abs(targetWorldPos.x - origWorldPos.x);
        float yDist = Mathf.Abs(targetWorldPos.y - origWorldPos.y);

        int xSignDir = targetWorldPos.x > origWorldPos.x ? 1 : -1;
        int ySignDir = targetWorldPos.y > origWorldPos.y ? 1 : -1;

        targetWorldCenterPos.x = targetWorldPos.x - (xSignDir * (xDist % cellWorldWidth)) + (cellWorldHalfWidth * xSignDir);
        targetWorldCenterPos.y = targetWorldPos.y - (ySignDir * (yDist % cellWorldHeight)) + (cellWorldHalfHeight * ySignDir);
        targetWorldCenterPos.z = 0;

        return targetWorldCenterPos;
    }

    public bool IsGridShowing()
    {
        return showGrid;
    }
}
