using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VectorVisualizer
{
    [Serializable]
    public class VectorVisualizer : ScriptableSingleton<VectorVisualizer>
    {
        [SerializeField] private VectorVisualizerProfileHandler m_profileHandler;
        [SerializeField] private List<VectorVisualizeObject> m_visualizeObjects = new List<VectorVisualizeObject>();

        private VectorVisualizerPanelDrawer _drawer = new VectorVisualizerPanelDrawer();
        private VectorSceneViewDrawer _sceneViewDrawer = new VectorSceneViewDrawer();
        private VectorVisualizerSettings _settings = new VectorVisualizerSettings();

        // Vector drawers to be used by the visualizer, if you want to add a custom drawer, add it here
        private IVectorDrawer[] _drawers = new IVectorDrawer[]
        {
            new CubeDrawer(),
            new SphereDrawer(),
            new PyramidDrawer(),
        };

        // Initializes the visualizer, loads settings and resets events
        public void Init()
        {
            m_visualizeObjects.ForEach(x => x.CreateSerializedProperties());
            ResetVisualizeObjectEvents();

            _settings.LoadSettingsOnEditorPrefs();

            _drawer.Init(m_profileHandler, m_visualizeObjects, _drawers, _settings);
            _sceneViewDrawer.Init(m_visualizeObjects, _drawers,_settings);

            EditorApplication.update -= CheckNullVisualizeObjects;
            EditorApplication.update += CheckNullVisualizeObjects;
        }

        // Adds a property to the visualizer
        public void AddProperty(SerializedProperty prop)
        {
            var defaultDrawSettings = m_profileHandler.GetDefaultDrawSettings();
            var debugObject = new VectorVisualizeObject(prop.serializedObject.targetObject, prop.propertyPath,
                defaultDrawSettings);
            debugObject.CreateSerializedProperties();
            AddVisualizeObject(debugObject);
        }

        // Removes a property from the visualizer
        public void RemoveProperty(SerializedProperty prop)
        {
            var debugObject = m_visualizeObjects.FirstOrDefault(x =>
                x.Object == prop.serializedObject.targetObject && x.PropertyPath == prop.propertyPath);
            RemoveVisualizeObject(debugObject);
        }

        // Removes a specific visualize object from the visualizer
        public void RemoveVisualizeObject(VectorVisualizeObject visualizeObject)
        {
            _drawer.RemoveVisualizeObject(visualizeObject);
            _sceneViewDrawer.RemoveVisualizeObject(visualizeObject);
            m_visualizeObjects.Remove(visualizeObject);
        }

        // Removes all visualize objects from the visualizer
        public void RemoveAllVisualizeObjects()
        {
            for (var index = m_visualizeObjects.Count - 1; index >= 0; index--)
            {
                var debugObject = m_visualizeObjects[index];
                RemoveVisualizeObject(debugObject);
            }
        }

        // Selects a specific property and triggers the OnPropertySelected event if available
        public void SelectProperty(SerializedProperty prop)
        {
            var debugObject = m_visualizeObjects.FirstOrDefault(x =>
                x.Object == prop.serializedObject.targetObject && x.PropertyPath == prop.propertyPath);
            debugObject?.OnPropertySelected?.Invoke();
        }

        // Checks has a property in the visualizer
        public bool HasProperty(Object obj, string propertyPath)
        {
            return m_visualizeObjects.Any(x => x.Object == obj && x.PropertyPath == propertyPath);
        }
        
        // Adds a visualize object to the visualizer
        private void AddVisualizeObject(VectorVisualizeObject visualizeObject)
        {
            _drawer.AddVisualizeObject(visualizeObject);
            _sceneViewDrawer.AddVisualizeObject(visualizeObject);
            m_visualizeObjects.Add(visualizeObject);
        }

        // Checks for null or invalid visualize objects and removes them
        private void CheckNullVisualizeObjects()
        {
            for (var index = m_visualizeObjects.Count - 1; index >= 0; index--)
            {
                var debugObject = m_visualizeObjects[index];

                if (debugObject.Object == null)
                {
                    RemoveVisualizeObject(debugObject);
                    continue;
                }

                if (debugObject.SerializedObject.FindProperty(debugObject.PropertyPath) == null)
                {
                    RemoveVisualizeObject(debugObject);
                    continue;
                }
            }
        }
        
        // Resets the events of all visualize objects
        private void ResetVisualizeObjectEvents()
        {
            m_visualizeObjects.ForEach(x => x.OnPropertySelected = null);
            m_visualizeObjects.ForEach(x => x.OnHandleClicked = null);
        }
    }
}