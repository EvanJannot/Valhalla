using System.Text;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Equipment;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Items;

namespace RogueSharpRLNetSamples.Systems
{
   public class CommandSystem
   {
      public bool IsPlayerTurn { get; set; }

      public bool MovePlayer( Direction direction )
      {
         int x;
         int y;

         switch ( direction )
         {
            case Direction.Up:
            {
               x = Game.Player.X;
               y = Game.Player.Y - 1;
               break;
            }
            case Direction.Down:
            {
               x = Game.Player.X;
               y = Game.Player.Y + 1;
               break;
            }
            case Direction.Left:
            {
               x = Game.Player.X - 1;
               y = Game.Player.Y;
               break;
            }
            case Direction.Right:
            {
               x = Game.Player.X + 1;
               y = Game.Player.Y;
               break;
            }
            default:
            {
               return false;
            }
         }

         if ( Game.DungeonMap.SetActorPosition( Game.Player, x, y ) )
         {
            return true;
         }

         Monster monster = Game.DungeonMap.GetMonsterAt( x, y );
            //Faire classe pnj et si il y a un pnj, déclencher dialogue
         if ( monster != null )
         {
            Attack( Game.Player, monster );
            return true;
         }

         return false;
      }

      public void ActivateMonsters()
      {
         IScheduleable scheduleable = Game.SchedulingSystem.Get();
         if ( scheduleable is Player )
         {
            IsPlayerTurn = true;
            Game.SchedulingSystem.Add( Game.Player );
         }
         else
         {
            Monster monster = scheduleable as Monster;

            if ( monster != null )
            {
               monster.PerformAction( this );
               Game.SchedulingSystem.Add( monster );
            }

            ActivateMonsters();
         }
      }

      public void MoveMonster( Monster monster, Cell cell )
      {
         if ( !Game.DungeonMap.SetActorPosition( monster, cell.X, cell.Y ) )
         {
            if ( Game.Player.X == cell.X && Game.Player.Y == cell.Y )
            {
               Attack( monster, Game.Player );
            }
         }
      }

      public void Attack( Actor attacker, Actor defender )
      {

         int hits = ResolveAttack( attacker, defender);

         int blocks = ResolveDefense( defender, hits);

         int damage = hits - blocks;

         ResolveDamage( defender, damage );
      }

      private static int ResolveAttack( Actor attacker, Actor defender )
      {
         int hits = 0;
        
         DiceExpression attackDice = new DiceExpression().Dice( attacker.Attack, 100 );

         DiceResult attackResult = attackDice.Roll();
         foreach ( TermResult termResult in attackResult.Results )
         {
            if ( termResult.Value >= 100 - attacker.AttackChance )
            {
               hits++;
            }
         }

         return hits;
      }

      private static int ResolveDefense( Actor defender, int hits)
      {
         int blocks = 0;

         if ( hits > 0 )
         {
            DiceExpression defenseDice = new DiceExpression().Dice( defender.Defense, 100 );
            DiceResult defenseRoll = defenseDice.Roll();
            foreach ( TermResult termResult in defenseRoll.Results )
            {
               if ( termResult.Value >= 100 - defender.DefenseChance )
               {
                  blocks++;
               }
            }
         }

         return blocks;
      }

      private static void ResolveDamage( Actor defender, int damage )
      {
         if ( damage > 0 )
         {
            defender.Health = defender.Health - damage;

            if ( defender.Health <= 0 )
            {
               ResolveDeath( defender );
            }
         }
      }

      private static void ResolveDeath( Actor defender )
      {
            StringBuilder espace = new StringBuilder();
         if ( defender is Player )
         {
            Game.MessageLog.Add( $"{defender.Name} est mort, FIN DE LA PARTIE" );
            Game.EndGame();
         }
         else if ( defender is Monster )
         {
            if ( defender.Head != null && defender.Head != HeadEquipment.None() )
            {
               Game.DungeonMap.AddTreasure( defender.X, defender.Y, defender.Head );
            }
            if ( defender.Body != null && defender.Body != BodyEquipment.None() )
            {
               Game.DungeonMap.AddTreasure( defender.X, defender.Y, defender.Body );
            }
            if ( defender.Hand != null && defender.Hand != HandEquipment.None() )
            {
               Game.DungeonMap.AddTreasure( defender.X, defender.Y, defender.Hand );
            }
            if ( defender.Feet != null && defender.Feet != FeetEquipment.None() )
            {
               Game.DungeonMap.AddTreasure( defender.X, defender.Y, defender.Feet );
            }
            Game.DungeonMap.AddGold( defender.X, defender.Y, defender.Gold );
            Game.Player.AddXp(defender.Xp);
            Game.DungeonMap.RemoveMonster( (Monster) defender );

                Game.MessageLog.Add($"{defender.Name} est mort et a lache {defender.Gold} d'or");
                Game.MessageLog.Add(espace.ToString());
         }
      }

      public bool HandleKey( RLKey key )
      {
         if ( key == RLKey.Q )
         {
            return Game.Player.QAbility.Perform();
         }
         if ( key == RLKey.W )
         {
            return Game.Player.WAbility.Perform();
         }
         if ( key == RLKey.E )
         {
            return Game.Player.EAbility.Perform();
         }
         if ( key == RLKey.R )
         {
            return Game.Player.RAbility.Perform();
         }


         bool didUseItem = false;
         if ( key == RLKey.Number1 )
         {
            didUseItem = Game.Player.Item1.Use();
         }
         else if ( key == RLKey.Number2 )
         {
            didUseItem = Game.Player.Item2.Use();
         }
         else if ( key == RLKey.Number3 )
         {
            didUseItem = Game.Player.Item3.Use();
         }
         else if ( key == RLKey.Number4 )
         {
            didUseItem = Game.Player.Item4.Use();
         }

         if ( didUseItem )
         {
            RemoveItemsWithNoRemainingUses();
         }

         return didUseItem;
      }

      private static void RemoveItemsWithNoRemainingUses()
      {
         if ( Game.Player.Item1.RemainingUses <= 0 )
         {
            Game.Player.Item1 = new NoItem();
         }
         if ( Game.Player.Item2.RemainingUses <= 0 )
         {
            Game.Player.Item2 = new NoItem();
         }
         if ( Game.Player.Item3.RemainingUses <= 0 )
         {
            Game.Player.Item3 = new NoItem();
         }
         if ( Game.Player.Item4.RemainingUses <= 0 )
         {
            Game.Player.Item4 = new NoItem();
         }
      }

      public void EndPlayerTurn()
      {
         IsPlayerTurn = false;
         Game.Player.Tick();
      }
   }
}
