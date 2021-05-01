using RogueSharp;

namespace Valhalla.Interfaces
{
    //Permet de compléter les classes des capacités devant viser un ennemi (boule de feu, éclair ou encore le missile)
    public interface ITargetable
    {
        void SelectTarget(Point target);
    }
}
