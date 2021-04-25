namespace Valhalla.Core
{
   // Stock les directions pouvant �tre prises afin d'�tre r�utilis�es lors du d�placement du joueur 
   // A noter : RLNET n'a pas l'air de supporter les d�placements en diagonal
   public enum Direction
   {
      None = 0,
      DownLeft = 1,
      Down = 2,
      DownRight = 3,
      Left = 4,
      Center = 5,
      Right = 6,
      UpLeft = 7,
      Up = 8,
      UpRight = 9
   }
}