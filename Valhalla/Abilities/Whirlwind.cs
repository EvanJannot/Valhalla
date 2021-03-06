using System.Collections.Generic;
using System.Text;
using RogueSharp;
using RogueSharpRLNetSamples.Core;

namespace RogueSharpRLNetSamples.Abilities
{
   public class Whirlwind : Ability
   {
        StringBuilder espace = new StringBuilder();
      public Whirlwind()
      {
         Name = "Tourbillon";
         TurnsToRefresh = 20;
         TurnsUntilRefreshed = 0;
      }

      protected override bool PerformAbility()
      {
         DungeonMap map = Game.DungeonMap;
         Player player = Game.Player;

         Game.MessageLog.Add( $"{player.Name} utile une attaque tourbillon sur les ennemis adjacents" );
            Game.MessageLog.Add(espace.ToString());

            List<Point> monsterLocations = new List<Point>();

         foreach ( Cell cell in map.GetCellsInArea( player.X, player.Y, 1 ) )
         {
            foreach ( Point monsterLocation in map.GetMonsterLocations() )
            {
               if ( cell.X == monsterLocation.X && cell.Y == monsterLocation.Y )
               {
                  monsterLocations.Add( monsterLocation );
               }
            }
         }

         foreach ( Point monsterLocation in monsterLocations )
         {
            Monster monster = map.GetMonsterAt( monsterLocation.X, monsterLocation.Y );
            if ( monster != null )
            {
               Game.CommandSystem.Attack( player, monster );
            }
         }

         return true;
      }
   }
}
