using UnityEngine;
using System;
using System.Globalization;
using System.IO;

public class CommandReceiver : MonoBehaviour
{
    public GameObject cube; // Assign in Unity Inspector
    private string lastCommand = "";
    private string adbFilePath = "/data/local/tmp/adb_command.txt"; // New location
    private Renderer cubeRenderer; // To change color

    void Start()
    {
        cubeRenderer = cube.GetComponent<Renderer>();
        if (cubeRenderer == null)
        {
            Debug.LogError("❌ Cube does not have a Renderer component!");
        }

        Debug.Log($"✅ ADBCommandReceiver started. Listening for ADB commands at: {adbFilePath}");
    }

    void Update()
    {
        if (File.Exists(adbFilePath))
        {
            string adbCommand = File.ReadAllText(adbFilePath).Trim();
            if (!string.IsNullOrEmpty(adbCommand) && adbCommand != lastCommand)
            {
                Debug.Log($"📩 Received ADB Command from File: {adbCommand}");
                lastCommand = adbCommand;
                ApplyTransform(adbCommand);
            }
        }
        else
        {
            Debug.LogWarning($"⚠ ADB Command file not found: {adbFilePath}");
        }
    }

    void ApplyTransform(string command)
    {
        try
        {
            Debug.Log($"🔍 Parsing ADB command: {command}");

            string[] parts = command.Split(';');
            if (parts.Length < 3)
            {
                Debug.LogError("❌ Invalid command format. Expected format: 'x,y,z;x,y,z;x,y,z;R,G,B'");
                return;
            }

            Vector3 position = ParseVector(parts[0]);
            Vector3 rotation = ParseVector(parts[1]);
            Vector3 scale = ParseVector(parts[2]);

            // Apply transformations
            cube.transform.position = position;
            cube.transform.eulerAngles = rotation;
            cube.transform.localScale = scale;

            Debug.Log($"✅ Cube updated: Position={position}, Rotation={rotation}, Scale={scale}");

            // Change color if color data exists
            if (parts.Length >= 4)
            {
                ChangeColor(parts[3]);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error parsing ADB command: {ex.Message}");
        }
    }

    Vector3 ParseVector(string vectorString)
    {
        try
        {
            string[] values = vectorString.Split(',');
            if (values.Length != 3)
            {
                Debug.LogError("❌ Invalid vector format. Expected 'x,y,z'.");
                return Vector3.zero;
            }

            return new Vector3(
                float.Parse(values[0], CultureInfo.InvariantCulture),
                float.Parse(values[1], CultureInfo.InvariantCulture),
                float.Parse(values[2], CultureInfo.InvariantCulture)
            );
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error parsing vector: {vectorString}. Exception: {ex.Message}");
            return Vector3.zero;
        }
    }

    void ChangeColor(string colorString)
    {
        try
        {
            string[] values = colorString.Split(',');
            if (values.Length != 3)
            {
                Debug.LogError("❌ Invalid color format. Expected 'R,G,B'.");
                return;
            }

            float r = int.Parse(values[0], CultureInfo.InvariantCulture) / 255f;
            float g = int.Parse(values[1], CultureInfo.InvariantCulture) / 255f;
            float b = int.Parse(values[2], CultureInfo.InvariantCulture) / 255f;

            Color newColor = new Color(r, g, b);
            if (cubeRenderer != null)
            {
                cubeRenderer.material.color = newColor;
                Debug.Log($"🎨 Cube color updated to: {newColor}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error parsing color: {colorString}. Exception: {ex.Message}");
        }
    }
}
