using RLNET;
using RogueSharp;

namespace Valhalla.Core
{
    public class Stairs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsUp { get; set; }

        //Fonction pour dessiner un escalier 
        public void Draw(RLConsole console, IMap map, int lvl)
        {
            if (!map.GetCell(X, Y).IsExplored) //Si la case n'est pas explorée on ne dessine rien
            {
                return;
            }
            if (lvl == 3) //Pour le monde 3 on dessine des escaliers 
            {
                if (map.IsInFov(X, Y))
                {
                    if (IsUp)
                    {
                        console.Set(X, Y, RLColor.LightGray, RLColor.Black, (char)18);
                    }
                    else
                    {
                        console.Set(X, Y, RLColor.LightGray, RLColor.Black, (char)19);
                    }
                }
                else
                {
                    if (IsUp)
                    {
                        console.Set(X, Y, RLColor.Gray, RLColor.Black, (char)18);
                    }
                    else
                    {
                        console.Set(X, Y, RLColor.Gray, RLColor.Black, (char)19);
                    }
                }
            }
            else //Sinon des échelles 
            {
                if (map.IsInFov(X, Y))
                {
                    if (IsUp)
                    {
                        console.Set(X, Y, Colors.Player, RLColor.Black, (char)151);
                    }
                    else
                    {
                        console.Set(X, Y, Colors.Player, RLColor.Black, (char)150);
                    }
                }
                else
                {
                    if (IsUp)
                    {
                        console.Set(X, Y, Colors.Floor, RLColor.Black, (char)151);
                    }
                    else
                    {
                        console.Set(X, Y, Colors.Floor, RLColor.Black, (char)150);
                    }
                }

            }


        }
    }
}