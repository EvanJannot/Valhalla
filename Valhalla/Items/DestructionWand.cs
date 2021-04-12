using RogueSharp;
using RogueSharp.Random;
using RogueSharpRLNetSamples.Core;
using System.Text;

namespace RogueSharpRLNetSamples.Items
{
   public class DestructionWand : Item
   {
        StringBuilder espace = new StringBuilder();
      public DestructionWand()
      {
         Name = "Baguette de destruction";
         RemainingUses = 3;
      }

      protected override bool UseItem()
      {
         DungeonMap map = Game.DungeonMap;
         Player player = Game.Player;
         Point edgePoint = GetRandomEdgePoint( map );

         Game.MessageLog.Add( $"{player.Name} utilise la {Name} et libère un rayon de vide chaotique" );
            Game.MessageLog.Add(espace.ToString());
            Actor voidAttackActor = new Actor {
            Attack = 6,
            AttackChance = 90,
            Name = "The Void"
         };
         Cell previousCell = null;
         foreach ( Cell cell in map.GetCellsAlongLine( player.X, player.Y, edgePoint.X, edgePoint.Y ) )
         {
            if ( cell.X == player.X && cell.Y == player.Y )
            {
               continue;
            }

            Monster monster = map.GetMonsterAt( cell.X, cell.Y );
            if ( monster != null )
            {
               Game.CommandSystem.Attack( voidAttackActor, monster );
            }
            else
            {
               map.SetCellProperties( cell.X, cell.Y, true, true, true );
               if ( previousCell != null )
               {
                  if ( cell.X != previousCell.X || cell.Y != previousCell.Y )
                  {
                     map.SetCellProperties( cell.X + 1, cell.Y, true, true, true );
                  }
               }
               previousCell = cell;
            }
         }

         RemainingUses--;

         return true;
      }

      private Point GetRandomEdgePoint( DungeonMap map )
      {
         var random = new DotNetRandom();
         int result = random.Next( 1, 4 );
         switch ( result )
         {
            case 1: // TOP
            {
               return new Point( random.Next( 3, map.Width - 3 ), 3 );
            }
            case 2: // BOTTOM
            {
               return new Point( random.Next( 3, map.Width - 3 ), map.Height - 3 );
            }
            case 3: // RIGHT
            {
               return new Point( map.Width - 3, random.Next( 3, map.Height - 3 ) );
            }
            case 4: // LEFT
            {
               return new Point( 3, random.Next( 3, map.Height - 3 ) );
            }
            default:
            {
               return new Point( 3, 3 );
            }
         }
      }
   }
}

