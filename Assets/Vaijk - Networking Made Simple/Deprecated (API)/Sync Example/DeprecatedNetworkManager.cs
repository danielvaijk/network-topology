using UnityEngine;
using System.Collections;

/// Coded by Daniel van Dijk @ 2015 (last edited in 13/11/2015).
// Thank you for purchasing this package and supporting me. Visit http://www.vaijk.com.

// Used to manage Player spawning over the Network.

#pragma warning disable 0618

public class DeprecatedNetworkManager : MonoBehaviour
{
    public GameObject playerPrefab;

    public Transform spawnPosition;

    private NetworkView thisNetworkView = null;

    private void Start ()
    {
        thisNetworkView = GetComponent<NetworkView>();

        // Spawns this <playerPrefab> on all clients (including yourself).
        thisNetworkView.RPC("SpawnPlayer", RPCMode.AllBuffered, Network.AllocateViewID());
    }

    // Instantiates the <playerPrefab> on the target NetworkClient receivers.
    [RPC]
    private void SpawnPlayer (NetworkViewID viewID)
    {
        // Instantiate the <playerPrefab> at the <spawnPosition.position> position.
        GameObject clone = Instantiate(playerPrefab, spawnPosition.position, Quaternion.identity) as GameObject;

        // Rename the <playerPrefab> GameObject's name.
        clone.name = string.Format("{0} ({1})", viewID, Network.isServer ? "Server" : "Client");

        NetworkView cloneNetworkView = clone.GetComponent<NetworkView>();

        // Set the <playerPrefab> GameObject's NetworkViewID to the one received from the
        // Player that called the spawn RPC (every <playerPrefab> must have a unique Network View ID in the scene).
        cloneNetworkView.viewID = viewID;
    }
}