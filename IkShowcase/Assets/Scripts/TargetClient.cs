using UnityEngine;

[System.Serializable]
public class MoveObjectParams { public float x; public float y; }

public class TargetClient : RpcManager
{
    public BoundedMovement boundedMovement;

    protected override void RegisterCustomMethods()
    {
        base.RegisterCustomMethods();
        _server.RegisterMethod("MoveObject", HandleMoveObject);
    }

    private string HandleMoveObject(string requestJson)
    {
        var request = JsonUtility.FromJson<JsonRpcRequest<MoveObjectParams>>(requestJson);
        var p = request.@params;

        _mainThreadActions.Enqueue(() =>
        {
            boundedMovement.UpdateTargetPosition(p.x, p.y);
        });

        return "Move command has been queued for execution.";
    }
}