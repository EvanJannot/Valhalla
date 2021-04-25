namespace Valhalla.Interfaces
{
    //Interface permettant de compléter la classe des objets 
   public interface IItem
   {
      string Name { get; }
      int RemainingUses { get; }

      bool Use();
   }
}
