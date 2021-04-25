using RLNET;
using RogueSharp;
using Valhalla.Interfaces;

namespace Valhalla.Core
{
    //Classe définissant les portes
    public class Door : IDrawable
    {
        public Door()
        {
            Symbol = (char)143;
            Color = Colors.Door;
            BackgroundColor = Colors.DoorBackground;
        }
        public bool IsOpen { get; set; }

        public RLColor Color { get; set; }
        public RLColor BackgroundColor { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Draw(RLConsole console, IMap map, int lvl)
        {
            if (lvl == 1)//Dans le monde 1
            {
                if (!map.GetCell(X, Y).IsExplored) //Si la porte n'est pas explorée on l'affiche pas
                {
                    return;
                }

                Symbol = IsOpen ? (char)143 : (char)142; //En fonction de l'état de la porte (ouvert/fermé) le symbole est différent 
                if (map.IsInFov(X, Y)) //Suivant si la map est dans le champ de vision ou non, la couleur est différente
                {
                    Color = RLColor.Brown;
                    BackgroundColor = RLColor.Black;
                }
                else
                {
                    Color = RLColor.Brown;
                    BackgroundColor = RLColor.Black;
                }

                console.Set(X, Y, Color, BackgroundColor, Symbol);
            }
            else if (lvl == 2) //Même principe dans le monde 2
            {
                if (!map.GetCell(X, Y).IsExplored)
                {
                    return;
                }

                Symbol = IsOpen ? (char)153 : (char)152;
                if (map.IsInFov(X, Y))
                {
                    Color = RLColor.LightGray;
                    BackgroundColor = RLColor.Black;
                }
                else
                {
                    Color = RLColor.Gray;
                    BackgroundColor = RLColor.Black;
                }

                console.Set(X, Y, Color, BackgroundColor, Symbol);
            }
            else if (lvl == 3) //Et le 3
            {
                if (!map.GetCell(X, Y).IsExplored)
                {
                    return;
                }

                Symbol = IsOpen ? (char)153 : (char)152;
                if (map.IsInFov(X, Y))
                {
                    Color = RLColor.LightGray;
                    BackgroundColor = RLColor.Black;
                }
                else
                {
                    Color = RLColor.Gray;
                    BackgroundColor = RLColor.Black;
                }

                console.Set(X, Y, Color, BackgroundColor, Symbol);
            }
            else //Et pour les mondes d'après 
            {
                if (!map.GetCell(X, Y).IsExplored)
                {
                    return;
                }

                Symbol = IsOpen ? (char)143 : (char)142;
                if (map.IsInFov(X, Y))
                {
                    Color = RLColor.Brown;
                    BackgroundColor = RLColor.Black;
                }
                else
                {
                    Color = RLColor.Brown;
                    BackgroundColor = RLColor.Black;
                }

                console.Set(X, Y, Color, BackgroundColor, Symbol);
            }

        }
    }
}