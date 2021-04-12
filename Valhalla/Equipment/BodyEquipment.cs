namespace RogueSharpRLNetSamples.Equipment
{
   public class BodyEquipment : Core.Equipment
   {
      public static BodyEquipment None()
      {
         return new BodyEquipment { Name = "Rien" };
      }

      public static BodyEquipment Leather()
      {
         return new BodyEquipment() {
            Defense = 1,
            DefenseChance = 10,
            Name = "Cuir"
         };
      }

      public static BodyEquipment Chain()
      {
         return new BodyEquipment() {
            Defense = 2,
            DefenseChance = 5,
            Name = "Maille"
         };
      }

      public static BodyEquipment Plate()
      {
         return new BodyEquipment() {
            Defense = 2,
            DefenseChance = 10,
            Name = "Plaque"
         };
      }
   }
}