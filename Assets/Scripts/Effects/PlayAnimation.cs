using UnityEngine;

namespace Effects
{
    public class PlayAnimation : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Animation>().Play();
        }
    }
}
