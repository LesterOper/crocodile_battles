using Library.Utils;
using UnityEngine;

namespace Library.View
{
    public class GameMovingObject : ProgressGameObject
    {
        private Vector3 _destPosition;
        private Transform _destTransform;
        private Vector3 _middlePosition;
        private Vector3 _startPosition;

        public void Move(Vector3 start, Vector3 dest, CompleteHandler completeHandler)
        {
            _startPosition = start;
            _middlePosition = Vector3.negativeInfinity;
            _destPosition = dest;

            Run(completeHandler);
        }

        public void Move(Vector3 start, Vector3 middle, Vector3 dest, CompleteHandler completeHandler)
        {
            _startPosition = start;
            _middlePosition = middle;
            _destPosition = dest;

            Run(completeHandler);
        }


        public void Move(Vector3 start, Vector3 middle, Transform destTransform, CompleteHandler completeHandler)
        {
            _startPosition = start;
            _middlePosition = middle;
            _destTransform = destTransform;

            Run(completeHandler);
        }

        protected override void OnProgressChange(float progress)
        {
            if (float.IsNegativeInfinity(_middlePosition.x))
            {
                transform.position = Vector3.Lerp(_startPosition, _destPosition, progress);
            }
            else
            {
                if (_destTransform != null)
                    transform.position =
                        MathA.Bezier3(_startPosition, _middlePosition, _destTransform.position, progress);
                else
                    transform.position = MathA.Bezier3(_startPosition, _middlePosition, _destPosition, progress);
            }
        }
    }
}