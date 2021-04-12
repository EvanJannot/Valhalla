namespace RogueSharpRLNetSamples.Equipment
{
   public class HeadEquipment : Core.Equipment
   {
      public static HeadEquipment None()
      {
         return new HeadEquipment { Name = "Rien" };
      }

      public static HeadEquipment Leather()
      {
         return new HeadEquipment() {
            Defense = 1,
            DefenseChance = 5,
            Name = "Cuir"
         };
      }

      public static HeadEquipment Chain()
      {
         return new HeadEquipment() {
            Defense = 1,
            DefenseChance = 10,
            Name = "Maille"
         };
      }

      public static HeadEquipment Plate()
      {
         return new HeadEquipment() {
            Defense = 1,
            DefenseChance = 15,
            Name = "Plaque"
         };
      }
   }
}