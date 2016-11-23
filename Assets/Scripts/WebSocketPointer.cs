using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using WebSocketSharp;
using WebSocketSharp.Server;
using System.Net;
using System.Net.Sockets;
using HttpStatusCode = WebSocketSharp.Net.HttpStatusCode;

public class WebSocketPointer : MonoBehaviour
{
    public bool disable;

    public int port = 4649;

    HttpServer server;

    public float Orientation { get; internal set; }
    public bool IsListening { get { return server.IsListening; } }
    public string Host { get; private set; }

    void Awake()
    {
        if(disable) {
            return;
        }

        server = new HttpServer(port) {
            RootPath = Path.Combine(Application.streamingAssetsPath, "Public").Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
        };

        server.OnGet += (sender, e) => {
            var req = e.Request;
            var res = e.Response;

            var path = req.RawUrl;
            if(path == "/") {
                path += "index.html";
            }

            var content = server.GetFile(path);
            if(content == null) {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if(path.EndsWith(".html")) {
                res.ContentType = "text/html";
                res.ContentEncoding = Encoding.UTF8;
            }
            else if(path.EndsWith(".js")) {
                res.ContentType = "application/javascript";
                res.ContentEncoding = Encoding.UTF8;
            }

            res.WriteContent(content);
        };

        server.AddWebSocketService("/Orientation", () => new Orientation(this));

        server.Start();

        Host = IPUtil.GetLocalIPAddress() + ":" + port;
        Debug.Log("Listening at: " + Host);
    }
    void OnDestroy()
    {
        if(server != null) {
            server.Stop();
            server = null;
        }
    }
}

internal class Orientation : WebSocketBehavior
{
    readonly WebSocketPointer parent;

    public Orientation(WebSocketPointer parent)
    {
        this.parent = parent;
    }
    protected override void OnMessage(MessageEventArgs e)
    {
        float o;
        if(float.TryParse(e.Data, out o)) {
            parent.Orientation = o;
        }
        else {
            Debug.Log("Can't parse orientation: [" + e.Data + "]");
        }
    }
}

//From: https://stackoverflow.com/questions/6803073/get-local-ip-address
internal static class IPUtil
{
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach(var ip in host.AddressList) {
            if(ip.AddressFamily == AddressFamily.InterNetwork) {
                return ip.ToString();
            }
        }
        throw new Exception("Local IP Address Not Found!");
    }
}
