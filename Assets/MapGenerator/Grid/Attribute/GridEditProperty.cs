public enum MapGeneratorGUIGridLineDirection
{
    Horizontal,
    Vertical
}

public enum RectSide
{
    LEFT,
    TOP,
    RIGHT,
    BOTTOM
}

public enum GridEditUpdateFlag
{
    None = 0,
    MouseMove = 0x1,
    Reposition = 0x1 << 1,
    Regenerate = 0x1 << 2,
}