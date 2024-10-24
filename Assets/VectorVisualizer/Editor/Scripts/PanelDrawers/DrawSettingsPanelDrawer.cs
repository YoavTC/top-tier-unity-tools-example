using System;
using UnityEditor;
using UnityEngine;

namespace VectorVisualizer
{
    public class DrawSettingsPanel
    {
        public bool Draw(VectorDrawSettings drawSettings, IVectorDrawer[] drawers,Rect windowRect,int menuItemCount)
        {
            var settingsUpdated = false;
            
            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            
            DrawSelectButtons(drawSettings, drawers,windowRect,menuItemCount);
            
            //Find active drawer for draw settings
            var activeDrawer = Array.Find(drawers, x => x.Id == drawSettings.DrawerId);
            
            if (activeDrawer != null)
            {
                DrawSettings(activeDrawer.Properties, drawSettings);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                settingsUpdated = true;
            }

            return settingsUpdated;
        }

        private void DrawSelectButton(VectorDrawSettings drawSettings, string drawerId, string label,float width)
        {
            var oldColor = GUI.color;
            if (drawSettings.DrawerId == drawerId)
            {
                GUI.color = Color.green;
            }

            if (GUILayout.Button(label, GUILayout.Width(width)))
            {
                Undo.RecordObject(VectorVisualizer.instance, "VectorVisualizerDrawerChange");
                drawSettings.SetDrawerId(drawerId);
            }

            GUI.color = oldColor;
        }

        private void DrawSelectButtons(VectorDrawSettings drawSettings, IVectorDrawer[] drawers,Rect windowRect,int menuItemCount)
        {
            var buttonWidth = (windowRect.width - 20) / menuItemCount;
            GUILayout.BeginVertical();
            for (int i = 0; i < drawers.Length; i += menuItemCount)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                for (int j = 0; j < menuItemCount; j++)
                {
                    if (i + j < drawers.Length)
                    {
                        var drawer = drawers[i + j];
                        DrawSelectButton(drawSettings, drawer.Id, drawer.MenuName, buttonWidth);
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical(); 
        }

        //Draws required settings for the drawer
        private void DrawSettings(VectorDrawerProperty properties, VectorDrawSettings drawSettings)
        {
            if (properties.HasFlag(VectorDrawerProperty.Color)) DrawColorPicker(drawSettings);
            if (properties.HasFlag(VectorDrawerProperty.SizeFloat)) DrawSizeFloat(drawSettings);
            if (properties.HasFlag(VectorDrawerProperty.SizeVector2)) DrawSizeVector2(drawSettings);
            if (properties.HasFlag(VectorDrawerProperty.SizeVector3)) DrawSizeVector3(drawSettings);
            if (properties.HasFlag(VectorDrawerProperty.Rotation)) DrawQuaternion(drawSettings);
            if (properties.HasFlag(VectorDrawerProperty.WireView)) DrawWireSelection(drawSettings);
            DrawPositionHandleBool(drawSettings);
        }

        private void DrawPositionHandleBool(VectorDrawSettings drawSettings)
        {
            EditorGUI.BeginChangeCheck();
            var hideHandle = EditorGUILayout.Toggle("Hide Handle", drawSettings.HideHandle);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(VectorVisualizer.instance, "VectorVisualizerHideHandleChange");
                drawSettings.SetHideHandles(hideHandle);
            }
        }

        private void DrawWireSelection(VectorDrawSettings drawSettings)
        {
            EditorGUI.BeginChangeCheck();
            var isWire = EditorGUILayout.Toggle("Wire View", drawSettings.IsWire);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(VectorVisualizer.instance, "VectorVisualizerWireViewChange");
                drawSettings.SetIsWire(isWire);
            }
        }

        private void DrawSizeVector3(VectorDrawSettings drawSettings)
        {
            EditorGUI.BeginChangeCheck();
            var newSize = EditorGUILayout.Vector3Field("Size", drawSettings.SizeVector3);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(VectorVisualizer.instance, "VectorVisualizerSizeChange");
                drawSettings.SetSizeVector3(newSize);
            }
        }

        private void DrawSizeVector2(VectorDrawSettings drawSettings)
        {
            EditorGUI.BeginChangeCheck();
            var newSize = EditorGUILayout.Vector2Field("Size", drawSettings.SizeVector2);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(VectorVisualizer.instance, "VectorVisualizerSizeChange");
                drawSettings.SetSizeVector2(newSize);
            }
        }
        
        private void DrawColorPicker(VectorDrawSettings drawSettings)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Color");
            EditorGUI.BeginChangeCheck();
            var color = EditorGUILayout.ColorField(drawSettings.Color);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(VectorVisualizer.instance, "VectorVisualizerSizeChange");
                drawSettings.SetColor(color);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSizeFloat(VectorDrawSettings drawSettings)
        {
            EditorGUI.BeginChangeCheck();
            var size = EditorGUILayout.FloatField("Size", drawSettings.SizeFloat);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(VectorVisualizer.instance, "VectorVisualizerSizeChange");
                drawSettings.SetSize(size); 
            }
        }
        
        private void DrawQuaternion(VectorDrawSettings drawSettings)
        {
            EditorGUI.BeginChangeCheck();
            var newEuler = EditorGUILayout.Vector3Field("Rotation", drawSettings.EulerAngles);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(VectorVisualizer.instance, "VectorVisualizerRotationChange");
                drawSettings.SetRotation(newEuler);
            }
        }
    }
}