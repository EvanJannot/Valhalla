using Valhalla.Core;
using System.Text;

namespace Valhalla.Abilities
{
    //Capacité présente lorsqu'aucune autre n'a été ramassée 
   public class DoNothing : Ability
   {
        StringBuilder espace = new StringBuilder();
      public DoNothing() //La capacité ne se recharge pas et il est inscrit que l'emplacement est vide
      {
         Name = "Vide";
         TurnsToRefresh = 0;
         TurnsUntilRefreshed = 0;
      }

      protected override bool PerformAbility() //Lorsque le joueur souhaite utiliser cette capacité on renvoie un message d'erreur et on renvoie false pour dire que rien n'a été fait 
      {
         Game.MessageLog.Add( "Aucune capacite a cet emplacement" );
            Game.MessageLog.Add(espace.ToString());
            return false;
      }
   }
}
