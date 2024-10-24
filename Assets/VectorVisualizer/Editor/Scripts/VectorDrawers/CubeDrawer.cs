using UnityEditor;
using UnityEngine;

namespace VectorVisualizer
{
    public class CubeDrawer : IVectorDrawer
    {
        public string Id => "Cube";
        public string MenuName => "□";

        public VectorDrawerProperty Properties => VectorDrawerProperty.SizeVector3 | VectorDrawerProperty.Color |
                                                  VectorDrawerProperty.Rotation | VectorDrawerProperty.WireView;

        public void DrawVectorOnSceneView(VectorVisualizeObject visualizeObject, VectorDrawSettings settings)
        {
            var prop = visualizeObject.SerializedProperty;
            if (Event.current.type != EventType.Repaint) return;

            var oldHandlesColor = Handles.color;
            Handles.color = settings.Color;

            Vector3 center = prop.propertyType == SerializedPropertyType.Vector3
                ? prop.vector3Value
                : prop.vector2Value;

            // Calculate half dimensions
            var halfWidth = settings.SizeVector3.x / 2.0f;
            var halfHeight = settings.SizeVector3.y / 2.0f;
            var halfDepth = settings.SizeVector3.z / 2.0f;


            // Define the corners of the cube relative to the center
            Vector3[] corners = new Vector3[8];
            corners[0] = new Vector3(-halfWidth, -halfHeight, -halfDepth);
            corners[1] = new Vector3(halfWidth, -halfHeight, -halfDepth);
            corners[2] = new Vector3(halfWidth, halfHeight, -halfDepth);
            corners[3] = new Vector3(-halfWidth, halfHeight, -halfDepth);
            corners[4] = new Vector3(-halfWidth, -halfHeight, halfDepth);
            corners[5] = new Vector3(halfWidth, -halfHeight, halfDepth);
            corners[6] = new Vector3(halfWidth, halfHeight, halfDepth);
            corners[7] = new Vector3(-halfWidth, halfHeight, halfDepth);

            // Apply rotation to the corners
            Quaternion rotation = Quaternion.Euler(settings.EulerAngles);
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = center + rotation * corners[i];
            }

            // Draw the cube
            if (settings.IsWire)
            {
                DrawWireCube(corners);
            }
            else
            {
                DrawSolidCube(corners);
            }

            Handles.color = oldHandlesColor;
        }

        // Method to draw the wireframe of the cube
        private void DrawWireCube(Vector3[] corners)
        {
            Handles.DrawLine(corners[0], corners[1]);
            Handles.DrawLine(corners[1], corners[5]);
            Handles.DrawLine(corners[5], corners[4]);
            Handles.DrawLine(corners[4], corners[0]);
            Handles.DrawLine(corners[3], corners[2]);
            Handles.DrawLine(corners[2], corners[6]);
            Handles.DrawLine(corners[6], corners[7]);
            Handles.DrawLine(corners[7], corners[3]);
            Handles.DrawLine(corners[0], corners[3]);
            Handles.DrawLine(corners[1], corners[2]);
            Handles.DrawLine(corners[4], corners[7]);
            Handles.DrawLine(corners[5], corners[6]);
        }

        // Method to draw the solid cube with outlines
        private void DrawSolidCube(Vector3[] corners)
        {
            
            Handles.DrawSolidRectangleWithOutline(new[] {corners[0], corners[1], corners[2], corners[3]}, Color.white,
                Color.black);
            Handles.DrawSolidRectangleWithOutline(new[] {corners[4], corners[5], corners[6], corners[7]}, Color.white,
                Color.black);
            Handles.DrawSolidRectangleWithOutline(new[] {corners[0], corners[3], corners[7], corners[4]}, Color.white,
                Color.black);
            Handles.DrawSolidRectangleWithOutline(new[] {corners[1], corners[2], corners[6], corners[5]}, Color.white,
                Color.black);
            Handles.DrawSolidRectangleWithOutline(new[] {corners[3], corners[2], corners[6], corners[7]}, Color.white,
                Color.black);
            Handles.DrawSolidRectangleWithOutline(new[] {corners[0], corners[1], corners[5], corners[4]}, Color.white,
                Color.black);
        }
    }
}