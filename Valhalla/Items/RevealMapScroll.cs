using RogueSharp;
using RogueSharpRLNetSamples.Core;
using System.Text;

namespace RogueSharpRLNetSamples.Items
{
   public class RevealMapScroll : Item
   {
        StringBuilder espace = new StringBuilder();
      public RevealMapScroll()
      {
         Name = "Carte magique";
         RemainingUses = 1;
      }

      protected override bool UseItem()
      {
         DungeonMap map = Game.DungeonMap;

         Game.MessageLog.Add( $"{Game.Player.Name} lit la {Name} et acquiert des connaissances sur la zone" );
            Game.MessageLog.Add(espace.ToString());

            foreach ( Cell cell in map.GetAllCells() )
         {
            if ( cell.IsWalkable )
            {
               map.SetCellProperties( cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true );
            }
         }
         
         RemainingUses--;

         return true;
      }
   }
}
