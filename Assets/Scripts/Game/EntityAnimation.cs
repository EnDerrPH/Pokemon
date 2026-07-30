using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public abstract class EntityAnimation : MonoBehaviour
{
    [Header("Render")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private CancellationTokenSource _animCts;
    private Tween _fxTween;

    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    private Transform FxTarget => _spriteRenderer != null ? _spriteRenderer.transform : transform;

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

    public UniTask PlayJumpFx(float jumpHeight, float duration)
    {
        Transform target = FxTarget;
        if (target == null)
            return UniTask.CompletedTask;

        _fxTween?.Kill();

        float halfDuration = Mathf.Max(0.01f, duration * 0.5f);
        float baseY = target.localPosition.y;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(target.DOLocalMoveY(baseY + jumpHeight, halfDuration));
        sequence.Append(target.DOLocalMoveY(baseY, halfDuration));
        _fxTween = sequence;

        return WaitForTween(sequence);
    }

    public UniTask PlayPunchScaleFx(Vector3 punchScale, float duration, int vibrato, float elasticity)
    {
        Transform target = FxTarget;
        if (target == null)
            return UniTask.CompletedTask;

        _fxTween?.Kill();

        Vector3 baseScale = target.localScale;
        target.localScale = baseScale;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(target.DOPunchScale(punchScale, duration, vibrato, elasticity));
        sequence.Append(target.DOPunchScale(punchScale, duration, vibrato, elasticity));
        sequence.OnKill(() =>
        {
            if (target != null)
                target.localScale = baseScale;
        });
        sequence.OnComplete(() =>
        {
            if (target != null)
                target.localScale = baseScale;
        });

        _fxTween = sequence;
        return WaitForTween(sequence);
    }

    public UniTask PlayShakeFx(float duration, float strength)
    {
        Transform target = FxTarget;
        if (target == null)
            return UniTask.CompletedTask;

        _fxTween?.Kill();

        Tween shakeTween = target.DOShakePosition(
            duration,
            new Vector3(strength, strength, 0f),
            20,
            90f,
            false,
            true);

        _fxTween = shakeTween;
        return WaitForTween(shakeTween);
    }

    private static UniTask WaitForTween(Tween tween)
    {
        if (tween == null)
            return UniTask.CompletedTask;

        UniTaskCompletionSource completion = new UniTaskCompletionSource();
        bool finished = false;

        tween.OnComplete(() =>
        {
            if (finished)
                return;

            finished = true;
            completion.TrySetResult();
        });

        tween.OnKill(() =>
        {
            if (finished)
                return;

            finished = true;
            completion.TrySetResult();
        });

        return completion.Task;
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
        _fxTween?.Kill();
        _fxTween = null;
    }
}
