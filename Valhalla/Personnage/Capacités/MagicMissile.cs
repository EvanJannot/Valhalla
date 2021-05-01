using RogueSharp;
using Valhalla.Core;
using Valhalla.Interfaces;
using System.Text;

namespace Valhalla.Abilities
{
    //Sort de missile autoguidé 
    public class MagicMissile : Ability, ITargetable
    {
        StringBuilder espace = new StringBuilder();
        private readonly int _attack;
        private readonly int _attackChance;

        public MagicMissile(int attack, int attackChance) //On définit le nom, le temps de rechargement, les dégats et les chances de toucher 
        {
            Name = "Missile Magique";
            TurnsToRefresh = 10;
            TurnsUntilRefreshed = 0;
            _attack = attack;
            _attackChance = attackChance;
        }

        protected override bool PerformAbility() //On exécute le sort ici on ne sélectionne pas de zone ou de ligne de points mais directement un ennemi
        {
            return Game.TargetingSystem.SelectMonster(this);
        }

        public void SelectTarget(Point target) //Lorsque le joueur a choisi sur quel ennemi tirer et qu'il a appuyé sur entrée on lance l'attaque 
        {
            DungeonMap map = Game.DungeonMap;
            Player player = Game.Player;
            Monster monster = map.GetMonsterAt(target.X, target.Y); //On récupère le monstre 
            if (monster != null) //Si il y a bien un monstre
            {
                Game.MessageLog.Add($"{player.Name} lance un {Name} sur {monster.Name}"); //On affiche le message de confirmation
                Game.MessageLog.Add(espace.ToString());
                Actor magicMissleActor = new Actor //On définit un nouvel acteur qui est le missile 
                {
                    Attack = _attack,
                    AttackChance = _attackChance,
                    Name = Name
                };
                Game.CommandSystem.Attack(magicMissleActor, monster); //On attaque le monstre avec le missile 
            }
        }
    }
}
