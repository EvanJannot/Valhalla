using Valhalla.Equipment;

namespace Valhalla.Systems
{
    //Classe permettant la génération de l'équipement au sol lors de la création de la carte 
   public class EquipmentGenerator
   {
      private readonly Pool<Core.Equipment> _equipmentPool;

        //Fonction générant l'équipement au sein d'une liste pondérée, chaque équipement a un poids et donc plus ou moins de chance d'être sélectionné
      public EquipmentGenerator( int monde) //Dépend du monde 
      {
         _equipmentPool = new Pool<Core.Equipment>();

         if ( monde == 1 ) //Dans le monde 1 l'équipement est en cuir avec peu de chance d'obtenir de la maille
         {
            _equipmentPool.Add( BodyEquipment.Leather(), 20 );
            _equipmentPool.Add( HeadEquipment.Leather(), 20 );
            _equipmentPool.Add( FeetEquipment.Leather(), 20 );
            _equipmentPool.Add( HandEquipment.Dagger(), 25 ); //On peut obtenir une dague ou si on est très chanceux une épée
            _equipmentPool.Add( HandEquipment.Sword(), 5 );
            _equipmentPool.Add( HeadEquipment.Chain(), 5 );
            _equipmentPool.Add( BodyEquipment.Chain(), 5 );
         }
         else if ( monde == 2) //Dans le monde 2 l'équipement est en maille avec peu de chance d'obtenir des plaques 
            {
            _equipmentPool.Add( BodyEquipment.Chain(), 20 );
            _equipmentPool.Add( HeadEquipment.Chain(), 20 );
            _equipmentPool.Add( FeetEquipment.Chain(), 20 );
            _equipmentPool.Add( HandEquipment.Sword(), 15 ); //On peut obtenir une épée ou bien une hache 
            _equipmentPool.Add( HandEquipment.Axe(), 15 );
            _equipmentPool.Add( HeadEquipment.Plate(), 5 );
            _equipmentPool.Add( BodyEquipment.Plate(), 5 );
         }
         else //Dans les mondes suivants il n'y a que des plaques et des claymores 
         {
            _equipmentPool.Add( BodyEquipment.Plate(), 25 );
            _equipmentPool.Add( HeadEquipment.Plate(), 25 );
            _equipmentPool.Add( FeetEquipment.Plate(), 25 );
            _equipmentPool.Add( HandEquipment.TwoHandedSword(), 25 );
         }
      }

        //Sélectionne un équipement de la liste pondérée pour le créer 
      public Core.Equipment CreateEquipment()
      { 
         return _equipmentPool.Get();
      }
   }
}
