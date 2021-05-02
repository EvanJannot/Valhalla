using RogueSharp;
using Valhalla.Core;

namespace Valhalla.Abilities
{
    //Sort permettant de réveler la carte autour du joueur 
    public class Foresight : Ability
    {
        private readonly int _revealDistance;

        public Foresight(int revealDistance) //On définit le nom, le temps de rechargement ainsi que la distance à laquelle la map va être découverte 
        {
            Name = "Clairvoyance";
            TurnsToRefresh = 100;
            TurnsUntilRefreshed = 0;
            _revealDistance = revealDistance;
        }

        protected override bool PerformAbility() //On exécute le sort 
        {
            DungeonMap map = Game.DungeonMap;
            Player player = Game.Player;

            foreach (Cell cell in map.GetCellsInArea(player.X, player.Y, _revealDistance)) //Pour toutes les cases dans la zone à révéler 
            {
                if (cell.IsWalkable) //Si on peut marcher dessus 
                {
                    map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true); //On la découvre 
                }
            }

            return true;
        }
    }
}
