using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Visualiser : MonoBehaviour
{
    private GameObject parent;

    public Transform canvas;
    public GameObject imagePref;
    public GameObject gridParentPref;

    private int textureSideLength;
    private int gridSize;
    private List<Texture2D> tiles;
    public void Setup(int gridSize, int textureSideLength, List<Texture2D> tiles)
    {
        this.gridSize = gridSize;
        this.textureSideLength = textureSideLength;
        this.tiles = tiles;
    }


    void ResetCanvasParent()
    {
        if (Application.isPlaying)
            Destroy(parent);
        else
            DestroyImmediate(parent);

        parent = new GameObject("parent");
        parent.transform.SetParent(canvas);
        parent.transform.localPosition = Vector3.zero;
        parent.transform.localScale = new Vector2(1, 1);
    }

    public void VisualiseConstraints(TileConstraintsData data, List<List<int>> colorSamplePos)
    {
        ResetCanvasParent();

        for (int j = 0; j < data.tiles.Count; j++)
        {
            Vector2 startPos = new Vector2(j * textureSideLength, 0);

            GameObject temp = Instantiate(imagePref, parent.transform);
            temp.transform.localPosition = startPos;

            Image image = temp.GetComponent<Image>();
            Texture2D tex = GetFlatColorTexture(Color.green, textureSideLength);

            for (int i = 0; i < colorSamplePos.Count; i++)
            {
                tex.SetPixel(colorSamplePos[i][0] % textureSideLength, colorSamplePos[i][0] / textureSideLength, data.tiles[j].constraints[i].col1);
                tex.SetPixel(colorSamplePos[i][1] % textureSideLength, colorSamplePos[i][1] / textureSideLength, data.tiles[j].constraints[i].col2);
                tex.SetPixel(colorSamplePos[i][2] % textureSideLength, colorSamplePos[i][2] / textureSideLength, data.tiles[j].constraints[i].col3);

            }

            tex.Apply(false);

            image.sprite = Sprite.Create(tex, new Rect(0, 0, textureSideLength, textureSideLength), new Vector2(0.5f, 0.5f));
        }


    }

    public void VisualiseTileVariants(TileConstraintsData data)
    {
        for (int i = 0; i < data.tiles.Count; i++)
        {
            Vector2 startPos = new Vector2(i * textureSideLength, 0);
            GameObject temp = Instantiate(imagePref,parent.transform);
            temp.transform.localPosition = startPos;

            temp.GetComponent<Image>().sprite = Sprite.Create(tiles[data.tiles[i].tileID], new Rect(0, 0, textureSideLength, textureSideLength), new Vector2(0.5f, 0.5f));

            temp.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90 * data.tiles[i].rotID));

        }
    }

    public void VisualiseGrid(Grid grid)
    {
        ResetCanvasParent();


        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2 startPos = new Vector2(x * textureSideLength, y * textureSideLength);

                if (grid.grid[x][y].allowed.Count == 1)
                {
                    Tile toSpawn = grid.grid[x][y].allowed[0];

                    CreateTile(startPos, toSpawn.tileName, tiles[toSpawn.tileID], toSpawn.rotID, textureSideLength);
                }
                else if (grid.grid[x][y].allowed.Count == 0)
                {
                    CreateTile(startPos, "empty", GetFlatColorTexture(Color.green, textureSideLength), 0, textureSideLength);
                    // Debug.LogWarning("EMPTY TILE "+ x+ " ; "+ y);
                }
                else
                {
                    CreateTile(startPos, "more than 2", GetFlatColorTexture(Color.red, textureSideLength), 0, textureSideLength);

                }

            }
        }
    }

    public void DisplayOptionPool(Grid grid, Action<int,Vector2Int> clickAction)
    {
        ResetCanvasParent();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2 startPos = new Vector2(x * textureSideLength, y * textureSideLength);
                GameObject variantsGrid = Instantiate(gridParentPref, parent.transform);
                variantsGrid.transform.localPosition = startPos;

                if (grid.grid[x][y].allowed.Count == 1)
                {
                    GridLayoutGroup gridLayoutGroup = variantsGrid.GetComponent<GridLayoutGroup>();
                    gridLayoutGroup.cellSize = new Vector2(textureSideLength, textureSideLength);
                    gridLayoutGroup.spacing = Vector2.zero;

                    SpawnTileForDemo(grid.grid[x][y].allowed[0], variantsGrid.transform, startPos);
                }
                
                for (int i = 0; i < grid.grid[x][y].allowed.Count; i++)
                {
                    GameObject variant = SpawnTileForDemo(grid.grid[x][y].allowed[i], variantsGrid.transform, startPos);
                    variant.AddComponent<TileParams>().Setup(clickAction, i, new Vector2Int(x, y));
                }
            }
        }

    }

    GameObject SpawnTileForDemo(Tile toSpawn,Transform parent, Vector2 startPos)
    {
        GameObject variant = CreateTile(startPos, toSpawn.tileName, tiles[toSpawn.tileID], toSpawn.rotID, textureSideLength);
        variant.transform.SetParent(parent);
        return variant;
    }

    GameObject CreateTile(Vector2 startPos, string name, Texture2D tex, int rotID, float size)
    {
        GameObject temp = Instantiate(imagePref, parent.transform);
        temp.transform.localPosition = startPos;
        temp.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        temp.name = name;
        temp.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90 * rotID));

        return temp;
    }

    Texture2D GetFlatColorTexture(Color color, int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                tex.SetPixel(x, y, color);
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        return tex;
    }
}
