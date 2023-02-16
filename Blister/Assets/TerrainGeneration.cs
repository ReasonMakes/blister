using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    //Declare variables
    public int width = 512;     //x
    public int depth = 20;      //y
    public int height = 512;    //z
    public float scale = 20f;
    public float offsetX = 100f;
    public float offsetY = 100f;
    
    //Generate a random offset for x and y when launched
    private void Start()
    {
        offsetX = Random.Range(0, 9999f);        
        offsetY = Random.Range(0, 9999f);
    }

    //Update terrain data and have it move through offsets of x and y giving flowing motion
    private void Update()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        
        offsetX += Time.deltaTime * 2f;
        offsetY += Time.deltaTime * 2f;
    }
    
    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float) x / width * scale + offsetX;
        float yCoord = (float) y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
