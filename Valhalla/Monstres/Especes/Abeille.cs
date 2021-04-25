using RLNET;
using RogueSharp.DiceNotation;
using Valhalla.Behaviors;
using Valhalla.Core;
using Valhalla.Systems;

namespace Valhalla.Monsters
{
    //Classe de héritant de la classe monstre et qui définit les caractéristiques de l'abeille qui va chercher de l'aide, se soigne et fuit 
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

            if (_turnsSpentRunning.HasValue && _turnsSpentRunning.Value > 15) //Si le monstre a passé plus de 15 tours à fuir, il se soigne
            {
                fullyHealBehavior.Act(this, commandSystem);
                _turnsSpentRunning = null;
            }
            else if (Health < MaxHealth) //Si il a perdu de la vie
            {
                runAwayBehavior.Act(this, commandSystem); //Il fuit 
                if (_turnsSpentRunning.HasValue) //A chaque tour le nombre de tours à fuir augmente de 1
                {
                    _turnsSpentRunning += 1;
                }
                else
                {
                    _turnsSpentRunning = 1;
                }

                if (!_shoutedForHelp) //Si il n'a pas chercher d'aide il y va 
                {
                    _shoutedForHelp = shoutForHelpBehavior.Act(this, commandSystem);
                }
            }
            else //Sinon il frappe juste le joueur 
            {
                base.PerformAction(commandSystem);
            }
        }
    }
}
