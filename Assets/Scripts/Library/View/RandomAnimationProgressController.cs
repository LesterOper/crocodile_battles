using UnityEngine;

public class RandomAnimationProgressController : MonoBehaviour
{
    private Animation _animation;

    private void Start()
    {
        _animation = GetComponent<Animation>();
        _animation[_animation.clip.name].time = Random.Range(0.0f, _animation[_animation.clip.name].clip.length);
    }
}