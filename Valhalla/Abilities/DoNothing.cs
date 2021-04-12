using RogueSharpRLNetSamples.Core;
using System.Text;

namespace RogueSharpRLNetSamples.Abilities
{
   public class DoNothing : Ability
   {
        StringBuilder espace = new StringBuilder();
      public DoNothing()
      {
         Name = "Vide";
         TurnsToRefresh = 0;
         TurnsUntilRefreshed = 0;
      }

      protected override bool PerformAbility()
      {
         Game.MessageLog.Add( "Aucune capacite a cet emplacement" );
            Game.MessageLog.Add(espace.ToString());
            return false;
      }
   }
}
