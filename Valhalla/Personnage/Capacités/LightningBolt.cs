using RogueSharp;
using Valhalla.Core;
using Valhalla.Interfaces;
using System.Text;

namespace Valhalla.Abilities
{
    //Sort de foudre 
   public class LightningBolt : Ability, ITargetable
   {
        StringBuilder espace = new StringBuilder();
      private readonly int _attack;
      private readonly int _attackChance;

      public LightningBolt( int attack, int attackChance ) //On définit ici aussi le nom, le temps de rechargement les dégats et les chances de toucher 
      {
         Name = "Eclair";
         TurnsToRefresh = 40;
         TurnsUntilRefreshed = 0;
         _attack = attack;
         _attackChance = attackChance;
      }

      protected override bool PerformAbility() //Contrairement à la boule de feu, ici ce n'est pas une zone mais une ligne de points 
      {
         return Game.TargetingSystem.SelectLine( this );
      }

      public void SelectTarget( Point target ) //Lorsque le joueur appuie sur entrée on déclenche l'attaque 
      {
         DungeonMap map = Game.DungeonMap;
         Player player = Game.Player;
         Game.MessageLog.Add( $"{player.Name} lance un {Name}" ); //On ajoute un message pour confirmer que l'attaque est lancée 
            Game.MessageLog.Add(espace.ToString());

            Actor lightningBoltActor = new Actor { //On définit un acteur eclair qui va attaquer les monstres 
            Attack = _attack,
            AttackChance = _attackChance,
            Name = Name
         };
         foreach ( Cell cell in map.GetCellsAlongLine( player.X, player.Y, target.X, target.Y ) ) //Pour chaque case le long de la ligne 
         {
            if ( cell.IsWalkable ) //Si il n'y a pas de monstre on continue de parcourir la ligne 
            {
               continue;
            }

            if ( cell.X == player.X && cell.Y == player.Y ) //Si il y a le joueur on continue aussi 
            {
               continue;
            }

            Monster monster = map.GetMonsterAt( cell.X, cell.Y ); //Si il y a un monstre on l'attaque avec l'éclair 
            if ( monster != null ) 
            {
               Game.CommandSystem.Attack( lightningBoltActor, monster );
            }
            else //Sinon c'est que c'est un mur alors on arrete 
            {
               return;
            }
         }
      }
   }
}
