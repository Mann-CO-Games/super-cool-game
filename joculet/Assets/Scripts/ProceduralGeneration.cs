 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


 public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] private GameObject ground;
    [SerializeField] private int height;

    [SerializeField] private int width;
    private List<GameObject> groundTiles = new List<GameObject>();
    private List<GameObject> path =  new List<GameObject>();
    private void Generate(int height, int width)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < this.width; x++)
                groundTiles.Add(Instantiate(ground, new Vector2(x, y), Quaternion.identity));
        }

        var topEdges = getTopEdge(this.height, this.width);
        var bottomEdges = getBottomEdge(this.height, width);

        int startTile = Random.Range(0, bottomEdges.Count - 1);
        int endTile = Random.Range(width*(this.height-1), width*height);
        Destroy(groundTiles[startTile]);
        Destroy(groundTiles[endTile]);
    }

    private List<GameObject> getTopEdge(int height, int width)
    {
        var topEdges = new List<GameObject>();

        for (int i = width * (height - 1); i < width * height; i++)
        {
            topEdges.Add(groundTiles[i]);
        }

        return topEdges;
    }

    private List<GameObject> getBottomEdge(int height, int width)
    {
        var bottomEdges = new List<GameObject>();
        
        for(int i=0;i<this.width;i++)
            bottomEdges.Add(groundTiles[i]);

        return bottomEdges;
    }

    void Start()
    {
        Generate(height,width);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
