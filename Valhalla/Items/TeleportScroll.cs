using RogueSharp;
using RogueSharpRLNetSamples.Core;
using System.Text;

namespace RogueSharpRLNetSamples.Items
{
   public class TeleportScroll : Item
   {
        StringBuilder espace = new StringBuilder();
      public TeleportScroll()
      {
         Name = "Teleportation";
         RemainingUses = 1;
      }

      protected override bool UseItem()
      {
         DungeonMap map = Game.DungeonMap;
         Player player = Game.Player;

         Game.MessageLog.Add( $"{player.Name} utilise un {Name} et re apparait autre part" );
            Game.MessageLog.Add(espace.ToString());

            Point point = map.GetRandomLocation();
         
         map.SetActorPosition( player, point.X, point.Y );
         
         RemainingUses--;

         return true;
      }
   }
}

