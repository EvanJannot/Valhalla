using RogueSharp;
using Valhalla.Core;
using System.Text;

namespace Valhalla.Items
{
    //Classe définissant le parchemin de téléportation
    public class TeleportScroll : Item
    {
        StringBuilder espace = new StringBuilder();
        public TeleportScroll() //On définit le nom et le nombre d'utilisations 
        {
            Name = "Teleportation";
            RemainingUses = 1;
            Symbol = (char)166;
        }

        protected override bool UseItem() //Fonction d'utilisation de l'objet 
        {
            DungeonMap map = Game.DungeonMap;
            Player player = Game.Player;

            Game.MessageLog.Add($"{player.Name} utilise un {Name} et re apparait autre part"); //On confirme l'action par un message 
            Game.MessageLog.Add(espace.ToString());

            Point point = map.GetRandomLocation(); //On sélectionne un point aléatoire de la carte 

            map.SetActorPosition(player, point.X, point.Y); //On y place le joueur 

            RemainingUses--; // On diminue le nombre d'utilisations 

            return true;
        }
    }
}

