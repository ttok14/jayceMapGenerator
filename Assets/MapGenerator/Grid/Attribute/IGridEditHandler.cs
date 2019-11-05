using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridEditHandler 
{
    bool SetGrid(GridEditSetter setter);
    bool IsGridShowing();
    Vector3 GetCellScreenPos(Vector2 screenPos);
    Vector3 GetCellWorldPos(Vector2 screenPos);
}
