using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridEditCameraDirtyFlag
{
    None = 0,
    CameraMove = 0x1,
    OrthogonalSizeChange = 0x1 << 1
}

public interface IGridEditCamera
{
    Vector3 GetCamPos();
    float GetCornerWorldPos(RectSide corner);
    GridEditCameraDirtyFlag GetCamDirtyFlags();
    void SetCamNotDirty();
    Vector2 ScreenToWorld(Vector2 screenPos);
    Vector2 WorldToScreen(Vector3 worldPos);
}
