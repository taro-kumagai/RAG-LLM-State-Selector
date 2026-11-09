using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;


public class GameContext
{
    // List of Valid States for the Agent to transition to
    public List<State> ValidTransitionStates { get; set; } = new List<State>();

    // Agent the is the perspective of the given Game Context
    public CharacterContext? Agent { get; set; } = null;

    // List of Characters in the Game relevant to current situation
    public List<CharacterContext> Characters { get; private set; } = new List<CharacterContext>();
    public void AddCharacter(CharacterContext characterContext) { Characters.Add(characterContext); }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}
