using RLNET;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Behaviors;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples.Monsters
{
    public class Abeille : Monster
    {
        private int? _turnsSpentRunning = null;
        private bool _shoutedForHelp = false;

        public static Abeille Create(int level)
        {
            int health = Dice.Roll("1D5");
            return new Abeille
            {
                Attack = Dice.Roll("1D2") + level,
                AttackChance = Dice.Roll("10D5"),
                Awareness = 10,
                Color = RLColor.Yellow,
                Defense = Dice.Roll("1D2") + level,
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("1D20") * level,
                Health = health,
                MaxHealth = health,
                Name = "Abeille",
                Speed = 12,
                Symbol = (char)130,
                Xp = level * 10
            };
        }

        public override void PerformAction(CommandSystem commandSystem)
        {
            var fullyHealBehavior = new FullyHeal();
            var runAwayBehavior = new RunAway();
            var shoutForHelpBehavior = new ShoutForHelp();

            if (_turnsSpentRunning.HasValue && _turnsSpentRunning.Value > 15)
            {
                fullyHealBehavior.Act(this, commandSystem);
                _turnsSpentRunning = null;
            }
            else if (Health < MaxHealth)
            {
                runAwayBehavior.Act(this, commandSystem);
                if (_turnsSpentRunning.HasValue)
                {
                    _turnsSpentRunning += 1;
                }
                else
                {
                    _turnsSpentRunning = 1;
                }

                if (!_shoutedForHelp)
                {
                    _shoutedForHelp = shoutForHelpBehavior.Act(this, commandSystem);
                }
            }
            else
            {
                base.PerformAction(commandSystem);
            }
        }
    }
}
