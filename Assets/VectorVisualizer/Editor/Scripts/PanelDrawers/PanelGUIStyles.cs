using UnityEngine;

namespace VectorVisualizer
{
    public static class PanelGUIStyles
    {
        
        public static GUIStyle CenteredHeaderText
        {
            get
            {
                var style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontStyle = FontStyle.Bold;
                return style;
            }
        }
        
        public static GUIStyle CenteredSmallGreyText
        {
            get
            {
                var style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.grey;
                style.fontSize = 9;
                return style;
            }
        }
    }
}