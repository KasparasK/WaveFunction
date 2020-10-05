using System.Collections.Generic;
using UnityEngine;

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


            int id = ChoseRandomOption(pos, rng);
            LeaveOnlySelectedVariant(pos, id);
            CollapseAdjasent(pos);

            tryCount++;
        }
        Debug.Log("iterations: " + tryCount);

        return true;
    }

    public void SelectOneVariant(int id, Vector2Int pos)
    {
        LeaveOnlySelectedVariant(pos, id);
        CollapseAdjasent(pos);
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

    int ChoseRandomOption(Vector2Int pos, System.Random rng)
    {
        return rng.Next(0, grid[pos.x][pos.y].allowed.Count);

    }

    void LeaveOnlySelectedVariant(Vector2Int pos, int id)
    {
        Tile temp = new Tile(grid[pos.x][pos.y].allowed[id]);
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
                    Debug.Log(i);
                    Vector2Int neighPos = neighbours[i].pos;

                    if (grid[pos.x][pos.y].allowed.Count > 0)
                    {
                        bool changedAny = grid[neighPos.x][neighPos.y]
                            .Change(grid[pos.x][pos.y].allowed, i);
                        if (changedAny) // if any possibilieties were deleted
                        {
                            if (grid[neighPos.x][neighPos.y].allowed.Count > 0)
                                toCheck.Push(neighPos);
                        }
                    }


                }
            }

            checksCount++;
        }
    }

    List<Neighbour> GetNeighbours(Vector2Int pos)
    {
        List<Neighbour> neighbours = new List<Neighbour>();

        if (pos.y - 1 >= 0)//bot
            neighbours.Add(new Neighbour(new Vector2Int(pos.x, pos.y - 1), 0));
        else
            neighbours.Add(new Neighbour());

        if (pos.x - 1 >= 0) //left
            neighbours.Add(new Neighbour(new Vector2Int(pos.x - 1, pos.y), 1));
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



