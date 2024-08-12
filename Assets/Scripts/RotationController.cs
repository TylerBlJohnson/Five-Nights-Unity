using UnityEngine;

public class RotationController : MonoBehaviour
{
    public Transform targetObject; // Reference to the TargetObject.
    public float maxRotationAngle = 90f; // Maximum rotation angle allowed.
    private float startingRotation; // Starting rotation angle of the RotatingObject on the Z-axis.
    private float totalRotation; // Total rotation angle achieved so far.

    private void Start()
    {
        // Store the initial rotation angle on the Z-axis to reset later.
        startingRotation = transform.eulerAngles.z;
        totalRotation = 0f;
    }

    private void Update()
    {
        RotateBasedOnTargetPosition(); 
    }

    private void RotateBasedOnTargetPosition()
    {
        // Calculate the distance of the targetObject from the x-axis.  
        float distanceFromXAxis = targetObject.position.x; 

       // Calculate the rotation angle based on the distance and maxRotationAngle, reverse it by multiplying by -1
        float rotationAngle = -Mathf.Clamp(distanceFromXAxis * maxRotationAngle / 4.059042f, -maxRotationAngle, maxRotationAngle); 

        // Calculate the total rotation required based on the current rotation and the new rotation angle.
        float requiredRotation = totalRotation + rotationAngle; 
        
        // Rotate the object around the Z-axis now.
        transform.rotation = Quaternion.Euler(0f, 0f, startingRotation + requiredRotation);
    } 
}