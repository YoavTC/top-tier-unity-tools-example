using UnityEditor;

namespace VectorVisualizer
{
    [InitializeOnLoad]
    public class VectorVisualizerInitializer
    {
        static VectorVisualizerInitializer()
        {
            VectorVisualizer.instance.Init();
        }
    }
}