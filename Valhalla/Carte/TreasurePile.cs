using Valhalla.Interfaces;

namespace Valhalla.Core
{
    public class TreasurePile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ITreasure Treasure { get; set; }

        //Classe d'un trésor 
        public TreasurePile(int x, int y, ITreasure treasure)
        {
            X = x;
            Y = y;
            Treasure = treasure;

            IDrawable drawableTreasure = treasure as IDrawable;
            if (drawableTreasure != null) //Si le trésor est bien dessinable on défini ses coordonnées (x,y)
            {
                drawableTreasure.X = x;
                drawableTreasure.Y = y;
            }
        }
    }
}
