using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VectorVisualizer
{
    [Serializable]
    public class VectorVisualizerProfileSo : ScriptableObject
    {
        [SerializeField] private string m_name;
        [SerializeField] private VectorDrawSettings m_drawSettings;
        public string Name => m_name;
        public VectorDrawSettings DrawSettings => m_drawSettings;
        
        public void SetName(string profileName)
        {
            m_name = profileName;
        }
        
        public void SetDrawSettings(VectorDrawSettings settings)
        {
            m_drawSettings = settings;
        }
    }
}