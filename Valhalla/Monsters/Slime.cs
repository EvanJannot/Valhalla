using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Behaviors;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples.Monsters
{
   public class Slime : Monster
   {
      public static Slime Create( int level )
      {
         int health = Dice.Roll( "4D5" );
         return new Slime {
            Attack = Dice.Roll( "1D2" ) + level / 3,
            AttackChance = Dice.Roll( "10D5" ),
            Awareness = 10,
            Color = Colors.OozeColor,
            Defense = Dice.Roll( "1D2" ) + level / 3,
            DefenseChance = Dice.Roll( "10D4" ),
            Gold = Dice.Roll( "1D20" ),
            Health = health,
            MaxHealth = health,
            Name = "Slime",
            Speed = 14,
            Symbol = (char)141,
             Xp = level * 5
         };
      }

      public override void PerformAction( CommandSystem commandSystem )
      {
         var splitOozeBehavior = new SplitSlime();
         if ( !splitOozeBehavior.Act( this, commandSystem ) )
         {
            base.PerformAction( commandSystem );
         }
      }
   }
}
