using RLNET;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Behaviors;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples.Monsters
{
    class Ours : Monster
    {
        public static Ours Create(int level)
        {
            int health = 15;
            return new Ours
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
                Name = "Ours",
                Speed = 20,
                Symbol = (char)138,
                Xp = level * 150
            };
        }
    }
}
