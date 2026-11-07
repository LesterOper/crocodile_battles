using UnityEngine;

namespace Core.Utils
{
    public static class TransformExtensions
    {
        public static Matrix4x4 TransformTo(this Transform from, Transform to) 
            => to.worldToLocalMatrix * from.localToWorldMatrix;
    }
}