using UnityEngine;

[CreateAssetMenu(menuName = "Pokemon/Move Data")]
public class MoveData : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private string _moveName;
    [SerializeField] private int _power;
    [SerializeField] private int _pp;
    [SerializeField] private int _accuracy;
    [SerializeField] private int _priority;
    [SerializeField] private string _type;
    [SerializeField] private string _damageClass;

    public int Id => _id;
    public string MoveName => _moveName;
    public int Power => _power;
    public int Pp => _pp;
    public int Accuracy => _accuracy;
    public int Priority => _priority;
    public string Type => _type;
    public string DamageClass => _damageClass;

    public void Initialize(
        int id,
        string moveName,
        int power,
        int pp,
        int accuracy,
        int priority,
        string type,
        string damageClass)
    {
        _id = id;
        _moveName = moveName;
        _power = power;
        _pp = pp;
        _accuracy = accuracy;
        _priority = priority;
        _type = type;
        _damageClass = damageClass;
    }
}
