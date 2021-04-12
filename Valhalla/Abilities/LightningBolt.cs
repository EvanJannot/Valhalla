using RogueSharp;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Interfaces;
using System.Text;

namespace RogueSharpRLNetSamples.Abilities
{
   public class LightningBolt : Ability, ITargetable
   {
        StringBuilder espace = new StringBuilder();
      private readonly int _attack;
      private readonly int _attackChance;

      public LightningBolt( int attack, int attackChance )
      {
         Name = "Eclair";
         TurnsToRefresh = 40;
         TurnsUntilRefreshed = 0;
         _attack = attack;
         _attackChance = attackChance;
      }

      protected override bool PerformAbility()
      {
         return Game.TargetingSystem.SelectLine( this );
      }

      public void SelectTarget( Point target )
      {
         DungeonMap map = Game.DungeonMap;
         Player player = Game.Player;
         Game.MessageLog.Add( $"{player.Name} lance un {Name}" );
            Game.MessageLog.Add(espace.ToString());

            Actor lightningBoltActor = new Actor {
            Attack = _attack,
            AttackChance = _attackChance,
            Name = Name
         };
         foreach ( Cell cell in map.GetCellsAlongLine( player.X, player.Y, target.X, target.Y ) )
         {
            if ( cell.IsWalkable )
            {
               continue;
            }

            if ( cell.X == player.X && cell.Y == player.Y )
            {
               continue;
            }

            Monster monster = map.GetMonsterAt( cell.X, cell.Y );
            if ( monster != null )
            {
               Game.CommandSystem.Attack( lightningBoltActor, monster );
            }
            else
            {
               // We hit a wall so stop the bolt
               // TODO: consider having bolts and fireballs destroy walls and leave rubble
               return;
            }
         }
      }
   }
}
