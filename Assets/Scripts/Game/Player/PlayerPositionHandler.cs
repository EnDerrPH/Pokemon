using UnityEngine;

public class PlayerPositionHandler : MonoBehaviour
{
    private const string MewtwoName = "mewtwo";
    private const string VenomothName = "venomoth";
    private const string MukName = "muk";

    [SerializeField] private Transform _target;
    [SerializeField] private float _maxWidth = 1.2f;
    [SerializeField] private float _shiftFactor = 0.3f;
    [SerializeField] private float _mewtwoShiftFactor = 0.5f;
    [SerializeField] private float _smallShiftFactor = 0.2f;

    private float _baseLocalX;
    private bool _hasBaseLocalX;

    private Transform Target
    {
        get
        {
            if (_target != null)
                return _target;

            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            return spriteRenderer != null ? spriteRenderer.transform : transform;
        }
    }

    public void Apply(PokemonData data)
    {
        if (data == null)
        {
            Apply(null, _shiftFactor);
            return;
        }

        Apply(GetWidestSprite(data.BackAnimSprites), GetShiftFactor(data));
    }

    public void Reset()
    {
        if (!_hasBaseLocalX)
            return;

        Transform target = Target;
        Vector3 localPosition = target.localPosition;
        localPosition.x = _baseLocalX;
        target.localPosition = localPosition;
    }

    private void Apply(Sprite sprite, float shiftFactor)
    {
        CacheBaseLocalX();

        Transform target = Target;
        Vector3 localPosition = target.localPosition;
        localPosition.x = _baseLocalX;

        if (sprite != null)
        {
            float worldWidth = sprite.bounds.size.x * Mathf.Abs(target.lossyScale.x);
            if (worldWidth > _maxWidth)
                localPosition.x = _baseLocalX - (worldWidth - _maxWidth) * shiftFactor;
        }

        target.localPosition = localPosition;
    }

    private float GetShiftFactor(PokemonData data)
    {
        if (IsNamed(data, MewtwoName))
            return _mewtwoShiftFactor;

        if (IsNamed(data, VenomothName) || IsNamed(data, MukName))
            return _smallShiftFactor;

        return _shiftFactor;
    }

    private void CacheBaseLocalX()
    {
        if (_hasBaseLocalX)
            return;

        _baseLocalX = Target.localPosition.x;
        _hasBaseLocalX = true;
    }

    private static bool IsNamed(PokemonData data, string pokemonName)
    {
        return data != null
            && string.Equals(data.PokemonName, pokemonName, System.StringComparison.OrdinalIgnoreCase);
    }

    private static Sprite GetWidestSprite(Sprite[] frames)
    {
        if (frames == null || frames.Length == 0)
            return null;

        Sprite widest = frames[0];
        float widestWidth = widest != null ? widest.bounds.size.x : 0f;

        for (int i = 1; i < frames.Length; i++)
        {
            Sprite frame = frames[i];
            if (frame == null)
                continue;

            float width = frame.bounds.size.x;
            if (width > widestWidth)
            {
                widest = frame;
                widestWidth = width;
            }
        }

        return widest;
    }
}
