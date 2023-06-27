using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils : MonoBehaviour
{
    // Start is called before the first frame update
    public static Quaternion AngleToQuaternion(string axis, float angleInDegrees)
    {
        // Convert the angle to radians
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        // Calculate the half angle
        float halfAngle = angleInRadians * 0.5f;

        // Calculate the sine and cosine of the half angle
        float sinHalfAngle = Mathf.Sin(halfAngle);
        float cosHalfAngle = Mathf.Cos(halfAngle);

        // Create a new quaternion based on the specified axis
        Quaternion quaternion;
        switch (axis)
        {
            case "x":
                quaternion = new Quaternion(sinHalfAngle, 0f, 0f, cosHalfAngle);
                break;
            case "y":
                quaternion = new Quaternion(0f, sinHalfAngle, 0f, cosHalfAngle);
                break;
            case "z":
                quaternion = new Quaternion(0f, 0f, sinHalfAngle, cosHalfAngle);
                break;
            default:
                quaternion = Quaternion.identity;
                break;
        }

        return quaternion;
    }


    public static Quaternion RotateQuaternion(Quaternion originalQuaternion, Vector3 axis, float angleInDegrees)
    {
        // Convert the angle to radians
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        // Calculate the half angle
        float halfAngle = angleInRadians * 0.5f;

        // Calculate the sine and cosine of the half angle
        float sinHalfAngle = Mathf.Sin(halfAngle);
        float cosHalfAngle = Mathf.Cos(halfAngle);

        // Create a new quaternion representing the rotation
        Quaternion rotationQuaternion = new Quaternion(
            axis.x * sinHalfAngle,
            axis.y * sinHalfAngle,
            axis.z * sinHalfAngle,
            cosHalfAngle
        );

        // Multiply the original quaternion by the rotation quaternion
        Quaternion rotatedQuaternion = rotationQuaternion * originalQuaternion;

        // Normalize the resulting quaternion
        rotatedQuaternion.Normalize();

        return rotatedQuaternion;
    }

    public static float WrapAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f)
            angle += 360f;
        return angle;
    }


}
