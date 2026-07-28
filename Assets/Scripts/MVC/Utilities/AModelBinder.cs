using UnityEngine;

public abstract class AModelBinder<Model> : MonoBehaviour where Model : AModel, new()
{
    [Header("Model")]
    [SerializeField] protected string _modelName;

    protected Model _model;

    protected virtual void Awake()
    {
        BindModel(_modelName);
    }

    public virtual void BindModel(string id)
    {
        if (GlobalModelLocator.Instance == null) return;

        _model = GlobalModelLocator.Instance.GetModel<Model>(id);
    }
}
