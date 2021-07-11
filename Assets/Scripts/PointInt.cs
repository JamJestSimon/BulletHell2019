using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInt
{
    public int x, y;
    public PointInt(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public PointInt(float x, float y)
    {
        this.x = (int)x;
        this.y = (int)y;
    }
}
