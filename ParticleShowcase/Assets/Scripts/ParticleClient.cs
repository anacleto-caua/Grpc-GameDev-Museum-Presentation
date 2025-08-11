using UnityEngine;
using System;

[System.Serializable]
public class UpdateParticleParams
{
    public int particleCount;
    public float maxSpeed;
    public float maxSize;
    public float lifespan;
    public string particleColor;
}

public class ParticleClient : RpcManager
{
    public ParticleController particleController;

    protected override void RegisterCustomMethods()
    {
        base.RegisterCustomMethods();
        _server.RegisterMethod("UpdateParticles", HandleUpdateParticles);
    }

    private string HandleUpdateParticles(string requestJson)
    {
        var request = JsonUtility.FromJson<JsonRpcRequest<UpdateParticleParams>>(requestJson);
        var p = request.@params;

        _mainThreadActions.Enqueue(() =>
        {
            particleController.UpdateParticleSettings(p);
        });

        return "Particle update command has been queued for execution.";
    }
}