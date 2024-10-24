using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VectorVisualizer
{
    [Serializable]
    public class VectorVisualizerProfileHandler : ScriptableObject
    {
        [SerializeField] private VectorVisualizerProfileSo m_defaultProfileSo; 
        [SerializeField] private List<VectorVisualizerProfileSo> m_drawProfiles;

        public IEnumerable<VectorVisualizerProfileSo> GetDrawProfiles()
        {
            return m_drawProfiles.Where(x => x != null);
        }
        
        public void SetDefaultSettings(VectorDrawSettings settings)
        {
            m_defaultProfileSo.SetDrawSettings(settings);
            EditorUtility.SetDirty(m_defaultProfileSo);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // Creates a new profile so and saves it as an asset
        public VectorVisualizerProfileSo CreateNewProfile(string profileName, VectorDrawSettings settings)
        {
            var profile = CreateInstance<VectorVisualizerProfileSo>();
            profile.SetName(profileName);
            profile.SetDrawSettings(settings.Clone());
            var path = AssetDatabase.GetAssetPath(this);
            path = path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal));
            path = path + "/" + profileName + ".asset";
            m_drawProfiles.Add(profile);
            EditorUtility.SetDirty(this);
            AssetDatabase.CreateAsset(profile, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            var asset = AssetDatabase.LoadAssetAtPath<VectorVisualizerProfileSo>(path);
            return asset;
        }
        
        // Removes a profile so and deletes the asset
        public void RemoveProfile(VectorVisualizerProfileSo profileSo)
        {
            m_drawProfiles.Remove(profileSo);
            EditorUtility.SetDirty(this);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(profileSo));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        
        public VectorDrawSettings GetDefaultDrawSettings()
        {
            return m_defaultProfileSo.DrawSettings.Clone();
        }
    }
}