using System;
using UnityEngine;

namespace Library
{
    [ExecuteInEditMode]
    public class Parallax2D : MonoBehaviour
    {
        public TwoDeeLayer[] _layers;
        public Vector2 _offset = Vector2.zero;

        public Transform _targetCam;


        private void Reset()
        {
            _layers = new TwoDeeLayer[transform.childCount];

            for (var i = 0; i < transform.childCount; i++)
            {
                _layers[i].layer = transform.GetChild(i);
                _layers[i].move_multipler = (i + 1) / (transform.childCount + 1.0f);
                _layers[i].zPos = (i + 1) * 1.0f;
            }
        }

        // Use this for initialization
        private void Start()
        {
            if (!_targetCam) _targetCam = Camera.main.transform;
        }

        private void LateUpdate()
        {
            if (!_targetCam)
            {
                _targetCam = Camera.main.transform;
                return;
            }

            AdjustLayers(_targetCam.position);
        }

//		public void OnWillRenderObject()
//		{
//			if(!enabled)
//				return;
//
//			Camera cam = Camera.current;
//			if( !cam )
//				return;
//
//			AdjustLayers(cam.transform.position);
//
//		}

        private void AdjustLayers(Vector3 viewPoint)
        {
            viewPoint += (Vector3) _offset;

            var displacement = viewPoint - transform.position;
            var layerSpot = Vector3.zero;

            for (var i = 0; i < _layers.Length; i++)
            {
                layerSpot = displacement * _layers[i].move_multipler;
                layerSpot.z = _layers[i].zPos;

                _layers[i].layer.localPosition = layerSpot;
            }
        }

        [Serializable]
        public struct TwoDeeLayer
        {
            [Range(0.0f, 1.0f)] public float move_multipler;
            public float zPos;
            public Transform layer;
        }
    }
}