using RogueSharp;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Systems;
using System.Text;

namespace RogueSharpRLNetSamples.Behaviors
{
   public class ShoutForHelp : IBehavior
   {
        StringBuilder espace = new StringBuilder();
        public bool Act( Monster monster, CommandSystem commandSystem )
      {
         bool didShoutForHelp = false;
         DungeonMap dungeonMap = Game.DungeonMap;
         FieldOfView monsterFov = new FieldOfView( dungeonMap );

         monsterFov.ComputeFov( monster.X, monster.Y, monster.Awareness, true );
         foreach ( var monsterLocation in dungeonMap.GetMonsterLocations() )
         {
            if ( monsterFov.IsInFov( monsterLocation.X, monsterLocation.Y ) )
            {
               Monster alertedMonster = dungeonMap.GetMonsterAt( monsterLocation.X, monsterLocation.Y );
               if ( !alertedMonster.TurnsAlerted.HasValue )
               {
                  alertedMonster.TurnsAlerted = 1;
                  didShoutForHelp = true;
               }
            }
         }

         if ( didShoutForHelp )
         {
            Game.MessageLog.Add( $"{monster.Name} va chercher de l'aide" );
                Game.MessageLog.Add(espace.ToString());
            }

         return didShoutForHelp;
      }
   }
}
