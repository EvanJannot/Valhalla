using RogueSharp;
using Valhalla.Core;
using System.Text;

namespace Valhalla.Items
{
    //Classe définissant la carte magique 
    public class RevealMapScroll : Item
    {
        StringBuilder espace = new StringBuilder();
        public RevealMapScroll() //On définit le nom et le nombre d'utilisations
        {
            Name = "Carte magique";
            RemainingUses = 1;
            Symbol = (char)160;
        }

        protected override bool UseItem() //Fonction d'utilisation de l'objet 
        {
            DungeonMap map = Game.DungeonMap;

            Game.MessageLog.Add($"{Game.Player.Name} lit la {Name} et acquiert des connaissances sur la zone"); //On affiche le message de confirmation
            Game.MessageLog.Add(espace.ToString());

            foreach (Cell cell in map.GetAllCells()) //On parcourt l'ensemble des cases de la map
            {
                if (cell.IsWalkable) //Si la case est accessible 
                {
                    map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true); //On la découvre 
                }
            }

            RemainingUses--; //On diminue le nombre d'utilisations 

            return true;
        }
    }
}
