namespace Valhalla.Equipment
{
    //Classe définissant les différents equipements pour le torse 
   public class BodyEquipment : Core.Equipment
   {
      public static BodyEquipment None() //Aucun equipement 
      {
         return new BodyEquipment { Name = "Rien" };
      }

      public static BodyEquipment Leather() //Equipement en cuir 
      {
         return new BodyEquipment() {
            Defense = 1,
            DefenseChance = 10,
            Name = "Cuir"
         };
      }

      public static BodyEquipment Chain() //Equipement en maille 
      {
         return new BodyEquipment() {
            Defense = 2,
            DefenseChance = 5,
            Name = "Maille"
         };
      }

      public static BodyEquipment Plate() //Equipement en plaque 
      {
         return new BodyEquipment() {
            Defense = 2,
            DefenseChance = 10,
            Name = "Plaque"
         };
      }
   }
}