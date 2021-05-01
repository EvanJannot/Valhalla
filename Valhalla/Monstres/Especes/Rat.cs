using RLNET;
using RogueSharp.DiceNotation;
using Valhalla.Behaviors;
using Valhalla.Core;
using Valhalla.Systems;


namespace Valhalla.Monsters
{
    //Classe de héritant de la classe monstre et qui définit les caractéristiques du rat qui va chercher de l'aide, se soigne et fuit 
    public class Rat : Monster
    {
        private int? _turnsSpentRunning = null;
        private bool _shoutedForHelp = false;

        public static Rat Create(int level)
        {
            int health = Dice.Roll("3D5");
            return new Rat
            {
                Attack = Dice.Roll("3D2") + level / 3,
                AttackChance = Dice.Roll("15D5"),
                Awareness = 10,
                Color = RLColor.Gray,
                Defense = Dice.Roll("3D2") + level / 3,
                DefenseChance = Dice.Roll("15D4"),
                Gold = Dice.Roll("3D20"),
                Health = health,
                MaxHealth = health,
                Name = "Rat",
                Speed = 12,
                Symbol = (char)146,
                Xp = level * 30
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
