using RLNET;
using RogueSharp;

namespace Valhalla.Interfaces
{
    //Interface permettant de compléter les classes des objets dessinables (monstres, joueur, escaliers, portes, etc)
    public interface IDrawable
    {
        RLColor Color { get; set; }
        char Symbol { get; set; }
        int X { get; set; }
        int Y { get; set; }

        void Draw(RLConsole console, IMap map, int lvl);
    }
}