using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VectorVisualizer
{
    [Serializable]
    public class VectorSceneViewDrawer
    {
        private IVectorDrawer[] _drawers;
        private List<VectorVisualizeObject> _visualizeObjects;
        private VectorVisualizerSettings _settings;

        public void Init(List<VectorVisualizeObject> visualizeObject, IVectorDrawer[] drawers,
            VectorVisualizerSettings settings)
        {
            _settings = settings;
            _drawers = drawers;
            _visualizeObjects = new List<VectorVisualizeObject>(visualizeObject);
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        //Draws the vectors on the scene view
        private void OnSceneGUI(SceneView obj)
        {
            foreach (var visualizeObject in _visualizeObjects)
            {
                visualizeObject.SerializedObject.Update();
                if (visualizeObject.Object == null) continue;
                if (visualizeObject.SerializedObject.FindProperty(visualizeObject.PropertyPath) == null ||
                    visualizeObject.SerializedProperty.propertyType == SerializedPropertyType.Generic) continue;
                var activeDrawer = Array.Find(_drawers, x => x.Id == visualizeObject.VectorDrawSettings.DrawerId);
                var backupValue = Handles.zTest;
                if (_settings.HandleZTest) Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                activeDrawer?.DrawVectorOnSceneView(visualizeObject, visualizeObject.VectorDrawSettings);
                if (_settings.HandleZTest) Handles.zTest = backupValue;
                DrawPositionHandle(visualizeObject, visualizeObject.VectorDrawSettings);
            }
        }

        private void DrawPositionHandle(VectorVisualizeObject visualizeObject,
            VectorDrawSettings drawSettings)
        {
            if (drawSettings.HideHandle) return;
            var prop = visualizeObject.SerializedProperty;
            Vector3 objectPosition = prop.propertyType == SerializedPropertyType.Vector3
                ? prop.vector3Value
                : prop.vector2Value;

            //Position handle ids support added after 2022
#if UNITY_2022_1_OR_NEWER
            var isMouseClicked = Event.current.type == EventType.MouseDown;
            var handleIds = Handles.PositionHandleIds.@default;
            var newPos = Handles.PositionHandle(handleIds, objectPosition, Quaternion.Euler(drawSettings.EulerAngles));
#else
            var newPos = Handles.PositionHandle(objectPosition, Quaternion.Euler(drawSettings.EulerAngles));
#endif
            if (EditorGUI.EndChangeCheck())
            {
                if (prop.propertyType == SerializedPropertyType.Vector3)
                {
                    prop.vector3Value = newPos;
                }
                else
                {
                    prop.vector2Value = (Vector2) newPos;
                }

                prop.serializedObject.ApplyModifiedProperties();
            }

#if UNITY_2022_1_OR_NEWER

            if (!isMouseClicked) return;

            if (GUIUtility.hotControl != handleIds.x && GUIUtility.hotControl != handleIds.y &&
                GUIUtility.hotControl != handleIds.z) return;

            visualizeObject.OnHandleClicked?.Invoke();
#endif
        }

        public void AddVisualizeObject(VectorVisualizeObject visualizeObject)
        {
            _visualizeObjects.Add(visualizeObject);
            SceneView.RepaintAll();
        }

        public void RemoveVisualizeObject(VectorVisualizeObject visualizeObject)
        {
            _visualizeObjects.Remove(visualizeObject);
            SceneView.RepaintAll();
        }
    }
}