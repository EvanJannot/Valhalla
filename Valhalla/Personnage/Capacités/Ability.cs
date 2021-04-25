using RLNET;
using RogueSharp;
using Valhalla.Interfaces;
using System.Text;

namespace Valhalla.Core
{
    //Les sorts sont des trésors et apparaissent au sol comme des parchemins 
   public class Ability : IAbility, ITreasure, IDrawable
   {
        StringBuilder espace = new StringBuilder();
        public Ability()
      {
         Symbol = (char)148; //Définition de l'apparence en tant que vieux parchemin
         Color = RLColor.Yellow;
      }

        //Permet de récupérer lee nom, le temps de rechargement total et restant 
      public string Name { get; protected set; }

      public int TurnsToRefresh { get; protected set; }

      public int TurnsUntilRefreshed { get; protected set; }

        //Fonction permettant de lancer un sort
      public bool Perform()
      {
         if ( TurnsUntilRefreshed > 0 ) //Si la capacité est en cours de rechargement on retourne false car on ne fait rien 
         {
            return false;
         }

         TurnsUntilRefreshed = TurnsToRefresh; //Sinon on définit le temps de rechargement restant comme étant le temps total pour recharger le sort et on le lance

         return PerformAbility();
      }

      protected virtual bool PerformAbility() //Si il n'y a pas de sort cela retourne directement false, cette fonction est override dans les classes des sorts 
      {
         return false;
      }

        //Permet de gérer le temps de rechargement par rapport au système de temps du jeu
      public void Tick()
      {
         if ( TurnsUntilRefreshed > 0 )
         {
            TurnsUntilRefreshed--;
         }
      }

        //Permet de récupérer un sort
      public bool PickUp( IActor actor )
      {
         Player player = actor as Player;

         if ( player != null ) //Seul le joueur peut récupérer le sort 
         {
            if ( player.AddAbility( this ) ) //Si la capacité peut être récupérée on affiche un message et on retourne true 
            {
               Game.MessageLog.Add( $"{actor.Name} a appris la capacite {Name}" );
                    Game.MessageLog.Add(espace.ToString());
                    return true;
            }
         }

         return false; //Sinon on retourne false 
      }

      public RLColor Color { get; set; }
      public char Symbol { get; set; }
      public int X { get; set; }
      public int Y { get; set; }

        //Permet de dessiner le sort au sol
      public void Draw( RLConsole console, IMap map, int lvl )
      {
         if ( !map.IsExplored( X, Y ) ) //Si on a pas exploré la zone on ne l'affiche pas
         {
            return;
         }

         if ( map.IsInFov( X, Y ) ) //Si il est dans le champ de vision on l'affiche normalement et sinon on le floute 
         {
            console.Set( X, Y, Color, Colors.FloorBackgroundFov, Symbol );
         }
         else
         {
            console.Set( X, Y, RLColor.Blend( Color, RLColor.Gray, 0.5f ), Colors.FloorBackground, Symbol );
         }
      }
   }
}
