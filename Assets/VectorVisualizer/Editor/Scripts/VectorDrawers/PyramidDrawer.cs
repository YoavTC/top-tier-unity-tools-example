using UnityEditor;
using UnityEngine;

namespace VectorVisualizer
{
    public class PyramidDrawer : IVectorDrawer
    {
        public string Id => "Pyramid";
        public string MenuName => "△";

        public VectorDrawerProperty Properties => VectorDrawerProperty.SizeVector2 | VectorDrawerProperty.Color |
                                                  VectorDrawerProperty.Rotation | VectorDrawerProperty.WireView;

        public void DrawVectorOnSceneView(VectorVisualizeObject prop, VectorDrawSettings settings)
        {
            var serializedProperty = prop.SerializedProperty;
            if (Event.current.type != EventType.Repaint) return;
            
            var oldHandlesColor = Handles.color;
            Handles.color = settings.Color;

            Vector3 center = serializedProperty.propertyType == SerializedPropertyType.Vector3
                ? serializedProperty.vector3Value
                : serializedProperty.vector2Value;
            
            var rotation = Quaternion.Euler(settings.EulerAngles);

            // Calculate the vertices of the pyramid
            var baseSize =settings.SizeVector2.x;
            var height = settings.SizeVector2.y;

            var vertices = new Vector3[4];
            vertices[0] = center + new Vector3(-baseSize, 0, -baseSize);
            vertices[1] = center + new Vector3(baseSize, 0, -baseSize);
            vertices[2] = center + new Vector3(baseSize, 0, baseSize);
            vertices[3] = center + new Vector3(-baseSize, 0, baseSize);

            Vector3 peak = center + new Vector3(0, height, 0);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = rotation * (vertices[i] - center) + center;
            }

            peak = rotation * (peak - center) + center;


            if (settings.IsWire)
            {
                DrawWirePyramid(vertices, peak,settings.Color);
            }
            else
            {
                DrawSolidPyramid(vertices, peak,settings.Color);
                DrawWirePyramid(vertices, peak,Color.black);
            }

            Handles.color = oldHandlesColor;
        }

        // Draw a solid pyramid
        private void DrawSolidPyramid(Vector3[] vertices, Vector3 peak,Color color)
        {
            var oldColor = Handles.color;
            Handles.color = color;
            Handles.DrawAAConvexPolygon(vertices[0], vertices[1], vertices[2], vertices[3]);
            Handles.DrawAAConvexPolygon(vertices[0], vertices[1], peak);
            Handles.DrawAAConvexPolygon(vertices[1], vertices[2], peak);
            Handles.DrawAAConvexPolygon(vertices[2], vertices[3], peak);
            Handles.DrawAAConvexPolygon(vertices[3], vertices[0], peak);
            Handles.color = oldColor;
        }
    
        // Draw the wireframe of the pyramid
        private void DrawWirePyramid(Vector3[] vertices, Vector3 peak,Color color)
        {
            var oldColor = Handles.color;
            Handles.color = color;
            Handles.DrawLine(vertices[0], vertices[1]);
            Handles.DrawLine(vertices[1], vertices[2]);
            Handles.DrawLine(vertices[2], vertices[3]);
            Handles.DrawLine(vertices[3], vertices[0]);

            Handles.DrawLine(vertices[0], peak);
            Handles.DrawLine(vertices[1], peak);
            Handles.DrawLine(vertices[2], peak);
            Handles.DrawLine(vertices[3], peak);
            Handles.color = oldColor;
        }
    }
}