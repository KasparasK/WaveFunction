using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionController : MonoBehaviour
{
    public Visualiser visualiser;
    public Generator generator;
    public ConstrainsCreator constrainsCreator;

    public List<Texture2D> tiles;

    private const int textureSideLength = 32;
    public int gridSize = 3;


    public void CreateConstraints()
    {
        visualiser.Setup(gridSize, textureSideLength, tiles);
        constrainsCreator.Create(textureSideLength, tiles);
    }

    public void AutoGenerate()
    {
        visualiser.Setup(gridSize, textureSideLength, tiles);
        generator.Generate(gridSize);

    }
    public void GenerationDemo()
    {
        visualiser.Setup(gridSize, textureSideLength, tiles);
        generator.GenerationDemo(gridSize);

    }
}
