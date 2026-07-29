using System;

[Serializable]
public class MoveDTO
{
    public int id;
    public string name;
    public int power;
    public int pp;
    public int accuracy;
    public int priority;
    public NamedResourceDTO type;
    public NamedResourceDTO damage_class;
}
