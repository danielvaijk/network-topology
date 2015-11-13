using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

using System.Collections;

/// Coded by Daniel van Dijk @ 2015 (last edited in 13/11/2015).
// Thank you for purchasing this package and supporting me. Visit http://www.vaijk.com.

// Manages hosting/connecting to a Network (based on the High-Level API) and the examples inside it.

public class HighLevelNetwork : NetworkManager
{
    private string selection = "";
    private string myMessage = "My message";
    private string receivedMessage = "";

    private bool isServer = false;

    private NetworkClient myNetworkClient = new NetworkClient();

    private class MyMsgType
    {
        public static short ChangeSelection = MsgType.Highest + 1;
        public static short StringMessage = MsgType.Highest + 2;
        public static short ServerRelay = MsgType.Highest + 3;
    }

    private void Start ()
    {
        networkAddress = "127.0.0.1";
        networkPort = 25000;
    }

    private void OnGUI ()
    {
        GUILayout.Label(myNetworkClient.isConnected ? "Connected" : "Disconnected");
        GUILayout.Space(30);

        if (selection == "")
        {
            if (!myNetworkClient.isConnected)
            {
                GUILayout.Label("Server IP:");
                networkAddress = GUILayout.TextField(networkAddress);

                GUILayout.Label("Server Port:");
                networkPort = int.Parse(GUILayout.TextField(networkPort.ToString()));

                if (GUILayout.Button("Connect to Server"))
                {
                    myNetworkClient.RegisterHandler(MsgType.Connect, OnServerConnect);
                    myNetworkClient.RegisterHandler(MyMsgType.ChangeSelection, ChangeScene);
                    myNetworkClient.RegisterHandler(MyMsgType.StringMessage, OnReceiveStringMessage);

                    myNetworkClient.Connect(networkAddress, networkPort);
                }

                if (GUILayout.Button("Host Server"))
                {
                    NetworkServer.Listen(networkPort);

                    NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
                    NetworkServer.RegisterHandler(MyMsgType.ChangeSelection, ChangeScene);
                    NetworkServer.RegisterHandler(MyMsgType.ServerRelay, ServerRelayMessage);

                    myNetworkClient = ClientScene.ConnectLocalServer();
                    myNetworkClient.RegisterHandler(MsgType.Connect, OnServerConnect);
                    myNetworkClient.RegisterHandler(MyMsgType.ChangeSelection, ChangeScene);
                    myNetworkClient.RegisterHandler(MyMsgType.StringMessage, OnReceiveStringMessage);

                    isServer = true;
                }
            }
            else
            {
                if (isServer)
                {
                    if (GUILayout.Button("Network Message Example"))
                    {
                        NetworkServer.SendToAll(MyMsgType.ChangeSelection, new StringMessage("Network Message Example"));
                    }

                    if (GUILayout.Button("Remote Actions Example"))
                    {
                        //selection = "Remote Actions Example";
                    }

                    if (GUILayout.Button("Sync Example"))
                    {
                        //selection = "Sync Example";
                    }
                }
                else
                {
                    GUILayout.Label("Awaiting Server selection...");
                }
            }
        }
        else if (selection == "Network Message Example")
        {
            GUILayout.Label("Network Message Example");

            GUILayout.Space(30);

            GUILayout.Label("My message:");
            myMessage = GUILayout.TextField(myMessage);

            if (GUILayout.Button("Send Message"))
            {
                if (isServer)
                {
                    NetworkServer.SendToAll(MyMsgType.StringMessage, new StringMessage(myMessage));
                }
                else
                {
                    myNetworkClient.Send(MyMsgType.ServerRelay, new StringMessage(myMessage));
                }
            }

            GUILayout.Space(30);

            GUILayout.Label("Last Received Message: " + receivedMessage);
        }
    }

    // Called on the Server when a Client has connected to the Server.
    private void OnClientConnected (NetworkMessage networkMessage)
    {
        Debug.Log("A Client has connected to the Server: " + networkMessage.conn.connectionId);
    }

    private void ServerRelayMessage (NetworkMessage networkMessage)
    {
        var message = networkMessage.ReadMessage<StringMessage>();

        NetworkServer.SendToAll(MyMsgType.StringMessage, new StringMessage(message.value));
    }

    // Called locally when connected to a Server. (Called both on Client and Server).
    private void OnServerConnect (NetworkMessage networkMessage)
    {
        Debug.Log("Succesfully connected to a Server.");
    }

    private void ChangeScene (NetworkMessage networkMessage)
    {
        var selectionName = networkMessage.ReadMessage<StringMessage>();

        selection = selectionName.value;
    }

    private void OnReceiveStringMessage (NetworkMessage networkMessage)
    {
        var message = networkMessage.ReadMessage<StringMessage>();

        receivedMessage = message.value;
    }
}