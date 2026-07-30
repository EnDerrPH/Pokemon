using System;
using UnityEngine;

public class HealthModel : AModel
{
    public float CurrentHp { get; private set; }
    public float MaxHp { get; private set; }

    public Action HealthUpdated;

    public void SetHealth(float currentHp, float maxHp)
    {
        MaxHp = Mathf.Max(0f, maxHp);
        CurrentHp = Mathf.Clamp(currentHp, 0f, MaxHp);
        HealthUpdated?.Invoke();
    }

    public void SetFromPokemon(PokemonData pokemonData)
    {
        if (pokemonData == null)
        {
            SetHealth(0f, 0f);
            return;
        }

        float hp = pokemonData.Hp;
        SetHealth(hp, hp);
    }

    public void ApplyDamage(float damage)
    {
        if (damage <= 0f)
            return;

        SetHealth(CurrentHp - damage, MaxHp);
    }

    public void Clear()
    {
        SetHealth(0f, 0f);
    }
}

