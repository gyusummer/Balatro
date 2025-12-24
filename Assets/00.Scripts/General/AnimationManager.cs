using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{
    public static float PunchTime = 0.1f;
    public static float CardMoveTime = 0.2f;
    public static float CardSequenceGap = 0.1f;
    
    private Queue<IEnumerator> _animationQueue = new Queue<IEnumerator>();
    private bool _isPlayingAnimation = false;
    
    public void EnqueueAnimation(IEnumerator animationCoroutine)
    {
        _animationQueue.Enqueue(animationCoroutine);
        if (!_isPlayingAnimation)
        {
            StartNextAnimation();
        }
    }

    private void StartNextAnimation()
    {
        if (_animationQueue.Count > 0)
        {
            _isPlayingAnimation = true;
            StartCoroutine(AnimationRoutine(_animationQueue.Dequeue()));
        }
        else
        {
            _isPlayingAnimation = false;
        }
    }

    private IEnumerator AnimationRoutine(IEnumerator animationCoroutine)
    {
        yield return animationCoroutine;
        StartNextAnimation(); // Once current animation is done, start the next one
    }
}
