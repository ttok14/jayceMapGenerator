using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridEditCameraHandler : MonoBehaviour, IGridEditCamera
{
    public Camera cam;

    public float moveSpeed = 3;
    public float speedAccelerationMultiplier = 7;

    GridEditCameraDirtyFlag dirtyFlag = GridEditCameraDirtyFlag.None;

    Vector3 lastUpdatedCameraPos;

    float[] cornerWorldPos;
    float lastUpdateOrthogonalSize = -1;

    void Awake()
    {
        if (cam == null)
        {
            Debug.LogError("GridEditCameraHandler : No camera found");
        }

        var cams = FindObjectsOfType<Camera>();

        if(cams != null && 
            cams.Length > 1)
        {
            Debug.LogWarning("GridEditCameraHandler : Multiple Cam found, this package has its own camera, so might be better to delete existing scene camera");
        }

        lastUpdateOrthogonalSize = cam.orthographicSize;

        cornerWorldPos = new float[(Enum.GetValues(typeof(RectSide)).Length)];

        UpdateCornerPos();
    }

    void Update()
    {
        if (cam == null)
            return;

        float multiplier = Input.GetKey(KeyCode.LeftShift) ? speedAccelerationMultiplier : 1;

        cam.transform.Translate(Input.GetAxis("Horizontal") * moveSpeed * multiplier * Time.deltaTime, 0, 0);
        cam.transform.Translate(0, Input.GetAxis("Vertical") * moveSpeed * multiplier * Time.deltaTime, 0);

        if (lastUpdatedCameraPos != cam.transform.position)
        {
            lastUpdatedCameraPos = cam.transform.position;
            dirtyFlag |= GridEditCameraDirtyFlag.CameraMove;
        }

        if(lastUpdateOrthogonalSize != cam.orthographicSize)
        {
            lastUpdateOrthogonalSize = cam.orthographicSize;
            dirtyFlag |= GridEditCameraDirtyFlag.OrthogonalSizeChange;
        }

        if (IsCamDirty())
        {
            UpdateCornerPos();
        }
    }

    void UpdateCornerPos()
    {
        Vector3 zeroWorldPos = cam.ScreenToWorldPoint(Vector3.zero);

        cornerWorldPos[(int)RectSide.LEFT] = zeroWorldPos.x;
        cornerWorldPos[(int)RectSide.RIGHT] = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        cornerWorldPos[(int)RectSide.TOP] = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        cornerWorldPos[(int)RectSide.BOTTOM] = zeroWorldPos.y;
    }

    public Vector3 GetCamPos()
    {
        return cam.transform.position;
    }

    public float GetCornerWorldPos(RectSide corner)
    {
        return cornerWorldPos[(int)corner];
    }

    public bool IsCamDirty()
    {
        return dirtyFlag != GridEditCameraDirtyFlag.None;
    }

    public void SetCamNotDirty()
    {
        dirtyFlag = GridEditCameraDirtyFlag.None;
    }

    public Vector2 ScreenToWorld(Vector2 screenPos)
    {
        return cam.ScreenToWorldPoint(screenPos);
    }

    public Vector2 WorldToScreen(Vector3 worldPos)
    {
        return cam.WorldToScreenPoint(worldPos);
    }

    public float GetOrthogonalSize()
    {
        return cam.orthographicSize;
    }

    public GridEditCameraDirtyFlag GetCamDirtyFlags()
    {
        return dirtyFlag;
    }
}
