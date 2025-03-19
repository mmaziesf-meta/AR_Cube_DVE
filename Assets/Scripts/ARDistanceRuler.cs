using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class ARDistanceRuler : MonoBehaviour
{
    public GameObject cube; // Assign the Cube in Unity Inspector
    public TextMeshProUGUI distanceText; // Assign a UI TextMeshPro component

    void Update()
    {
        if (cube != null)
        {
            Vector3 headsetPos = GetHeadsetPosition();
            Vector3 cubePos = cube.transform.position;

            // Calculate and display the distance
            float distance = Vector3.Distance(headsetPos, cubePos);
            if (distanceText != null)
            {
                distanceText.text = $"Distance: {distance:F2}m"; // Show 2 decimal places
            }
        }
    }

    Vector3 GetHeadsetPosition()
    {
        // Fetch headset position using OpenXR
        InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
        return position;
    }
}
