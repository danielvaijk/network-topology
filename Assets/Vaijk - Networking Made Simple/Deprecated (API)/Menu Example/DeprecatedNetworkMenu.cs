using UnityEngine;
using System.Collections;

/// Coded by Daniel van Dijk @ 2015 (last edited in 13/11/2015).
// Thank you for purchasing this package and supporting me. Visit http://www.vaijk.com.

// Creates a interface for hosting/connecting to Servers, and also is a simple
// Network manager, used to root connections to certain example scenes.

#pragma warning disable 0618

[RequireComponent(typeof(NetworkView))]

public class DeprecatedNetworkMenu : MonoBehaviour
{
    public string ipAddress;
    public string port;

    public int maxConnections;

    private NetworkView thisNetworkView = null;

    private void Start ()
    {
        thisNetworkView = GetComponent<NetworkView>();
    }

    private void OnGUI ()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            // If we are not connected to a network (either as server or client) then give the
            // player options to input data and either host or connect to a server.

            ipAddress = GUILayout.TextField(ipAddress);
            port = GUILayout.TextField(port);

            GUILayout.Space(20);

            if (GUILayout.Button("Host"))
            {
                // Start a server based on inputed Port and Max allowed connections.
                Network.InitializeServer(maxConnections, int.Parse(port), false);
            }

            if (GUILayout.Button("Connect"))
            {
                // Connect to a server based on inputed IP and Port addresses.
                Network.Connect(ipAddress, int.Parse(port));
            }
        }
        else
        {
            // If we are connected to a network (either as server or client) then give the
            // player options to choose wich example scene to go to.

            if (GUILayout.Button("RPC Example Scene"))
            {
                // Request to change the level over the Network for all clients (including server).
                thisNetworkView.RPC("ChangeLevel", RPCMode.AllBuffered, "DAPI - RPC Example");
            }

            if (GUILayout.Button("Sync Example Scene"))
            {
                // Request to change the level over the Network for all clients (including server).
                thisNetworkView.RPC("ChangeLevel", RPCMode.AllBuffered, "DAPI - Sync Example");
            }
        }
    }

    // Called when you have succesfully initialized a Server.
    private void OnServerInitialized ()
    {
        Debug.Log("Server initialized.");
    }

    // Called when you have succesfully connected to a Server.
    private void OnConnectedToServer ()
    {
        Debug.Log("Connected to the server.");
    }

    // Changes the current level to <levelName> on the NetworkClient it was called on.
    [RPC]
    private void ChangeLevel (string levelName)
    {
        Application.LoadLevel(levelName);
    }
}