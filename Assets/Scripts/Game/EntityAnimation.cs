using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class EntityAnimation : MonoBehaviour
{
    [Header("Render")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private CancellationTokenSource _animCts;

    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    public void Play(Sprite[] frames, float[] delays)
    {
        Stop();

        if (frames == null || frames.Length == 0)
            return;

        SetSprite(frames[0]);

        _animCts = new CancellationTokenSource();
        PlayAsync(frames, delays, _animCts.Token).Forget();
    }

    public void Stop()
    {
        if (_animCts == null)
            return;

        _animCts.Cancel();
        _animCts.Dispose();
        _animCts = null;
    }

    protected void SetSprite(Sprite sprite)
    {
        if (_spriteRenderer == null || sprite == null)
            return;

        _spriteRenderer.sprite = sprite;
    }

    private async UniTaskVoid PlayAsync(Sprite[] frames, float[] delays, CancellationToken token)
    {
        int index = 0;

        try
        {
            while (!token.IsCancellationRequested)
            {
                SetSprite(frames[index]);

                float delay = 0.1f;
                if (delays != null && delays.Length > 0)
                    delay = delays[Mathf.Clamp(index, 0, delays.Length - 1)];

                if (delay < 0.003f)
                    delay = 0.003f;

                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);

                index++;
                if (index >= frames.Length)
                    index = 0;
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    protected virtual void OnDestroy()
    {
        Stop();
    }
}
