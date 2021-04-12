using RogueSharpRLNetSamples.Abilities;
using RogueSharpRLNetSamples.Core;
using System.Text;

namespace RogueSharpRLNetSamples.Items
{
   public class HealingPotion : Item
   {
        StringBuilder espace = new StringBuilder();
      public HealingPotion()
      {
         Name = "Potion de Soin";
         RemainingUses = 1;
      }

      protected override bool UseItem()
      {
         int healAmount = 15;
         Game.MessageLog.Add( $"{Game.Player.Name} consomme une {Name} et recupere {healAmount} points de vie" );
            Game.MessageLog.Add(espace.ToString());

            Heal heal = new Heal( healAmount );

         RemainingUses--;

         return heal.Perform();
      }
   }
}
