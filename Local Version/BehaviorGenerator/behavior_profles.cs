using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

public class BehaviorProfile
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> ScenarioChunks { get; set; }

    public BehaviorProfile(string name, string description)
    {
        Name = name;
        Description = description;
        ScenarioChunks = new List<string>();
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}

public class Program
{
    public static void Main()
    {
        var behaviorProfiles = new List<BehaviorProfile>();

        // Building Aggressive_Melee Behavior Profile
        string aggressive_melee_description = "An aggressive agent that prefers to attack rather than defend itself or others. The agent is only capable in melee combat and will not perform any long range attacks";
        var Aggressive_Melee = new BehaviorProfile("Aggressive Melee", aggressive_melee_description);

        Aggressive_Melee.ScenarioChunks.Add("When enemy is low HP, the agent will prioritize that target.");
        Aggressive_Melee.ScenarioChunks.Add("Agent does not attack allies.");
        Aggressive_Melee.ScenarioChunks.Add("Agent tend to target the closest enemy");
        Aggressive_Melee.ScenarioChunks.Add("When there are no enemies, agent will return to starting location");
        Aggressive_Melee.ScenarioChunks.Add("If Enemies are out of range the agent will need to approach its target");

        behaviorProfiles.Add(Aggressive_Melee);

        // Building Doctor Behavior Profile
        string doctor_description = "An agent without any combat capabilities. Helps allies by healing and supporting them.";
        var Doctor = new BehaviorProfile("Doctor", doctor_description);

        Doctor.ScenarioChunks.Add("When an ally has low HP, the agent will prioritize healing that ally.");
        Doctor.ScenarioChunks.Add("When multiple allies are injured, the agent will prioritize the ally with the lowest HP percentage.");
        Doctor.ScenarioChunks.Add("Agent cannot attack enemies under any circumstances.");
        Doctor.ScenarioChunks.Add("When an enemy approaches, the agent will attempt to retreat to a safe location.");
        Doctor.ScenarioChunks.Add("When no allies require healing, the agent will remain near friendly units.");
        Doctor.ScenarioChunks.Add("If an injured ally is out of healing range, the agent will approach that ally.");
        Doctor.ScenarioChunks.Add("Agent will prioritize its own survival when threatened by nearby enemies.");

        behaviorProfiles.Add(Doctor);

        // Building Guardian Behavior Profile
        string guardian_description = "An agent with high defensive capabilties. Protects allies and prioritizes survival. Capable of attacking but will prioritize survival and protection over attacking enemies.";
        var Guardian = new BehaviorProfile("Guardian", guardian_description);

        Guardian.ScenarioChunks.Add("When an ally is threatened by an enemy, the agent will prioritize protecting that ally.");
        Guardian.ScenarioChunks.Add("When multiple allies are threatened, the agent will prioritize protecting the ally with the lowest HP.");
        Guardian.ScenarioChunks.Add("Agent will prioritize defensive actions over offensive actions.");
        Guardian.ScenarioChunks.Add("When an enemy approaches a vulnerable ally, the agent will attempt to intercept that enemy.");
        Guardian.ScenarioChunks.Add("Agent will attack enemies when doing so does not compromise the safety of its allies.");
        Guardian.ScenarioChunks.Add("When the agent has low HP, it will prioritize defensive actions and survival.");
        Guardian.ScenarioChunks.Add("When no allies are threatened, the agent will remain near friendly units.");
        Guardian.ScenarioChunks.Add("Agent will avoid pursuing distant enemies if doing so leaves allies unprotected.");

        behaviorProfiles.Add(Guardian);

        // Converting to json
        foreach (var profile in behaviorProfiles)
        {
            // Converting the profile list to JSON
            string json = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });

            // Saving JSON to a file
            File.WriteAllText("behavior_profile_" + profile.Name + ".json", json);
        }

    }
}
