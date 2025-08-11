using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Concurrent;
using System.Linq;
using ZXing;
using ZXing.QrCode;

[Serializable]
public class SayHelloParams { public string name; }

[Serializable]
public class ShowLinkQrCodeParams { public string link; }

public class RpcManager : MonoBehaviour
{
    protected JsonRpcServer _server;
    protected readonly ConcurrentQueue<Action> _mainThreadActions = new ConcurrentQueue<Action>();

    [Header("QR Code UI")]
    public RawImage qrCodeImage;
    private Texture2D _generatedQRCode;

    protected virtual void Start()
    {
        StartServer("*", "8082");
    }

    protected virtual void Update()
    {
        if (_mainThreadActions.TryDequeue(out Action actionToExecute))
        {
            actionToExecute?.Invoke();
        }
    }

    protected virtual void OnDestroy()
    {
        _server?.Stop();
    }

    public void StartServer(string ip, string port)
    {
        string link = $"http://{ip}:{port}/";
        _server = new JsonRpcServer(link);

        _server.RegisterMethod("SayHello", HandleSayHello);
        _server.RegisterMethod("ShowLinkQrCode", HandleShowLinkQrCode);

        // Allow child classes to register their own custom methods.
        RegisterCustomMethods();

        _server.Start();
        ApplyQrCode(link);
    }

    protected virtual void RegisterCustomMethods() { }

    private string HandleSayHello(string requestJson)
    {
        var request = JsonUtility.FromJson<JsonRpcRequest<SayHelloParams>>(requestJson);
        return $"Hello, {request.@params.name}! This is the base server.";
    }

    private string HandleShowLinkQrCode(string requestJson)
    {
        var request = JsonUtility.FromJson<JsonRpcRequest<ShowLinkQrCodeParams>>(requestJson);
        _mainThreadActions.Enqueue(() => ApplyQrCode(request.@params.link));
        return "Link received, QR code update queued.";
    }

    private void ApplyQrCode(string textToEncode)
    {
        if (qrCodeImage == null) return;
        _generatedQRCode = GenerateQRCode(textToEncode);
        qrCodeImage.texture = _generatedQRCode;
    }

    private Texture2D GenerateQRCode(string text)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions { Height = 256, Width = 256 }
        };
        var pixels = writer.Write(text);
        var texture = new Texture2D(256, 256);
        texture.SetPixels32(pixels);
        texture.Apply();
        return texture;
    }
}