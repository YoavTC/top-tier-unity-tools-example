using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VectorVisualizer
{
    [Serializable]
    public class VectorVisualizerPanelDrawer
    {
        private Rect _windowRect;

        private DrawSettingsPanel _drawSettingsPanel = new DrawSettingsPanel();
        private ProfileSelectionPanelDrawer _profileSelectionPanelDrawer;
        private VectorVisualizerSettings _settings;
        private int _selectedIndex;
        private string[] _visualizeObjectNames;
        private List<VectorVisualizeObject> _visualizeObjects = new List<VectorVisualizeObject>();

        private IVectorDrawer[] _drawers;
        private bool _foldout;

        private int _windowId;

        private float _windowWidth = 200;
        private float _foldoutWidth = 60;


        public void Init(VectorVisualizerProfileHandler profileHandler,
            List<VectorVisualizeObject> visualizeObjects,
            IVectorDrawer[] drawers, VectorVisualizerSettings settings)
        {
            _settings = settings;
            _profileSelectionPanelDrawer = new ProfileSelectionPanelDrawer(profileHandler);
            _drawers = drawers;
            _visualizeObjects = new List<VectorVisualizeObject>();

            foreach (var visualizeObject in visualizeObjects)
            {
                RegisterVisualizeObject(visualizeObject);
            }

            CreateVisualizeObjectNameArray();

            SceneView.duringSceneGui += OnSceneGUI;

            _windowRect = new Rect(10, 10, _windowWidth, 0);
            _windowId = Guid.NewGuid().GetHashCode();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (_visualizeObjects.Count <= 0) return;


            var windowStyle = new GUIStyle(GUI.skin.window);
            windowStyle.padding.top = 0;
            _windowRect.height = 0;

            _windowRect.width = _foldout ? _foldoutWidth : _windowWidth;
            var windowX = _settings.WindowX;
            var windowY = _settings.WindowY;
            _windowRect.x = windowX;
            _windowRect.y = windowY;

            _windowRect = GUILayout.Window(_windowId, _windowRect, DrawWindow, "", windowStyle);

            CheckBounds(sceneView);

            // Save window position if it has changed
            if (windowX != _windowRect.x || windowY != _windowRect.y)
            {
                _settings.SetWindowPosition(_windowRect.x, _windowRect.y);
            }
        }

        // Check if the window is out of bounds in scene view
        private void CheckBounds(SceneView sceneView)
        {
            if (_windowRect.x < 0) _windowRect.x = 0;
            if (_windowRect.y < 0) _windowRect.y = 0;
            if (_windowRect.x > sceneView.position.width - _windowRect.width)
                _windowRect.x = sceneView.position.width - _windowRect.width;
            if (_windowRect.y > sceneView.position.height - _windowRect.height)
                _windowRect.y = sceneView.position.height - _windowRect.height;
        }

        private void DrawWindow(int id)
        {
            DrawDragArea();
            if (_foldout) return;
            DrawTargetVector();
            if (_visualizeObjects.Count <= 0) return;
            GUILayout.Space(5);
            GUILayout.Label("Draw Settings", PanelGUIStyles.CenteredHeaderText);
            var settingsUpdated =
                _drawSettingsPanel.Draw(_visualizeObjects[_selectedIndex].VectorDrawSettings, _drawers, _windowRect,
                    3);
            _profileSelectionPanelDrawer.Draw(_visualizeObjects[_selectedIndex]);
            if (settingsUpdated)
            {
                _profileSelectionPanelDrawer.SetDirtySelectedProfile();
            }
        }

        private void DrawTargetVector()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            var selectedIndex = EditorGUILayout.Popup(_selectedIndex, _visualizeObjectNames);
            if (EditorGUI.EndChangeCheck())
            {
                SelectVector(selectedIndex, _settings.AutoFocusOnSelection);
            }

            if (GUILayout.Button("Remove"))
            {
                VectorVisualizer.instance.RemoveVisualizeObject(_visualizeObjects[_selectedIndex]);
            }

            if (GUILayout.Button("Focus"))
            {
                FocusVisualizeObject();
            }

            GUILayout.EndHorizontal();
        }

        private void FocusVisualizeObject()
        {
            SceneView.lastActiveSceneView.LookAt(_visualizeObjects[_selectedIndex].GetVector3Value(),
                Quaternion.Euler(_settings.FocusEulerAngles), _settings.FocusDistance);
        }

        private void DrawDragArea()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_foldout ? "▲" : "▼", GUILayout.Width(20)))
            {
                _foldout = !_foldout;
            }

            GUILayout.Label(_foldout ? "VV" : "Vector Visualizer", PanelGUIStyles.CenteredHeaderText);
            if (!_foldout && GUILayout.Button("⋮", GUILayout.Width(20)))
            {
                VisualizerSettingsWindowDrawer.CreateWindow(_settings);
            }

            if (!_foldout && GUILayout.Button("X", GUILayout.Width(20)))
            {
                VectorVisualizer.instance.RemoveAllVisualizeObjects();
            }

            GUILayout.EndHorizontal();
            GUI.DragWindow(new Rect(0, 0, _windowRect.width, 20));
        }


        private void RegisterVisualizeObject(VectorVisualizeObject visualizeObject)
        {
            _visualizeObjects.Add(visualizeObject);
            visualizeObject.OnHandleClicked += () => SelectVector(_visualizeObjects.IndexOf(visualizeObject), false);
            visualizeObject.OnPropertySelected += () => SelectVector(_visualizeObjects.IndexOf(visualizeObject), false);
        }

        public void AddVisualizeObject(VectorVisualizeObject visualizeObject)
        {
            RegisterVisualizeObject(visualizeObject);
            CreateVisualizeObjectNameArray();
            SelectVector(_visualizeObjects.Count - 1, _settings.AutoFocusOnAdd);
        }


        // Create the array of visualize object names for the dropdown
        private void CreateVisualizeObjectNameArray()
        {
            var visualizeObjectNames = new List<string>();
            _visualizeObjectNames = Array.Empty<string>();
            foreach (var visualizeObject in _visualizeObjects)
            {
                var visualizeName = GetVisualizeName(visualizeObject, visualizeObjectNames);
                visualizeObjectNames.Add(visualizeName);
            }

            _visualizeObjectNames = visualizeObjectNames.ToArray();
        }

        private void SelectVector(int index, bool focus)
        {
            _selectedIndex = index;
            _profileSelectionPanelDrawer.SetSelectedProfile(_visualizeObjects[_selectedIndex]);
            if (focus) FocusVisualizeObject();
        }

        public void RemoveVisualizeObject(VectorVisualizeObject visualizeObject)
        {
            var selectedVisualizeObject = _visualizeObjects[_selectedIndex];
            _visualizeObjects.Remove(visualizeObject);
            _selectedIndex = selectedVisualizeObject == visualizeObject
                ? 0
                : _visualizeObjects.IndexOf(selectedVisualizeObject);
            CreateVisualizeObjectNameArray();
        }

        // Get a unique name for the visualize object
        private string GetVisualizeName(VectorVisualizeObject visualizeObject, List<string> visualizeObjectNames)
        {
            var createdName = $"{visualizeObject.Object}:{visualizeObject.PropertyPath}";
            if (visualizeObjectNames.Any(x => StringComparer.InvariantCulture.Compare(createdName, x) == 0))
            {
                var index = 1;
                while (visualizeObjectNames.Any(x =>
                           StringComparer.InvariantCulture.Compare($"{createdName} ({index})", x) == 0))
                {
                    index++;
                }

                return $"{createdName} ({index})";
            }

            return createdName;
        }
    }
}