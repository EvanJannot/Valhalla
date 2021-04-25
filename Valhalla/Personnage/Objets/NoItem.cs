using Valhalla.Core;

namespace Valhalla.Items
{
    //Classe définissant l'objet "vide" lorsque le joueur n'a pas d'objet
   public class NoItem : Item
   {
      public NoItem() //On définit le nom et le nombre d'utilisations
      {
         Name = "Vide";
         RemainingUses = 1;
      }

      protected override bool UseItem() //Lorsqu'il veut utiliser cet objet il ne se passe rien
      {
         return false;
      }
   }
}
