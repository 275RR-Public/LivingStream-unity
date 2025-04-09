using UnityEngine;

public class AdjustScene : MonoBehaviour
{
    public Transform cameraTransform; // Assign your camera here in the Inspector
    public Vector3 desiredPosition = new Vector3(0, 5, 0); // Desired camera position
    public Vector3 desiredEulerAngles = new Vector3(90, 0, 0); // Desired camera rotation

    [ContextMenu("Adjust Scene")]
    public void AdjustSceneNow()
    {
        // Get the camera's current local position and rotation (relative to SceneRoot)
        Vector3 C_pos = cameraTransform.localPosition;
        Quaternion C_rot = cameraTransform.localRotation;

        // Define the desired world position and rotation for the camera
        Vector3 D_pos = desiredPosition;
        Quaternion D_rot = Quaternion.Euler(desiredEulerAngles);

        // Calculate SceneRoot's rotation
        Quaternion S_rot = D_rot * Quaternion.Inverse(C_rot);

        // Calculate SceneRoot's position
        Vector3 S_pos = D_pos - S_rot * C_pos;

        // Apply the transformation to SceneRoot
        transform.rotation = S_rot;
        transform.position = S_pos;
    }
}
