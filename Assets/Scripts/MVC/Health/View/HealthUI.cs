using UnityEngine;
using UnityEngine.UI;

public abstract class HealthUI : AView<HealthModel>
{
    [Header("Health")]
    [SerializeField] private float _currentHP;
    [SerializeField] private float _maxHP;
    [SerializeField] private Image _healthBar;

    public float CurrentHP => _currentHP;
    public float MaxHP => _maxHP;

    private void OnEnable()
    {
        if (_model == null)
            BindModel(_modelName);

        if (_model == null)
            return;

        _model.HealthUpdated -= HandleHealthUpdated;
        _model.HealthUpdated += HandleHealthUpdated;
        HandleHealthUpdated();
    }

    private void OnDisable()
    {
        if (_model == null)
            return;

        _model.HealthUpdated -= HandleHealthUpdated;
    }

    private void HandleHealthUpdated()
    {
        if (_model == null)
            return;

        _currentHP = _model.CurrentHp;
        _maxHP = _model.MaxHp;

        if (_healthBar != null)
            _healthBar.fillAmount = _maxHP > 0f ? _currentHP / _maxHP : 0f;

        OnHealthUpdated();
    }

    protected virtual void OnHealthUpdated()
    {
    }
}
