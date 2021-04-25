namespace Valhalla.Interfaces
{
    //Interface permettant de compléter la classe acteur et de gérer la gestion du temps en créant des objets spécifiquement pour cette tâche 
   public interface IScheduleable
   {
      int Time { get; }
   }
}