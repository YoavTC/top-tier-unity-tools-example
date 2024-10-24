using UnityEditor;
using UnityEngine;

namespace VectorVisualizer
{
    public class SphereDrawer : IVectorDrawer
    {
        public string Id => "Sphere";
        public string MenuName => "○";

        public VectorDrawerProperty Properties => VectorDrawerProperty.SizeFloat | VectorDrawerProperty.Color |
                                                  VectorDrawerProperty.Rotation | VectorDrawerProperty.WireView;

        public void DrawVectorOnSceneView(VectorVisualizeObject visualizeObject, VectorDrawSettings settings)
        {
            var prop = visualizeObject.SerializedProperty;
            if (Event.current.type != EventType.Repaint) return;
            
            Vector3 canter = prop.propertyType == SerializedPropertyType.Vector3
                ? prop.vector3Value
                : prop.vector2Value;
            
            
            var rotation = Quaternion.Euler(settings.EulerAngles);
            
            var oldColor = Handles.color;
            Handles.color = settings.Color;
                
            if (settings.IsWire)
            {
                Handles.DrawWireDisc(canter, rotation * Vector3.up,
                    settings.SizeFloat / 2); 
                Handles.DrawWireDisc(canter, rotation * Vector3.right,
                    settings.SizeFloat / 2); 
                Handles.DrawWireDisc(canter, rotation * Vector3.forward,
                    settings.SizeFloat / 2);
            }
            else
            {
                Handles.SphereHandleCap(0, canter, rotation,
                    settings.SizeFloat, EventType.Repaint);
            }

            Handles.color = oldColor;
        }
    }
}