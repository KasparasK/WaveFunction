using UnityEngine;

public class Neighbour
{
    public Neighbour(Vector2Int pos, int dirId)
    {
        this.pos = pos;
        this.dirId = dirId;
        exists = true;
    }
    public Neighbour()
    {
        exists = false;
    }
    public Vector2Int pos;
    public int dirId;
    public bool exists;
}
