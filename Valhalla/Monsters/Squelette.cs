using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Core;


namespace RogueSharpRLNetSamples.Monsters
{
    public class Squelette : Monster
    {
        public static Squelette Create(int level)
        {
            int health = Dice.Roll("2D5");
            return new Squelette
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
                Name = "Squelette",
                Speed = 14,
                Symbol = (char)46,
                Xp = level * 40
            };
        }
    }
}
