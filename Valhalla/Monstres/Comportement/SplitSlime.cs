using RogueSharp;
using Valhalla.Core;
using Valhalla.Interfaces;
using Valhalla.Monsters;
using Valhalla.Systems;

namespace Valhalla.Behaviors
{
    //Classe d'un comportement utilisé par les slimes et les fantomes 
    public class SplitSlime : IBehavior
    {
        public bool Act(Monster monster, CommandSystem commandSystem) //Permet au monstre de se séparer en deux 
        {
            DungeonMap map = Game.DungeonMap;

            // Les slimes se séparent que lorsqu'ils sont blessés
            if (monster.Health >= monster.MaxHealth)
            {
                return false;
            }

            int halfHealth = monster.MaxHealth / 2; //La vie du nouveau monstre est divisée par deux 
            if (halfHealth <= 0)
            {
                // Si la vie est trop basse on ne le fait pas 
                return false;
            }

            Cell cell = FindClosestUnoccupiedCell(map, monster.X, monster.Y); //On cherche la case la plus proche pour placer le nouveau monstre 

            if (cell == null)
            {
                // Si il n'y a pas de case on ne le fait pas 
                return false;
            }

            // On fait un nouveau monstre en clonant le celui qui a été frappé 
            Slime newOoze = Monster.Clone(monster) as Slime;
            if (newOoze != null) //Si le monstre est bien cloné on défini ses nouvelles caractéristiques 
            {
                newOoze.TurnsAlerted = 1;
                newOoze.X = cell.X;
                newOoze.Y = cell.Y;
                newOoze.MaxHealth = halfHealth;
                newOoze.Health = halfHealth;
                map.AddMonster(newOoze);
            }
            else
            {
                // Si ce n'est pas le bon type de monstre on ne le fait pas 
                return false;
            }

            // On divise par deux la vie du slime touché 
            monster.MaxHealth = halfHealth;
            monster.Health = halfHealth;

            return true; //Le monstre a joué son tour 
        }

        private Cell FindClosestUnoccupiedCell(DungeonMap dungeonMap, int x, int y) //Fonction qui trouve la case la plus proche accessible 
        {
            for (int i = 1; i < 5; i++) //On cherche une case accessible dans un rayon de 4 cases en partant du plus proche vers le plus loin
            {
                foreach (Cell cell in dungeonMap.GetBorderCellsInArea(x, y, i))
                {
                    if (cell.IsWalkable)
                    {
                        return cell;
                    }
                }
            }

            return null;
        }
    }
}
