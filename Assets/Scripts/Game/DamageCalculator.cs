using UnityEngine;

public static class DamageCalculator
{
    /// <summary>
    /// Scales base-stat damage so typical hits chip HP instead of OHKOing.
    /// Higher = less damage.
    /// </summary>
    private const int DamageDivisor = 15;

    public static int Calculate(PokemonData attacker, PokemonData defender, MoveData move)
    {
        if (attacker == null || defender == null || move == null)
            return 0;

        if (move.Power <= 0)
            return 0;

        if (IsStatusMove(move))
            return 0;

        int attack = Mathf.Max(1, attacker.Attack);
        int defense = Mathf.Max(1, defender.Defense);
        int power = move.Power;

        int damage = (power * attack) / (defense * DamageDivisor);
        return Mathf.Max(1, damage);
    }

    private static bool IsStatusMove(MoveData move)
    {
        if (string.IsNullOrEmpty(move.DamageClass))
            return false;

        return string.Equals(move.DamageClass, "status", System.StringComparison.OrdinalIgnoreCase);
    }
}
