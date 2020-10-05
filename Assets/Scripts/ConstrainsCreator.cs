using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
public class ConstrainsCreator : MonoBehaviour
{
    public List<Texture2D> tiles;
    private const int textureSideLength = 32;
    private int step;

    public Transform canvas;
    public GameObject imagePref;
    private GameObject parent;

    public int gridSize = 3;

    private List<List<int>> colorSamplePos;
    private System.Random rng;
    public void Create()
    {


        TileConstraintsData data = new TileConstraintsData();
        colorSamplePos = GetSamplePos();
        step = textureSideLength / 4;

        DestroyImmediate(parent);
        parent = new GameObject("parent");
        parent.transform.SetParent(canvas);


        for (int i = 0; i < tiles.Count; i++)
        {
            ProcessTile(tiles[i], i, ref data);
        }



        DataSerializationUtility.Save(data,TileConstraintsData.pathEnd);

        VisualiseConstraints(data);
        //VisualiseTileVariants(data);
    }

    List<List<int>> GetSamplePos()
    {
        int pos1 = 0;
        int pos2 = 0;
        int pos3 = 0;

        List<List<int>>  pos = new List<List<int>>();
        for (int i = 0; i < 4; i++)
        {
            pos.Add(new List<int>());
            switch (i)
            {
                case 0:    //bot
                    pos1 = step - 1;
                    pos2 = step * 2 - 1;
                    pos3 = step * 3 - 1;
                    break;
                case 1: //left
                    pos1 = textureSideLength * step - textureSideLength;
                    pos2 = textureSideLength * step * 2 - textureSideLength;
                    pos3 = textureSideLength * step * 3 - textureSideLength;
                    break;
                case 2: //top
                    pos1 = textureSideLength * textureSideLength - textureSideLength + step - 1;
                    pos2 = textureSideLength * textureSideLength - textureSideLength + step * 2 - 1;
                    pos3 = textureSideLength * textureSideLength - textureSideLength + step * 3 - 1;
                    break;
                case 3: //right
                    pos1 = textureSideLength * (step) - 1;
                    pos2 = textureSideLength * (step) * 2 - 1;
                    pos3 = textureSideLength * (step) * 3 - 1;
                    break;

            }
            pos[i].Add(pos1);
            pos[i].Add(pos2);
            pos[i].Add(pos3);
        }

        return pos;
    }

    void ProcessTile(Texture2D img, int tileID,ref TileConstraintsData data )
    {
        Color [] pixels = img.GetPixels();

        Tile tile = new Tile();

        for (int i = 0; i < 4; i++)
        {
            tile.constraints.Add(new BorderConstraints(pixels[colorSamplePos[i][0]], pixels[colorSamplePos[i][1]], pixels[colorSamplePos[i][2]]));
        }
        
        tile.rotID = 0;
        tile.tileID = tileID;
        tile.tileName = tileID.ToString();
        data.tiles.Add(tile);

        if (!tile.isSymetrical())
            RotateConstraints( ref data, tile);

    }

    void RotateConstraints( ref TileConstraintsData data, Tile tileToRot)
    {
        Tile rotatedTile = new Tile(tileToRot);
        for (int i = 1; i <= 3; i++)
        {
            rotatedTile.RotateClockwise();
            rotatedTile.tileName = tileToRot.tileID + "-" + rotatedTile.rotID;
            data.tiles.Add(new Tile(rotatedTile));
        }
    }

    void VisualiseConstraints(TileConstraintsData data)
    {
        for (int j = 0; j < data.tiles.Count; j++)
        {
            Vector2 startPos = new Vector2(j * textureSideLength, 0);
            GameObject temp = Instantiate(imagePref, startPos, Quaternion.identity, parent.transform);

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

    void VisualiseTileVariants(TileConstraintsData data)
    {
        for (int i = 0; i < data.tiles.Count; i++)
        {
            Vector2 startPos = new Vector2(i * textureSideLength, 0);
            GameObject temp = Instantiate(imagePref, startPos, Quaternion.identity, parent.transform);

            temp.GetComponent<Image>().sprite = Sprite.Create(tiles[data.tiles[i].tileID], new Rect(0, 0, textureSideLength, textureSideLength), new Vector2(0.5f, 0.5f));

            temp.transform.rotation = Quaternion.Euler(new Vector3(0,0,-90* data.tiles[i].rotID));

        }
    }

   public void Generate()
    {
        DestroyImmediate(parent);
        parent = new GameObject("parent");
        parent.transform.SetParent(canvas);
        rng = new System.Random(0);

        TileConstraintsData data = DataSerializationUtility.Load<TileConstraintsData>(TileConstraintsData.pathEnd);

        Grid grid = new Grid(data, gridSize);

        const int tryCount = 1000;
        int tries = 0;
        while (tries < tryCount && !grid.CollapseAll(rng))
        {
            tries++;

        }
        Debug.Log("try count "+tries);
     
       VisualiseGrid(grid);

    }

   void VisualiseGrid(Grid grid)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2 startPos = new Vector2(x * textureSideLength, y * textureSideLength);

                if (grid.grid[x][y].allowed.Count == 1)
                {
                    Tile toSpawn = grid.grid[x][y].allowed[0];

                    CreateTile(startPos, toSpawn.tileName, tiles[toSpawn.tileID], toSpawn.rotID);
                }
                else if(grid.grid[x][y].allowed.Count == 0)
                {
                    CreateTile(startPos, "empty", GetFlatColorTexture(Color.green, textureSideLength), 0);
                   // Debug.LogWarning("EMPTY TILE "+ x+ " ; "+ y);
                }
                else
                {
                    CreateTile(startPos, "more than 2", GetFlatColorTexture(Color.red, textureSideLength), 0);

                }

            }
        }
   }

   GameObject CreateTile(Vector2 startPos,string name,Texture2D tex,int rotID)
   {
       GameObject temp = Instantiate(imagePref, startPos, Quaternion.identity, parent.transform);
       temp.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, textureSideLength, textureSideLength), new Vector2(0.5f, 0.5f));
       temp.name = name;
       temp.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90 * rotID));

       return temp;
   }

   Texture2D GetFlatColorTexture(Color color, int size)
   {
        Texture2D tex = new Texture2D(size,size, TextureFormat.ARGB32, false);
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

public class PosAndColor
{
    public PosAndColor(Color col, int pos)
    {
        color = col;
        this.pos = pos;
    }
    public Color color;
    public int pos;
}


public class Grid
{
   public List<List<OptionsPool>> grid = new List<List<OptionsPool>>();
    private int _gridSize;
    public Grid(TileConstraintsData data, int gridSize)
    {
        _gridSize = gridSize;
        for (int x = 0; x < _gridSize; x++)
        {
            grid.Add(new List<OptionsPool>());
            for (int y = 0; y < _gridSize; y++)
            {
                grid[x].Add(new OptionsPool(data.tiles));
            }
        }
    }

    public bool CollapseAll(System.Random rng)
    {
        const int maxTryCount = 1000;
        int tryCount = 0;
        bool contradiction = false;

        while (UncolapsedLeft(ref contradiction) && tryCount <= maxTryCount)
        {
            if (contradiction)
                return false;

            Vector2Int pos;
            if (tryCount == 0)
                pos = new Vector2Int(rng.Next(0, _gridSize), rng.Next(0, _gridSize));
            else
                pos = FindLowestEntropy();


            ChoseRandomOption(pos, rng);
            CollapseAdjasent(pos);

            tryCount++;
        }
        Debug.Log("iterations: "+tryCount);

        return true;
    }

    Vector2Int FindLowestEntropy()
    {
        Vector2Int lowestPos = Vector2Int.zero;
        int lowestEntropy = grid[lowestPos.x][lowestPos.y].GetEntropy();

        bool firstSet = false;

        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                int tempEntropy = grid[x][y].GetEntropy();
                if (!firstSet)
                {
                    if (tempEntropy >= 2)
                    {
                        lowestPos = new Vector2Int(x, y);
                        lowestEntropy = tempEntropy;
                        firstSet = true;
                    }
                }
                else
                {
                    if (tempEntropy < lowestEntropy && tempEntropy >= 2)
                    {
                        lowestPos = new Vector2Int(x, y);
                        lowestEntropy = tempEntropy;
                    }
                }

               
              
            }
        }
        Debug.Log(lowestEntropy);
        return lowestPos;
    }

    bool UncolapsedLeft(ref bool contradiction)
    {
      /*  for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                if (grid[x][y].GetEntropy() == 0)
                {
                    return false;
                }
            }
        }
        */
        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                int entropy = grid[x][y].GetEntropy();
                if (entropy > 1)
                {
                    return true;
                }

                if (entropy == 0)
                {
                    contradiction = true;
                    return true;

                }
            }
        }
        Debug.Log("all collaped");
        return false;
    }

    void ChoseRandomOption(Vector2Int pos, System.Random rng)
    {
        int rngID = rng.Next(0, grid[pos.x][pos.y].allowed.Count);
        Tile temp = new Tile(grid[pos.x][pos.y].allowed[rngID]);
        grid[pos.x][pos.y].allowed = new List<Tile>();
        grid[pos.x][pos.y].allowed.Add(temp);
    }

    void CollapseAdjasent(Vector2Int start)
    {
        Stack<Vector2Int> toCheck = new Stack<Vector2Int>();
        toCheck.Push(start);

        int checksLimit = 1;
        int checksCount = 0;
        while (toCheck.Count > 0)
        //for (int c = 0; c < checksLimit; c++)
        {
            Vector2Int pos = toCheck.Pop();
            List<Neighbour> neighbours = GetNeighbours(pos);
     
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours[i].exists)
                {
                    Vector2Int neighPos = neighbours[i].pos;
                 //   Debug.Log(grid[neighPos.x][neighPos.y].allowed.Count);
                    if(grid[pos.x][pos.y].allowed.Count > 0)
                        if (grid[neighPos.x][neighPos.y].Change(grid[pos.x][pos.y].allowed[0].constraints[i], i))
                        {
                            if(grid[neighPos.x][neighPos.y].allowed.Count > 0)
                            toCheck.Push(neighPos);
                        }

                }
            }

            checksCount++;
        }
        Debug.Log(checksCount);
    }

    List<Neighbour> GetNeighbours(Vector2Int pos)
    {
        List<Neighbour> neighbours = new List<Neighbour>();

        if (pos.y - 1 >= 0)//bot
            neighbours.Add(new Neighbour(new Vector2Int(pos.x, pos.y - 1), 0));
        else
            neighbours.Add(new Neighbour());

        if (pos.x-1 >= 0) //left
            neighbours.Add(new Neighbour(new Vector2Int(pos.x - 1, pos.y),1));
        else
            neighbours.Add(new Neighbour());

        if (pos.y + 1 < _gridSize) //top
            neighbours.Add(new Neighbour(new Vector2Int(pos.x, pos.y + 1), 2));
        else
            neighbours.Add(new Neighbour());

        if (pos.x + 1 < _gridSize) //right
            neighbours.Add(new Neighbour(new Vector2Int(pos.x + 1, pos.y), 3));
        else
            neighbours.Add(new Neighbour());
        return neighbours;
    }
}
public class OptionsPool
{
    public List<Tile> allowed;

    public OptionsPool(List<Tile> allowed)
    {
        this.allowed = new List<Tile>(allowed);
    }

    public bool Change(BorderConstraints other, int dirID)
    {
        bool changedAny = false;
        List<int> toRemove = new List<int>();

        for (int i = 0; i < allowed.Count; i++)
        {
            if (allowed[i].AreConflicting(other, dirID))
            {
                toRemove.Add(i);
                changedAny = true;
            }
        }

        for (int i = toRemove.Count -1; i >= 0; i--)
        {
            allowed.RemoveAt(toRemove[i]);
        }

        return changedAny;
    }

    public int GetEntropy()
    {
        return allowed.Count;
    }
}

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