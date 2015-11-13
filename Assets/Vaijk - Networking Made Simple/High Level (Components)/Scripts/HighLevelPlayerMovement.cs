using UnityEngine;
using UnityEngine.Networking;

using System.Collections;

/// Coded by Daniel van Dijk @ 2015 (last edited in 13/11/2015).
// Thank you for purchasing this package and supporting me. Visit http://www.vaijk.com.

// Changes the this GameObjects <transform.position> based on given input.

public class HighLevelPlayerMovement : NetworkBehaviour
{
    public float speed;

    private void Update ()
    {
        // Only allow us to move this object is it is ours (over the Network).
        if (isLocalPlayer)
        {
            // Receive the inputs <horizontal> and <vertical> and use them as movement directions in a Vector3.
            Vector3 movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            // Transform the movementDirection from Global to Local.
            Vector3 fromWorldtoLocal = transform.TransformDirection(movementDirection);

            // Change the Player's position based on the given direction and speed overtime.
            transform.position += fromWorldtoLocal * speed * Time.deltaTime;
        }
    }
}