using UnityEngine;
using System.Collections.Concurrent;
using System;

#region Method-Specific Parameter Classes
[Serializable]
public class SayHelloParams { public string name; }

[Serializable]
public class MoveObjectParams { public float x; public float y; }
#endregion

public class TargetClient : MonoBehaviour
{
    private JsonRpcServer _server;

    // A thread-safe queue to pass actions from the server thread to the main thread
    private readonly ConcurrentQueue<Action> _mainThreadActions = new ConcurrentQueue<Action>();

    public BoundedMovement boundedMovement;

    void Start()
    {
        _server = new JsonRpcServer("http://127.0.0.1:8080/");
        _server.RegisterMethod("SayHello", HandleSayHello);
        _server.RegisterMethod("MoveObject", HandleMoveObject);
        _server.Start();
    }

    void Update()
    {
        if (_mainThreadActions.TryDequeue(out Action actionToExecute))
        {
            actionToExecute?.Invoke();
        }
    }

    void OnDestroy()
    {
        _server?.Stop();
    }

    #region rpcMethods
    private string HandleSayHello(string requestJson)
    {
        var request = JsonUtility.FromJson<JsonRpcRequest<SayHelloParams>>(requestJson);
        return $"Hello, {request.@params.name}! This is Project TargetClient!";
    }

    private string HandleMoveObject(string requestJson)
    {
        var request = JsonUtility.FromJson<JsonRpcRequest<MoveObjectParams>>(requestJson);
        var p = request.@params;

        // Queue the Unity API call to be executed on the main thread.
        // The "transform" property can't be changed outside the main method.
        _mainThreadActions.Enqueue(() =>
        {
            boundedMovement.UpdateTargetPosition(p.x, p.y);
        });

        return "Move command has been queued for execution.";
    }
    #endregion

}