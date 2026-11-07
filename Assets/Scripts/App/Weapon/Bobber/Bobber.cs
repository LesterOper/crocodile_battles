using System.Collections.Generic;
using App.Weapon.WeaponBases;
using UnityEngine;

namespace App.Weapon.Bobber
{
    public class Bobber : EquipmentBase
    {
        [Header("Bobber")] 
        [SerializeField] private ParticleSystem ripple;
        [SerializeField] private List<GameObject> ropeSegments = new List<GameObject>();
        [SerializeField] private LineRenderer lineRenderer = null;
        [SerializeField] private GameObject anchor = null;
        [SerializeField] private GameObject bobber = null;
        [SerializeField] private GameObject bobberParent = null;
        private SpringJoint2D _connectedObjectJoint;
        private bool isOnTriggerStay;
        private void OnDestroy() => Destroy(bobberParent);

        private void Start()
        {
            bobberParent.transform.parent = null;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("WaterSurface"))
                ripple.Play();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!isOnTriggerStay)
            {
                var main = ripple.main;
                main.loop = true;
                ripple.Play();
                isOnTriggerStay = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (isOnTriggerStay)
            {
                var main = ripple.main;
                main.loop = false;
                ripple.Play();
                isOnTriggerStay = false;
            }
        }

        private void Update()
        {
            if (_connectedObjectJoint == null)
                _connectedObjectJoint = bobber.GetComponent<SpringJoint2D>();
            lineRenderer.positionCount = ropeSegments.Count + 2;
            lineRenderer.SetPosition(0, anchor.transform.position);

            for (int i = 0; i < ropeSegments.Count; i++)
            {
                lineRenderer.SetPosition(i+1, ropeSegments[i].transform.position);
            }
        
            lineRenderer.SetPosition(ropeSegments.Count+1, bobber.transform.TransformPoint(_connectedObjectJoint.anchor));
        }
    }
}
