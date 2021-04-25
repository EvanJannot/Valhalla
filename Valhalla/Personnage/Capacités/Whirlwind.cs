using System.Collections.Generic;
using System.Text;
using RogueSharp;
using Valhalla.Core;

namespace Valhalla.Abilities
{
    //Attaque circulaire autour du joueur 
   public class Whirlwind : Ability
   {
        StringBuilder espace = new StringBuilder();
      public Whirlwind() //On définit le nom et le temps de rechargement 
      {
         Name = "Tourbillon";
         TurnsToRefresh = 20;
         TurnsUntilRefreshed = 0;
      }

      protected override bool PerformAbility() //Fonction pour exécuter le sort 
      {
         DungeonMap map = Game.DungeonMap;
         Player player = Game.Player;

         Game.MessageLog.Add( $"{player.Name} utile une attaque tourbillon sur les ennemis adjacents" ); //On écrit le message de confirmation
            Game.MessageLog.Add(espace.ToString());

            List<Point> monsterLocations = new List<Point>(); //Liste contenant les emplacements des monstres autour du joueur 

         foreach ( Cell cell in map.GetCellsInArea( player.X, player.Y, 1 ) ) //Pour chaque case autour du joueur 
         {
            foreach ( Point monsterLocation in map.GetMonsterLocations() ) //Pour chaque emplacement de monstre sur la carte 
            {
               if ( cell.X == monsterLocation.X && cell.Y == monsterLocation.Y ) //Si le monstre est située sur une case autour du joueur 
               {
                  monsterLocations.Add( monsterLocation ); //On ajoute son emplacement à la liste 
               }
            }
         }

         foreach ( Point monsterLocation in monsterLocations ) //Pour chaque emplacement de la liste 
         {
            Monster monster = map.GetMonsterAt( monsterLocation.X, monsterLocation.Y ); //On récupère le monstre 
            if ( monster != null ) //Si il s'agit bien d'un monstre 
            {
               Game.CommandSystem.Attack( player, monster ); //Le joueur attaque le monstre 
            }
         }

         return true;
      }
   }
}
