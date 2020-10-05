using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileParams : MonoBehaviour
{
    private int tileVariantID;
    private Vector2Int gridPos;

    private Action<int, Vector2Int> onClick;

    public void Setup(Action<int, Vector2Int> onClick, int tileVariantID, Vector2Int gridPos)
    {
        GetComponent<Button>().onClick.AddListener(Click);

        this.onClick = onClick;
        this.tileVariantID = tileVariantID;
        this.gridPos = gridPos;
    }

    void Click()
    {
        onClick?.Invoke(tileVariantID,gridPos);
    }
}
