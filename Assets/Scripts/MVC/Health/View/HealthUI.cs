using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class HealthUI : AView<HealthModel>
{
    [Header("Health")]
    [SerializeField] private float _currentHP;
    [SerializeField] private float _maxHP;
    [SerializeField] private Image _healthBar;
    [SerializeField] private TextMeshProUGUI _hpPercentText;

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

        float ratio = _maxHP > 0f ? _currentHP / _maxHP : 0f;

        if (_healthBar != null)
            _healthBar.fillAmount = ratio;

        if (_hpPercentText != null)
        {
            int percent = Mathf.RoundToInt(ratio * 100f);
            _hpPercentText.SetText($"{percent}%");
        }

        OnHealthUpdated();
    }

    protected virtual void OnHealthUpdated()
    {
    }
}
