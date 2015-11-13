using UnityEngine;
using System.Collections;

/// Coded by Daniel van Dijk @ 2015 (last edited in 13/11/2015).
// Thank you for purchasing this package and supporting me. Visit http://www.vaijk.com.

// Represents a simple send/receive message system over the Network.

#pragma warning disable 0618

[RequireComponent(typeof(NetworkView))]

public class DeprecatedRPC : MonoBehaviour
{
    private string myMessage = "My message";
    private string receivedMessage = string.Empty;

    private NetworkView thisNetworkView = null;

    private void Start ()
    {
        thisNetworkView = GetComponent<NetworkView>();
    }

    private void OnGUI ()
    {
        // The last message received over the network.
        GUILayout.Label("Current message: " + receivedMessage);

        GUILayout.Space(30);

        myMessage = GUILayout.TextField(myMessage);

        if (GUILayout.Button("Send message"))
        {
            // Sends your message over the Network to everyone (including yourself).
            thisNetworkView.RPC("SendNetworkMessage", RPCMode.All, myMessage);
        }
    }

    // Sets the <receivedMessage> to the received <message> over the Network.
    [RPC]
    private void SendNetworkMessage (string message)
    {
        receivedMessage = message;
    }
}