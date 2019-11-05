using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * class to be accessed from GridEditHandler to get actualy data for grid 
 * 그리드를 생성할때 데이터를 가져올 인터페이스 
 * */
[System.Serializable]
public abstract class GridEditSetter : MonoBehaviour
{
    abstract public void SetGrid(float worldOriPosX, float worldOriPosY, float cellWidth, float cellHeight);

    abstract public float WorldOriginX();
    abstract public float WorldOriginY();
    abstract public float CellWidth();
    abstract public float CellHeight();
}
