using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace VectorVisualizer
{
    public class ProfileSelectionPanelDrawer
    {
        private Dictionary<string, VectorVisualizerProfileSo> _profileDictionary;
        private string[] _profileNames;
        private int _selectedIndex;
        private bool _creationMode;
        private string _newProfileName;
        private readonly VectorVisualizerProfileHandler _profileHandler;

        public ProfileSelectionPanelDrawer(VectorVisualizerProfileHandler profileHandler)
        {
            _profileHandler = profileHandler;
            
            _profileDictionary = new Dictionary<string, VectorVisualizerProfileSo> {{"-", null}};

            foreach (var profile in profileHandler.GetDrawProfiles())
            {
                _profileDictionary.Add(profile.Name, profile);
            }
            
            _profileNames = _profileDictionary.Keys.ToArray();
        }

        //check and set selected profile for visualizer object
        public void SetSelectedProfile(VectorVisualizeObject visualizeObject)
        {
            var match = _profileDictionary.Values.FirstOrDefault(x =>
                x != null && x.DrawSettings == visualizeObject.VectorDrawSettings);
            _selectedIndex = match != null ? _profileDictionary.Keys.ToList().IndexOf(match.Name) : 0;
        }
        
        public void SetDirtySelectedProfile()
        {
            var selectedProfile = _profileDictionary.Values.ElementAt(_selectedIndex);
            if (selectedProfile == null) return;
            EditorUtility.SetDirty(selectedProfile);
        }

        public void Draw(VectorVisualizeObject visualizeObject)
        {
            GUILayout.Space(5);
            DrawSetDefaultButton(visualizeObject);
            DrawProfileSelection(visualizeObject);
            DrawCreateProfileRow(visualizeObject);
            DrawUnsavedChangesWarning();
        }

        private void DrawUnsavedChangesWarning()
        {
            var anyIsDirty = _profileDictionary.Values.Any(EditorUtility.IsDirty);
            if (!anyIsDirty) return;
            GUILayout.Label("Save profile changes with 'Ctrl + S'",PanelGUIStyles.CenteredSmallGreyText);
        }

        private void DrawCreateProfileRow(VectorVisualizeObject visualizeObject)
        {
            if (!_creationMode) return;
            GUILayout.BeginHorizontal();

            _newProfileName = GUILayout.TextField(_newProfileName);
            if (GUILayout.Button("Create", GUILayout.Width(50)))
            {
                if (string.IsNullOrEmpty(_newProfileName))
                {
                    Debug.LogError("Profile name cannot be empty");
                    return;
                }
                CreateProfile(visualizeObject);
                UpdateVisualizeObjectProfile(visualizeObject);
                _creationMode = false;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSetDefaultButton(VectorVisualizeObject visualizeObject)
        {
            if (GUILayout.Button("Set as default settings"))
            {
                _profileHandler.SetDefaultSettings(visualizeObject.VectorDrawSettings);
            }
            
        }
        
        private void UpdateVisualizeObjectProfile(VectorVisualizeObject visualizeObject)
        {
            var selectedProfile = _profileDictionary.Values.ElementAt(_selectedIndex);
            visualizeObject.SetDrawSettings(selectedProfile != null
                ? selectedProfile.DrawSettings
                : _profileHandler.GetDefaultDrawSettings());
        }

        private void DrawProfileSelection(VectorVisualizeObject visualizeObject)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Profile: ");
            EditorGUI.BeginChangeCheck();
            _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _profileNames);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateVisualizeObjectProfile(visualizeObject);
            }

            if (GUILayout.Button("+"))
            {
                _creationMode = !_creationMode;
            }

            if (_selectedIndex != 0 && GUILayout.Button("-"))
            {
                RemoveProfile();
                UpdateVisualizeObjectProfile(visualizeObject);
            }

            EditorGUILayout.EndHorizontal();
        }
        
        private void CreateProfile( VectorVisualizeObject visualizeObject)
        {
            var newProfile = _profileHandler.CreateNewProfile(_newProfileName,visualizeObject.VectorDrawSettings);
            _profileDictionary.Add(newProfile.Name, newProfile);
            _profileNames = _profileDictionary.Keys.ToArray();
            _selectedIndex = _profileNames.Length - 1;
        }
        
        private void RemoveProfile()
        {
            _profileHandler.RemoveProfile(_profileDictionary.Values.ElementAt(_selectedIndex));
            _profileDictionary.Remove(_profileDictionary.Keys.ElementAt(_selectedIndex));
            _profileNames = _profileDictionary.Keys.ToArray();
            _selectedIndex = 0;
        }
    }
}