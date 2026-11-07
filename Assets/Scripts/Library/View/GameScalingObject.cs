using Library.Utils;
using UnityEngine;

namespace Library.View
{
    public class GameScalingObject : ProgressGameObject
    {
        private Vector3 _destScale;
        private Vector3 _middleScale;
        private Vector3 _startScale;

        public void Scale(Vector3 start, Vector3 dest, CompleteHandler completeHandler)
        {
            _startScale = start;
            _middleScale = Vector3.negativeInfinity;
            _destScale = dest;

            Run(completeHandler);
        }

        public void Scale(Vector3 start, Vector3 middle, Vector3 dest, CompleteHandler completeHandler)
        {
            _startScale = start;
            _middleScale = middle;
            _destScale = dest;

            Run(completeHandler);
        }

        protected override void OnProgressChange(float progress)
        {
            if (float.IsNegativeInfinity(_middleScale.x))
                transform.localScale = Vector3.Lerp(_startScale, _destScale, progress);
            else
                transform.localScale = MathA.Bezier3(_startScale, _middleScale, _destScale, progress);
        }
    }
}