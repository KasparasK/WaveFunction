using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
public class ConstrainsCreator : MonoBehaviour
{

    public Visualiser visualiser;

    private int step;

    private List<List<int>> colorSamplePos;
    private int textureSideLength;
    public void Create(int textureSideLength, List<Texture2D> tiles)
    {
        this.textureSideLength = textureSideLength;

        TileConstraintsData data = new TileConstraintsData();
        colorSamplePos = GetSamplePos();
        step = textureSideLength / 4;

        for (int i = 0; i < tiles.Count; i++)
        {
            ProcessTile(tiles[i], i, ref data);
        }

        DataSerializationUtility.Save(data,TileConstraintsData.pathEnd);

        visualiser.VisualiseConstraints(data, colorSamplePos);
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

  

   
}


