using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Equipment;
using System.Text;

namespace RogueSharpRLNetSamples.Items
{
   public class ArmorScroll : Item
   {
        StringBuilder espace = new StringBuilder();
        public ArmorScroll()
      {
         Name = "Amrure++";
         RemainingUses = 1;
      }

      protected override bool UseItem()
      {
         Player player = Game.Player;

         if ( player.Body == BodyEquipment.None() )
         {
            Game.MessageLog.Add( $"{player.Name} ne porte pas d'armrue a enchanter" );
                Game.MessageLog.Add(espace.ToString());
            }
         else if ( player.Defense >= 8 )
         {
            Game.MessageLog.Add( $"{player.Name} ne peut pas plus enchanter son {player.Body.Name}" );
                Game.MessageLog.Add(espace.ToString());
            }
         else
         {
            Game.MessageLog.Add( $"{player.Name} utilise un {Name} pour enchanter son {player.Body.Name}" );
                Game.MessageLog.Add(espace.ToString());
                player.Body.Defense += 1;
            RemainingUses--;
         }

         return true;
      }
   }
}

