using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace VectorVisualizer
{
    [Serializable]
    public class VectorVisualizeObject
    {
        [SerializeField] private VectorDrawSettings m_vectorDrawSettings;
        [SerializeField] private string m_propertyPath;
        [SerializeField] private int m_id;
        [SerializeField] private Object m_object;
        
        private SerializedObject _serializedObject;
        private SerializedProperty _property;
        public VectorDrawSettings VectorDrawSettings => m_vectorDrawSettings;

        public Vector3 GetVector3Value()
        {
            if (_property.propertyType == SerializedPropertyType.Vector3)
            {
                return _property.vector3Value;
            }

            if (_property.propertyType == SerializedPropertyType.Vector2)
            {
                return _property.vector2Value;
            }

            return Vector3.zero;
        }

        public Object Object
        {
            get
            {
                //try restoring object from id
                if (m_object == null && m_id != 0)
                {
                    m_object = EditorUtility.InstanceIDToObject(m_id);
                }

                return m_object;
            }
        }

        public string PropertyPath => m_propertyPath;
        public Action OnHandleClicked;
        public Action OnPropertySelected;
        public SerializedObject SerializedObject => _serializedObject;
        public SerializedProperty SerializedProperty => _property;

        public VectorVisualizeObject(Object obj, string propertyPath, VectorDrawSettings vectorDrawSettings)
        {
            m_object = obj;
            m_id = obj.GetInstanceID();
            m_propertyPath = propertyPath;
            m_vectorDrawSettings = vectorDrawSettings;
        }

        public void UpdatePropertyPath(string propertyPath)
        {
            m_propertyPath = propertyPath;
        }

        public void CreateSerializedProperties()
        {
            _serializedObject = new SerializedObject(Object);
            _property = _serializedObject.FindProperty(PropertyPath);
        }

        public void SetDrawSettings(VectorDrawSettings drawSettings)
        {
            m_vectorDrawSettings = drawSettings;
        }
    }
}