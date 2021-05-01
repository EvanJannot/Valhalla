using System;
using RLNET;
using Valhalla.Behaviors;
using Valhalla.Monsters;
using Valhalla.Systems;

namespace Valhalla.Core
{
    //Les monstres sont des acteurs et il existe de nombreux monstres différents. Ainsi chaque monstre a sa propre classe qui hérite de celle-ci
    public class Monster : Actor
    {
        //Permet de déterminer pendant combien de tours un monstre a ciblé le joueur
        public int? TurnsAlerted { get; set; }

        //Permet de dessiner les statistiques d'un monstre dans le menu des statistiques (la vie totale et restante sous la forme d'une barre de vie, ainsi que le nom)
        public void DrawStats(RLConsole statConsole, int position)
        {
            int yPosition = 15 + (position * 2);
            statConsole.Print(1, yPosition, Symbol.ToString(), Color);
            int width = Convert.ToInt32(((double)Health / (double)MaxHealth) * 16.0);
            int remainingWidth = 16 - width; //La longeur de base est 16 puis elle descend lorsque la vie descend, en effet si la vie est au max, le calcul ci dessus vaut 16 puis tend vers 0 lorsque la vie diminue
            statConsole.SetBackColor(3, yPosition, width, 1, Swatch.Primary); //Permet de différencier à l'aide des couleurs, la vie restante et totale 
            statConsole.SetBackColor(3 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarkest);
            statConsole.Print(2, yPosition, $": {Name}", RLColor.White);
        }


        //Fonction permettant de cloner un monstre (utile pour les monstres qui se dédoublent comme les fantomes et les slimes 
        public static Monster Clone(Monster anotherMonster)
        {
            //On retourne simplement un nouveau monstre possédant les caractéristiques de celui dont il est issu (l'expérience n'est pas clonée pour éviter tout abus)
            return new Slime
            {
                Attack = anotherMonster.Attack,
                AttackChance = anotherMonster.AttackChance,
                Awareness = anotherMonster.Awareness,
                Color = anotherMonster.Color,
                Defense = anotherMonster.Defense,
                DefenseChance = anotherMonster.DefenseChance,
                Gold = anotherMonster.Gold,
                Health = anotherMonster.Health,
                MaxHealth = anotherMonster.MaxHealth,
                Name = anotherMonster.Name,
                Speed = anotherMonster.Speed,
                Symbol = anotherMonster.Symbol
            };
        }

        //Fonction qui va permettre aux différentes classes filles (les monstres) d'effectuer une action (chercher de l'aide, se dédoubler, etc)
        public virtual void PerformAction(CommandSystem commandSystem)
        {
            var behavior = new StandardMoveAndAttack();
            behavior.Act(this, commandSystem);
        }
    }
}