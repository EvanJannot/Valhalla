using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Equipment;
using System.Text;

namespace RogueSharpRLNetSamples.Items
{
   public class Whetstone : Item
   {
        StringBuilder espace = new StringBuilder();
      public Whetstone()
      {
         Name = "Aiguisage";
         RemainingUses = 5;
      }

      protected override bool UseItem()
      {
         Player player = Game.Player;

         if ( player.Hand == HandEquipment.None() )
         {
            Game.MessageLog.Add( $"{player.Name} ne possede rien a aiguiser" );
                Game.MessageLog.Add(espace.ToString());
            }
         else if ( player.AttackChance >= 80 )
         {
            Game.MessageLog.Add( $"{player.Name} ne peut pas rendre sa {player.Hand.Name} plus coupante" );
                Game.MessageLog.Add(espace.ToString());
            }
         else
         {
            Game.MessageLog.Add( $"{player.Name} utilise une pierre à aiguiser pour rendre sa {player.Hand.Name} plus coupante" );
                Game.MessageLog.Add(espace.ToString());
                player.Hand.AttackChance += Dice.Roll( "1D3" );
            RemainingUses--;
         }

         return true;
      }
   }
}

