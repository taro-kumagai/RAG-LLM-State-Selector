using System;
using System.IO;
using System.Collections.Generic;

public class CharacterContext
{
    public string Name { get; private set; } = "INVALID_CHARACTER";
    public List<string> Context { get; private set; } = new List<string>();

    public CharacterContext(string name)
    {
        Name = name;
    }
}

public class Character
{
    public string Name { get; private set; } = "INVALID_CHARACTER";
    public State? CurrentState;
    public float HP;
    public float MAXHP;
    public float MeleeAttackRange;
    public float ProjectileAttackRange;
    public float Distance;
    public Character? Target = null;

    public Character(string name)
    {
        Name = name;
    }

    public CharacterContext ConvertToContext()
    {
        var context = new CharacterContext(Name);

        // State
        if (CurrentState != null)
        context.Context.Add("State: " + CurrentState.Name);

        // HP
        if (MAXHP > 0)
            context.Context.Add("HP: " + HP.ToString() + "/" + MAXHP.ToString() + " (" +(HP / MAXHP * 100f).ToString("F1") + "%)");
        else
            context.Context.Add("HP: INVALID HP");

        // MeleeAttackRange
        context.Context.Add("Melee Attack Range: " + MeleeAttackRange.ToString());

        //ProjectileAttackRange
        context.Context.Add("Projectile Attack Range: " + ProjectileAttackRange.ToString());


        // Distance
        context.Context.Add("Distance from Agent: " + Distance.ToString() + "m");

        // Target
        if (Target != null)
            context.Context.Add("Targeting: " + Target.Name);

        return context;
    }
}
