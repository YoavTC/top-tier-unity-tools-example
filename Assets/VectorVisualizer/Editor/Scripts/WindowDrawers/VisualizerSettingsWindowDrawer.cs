using UnityEditor;
using UnityEngine;

namespace VectorVisualizer
{
    public class VisualizerSettingsWindowDrawer : EditorWindow
    {
        private VectorVisualizerSettings _settings;

        public static void CreateWindow(VectorVisualizerSettings settings)
        {
            var window = GetWindow<VisualizerSettingsWindowDrawer>(true, "Vector Visualizer Settings");
            window.InitWindow(settings);
        }

        [MenuItem("Tools/Vector Visualizer/Settings")]
        public static void CreateDefaultWindow()
        {
            var window = GetWindow<VisualizerSettingsWindowDrawer>(true, "Vector Visualizer Settings");
            var settings = new VectorVisualizerSettings();
            settings.LoadSettingsOnEditorPrefs();
            window.InitWindow(settings);
        }

        private void InitWindow(VectorVisualizerSettings settings)
        {
            _settings = settings;
        }

        private void OnGUI()
        {
            if (_settings == null) return;
            var focusDistance = EditorGUILayout.FloatField("Focus distance", _settings.FocusDistance);
            if (focusDistance != _settings.FocusDistance)
            {
                _settings.SetFocusDistance(focusDistance);
            }
            var onlyTargetWithAttribute = EditorGUILayout.Toggle("Only target with attribute", _settings.OnlyTargetWithAttribute);
            if (onlyTargetWithAttribute != _settings.OnlyTargetWithAttribute)
            {
                _settings.SetOnlyTargetWithAttribute(onlyTargetWithAttribute);
            }
            var autoFocusOnSelection = EditorGUILayout.Toggle("Auto focus on selection", _settings.AutoFocusOnSelection);
            if (autoFocusOnSelection != _settings.AutoFocusOnSelection)
            {
                _settings.SetAutoFocusOnSelection(autoFocusOnSelection);
            }
            var autoFocusOnAdd = EditorGUILayout.Toggle("Auto focus on add", _settings.AutoFocusOnAdd);
            if (autoFocusOnAdd != _settings.AutoFocusOnAdd)
            {
                _settings.SetAutoFocusOnAdd(autoFocusOnAdd);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Handlers with Z-test against 3D objects");
            var handleZTest = EditorGUILayout.Toggle( _settings.HandleZTest);
            if (handleZTest != _settings.HandleZTest)
            {
                _settings.SetHandleZTest(handleZTest);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}