using System.IO;
using System.Collections.Generic;
using System.Text.Json;

public class Test
{
    public static void Main()
    {
        var GameContexts = new List<GameContext>();

        //--------------------------------
        // Setting up Valid State Transitions
        //--------------------------------
        var AllValidStates = State.BuildTestStateList();

        Character agent, allyA, allyB, enemyA, enemyB;

        //--------------------------------
        // Context 1
        //--------------------------------

        var GameContext1 = new GameContext();
        GameContext1.ValidTransitionStates = AllValidStates;

        // Agent
        agent = new Character("Agent")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Wait"),
            HP = 20,
            MAXHP = 20,
            MeleeAttackRange = 3
        };
        GameContext1.Agent = agent.ConvertToContext();

        // Low Health Enemy
        enemyA = new Character("Enemy A")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Melee_Attack"),
            HP = 1,
            MAXHP = 20,
            Distance = 1,
            MeleeAttackRange = 2,
            Target = agent
        };
        GameContext1.Characters.Add(enemyA.ConvertToContext());

        // High Health Enemy
        enemyB = new Character("Enemy B")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Melee_Attack"),
            HP = 20,
            MAXHP = 20,
            Distance = 1,
            MeleeAttackRange = 2,
            Target = agent
        };
        GameContext1.Characters.Add(enemyB.ConvertToContext());

        GameContexts.Add(GameContext1);

        //--------------------------------
        // Context 2
        //--------------------------------

        var GameContext2 = new GameContext();
        GameContext2.ValidTransitionStates = AllValidStates;

        // Agent
        agent = new Character("Agent")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Wait"),
            HP = 20,
            MAXHP = 20,
            MeleeAttackRange = 3
        };
        GameContext2.Agent = agent.ConvertToContext();

        // High Health Enemy
        enemyA = new Character("Enemy A")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Melee_Attack"),
            HP = 20,
            MAXHP = 20,
            Distance = 1,
            MeleeAttackRange = 2,
            Target = agent
        };
        GameContext2.Characters.Add(enemyA.ConvertToContext());

        // Low Health Enemy
        enemyB = new Character("Enemy B")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Melee_Attack"),
            HP = 1,
            MAXHP = 20,
            Distance = 1,
            MeleeAttackRange = 2,
            Target = agent
        };
        GameContext2.Characters.Add(enemyB.ConvertToContext());

        GameContexts.Add(GameContext2);

        //--------------------------------
        // Context 3
        //--------------------------------

        var GameContext3 = new GameContext();
        GameContext3.ValidTransitionStates = AllValidStates;

        // Agent
        agent = new Character("Agent")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Wait"),
            HP = 20,
            MAXHP = 20,
            MeleeAttackRange = 2
        };
        GameContext3.Agent = agent.ConvertToContext();

        // Low Health Ally
        allyA = new Character("Ally A")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Flee"),
            HP = 1,
            MAXHP = 20,
            Distance = 4
        };
        GameContext3.Characters.Add(allyA.ConvertToContext());

        // Enemy Attacking Ally
        enemyA = new Character("Enemy A")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Chase"),
            HP = 20,
            MAXHP = 20,
            Distance = 6,
            MeleeAttackRange = 2,
            Target = allyA
        };
        GameContext3.Characters.Add(enemyA.ConvertToContext());

        GameContexts.Add(GameContext3);

        //--------------------------------
        // Context 4
        //--------------------------------

        var GameContext4 = new GameContext();
        GameContext4.ValidTransitionStates = AllValidStates;

        // Agent
        agent = new Character("Agent")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Wait"),
            HP = 20,
            MAXHP = 20,
            MeleeAttackRange = 2
        };
        GameContext4.Agent = agent.ConvertToContext();

        // Low Health Ally
        allyA = new Character("Ally A")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Wait"),
            HP = 1,
            MAXHP = 20,
            Distance = 1
        };
        GameContext4.Characters.Add(allyA.ConvertToContext());

        GameContexts.Add(GameContext4);

        //--------------------------------
        // Context 5
        //--------------------------------

        var GameContext5 = new GameContext();
        GameContext5.ValidTransitionStates = AllValidStates;

        // Agent
        agent = new Character("Agent")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Wait"),
            HP = 20,
            MAXHP = 20,
            MeleeAttackRange = 2
        };
        GameContext5.Agent = agent.ConvertToContext();

        // Distant Enemy
        enemyA = new Character("Enemy A")
        {
            CurrentState = AllValidStates.Find(s => s.Name == "Wait"),
            HP = 1,
            MAXHP = 20,
            Distance = 20
        };
        GameContext5.Characters.Add(enemyA.ConvertToContext());

        GameContexts.Add(GameContext5);

        //--------------------------------
        // Building All Context to JSON
        //--------------------------------

        int i = 1;
        foreach (var gameContext in GameContexts)
        {
            string json = gameContext.ToJson();
            File.WriteAllText("game_context_" + i + ".json", json);
            i++;
        }
    }
}
