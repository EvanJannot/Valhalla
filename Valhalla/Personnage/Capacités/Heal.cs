using System;
using Valhalla.Core;

namespace Valhalla.Abilities
{
    //Sort de soin
    public class Heal : Ability
    {
        private readonly int _amountToHeal;

        public Heal(int amountToHeal) //On définit la quantité de soin, le nom ainsi que le temps de rechargement du sort 
        {
            Name = "Soin";
            TurnsToRefresh = 60;
            TurnsUntilRefreshed = 0;
            _amountToHeal = amountToHeal;
        }

        //Fonction permettant d'exécuter le sort
        protected override bool PerformAbility()
        {
            Player player = Game.Player;

            //On soigne le joueur puis on retourne true 
            player.Health = Math.Min(player.MaxHealth, player.Health + _amountToHeal); //On prend le nombre minimal entre la vie du joueur + le soin et sa vie max pour ne pas dépasser la vie max 

            return true;
        }
    }
}
