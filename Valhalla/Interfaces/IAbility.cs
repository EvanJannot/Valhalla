namespace Valhalla.Interfaces
{
    //Interface permettant de définir les capacités (permet de compléter la classe des capacités)
   public interface IAbility
   {
      string Name { get; }
      int TurnsToRefresh { get; }
      int TurnsUntilRefreshed { get; }

      bool Perform();
      void Tick();
   }
}
