using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public Visualiser visualiser;

    private System.Random rng;
    private Grid grid;
    public void Generate(int gridSize)
    {
        rng = new System.Random();

        TileConstraintsData data = DataSerializationUtility.Load<TileConstraintsData>(TileConstraintsData.pathEnd);

        Grid grid = new Grid(data, gridSize);

        const int tryCount = 1000;
        int tries = 0;
        while (tries < tryCount && !grid.CollapseAll(rng))
        {
            tries++;

        }
        Debug.Log("try count " + tries);

        visualiser.VisualiseGrid(grid);

    }

    
    public void GenerationDemo(int gridSize)
    {
        rng = new System.Random();
        TileConstraintsData data = DataSerializationUtility.Load<TileConstraintsData>(TileConstraintsData.pathEnd);

        grid = new Grid(data, gridSize);
        visualiser.DisplayOptionPool(grid, VariantSelect);
    }

  

    void VariantSelect(int id, Vector2Int pos)
    {
        grid.SelectOneVariant(id, pos);
        visualiser.DisplayOptionPool(grid, VariantSelect);
    }


   
}
