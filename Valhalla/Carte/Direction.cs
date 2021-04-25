namespace Valhalla.Core
{
   // Stock les directions pouvant être prises afin d'être réutilisées lors du déplacement du joueur 
   // A noter : RLNET n'a pas l'air de supporter les déplacements en diagonal
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