using RogueSharp;
using Valhalla.Core;
using Valhalla.Interfaces;
using System.Text;

namespace Valhalla.Abilities
{
    //Sort de boule de feu représenté par une zone d'explosion choisie par le joueur autour de lui et ayant une chance de blesser les ennemis 
    public class Fireball : Ability, ITargetable
    {
        StringBuilder espace = new StringBuilder();
        private readonly int _attack;
        private readonly int _attackChance;
        private readonly int _area;

        public Fireball(int attack, int attackChance, int area) //On définit le nom, les dégats, les chances de toucher, la zone d'explosion ainsi que le temps de rechargement
        {
            Name = "Boule de feu";
            TurnsToRefresh = 40;
            TurnsUntilRefreshed = 0;
            _attack = attack;
            _attackChance = attackChance;
            _area = area;
        }

        protected override bool PerformAbility() //Lorsque le joueur execute l'action on appelle la fonction permettant de sélectionner une zone 
        {
            return Game.TargetingSystem.SelectArea(this, _area);
        }

        //Lorsque le joueur est en train de sélectionner la zone et qu'il appuie sur entrée on appelle cette fonction qui déclence l'attaque 
        public void SelectTarget(Point target)
        {
            DungeonMap map = Game.DungeonMap;
            Player player = Game.Player;
            Game.MessageLog.Add($"{player.Name} lance une {Name}"); //On affiche un message comme quoi l'attaque est lancée 
            Game.MessageLog.Add(espace.ToString());
            Actor fireballActor = new Actor
            { //On définit un acteur boule de feu qui va attaquer les monstres dans la zone 
                Attack = _attack,
                AttackChance = _attackChance,
                Name = Name
            };
            foreach (Cell cell in map.GetCellsInArea(target.X, target.Y, _area)) //On parcourt les cases de la zone et si il y a des monstres on les attaque avec la boule de feu 
            {
                Monster monster = map.GetMonsterAt(cell.X, cell.Y);
                if (monster != null)
                {
                    Game.CommandSystem.Attack(fireballActor, monster);
                }
            }
        }
    }
}
