// You can copy this file into any Unity project without changing it.
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;

#region Core RPC Data Structures
[Serializable]
public class JsonRpcRequest<T> { public string jsonrpc; public string method; public T @params; public int id; }
[Serializable]
public class MethodNameHelper { public string method; }
[Serializable]
public class JsonRpcSuccessResponse { public string jsonrpc = "2.0"; public string result; public int id; }
#endregion


public class JsonRpcServer
{
    private readonly HttpListener _listener = new HttpListener();
    private readonly Dictionary<string, Func<string, string>> _methodHandlers = new Dictionary<string, Func<string, string>>();
    private Thread _listenerThread;

    public JsonRpcServer(string prefix) { _listener.Prefixes.Add(prefix); }

    public void RegisterMethod(string methodName, Func<string, string> handler) { _methodHandlers[methodName] = handler; }

    public void Start()
    {
        _listenerThread = new Thread(StartListener);
        _listenerThread.IsBackground = true;
        _listenerThread.Start();
        Debug.Log($"JSON-RPC Server started at {_listener.Prefixes.First()}");
    }

    public void Stop()
    {
        if (_listener != null && _listener.IsListening) { _listener.Stop(); _listener.Close(); }
        if (_listenerThread != null && _listenerThread.IsAlive) { _listenerThread.Abort(); }
    }

    private void StartListener()
    {
        _listener.Start();
        while (_listener.IsListening)
        {
            try { ProcessRequest(_listener.GetContext()); }
            catch (Exception ex) { if (_listener.IsListening) Debug.LogError($"Listener Error: {ex.Message}"); }
        }
    }

    private void ProcessRequest(HttpListenerContext context)
    {
        string requestBody;
        using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
        {
            requestBody = reader.ReadToEnd();
        }

        string jsonResponse;
        int requestId = -1;


        Debug.Log(requestBody);
        try
        {
            var methodHelper = JsonUtility.FromJson<MethodNameHelper>(requestBody);
            Debug.Log(methodHelper);
            requestId = JsonUtility.FromJson<JsonRpcRequest<object>>(requestBody).id;
            Debug.Log(requestId);

            if (_methodHandlers.TryGetValue(methodHelper.method, out var handler))
            {

                var result = handler(requestBody);
                Debug.Log(result);
                var response = new JsonRpcSuccessResponse { result = result, id = requestId };
                Debug.Log(response);
                jsonResponse = JsonUtility.ToJson(response);
            }
            else { throw new Exception("Method not found."); }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error processing request: {ex.Message}");
            jsonResponse = $"{{\"result\":\"Server-side error: {ex.Message}\"}}";
        }

        byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);
        context.Response.ContentType = "application/json";
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }
}