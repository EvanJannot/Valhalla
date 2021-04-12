using RLNET;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Behaviors;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Systems;


namespace RogueSharpRLNetSamples.Monsters
{
    public class Fantome : Monster
    {
        public static Fantome Create(int level)
        {
            int health = Dice.Roll("4D5");
            return new Fantome
            {
                Attack = Dice.Roll("1D2") + level / 3,
                AttackChance = Dice.Roll("10D5"),
                Awareness = 10,
                Color = RLColor.White,
                Defense = Dice.Roll("1D2") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("1D20"),
                Health = health,
                MaxHealth = health,
                Name = "Fantome",
                Speed = 14,
                Symbol = (char)93,
                Xp = level * 15
            };
        }

        public override void PerformAction(CommandSystem commandSystem)
        {
            var splitSlimeBehavior = new SplitSlime();
            if (!splitSlimeBehavior.Act(this, commandSystem))
            {
                base.PerformAction(commandSystem);
            }
        }
    }
}
