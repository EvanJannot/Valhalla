using RLNET;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Behaviors;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples.Monsters
{
    public class Golem : Monster
    {
        public static Golem Create(int level)
        {
            int health = 15;
            return new Golem
            {
                Attack = 10,
                AttackChance = 40,
                Awareness = 10,
                Color = RLColor.Brown,
                Defense = 2,
                DefenseChance = 70,
                Gold = 100,
                Health = health,
                MaxHealth = health,
                Name = "Grogolem",
                Speed = 20,
                Symbol = (char)7,
                Xp = level * 300
            };
        }
    }
}
