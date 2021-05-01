using RogueSharp.DiceNotation;
using Valhalla.Core;
using Valhalla.Equipment;
using System.Text;

namespace Valhalla.Items
{
    //Classe définissant la pire à aiguiser 
    public class Whetstone : Item
    {
        StringBuilder espace = new StringBuilder();
        public Whetstone() //Définition du nom et du nombre d'utilisations 
        {
            Name = "Aiguisage";
            RemainingUses = 1;
            Symbol = (char)165;
        }

        //Fonction d'utilisation de l'objet 
        protected override bool UseItem()
        {
            Player player = Game.Player;

            if (player.Hand == HandEquipment.None()) //Si le joueur n'a pas d'arme on ne fait rien 
            {
                Game.MessageLog.Add($"{player.Name} ne possede rien a aiguiser"); //Affichage d'un message d'erreur 
                Game.MessageLog.Add(espace.ToString());
            }
            else if (player.AttackChance >= 80) //Si le joueur a une attaque trop élevée on ne fait rien
            {
                Game.MessageLog.Add($"{player.Name} ne peut pas rendre sa {player.Hand.Name} plus coupante"); //Affichage d'un message d'erreur 
                Game.MessageLog.Add(espace.ToString());
            }
            else //Sinon on augmente les chances d'attaque du joueur 
            {
                Game.MessageLog.Add($"{player.Name} utilise une pierre à aiguiser pour rendre sa {player.Hand.Name} plus coupante"); //Message de confirmation
                Game.MessageLog.Add(espace.ToString());
                player.Hand.AttackChance += Dice.Roll("1D3"); //On augmente les chances entre 1 et 3%
                RemainingUses--; //On diminue le nombre d'utilisations restantes 
            }

            return true;
        }
    }
}

