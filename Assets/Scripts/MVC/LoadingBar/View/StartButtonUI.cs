using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButtonUI : AView<LoadingBarModel>
{
    [Header("Button")]
    [SerializeField] private Button _button;

    [Header("Scene")]
    [SerializeField] private string _gameSceneName = "GameScene";

    private void OnEnable()
    {
        if (_button != null)
        {
            _button.interactable = false;
            _button.onClick.AddListener(HandleClicked);
        }

        if (_model == null) return;

        _model.LoadingComplete += HandleLoadingComplete;

        if (_model.TotalCount > 0 && _model.LoadedCount >= _model.TotalCount)
            HandleLoadingComplete();
    }

    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(HandleClicked);

        if (_model != null)
            _model.LoadingComplete -= HandleLoadingComplete;
    }

    private void HandleLoadingComplete()
    {
        if (_button != null)
            _button.interactable = true;
    }

    private void HandleClicked()
    {
        SceneManager.LoadScene(_gameSceneName);
    }
}
