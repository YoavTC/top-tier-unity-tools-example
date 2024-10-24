using System;

namespace VectorVisualizer
{
    [Flags]
    public enum VectorDrawerProperty
    {
        None = 0,
        SizeFloat  = 1 << 0,
        SizeVector2 = 1 << 1,
        SizeVector3 = 1 << 2,
        Color = 1 << 3,
        Rotation = 1 << 4,
        WireView = 1 << 5
    }
}