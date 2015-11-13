using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// Coded by Daniel van Dijk @ 2015 (last edited in 13/11/2015).
// Thank you for purchasing this package and supporting me. Visit http://www.vaijk.com.

// This manager takes care of creating a simple non-authorative client-host network,
// where it can be used to send data across the network from peer to peer.

/// Things you should note about using the Unity Networking Transport Layer (as I went through development):
// 1. When using multi-clients on the same machine, each client must open a socket with a unique <localPort>.

public class TransportNetworkManager : MonoBehaviour
{
    public string ipAddress;
    public string port;

    public int maxConnections;

    private string myMessage = "Hello";
    private string receivedMessage = string.Empty;
    private string localPort = "25000";

    private int myReliableChannelID = 0;

    private int socketID = -1;
    private int connectionID = 0;

    private void Start ()
    {
        // Initialize the Transport Layer, has to be called before any other Network function.
        NetworkTransport.Init();
    }

    private void Update ()
    {
        // Variables to hold data when we receive a data stream from the Network stream.

        int receiveHostID;
        int receiveConnectionID;
        int receiveChannelID;

        byte[] receiveBuffer = new byte[1024];

        int bufferSize = 1024;
        int dataSize;

        byte error;

        // Receive the information and store the received values into the variables above and identify
        // the type of data event we have received.
        NetworkEventType receiveNetworkEvent = NetworkTransport.Receive
            (
                out receiveHostID, out receiveConnectionID,
                out receiveChannelID, receiveBuffer, bufferSize,
                out dataSize, out error
            );

        switch (receiveNetworkEvent)
        {
            // Called when nothing is happening (no data received).
            case NetworkEventType.Nothing:
                break;

            // Called when someone has connected to your socket or vice-versa.
            case NetworkEventType.ConnectEvent:
                Debug.Log("Connection event was received.");
                break;

            // Called when your have received a stream of data from the Network stream.
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(receiveBuffer);
                BinaryFormatter formatter = new BinaryFormatter();

                // Convert the data stream back into a byte array.
                // Here we are assuming it is a [string], it might not be, but that is of further
                // investigation as we are only overviewing this new system.
                receivedMessage = formatter.Deserialize(stream) as string;
                break;

            // Called when a your socket is disconneted.
            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnection event was received.");
                return;
        }
    }

    private void OnGUI ()
    {
        // Shows if we are connected to a socket or not.
        GUILayout.Label(connectionID < 1 ? "Disconnected" : "Connected");
        GUILayout.Space(30);

        // If our socket ID is of invalid value (< 0) that means we have not opened a socket.
        if (socketID > -1)
        {
            // If our connection ID is of invalid value (< 1) that means we are not connected to a socket.
            if (connectionID < 1)
            {
                // Target IP of the socket we want to connect to.
                GUILayout.Label("Target socket ip:");
                ipAddress = GUILayout.TextField(ipAddress, 12);

                // Target Port of the socket we want to connect to.
                GUILayout.Label("Target socket port:");
                port = GUILayout.TextField(port.ToString(), 5);

                // Connects us to the socket based on the inputed IP and port.
                if (GUILayout.Button("Connect to Socket"))
                    ConnectToSocket();
            }
            else
            {
                myMessage = GUILayout.TextField(myMessage);

                if (GUILayout.Button("Send Message"))
                {
                    // Send a message to the Network data stream.

                    SendSocketMessage(myMessage);
                    receivedMessage = myMessage;
                }

                GUILayout.Space(30);

                // Last <receivedMessage> received from the Network data stream.
                GUILayout.Label("Last Received Message: " + receivedMessage);

                GUILayout.Space(40);

                if (GUILayout.Button("Disconnect"))
                {
                    byte error;

                    // Closes our connection from the other peers.
                    NetworkTransport.Disconnect(socketID, connectionID, out error);
                }
            }
        }
        else
        {
            // The port of your socket.
            GUILayout.Label("Your socket port:");
            localPort = GUILayout.TextField(localPort.ToString(), 5);

            if (GUILayout.Button("Open your Socket"))
            {
                // Sets-up the configuration for the Network connection.
                ConnectionConfig config = new ConnectionConfig();

                // Adds a reliable channel ID to the connection configuration, used to send data.
                myReliableChannelID = config.AddChannel(QosType.Reliable);

                // Sets-up the Networks topology.
                HostTopology topology = new HostTopology(config, maxConnections);

                // Open a socket with the given topology and port and save the 'host' ID for it.
                socketID = NetworkTransport.AddHost(topology, int.Parse(localPort));

                Debug.Log("Socket opened. Socket ID: " + socketID);
            }
        }
    }

    // Connects you to a target socket based on the current <socketID>, <ipAddress> and <port> of that socket.
    public void ConnectToSocket ()
    {
        byte error;

        // Connect to a socket using out socket ID and the IP and port addresses from the target socket.
        connectionID = NetworkTransport.Connect(socketID, ipAddress, int.Parse(port), 0, out error);

        Debug.Log("Connected to a Socket. Connection ID: " + connectionID);
    }

    // Send a message to the Network data stream so that other sockets connected to you can receive it.
    public void SendSocketMessage (string message)
    {
        // The size of the byte buffer.
        int bufferSize = 1024;

        // Holds info on any error that might occurr when trying to send our data.
        byte error;

        // The byte buffer that holds our information;
        byte[] buffer = new byte[bufferSize];

        // Create a data stream from our byte buffer.
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();

        // Write the data from our message into the data stream.
        formatter.Serialize(stream, message);

        // Send the data stream into the network stream where the peers can 'catch' the sent data.
        NetworkTransport.Send(socketID, connectionID, myReliableChannelID, buffer, bufferSize, out error);
    }
}