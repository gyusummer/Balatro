using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance { get; private set; }

    private Queue<IEnumerator> _animationQueue = new Queue<IEnumerator>();
    private bool _isPlayingAnimation = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

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
