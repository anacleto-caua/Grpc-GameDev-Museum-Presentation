using UnityEngine;

[System.Serializable]
public class GenerateTerrainParams
{
    // Noise Settings
    public int seed;
    public float frequency;
    public string noiseType;

    // Fractal Settings
    public string fractalType;
    public int octaves;
    public float lacunarity;
    public float gain;
}
public class TerrainClient : RpcManager
{
    public TerrainController terrainController;

    protected override void RegisterCustomMethods()
    {
        base.RegisterCustomMethods();
        _server.RegisterMethod("GenerateTerrain", HandleGenerateTerrain);
    }

    private string HandleGenerateTerrain(string requestJson)
    {
        var request = JsonUtility.FromJson<JsonRpcRequest<GenerateTerrainParams>>(requestJson);
        var p = request.@params;

        _mainThreadActions.Enqueue(() =>
        {
            terrainController.GenerateNewWorld(p);
        });

        return "Move command has been queued for execution.";
    }
}