using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObjectPaletteItemGUI : MonoBehaviour
{
    public Image itemIcon;
    public Text itemName;

    public void SetInfo(Sprite itemSprite, string name)
    {
        itemIcon.sprite = itemSprite;
        itemName.text = name;
    }
}
