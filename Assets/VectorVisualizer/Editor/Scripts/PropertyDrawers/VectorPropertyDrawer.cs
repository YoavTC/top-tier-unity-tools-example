using System;
using UnityEditor;
using UnityEngine;

namespace VectorVisualizer
{
#if !VECTOR_VISUALIZER_WITH_ATTRIBUTE
    [CustomPropertyDrawer(typeof(Vector3))]
    [CustomPropertyDrawer(typeof(Vector2))]
#else
    [CustomPropertyDrawer(typeof(VisualizableVectorAttribute))]
#endif
    public class VectorPropertyDrawer : PropertyDrawer
    {
        private const float BUTTON_SIZE = 18;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var isVector3 = property.propertyType == SerializedPropertyType.Vector3;


            GUI.SetNextControlName("VectorVisualizerPropertyField");
            label = EditorGUI.BeginProperty(position, label, property);


            var rect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);


            var contentRect = isVector3
                ? new Rect(rect.x, rect.y, rect.width - BUTTON_SIZE, rect.height)
                : new Rect(rect.x, rect.y, rect.width, rect.height);


            DrawField(contentRect, property, "x", 0, "X");
            var yField = DrawField(contentRect, property, "y", 1, "Y");
            if (property.propertyType == SerializedPropertyType.Vector3)
            {
                DrawField(contentRect, property, "z", 2, "Z");
            }

            var buttonRect = isVector3
                ? new Rect(rect.xMax - BUTTON_SIZE, rect.y, BUTTON_SIZE, rect.height)
                : new Rect(yField.xMax, yField.y, BUTTON_SIZE, yField.height);


            EditorGUI.EndProperty();
            
            EditorGUI.indentLevel = indent;


            try
            {
                var visualizerHasProperty =
                    VectorVisualizer.instance.HasProperty(property.serializedObject.targetObject,
                        property.propertyPath);
                if (Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
                {
                    GUI.FocusControl("VectorVisualizerPropertyField");
                    if (visualizerHasProperty) VectorVisualizer.instance.SelectProperty(property);
                }

                var color = GUI.color;
                GUI.color = visualizerHasProperty ? Color.green : Color.white;
                if (GUI.Button(buttonRect, new GUIContent("V")))
                {
                    if (visualizerHasProperty)
                    {
                        VectorVisualizer.instance.RemoveProperty(property);
                    }
                    else
                    {
                        VectorVisualizer.instance.AddProperty(property);
                    }
                }


                GUI.color = color;
            }
            catch (Exception e)
            {
              Debug.LogError(e);
            }
        }

        private Rect DrawField(Rect position, SerializedProperty property, string propertyName, int index,
            string label)
        {
            // Get the specific component property
            SerializedProperty component = property.FindPropertyRelative(propertyName);

            // Calculate width for labels and fields

            float totalFields = 3f; // Adjust this number based on how many fields you want to draw
            float fieldWidth =
                (position.width - (totalFields - 1) * EditorGUIUtility.standardVerticalSpacing -
                 totalFields) / totalFields;

            // Calculate the position for this particular component
            var labelContent = new GUIContent(label);
            EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(labelContent).x;
            Rect fieldRect = new Rect(position.x + (index * (EditorGUIUtility.standardVerticalSpacing + fieldWidth)),
                position.y, fieldWidth, position.height);

            // Draw the label and the field
            EditorGUI.PropertyField(fieldRect, component, labelContent);
            return fieldRect;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}