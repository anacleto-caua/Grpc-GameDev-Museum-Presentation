using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Random = UnityEngine.Random;
using static FastNoiseLite;

[ExecuteInEditMode]
public class TerrainController : MonoBehaviour
{
    public Terrain terrain;
    public int width = 256;
    public int depth = 256;
    public int height = 35;

    [Header("Noise Settings")]
    public int seed = 1337;
    public float frequency = 0.01f;
    public NoiseType noiseType = NoiseType.OpenSimplex2;

    [Header("Fractal Settings")]
    public FractalType fractalType = FractalType.FBm;
    public int octaves = 5;
    public float lacunarity = 2.0f;
    public float gain = 0.5f;

    private FastNoiseLite noise;

    void Start()
    {
        seed = Random.Range(0, 100000);
        GenerateWorld();

    }

    // Public entry point to generate terrain with specific parameters.
    public void GenerateNewWorld(GenerateTerrainParams p)
    {
        // Update the controller's settings from the received parameters.
        this.seed = p.seed;
        this.frequency = p.frequency;
        this.octaves = p.octaves;
        this.lacunarity = p.lacunarity;
        this.gain = p.gain;

        // Safely parse string names to enum types.
        try
        {
            this.noiseType = (NoiseType)Enum.Parse(typeof(NoiseType), p.noiseType, true);
            this.fractalType = (FractalType)Enum.Parse(typeof(FractalType), p.fractalType, true);
        }
        catch (ArgumentException e)
        {
            Debug.LogError($"Failed to parse noise/fractal type: {e.Message}. Using default values.");
        }

        // Generate the world with the new settings.
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        if (terrain == null) return;

        noise = new FastNoiseLite(seed);
        noise.SetNoiseType(noiseType);
        noise.SetFrequency(frequency);

        noise.SetFractalType(fractalType);
        noise.SetFractalOctaves(octaves);
        noise.SetFractalLacunarity(lacunarity);
        noise.SetFractalGain(gain);

        TerrainData terrainData = terrain.terrainData;
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, height, depth);

        terrainData.SetHeights(0, 0, GenerateHeights());
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, depth];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < depth; y++)
            {
                float noiseValue = noise.GetNoise(x, y);
                heights[x, y] = (noiseValue + 1) / 2f;
            }
        }
        return heights;
    }

    // Called in the editor when a value is changed.
    private void OnValidate()
    {
        if (Application.isPlaying || terrain == null) return;
        GenerateWorld();
    }
}