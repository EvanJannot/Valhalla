namespace Valhalla.Equipment
{
    //Classe définissant les différents casques 
   public class HeadEquipment : Core.Equipment
   {
      public static HeadEquipment None() //Aucun equipement 
      {
         return new HeadEquipment { Name = "Rien" };
      }

      public static HeadEquipment Leather() //Equipement en cuir 
      {
         return new HeadEquipment() {
            Defense = 1,
            DefenseChance = 5,
            Name = "Cuir"
         };
      }

      public static HeadEquipment Chain() //Equipement en maille 
      {
         return new HeadEquipment() {
            Defense = 1,
            DefenseChance = 10,
            Name = "Maille"
         };
      }

      public static HeadEquipment Plate() //Equipement en plaque 
      {
         return new HeadEquipment() {
            Defense = 1,
            DefenseChance = 15,
            Name = "Plaque"
         };
      }
   }
}