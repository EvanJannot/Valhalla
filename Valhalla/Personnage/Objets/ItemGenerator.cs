using Valhalla.Core;
using Valhalla.Items;

namespace Valhalla.Systems
{
    //Classe permettant la génération des objets sur la map
   public static class ItemGenerator
   {
      public static Item CreateItem() //Fonction de création des objets
      {
         Pool<Item> itemPool = new Pool<Item>();

         itemPool.Add( new ArmorScroll(), 10 ); //Chaque objet a un certain poids, plus il est élevé plus l'objet a des chances d'être choisi dans la liste
         itemPool.Add( new DestructionWand(), 5 );
         itemPool.Add( new HealingPotion(), 20 );
         itemPool.Add( new RevealMapScroll(), 25 );
         itemPool.Add( new TeleportScroll(), 20 );
         itemPool.Add( new Whetstone(), 10 );

         return itemPool.Get(); //On retourne un des objets 
      }
   }
}
