using UnityEngine;

namespace Library.View
{
    public class SwipeDetector : MonoBehaviour
    {
        /*public delegate void SwipeHandler(SwipeDirection direction);

        public enum SwipeDirection
        {
            Up,
            Down,
            Right,
            Left
        }

        private const float GameSizeSwipeThreshold = 1.5f;
        private const bool DetectSwipeOnlyAfterRelease = true;

        private Vector2 _fingerDown;
        private Vector2 _fingerUp;


        private float _swipeThreshold;
        public event SwipeHandler OnSwipe;

        private void Awake()
        {
            _swipeThreshold = Screen.height / 2f / ViewUtils.ScreenHalfHeight * GameSizeSwipeThreshold;
        }

        private void Update()
        {
            if (Input.touchCount == 0)
                return;

            var touch = Input.touches[0];

            if (touch.phase == TouchPhase.Began)
            {
                _fingerUp = touch.position;
                _fingerDown = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
                if (!DetectSwipeOnlyAfterRelease)
                {
                    _fingerDown = touch.position;
                    CheckSwipe();
                }

            if (touch.phase == TouchPhase.Ended)
            {
                _fingerDown = touch.position;
                CheckSwipe();
            }
        }

        private void CheckSwipe()
        {
            if (VerticalMove() > _swipeThreshold && VerticalMove() > HorizontalValMove())
            {
                if (_fingerDown.y - _fingerUp.y > 0)
                    OnSwipe?.Invoke(SwipeDirection.Up);
                else if (_fingerDown.y - _fingerUp.y < 0)
                    OnSwipe?.Invoke(SwipeDirection.Down);

                _fingerUp = _fingerDown;
            }

            else if (HorizontalValMove() > _swipeThreshold && HorizontalValMove() > VerticalMove())
            {
                if (_fingerDown.x - _fingerUp.x > 0)
                    OnSwipe?.Invoke(SwipeDirection.Right);
                else if (_fingerDown.x - _fingerUp.x < 0)
                    OnSwipe?.Invoke(SwipeDirection.Left);

                _fingerUp = _fingerDown;
            }
        }

        private float VerticalMove()
        {
            return Mathf.Abs(_fingerDown.y - _fingerUp.y);
        }

        private float HorizontalValMove()
        {
            return Mathf.Abs(_fingerDown.x - _fingerUp.x);
        }*/
    }
}