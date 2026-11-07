using UnityEngine;

namespace Library
{
    public class RepeatingBackground : MonoBehaviour
    {
        private float _groundHorizontalLength;

        [SerializeField] private float _speed;
        private SpriteRenderer _spriteRenderer;


        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _groundHorizontalLength = _spriteRenderer.size.x;
        }


        private void Update()
        {
            if (transform.position.x < -_groundHorizontalLength)
                RepositionBackground();
            else
                transform.Translate(_speed * Time.deltaTime, 0, 0);
        }

        private void RepositionBackground()
        {
            var groundOffSet = new Vector2(_groundHorizontalLength * 2f, 0);
            transform.position = (Vector2) transform.position + groundOffSet;
        }
    }
}