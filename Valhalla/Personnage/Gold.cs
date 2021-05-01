using RLNET;
using RogueSharp;
using Valhalla.Interfaces;

namespace Valhalla.Core
{
    //L'or est un trésor qui est dessiné au sol après qu'un ennemi soit mort
    public class Gold : ITreasure, IDrawable
    {
        public int Amount { get; set; }

        public Gold(int amount)
        {
            Amount = amount;
            Symbol = (char)21; //L'or est un tas de pièces d'or
            Color = RLColor.Yellow;
        }

        public bool PickUp(IActor actor) //Permet récupérer l'or pour un acteur
        {
            actor.Gold += Amount;
            return true;
        }

        //Getter et setter associés
        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        //Permet de dessiner les pièces d'or au sol
        public void Draw(RLConsole console, IMap map, int lvl)
        {
            if (!map.IsExplored(X, Y)) //Si la case n'a pas été explorée on ne dessine rien
            {
                return;
            } //Cette partie n'est pas utile en soit car un monstre ne peut pas mourir sans que le joueur ne le tue et donc pour se faire il doit explorer la map mais on ne sait jamais

            if (map.IsInFov(X, Y)) //Si la zone est dans le champ de vision du personnage on dessine l'or de manière clair et sinon on floute
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                console.Set(X, Y, RLColor.Blend(Color, RLColor.Gray, 0.5f), Colors.FloorBackground, Symbol);
            }
        }
    }
}
