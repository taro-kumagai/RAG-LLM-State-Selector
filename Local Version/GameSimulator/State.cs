using System;
using System.IO;
using System.Collections.Generic;

public class State
{
    public string Name { get; set; }
    public string Description { get; set; }


    public State(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static List<State> BuildTestStateList()
    {

        // Building State List
        var AllStates = new List<State>();
        AllStates.Add(new State(
            "Melee_Attack",
            "Agent attacks an enemy Character within the Agent's melee range."
        ));
        AllStates.Add(new State(
            "Projectile_Attack",
            "Agent attacks an enemy Character within the Agent's projectile range."
        ));
        AllStates.Add(new State(
            "Defend_Self",
            "Agent takes a defensive stance to reduce damage or block incoming attacks."
        ));
        AllStates.Add(new State(
            "Evade",
            "Agent attempts to dodge or avoid an incoming attack."
        ));
        AllStates.Add(new State(
            "Approach",
            "Agent moves closer to a target Character."
        ));
        AllStates.Add(new State(
            "Retreat",
            "Agent moves away from a threat while remaining engaged in combat."
        ));
        AllStates.Add(new State(
            "Return",
            "Agent returns to its original position or designated location."
        ));
        AllStates.Add(new State(
            "Heal",
            "Agent restores HP to itself or another Character using a healing ability."
        ));
        AllStates.Add(new State(
            "Buff_Ally",
            "Agent applies a beneficial effect to an allied Character."
        ));
        AllStates.Add(new State(
            "Debuff_Enemy",
            "Agent applies a harmful effect or status condition to an enemy Character."
        ));
        AllStates.Add(new State(
            "Guard_Ally",
            "Agent protects an allied Character from incoming threats."
        ));
        AllStates.Add(new State(
            "Intercept",
            "Agent moves to intercept an enemy approaching or attacking another Character."
        ));
        AllStates.Add(new State(
            "Patrol",
            "Agent moves along a designated patrol route."
        ));
        AllStates.Add(new State(
            "Chase",
            "Agent pursues a moving or fleeing target Character."
        ));
        AllStates.Add(new State(
            "Take_Cover",
            "Agent moves behind nearby cover to reduce exposure to enemy attacks."
        ));
        AllStates.Add(new State(
            "Hold_Position",
            "Agent maintains its current position and does not advance or retreat."
        ));
        AllStates.Add(new State(
            "Reposition",
            "Agent moves to a more advantageous tactical position."
        ));
        AllStates.Add(new State(
            "Use_Item",
            "Agent uses an available item from its inventory."
        ));
        AllStates.Add(new State(
            "Wait",
            "Agent takes no immediate action and remains in its current state."
        ));
        AllStates.Add(new State(
            "Flee",
            "Agent attempts to escape combat and disengage from nearby threats."
        ));

        return AllStates;
    }

}
