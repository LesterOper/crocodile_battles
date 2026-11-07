using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Crocodile
{
    [CreateAssetMenu(menuName = "Setup/MovingAnimationCurvesContainer", fileName = "MovingAnimationCurvesContainer")]
    public class MovingAnimationCurvesContainer : ScriptableObject
    {
        [SerializeField] private List<AnimationCurveItem> animations;

        public AnimationCurve GetAnimationCurve(AnimationType type) =>
            animations.FirstOrDefault(curveItem => curveItem.type == type).Curve;
    }

    [Serializable]
    public struct AnimationCurveItem
    {
        [SerializeField]public AnimationType type;
        [SerializeField]public  AnimationCurve Curve;
    }

    public enum AnimationType
    {
        NONE = 0,
        LIGHT = 1,
        MEDIUM = 2,
        HEAVY = 3,
        LIGHT_1 = 4,
    
        SLOW = 99,
    }
}