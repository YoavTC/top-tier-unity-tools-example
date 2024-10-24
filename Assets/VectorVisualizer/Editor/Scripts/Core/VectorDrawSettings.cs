using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VectorVisualizer
{
    [Serializable]
    public class VectorDrawSettings
    {
        [SerializeField] private string m_drawerId;
        [SerializeField] private bool m_hideHandle;
        [SerializeField] private float m_sizeFloat;
        [SerializeField] private Vector2 m_sizeVector2;
        [SerializeField] private Vector3 m_sizeVector3;
        [SerializeField] private Color m_color;
        [SerializeField] private Vector3 m_eulerAngles;
        [SerializeField] private bool m_isWire;

        public bool HideHandle => m_hideHandle;
        public string DrawerId => m_drawerId;
        public Color Color => m_color;
        public Vector3 EulerAngles => m_eulerAngles;
        public float SizeFloat => m_sizeFloat;
        public Vector2 SizeVector2 => m_sizeVector2;
        public Vector3 SizeVector3 => m_sizeVector3;

        public bool IsWire => m_isWire;

        public void SetHideHandles(bool hide)
        {
            m_hideHandle = hide;
        }

        public void SetIsWire(bool isWire)
        {
            m_isWire = isWire;
        }

        public void SetDrawerId(string id)
        {
            m_drawerId = id;
        }

        public void SetSize(float size)
        {
            m_sizeFloat = size;
        }

        public void SetSizeVector2(Vector2 size)
        {
            m_sizeVector2 = size;
        }

        public void SetSizeVector3(Vector3 size)
        {
            m_sizeVector3 = size;
        }

        public void SetColor(Color color)
        {
            m_color = color;
        }

        public void SetRotation(Vector3 euler)
        {
            m_eulerAngles = euler;
        }

        public VectorDrawSettings Clone()
        {
            return new VectorDrawSettings()
            {
                m_hideHandle = m_hideHandle,
                m_sizeFloat = m_sizeFloat,
                m_sizeVector2 = m_sizeVector2,
                m_sizeVector3 = m_sizeVector3,
                m_color = m_color,
                m_eulerAngles = m_eulerAngles,
                m_isWire = m_isWire,
                m_drawerId = m_drawerId
            };
        }
    }
}