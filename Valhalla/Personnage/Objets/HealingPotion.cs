using Valhalla.Abilities;
using Valhalla.Core;
using System.Text;

namespace Valhalla.Items
{
    //Classe définissant la potion de soin
    public class HealingPotion : Item
    {
        StringBuilder espace = new StringBuilder();
        public HealingPotion() //On définit le nom et le nombre d'usages 
        {
            Name = "Potion de Soin";
            RemainingUses = 1;
        }

        protected override bool UseItem() //Fonction lorsque le joueur utilise l'objet 
        {
            int healAmount = 15; //On définit la quantité de soin
            Game.MessageLog.Add($"{Game.Player.Name} consomme une {Name} et recupere {healAmount} points de vie"); //On affiche un message de confirmation
            Game.MessageLog.Add(espace.ToString());

            Heal heal = new Heal(healAmount); //On soigne le joueur en utilisant le sort de soin mais avec la quantité de la potion de soin pour aller plus vite

            RemainingUses--; //On diminue le nombre d'utilisations

            return heal.Perform(); //On effectue le sort de soin qui retourne true à la fin
        }
    }
}
