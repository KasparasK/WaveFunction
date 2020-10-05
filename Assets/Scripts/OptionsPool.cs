using System.Collections.Generic;
using UnityEngine;

public class OptionsPool
{
    public List<Tile> allowed;

    public OptionsPool(List<Tile> allowed)
    {
        this.allowed = new List<Tile>(allowed);
    }

    public bool Change(List<Tile> allPossibleTiles, int dirID)
    {
        bool changedAny = false;
        List<int> toRemove = new List<int>();
        List<int> toKeep = new List<int>();

        for (int i = 0; i < allPossibleTiles.Count; i++)
        {
            for (int j = 0; j < allowed.Count; j++)
            {
                if (!allowed[j].AreConflicting(allPossibleTiles[i].constraints[dirID], GetOppositeDirID(dirID)))
                {
                    if (!toKeep.Contains(j))
                    {
                        toKeep.Add(j);
                    }
                    if (toRemove.Contains(j))
                        toRemove.Remove(j);
                }
                else
                {

                    if (!toKeep.Contains(j) && !toRemove.Contains(j))
                        toRemove.Add(j);
                }
            }
        }

        if (toRemove.Count > 0)
            changedAny = true;

        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            allowed.RemoveAt(toRemove[i]);
        }

        return changedAny;
    }

    int GetOppositeDirID(int originDirID)
    {
        switch (originDirID)
        {
            case 0:
                return 2;
            case 1:
                return 3;
            case 2:
                return 0;
            case 3:
                return 1;
        }
        Debug.LogError("wrong direction id : " + originDirID);
        return -1;
    }

    public int GetEntropy()
    {
        return allowed.Count;
    }
}
