using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
#if UNITY_EDITOR
        Selection.activeObject = null;
#endif
        SceneManager.LoadScene(_gameSceneName);
    }
}
