using UnityEngine;
using System.Collections;

/// Coded by Daniel van Dijk @ 2015 (last edited in 13/11/2015).
// Thank you for purchasing this package and supporting me. Visit http://www.vaijk.com.

// Changes the this GameObjects <transform.position> based on given input.

#pragma warning disable 0618

public class DeprecatedPlayerMovement : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        // Receive the inputs <horizontal> and <vertical> and use them as movement directions in a Vector3.
        Vector3 movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        // Transform the movementDirection from Global to Local.
        Vector3 fromWorldtoLocal = transform.TransformDirection(movementDirection);

        // Change the Player's position based on the given direction and speed overtime.
        transform.position += fromWorldtoLocal * speed * Time.deltaTime;
    }

    // Sends and receives bytes from the Network.
    private void OnSerializeNetworkView (BitStream stream)
    {
        Vector3 myPosition = Vector3.zero;
        Quaternion myRotation = Quaternion.identity;

        if (stream.isWriting)
        {
            // Send data to others.

            myPosition = transform.position;
            myRotation = transform.rotation;

            stream.Serialize(ref myPosition);
            stream.Serialize(ref myRotation);
        }
        else
        {
            // Receive data from others.

            stream.Serialize(ref myPosition);
            stream.Serialize(ref myRotation);

            transform.position = myPosition;
            transform.rotation = myRotation;
        }
    }
}