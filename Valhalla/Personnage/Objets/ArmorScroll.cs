using Valhalla.Core;
using Valhalla.Equipment;
using System.Text;

namespace Valhalla.Items
{
    //Classe définissant le parchemin d'amélioration de l'armure 
   public class ArmorScroll : Item
   {
        StringBuilder espace = new StringBuilder();
        public ArmorScroll() //On définit le nom et le nombre d'utilisations
      {
         Name = "Amrure++";
         RemainingUses = 1;
      }

        //Fonction lorsque le joueur utilise l'objet
      protected override bool UseItem()
      {
         Player player = Game.Player;

         if ( player.Body == BodyEquipment.None() ) //Si le joueur n'a pas de plastron
         {
            Game.MessageLog.Add( $"{player.Name} ne porte pas d'armrue a enchanter" ); //On renvoie un message d'erreur 
                Game.MessageLog.Add(espace.ToString());
            }
         else if ( player.Defense >= 8 ) //Si il a trop de défense 
         {
            Game.MessageLog.Add( $"{player.Name} ne peut pas plus enchanter son {player.Body.Name}" ); //On renvoie un message d'erreur 
                Game.MessageLog.Add(espace.ToString());
            }
         else //Sinon
         {
            Game.MessageLog.Add( $"{player.Name} utilise un {Name} pour enchanter son {player.Body.Name}" ); //On confirme 
                Game.MessageLog.Add(espace.ToString());
                player.Body.Defense += 1; //On ajoute 1 à la défense 
            RemainingUses--; //On enlève une utilisation
         }

         return true;
      }
   }
}

