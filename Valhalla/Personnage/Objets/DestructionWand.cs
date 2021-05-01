using RogueSharp;
using RogueSharp.Random;
using Valhalla.Core;
using System.Text;

namespace Valhalla.Items
{
    //Classe définissant la baguette de destruction qui détruit tout suivant un tracé aléatoire
    public class DestructionWand : Item
    {
        StringBuilder espace = new StringBuilder();
        public DestructionWand() //On définit le nom et le nombre d'utilisations 
        {
            Name = "Destruction";
            RemainingUses = 3;
            Symbol = (char)161;
        }

        //Fonction d'utilisation de l'objet 
        protected override bool UseItem()
        {
            DungeonMap map = Game.DungeonMap;
            Player player = Game.Player;
            Point edgePoint = GetRandomEdgePoint(map); //On récupère l'un des bords de la map

            Game.MessageLog.Add($"{player.Name} utilise la {Name} et libere un rayon de vide chaotique"); //On affiche un message de confirmation
            Game.MessageLog.Add(espace.ToString());
            Actor voidAttackActor = new Actor
            { //On définit un acteur qui va attaquer les monstres présents sur le tracé 
                Attack = 6,
                AttackChance = 90,
                Name = "The Void"
            };
            Cell previousCell = null;
            foreach (Cell cell in map.GetCellsAlongLine(player.X, player.Y, edgePoint.X, edgePoint.Y)) //Pour chaque case le long du chemin
            {
                if (cell.X == player.X && cell.Y == player.Y) //Si c'est le joueur on continue 
                {
                    continue;
                }

                Monster monster = map.GetMonsterAt(cell.X, cell.Y); //Si il y a un monstre on l'attaque avec le rayon
                if (monster != null)
                {
                    Game.CommandSystem.Attack(voidAttackActor, monster);
                }
                else //Sinon on passe la case en accessible et explorée 
                {
                    map.SetCellProperties(cell.X, cell.Y, true, true, true);
                    if (previousCell != null) //Si ce n'est pas la permière case 
                    {
                        if (cell.X != previousCell.X || cell.Y != previousCell.Y) //Et que l'on est plus sur la case d'avant 
                        {
                            map.SetCellProperties(cell.X + 1, cell.Y, true, true, true); //On passe la case d'a côté en accessible afin de tracer un chemin et pas juste une diagonale 
                        }
                    }
                    previousCell = cell; //La case d'avant devient la case venant d'être traitée 
                }
            }

            RemainingUses--; //Une fois fini on diminue de 1 le nombre d'usages restants 

            return true;
        }

        //Fonction permettant de récupérer un point au bord de la map
        private Point GetRandomEdgePoint(DungeonMap map)
        {
            var random = new DotNetRandom();
            int result = random.Next(1, 4); //On récupère un chiffre aléatoire entre 1 et 4 compris 
            switch (result)
            {
                case 1: //Si c'est 1 on récupère un point au hasard au bord supérieur de la map
                    {
                        return new Point(random.Next(3, map.Width - 3), 3);
                    }
                case 2: //Si c'est 2 on récupère un point au hasard au bord inférieur de la map
                    {
                        return new Point(random.Next(3, map.Width - 3), map.Height - 3);
                    }
                case 3: //Si c'est 3 on récupère un point au hasard au bord droit de la map
                    {
                        return new Point(map.Width - 3, random.Next(3, map.Height - 3));
                    }
                case 4: //Si c'est 4 on récupère un point au hasard au bord gauche de la map
                    {
                        return new Point(3, random.Next(3, map.Height - 3));
                    }
                default: //On définit un point par défaut qui est le coin en supérieur gauche 
                    {
                        return new Point(3, 3);
                    }
            }
        }
    }
}

