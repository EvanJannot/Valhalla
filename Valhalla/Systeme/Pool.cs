using System;
using System.Collections.Generic;
using RogueSharp.Random;

namespace Valhalla.Systems
{
    //Permet de créer des listes où chaque élément possède un poids 
   public class Pool<T>
   {
      private readonly List<PoolItem<T>> _poolItems;
      private static readonly IRandom _random = new DotNetRandom();
      private int _totalWeight;

      public Pool() //On initialise la liste 
      {
         _poolItems = new List<PoolItem<T>>();
      }

      public T Get() //Fonction pour récupérer un objet de la liste aléatoire 
      {
         int runningWeight = 0; //Compteur qui permet de savoir ou on en est grâce au poids 
         int roll = _random.Next( 1, _totalWeight ); //On jette un dé qui va de 1 à la somme du poids de tous les éléments 
         foreach ( var poolItem in _poolItems ) //Pour chaque element de la liste 
         {
            runningWeight += poolItem.Weight; //On augmente le compteur avec le poids de l'élément 
            if ( roll <= runningWeight ) //Si le compteur dépasse le lancer c'est que c'est l'élément actuel qui le fait dépasser et donc on choisi cet élément 
            {
               Remove( poolItem );  //on l'enlève de la liste 
               return poolItem.Item; //On le retourne 
            }
         }

         throw new InvalidOperationException( "Could not get an item from the pool" ); 
      }

        //Fonction qui ajoute un élément à la liste 
      public void Add( T item, int weight )
      {
         _poolItems.Add( new PoolItem<T> { Item = item, Weight = weight } ); //On l'ajoute 
         _totalWeight += weight; //On ajoute le poids de l'élément au poids total 
      }

        //Fonction qui retire un élément de la liste 
      public void Remove( PoolItem<T> poolItem )
      {
         _poolItems.Remove( poolItem ); //On retire l'élément de la liste
         _totalWeight -= poolItem.Weight; //On enlève le poids de l'élément au poids total 
      }
   }

    //Classe qui définit un élément de la liste qui est défini par son poids et par lui même 
   public class PoolItem<T>
   {
      public int Weight { get; set; } 
      public T Item { get; set; }
   }
}
