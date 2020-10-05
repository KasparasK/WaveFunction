using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileConstraintsData
{
    public const string pathEnd = "/TileConstraintsData.json";

    [SerializeField]
    public List<Tile> tiles;

    public TileConstraintsData()
    {
        tiles = new List<Tile>();
    }
}
[Serializable]
public class Tile
{
    [SerializeField]
    public string tileName;
    [SerializeField]
    public int tileID;
    [SerializeField] public int rotID;
    [SerializeField] public List<BorderConstraints> constraints;

    public Tile()
    {
        constraints = new List<BorderConstraints>();
    }
    public Tile(Tile toCopy)
    {
        tileName = toCopy.tileName;
        tileID = toCopy.tileID;
        rotID = toCopy.rotID;
        constraints = new List<BorderConstraints>(toCopy.constraints);
    }
    public bool isSymetrical()
    {
        if (constraints.Count < 4)
        {
            Debug.LogWarning("CONSTRAINTS NOT SETUP");
            return false;
        }

        if (AreEqual(constraints[0].col1, constraints[1].col3, constraints[2].col3, constraints[3].col3)) 
            if(AreEqual(constraints[0].col3, constraints[1].col1, constraints[2].col1, constraints[3].col1))
                if (AreEqual(constraints[0].col2, constraints[1].col2, constraints[2].col2, constraints[3].col2))
                    return true;

        return false;
    }

    bool AreEqual(Vector4 x, Vector4 y, Vector4 z, Vector4 t)
    {
        return ((x == y) && (x == z) && (x == t));
    }

    public bool AreConflicting(BorderConstraints other, int id)
    {

        if ((other.col1 == constraints[id].col1) && (other.col2 == constraints[id].col2) &&
            (other.col3 == constraints[id].col3))
        {
          //  Debug.Log("equal");
            return false;

        }
       // Debug.Log("not equal");
        return true;
    }

    public void RotateClockwise()
    {
        List<BorderConstraints> constraintsRot  = new List<BorderConstraints>();

        constraintsRot.Add(new BorderConstraints(constraints[3]));
        constraintsRot.Add(new BorderConstraints(constraints[0].col3, constraints[0].col2, constraints[0].col1));
        constraintsRot.Add(new BorderConstraints(constraints[1]));
        constraintsRot.Add(new BorderConstraints(constraints[2].col3, constraints[2].col2, constraints[2].col1));

        constraints = constraintsRot;
        rotID++;
    }
}

[Serializable]
public class BorderConstraints
{
    public BorderConstraints(BorderConstraints other)
    {
        col1 = other.col1;
        col2 = other.col2;
        col3 = other.col3;

    }
    public BorderConstraints(Vector4 col1, Vector4 col2, Vector4 col3)
    {
        this.col1 = col1;
        this.col2 = col2;
        this.col3 = col3;
    }

    [SerializeField]
    public Vector4 col1;
    [SerializeField]
    public Vector4 col2;
    [SerializeField]
    public Vector4 col3;
}