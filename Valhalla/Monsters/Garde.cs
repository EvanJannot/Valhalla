using RLNET;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Behaviors;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples.Monsters
{
    public class Garde : Monster
    {
        public static Garde Create(int level)
        {
            int health = Dice.Roll("2D5");
            return new Garde
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 10,
                Color = Colors.Player,
                Defense = Dice.Roll("1D3") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("5D5"),
                Health = health,
                MaxHealth = health,
                Name = "Garde",
                Speed = 14,
                Symbol = (char)145,
                Xp = level * 60
            };
        }
    }
}
