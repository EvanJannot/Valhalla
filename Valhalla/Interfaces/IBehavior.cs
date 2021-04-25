using Valhalla.Core;
using Valhalla.Systems;

namespace Valhalla.Interfaces
{
    //Interface permettant de compléter les différentes classes des comportements des monstres 
   public interface IBehavior
   {
      bool Act( Monster monster, CommandSystem commandSystem );
   }
}
