using System;
using RLNET;
using Valhalla.Abilities;
using Valhalla.Equipment;
using Valhalla.Interfaces;
using Valhalla.Items;

namespace Valhalla.Core
{
    //Le joueur est un acteur 
    public class Player : Actor
    {
        //Il possède 4 sorts/capcités associés aux touches Z,Q,S,D qui sont en anglais Q,W,E,R, c'est la seconde notation qui est utilisé car les commandes sont basées sur le QWERTY
        //En effet en appuyant sur la touche Z, RLKEY va lire Q
        public IAbility QAbility { get; set; }
        public IAbility WAbility { get; set; }
        public IAbility EAbility { get; set; }
        public IAbility RAbility { get; set; }

        //Un acteur possède aussi 4 objets qui sont eux utilisables avec les touches 1, 2, 3 et 4
        public IItem Item1 { get; set; }
        public IItem Item2 { get; set; }
        public IItem Item3 { get; set; }
        public IItem Item4 { get; set; }

        //Un joueur n'a au départ pas de capacité ni d'objet
        public Player()
        {
            QAbility = new DoNothing();
            WAbility = new DoNothing();
            EAbility = new DoNothing();
            RAbility = new DoNothing();
            Item1 = new NoItem();
            Item2 = new NoItem();
            Item3 = new NoItem();
            Item4 = new NoItem();
        }

        //Fonction permettant d'ajouter de l'expérience et de monter de niveau
        public void AddXp(int xp)
        {
            if (xp > 0) //Si on a gagné de l'xp
            {
                if ((Xp + xp) >= Lvl * 100) //Et que on a plus d'xp qu'il en faut pour monter de niveau
                {
                    Xp = (Xp + xp) - (Lvl * 100); //On  passe de niveau et on remet à 0 la jauge d'xp en ajoutant le surplus à celle du niveau suivant
                    Lvl++;
                    MaxHealth += 10; //On augmente de 10 la vie max et si le joueur a toute sa vie on augmente sa vie pour qu'elle corresponde à sa nouvelle vie max 
                    if (Health == (MaxHealth - 10))
                    {
                        Health = MaxHealth;
                    }
                    if ((AttackChance + 1) < 100) //On augmente de 1% les chances d'attaques 
                    {
                        AttackChance += 1;
                    }
                }
                else //Si on ne monte pas de niveau on ajoute juste l'xp
                {
                    Xp += xp;
                }
            }
        }

        //Fonction pour ajouter une capacité 
        public bool AddAbility(IAbility ability)
        {
            if (QAbility is DoNothing | QAbility == ability) //Si on a pas de capacité on ajoute la nouvelle ou si on a déjà cette capacité on la remplace ce qui permet de ne pas en avoir en double 
            {
                QAbility = ability;
                return true; //On renvoie true pour afficher le message 
            }
            else if (WAbility is DoNothing | WAbility == ability)
            {
                WAbility = ability;
                return true; //On renvoie true pour afficher le message 
            }
            else if (EAbility is DoNothing | EAbility == ability)
            {
                EAbility = ability;
                return true; //On renvoie true pour afficher le message 
            }
            else if (RAbility is DoNothing | RAbility == ability)
            {
                RAbility = ability;
                return true; //On renvoie true pour afficher le message 
            }
            else //Sinon on ajoute rien et on renvoie false pour ne pas afficher le message associé au fait que le joueur a ramassé la capacité
            {
                return false;
            }


        }

        //Fonction pour ajouter un objet au joueur
        public bool AddItem(IItem item)
        {
            if (Item1 is NoItem) //Le fonctionnement est identique sauf que ici on peut avoir en plusieurs fois un objet 
            {
                Item1 = item;
            }
            else if (Item2 is NoItem)
            {
                Item2 = item;
            }
            else if (Item3 is NoItem)
            {
                Item3 = item;
            }
            else if (Item4 is NoItem)
            {
                Item4 = item;
            }
            else
            {
                return false;
            }

            return true;
        }

        //Fonctions permettant d'afficher les statistiques du joueur, son inventaire, ses capacités et ses objets
        public void DrawStats(RLConsole statConsole) //On affiche juste les statistiques avec des symboles pour que cela soit plus visuel
        {
            if (AttackChance > 100) //Permet de ne pas dépasser les 100% de chances d'attaque 
            {
                AttackChance =100; 
            }
            statConsole.Print(1, 1, $"Nom: {Name}", RLColor.White);
            statConsole.Print(1, 3, $"Vie:     {Health}-{MaxHealth}" + (char)26, RLColor.White);
            statConsole.Print(1, 5, $"Attaque: {Attack} ({AttackChance}<)" + (char)12, RLColor.White);
            statConsole.Print(1, 7, $"Defense: {Defense} ({DefenseChance}<)" + (char)13, RLColor.White);
            statConsole.Print(1, 9, $"Or:      {Gold}" + (char)21, RLColor.White);
            statConsole.Print(1, 11, $"Xp:      {Xp}-{Lvl * 100}", RLColor.White);
            statConsole.Print(1, 13, $"Lvl:     {Lvl}", RLColor.White);
        }

        public void DrawInventory(RLConsole inventoryConsole) //On organise la console d'inventaire qui possède une colonne avec l'equipement, une avec les sorts et une denrière avec la magie 
        {
            inventoryConsole.Print(1, 1, "Equipement", Colors.InventoryHeading);
            inventoryConsole.Print(1, 3, $"Tete:   {Head.Name}", Head == HeadEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight);
            inventoryConsole.Print(1, 5, $"Torse:  {Body.Name}", Body == BodyEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight);
            inventoryConsole.Print(1, 7, $"Mains:  {Hand.Name}", Hand == HandEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight);
            inventoryConsole.Print(1, 9, $"Jambes: {Feet.Name}", Feet == FeetEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight);

            inventoryConsole.Print(20, 1, "Capacites", Colors.InventoryHeading); //Pour les capacités on appelle la fonction qui permet de les dessiner mais on définit leur emplacement
            DrawAbility(QAbility, inventoryConsole, 0);
            DrawAbility(WAbility, inventoryConsole, 1);
            DrawAbility(EAbility, inventoryConsole, 2);
            DrawAbility(RAbility, inventoryConsole, 3);

            inventoryConsole.Print(40, 1, "Objets", Colors.InventoryHeading); //Idem pour les objets
            DrawItem(Item1, inventoryConsole, 0);
            DrawItem(Item2, inventoryConsole, 1);
            DrawItem(Item3, inventoryConsole, 2);
            DrawItem(Item4, inventoryConsole, 3);
        }

        private void DrawAbility(IAbility ability, RLConsole inventoryConsole, int position)
        {
            char letter = 'A'; //Permet de savoir quelle lettre afficher en fonction de la position de la capacité 
            if (position == 0)
            {
                letter = 'A';
            }
            else if (position == 1)
            {
                letter = 'Z';
            }
            else if (position == 2)
            {
                letter = 'E';
            }
            else if (position == 3)
            {
                letter = 'R';
            }

            RLColor highlightTextColor = Swatch.DbOldStone;
            if (!(ability is DoNothing)) //Si le joueur n'a pas de capacité on ne fait rien, visuellement rien ne se passe 
            {
                if (ability.TurnsUntilRefreshed == 0)
                {
                    highlightTextColor = Swatch.DbLight;
                }
                else
                {
                    highlightTextColor = Swatch.DbSkin;
                }
            }

            int xPosition = 20;
            int xHighlightPosition = 20 + 4; //Permet d'afficher une barre de rechargement 
            int yPosition = 3 + (position * 2);
            inventoryConsole.Print(xPosition, yPosition, $"{letter} - {ability.Name}", highlightTextColor); //On affiche le nom de la capacité 

            if (ability.TurnsToRefresh > 0) //Si la capacité doit se recharger 
            {
                int width = Convert.ToInt32(((double)ability.TurnsUntilRefreshed / (double)ability.TurnsToRefresh) * 16.0); //On calcul le rapport entre le temps qu'il faut pour recharger la capacité et le temps restant que l'on multiplie pour l'avoir sur une échelle de 16
                int remainingWidth = 20 - width;
                inventoryConsole.SetBackColor(xHighlightPosition, yPosition, width, 1, Swatch.DbOldBlood); //On affiche la barre de rechargement qui a chaque tour va diminuer 
                inventoryConsole.SetBackColor(xHighlightPosition + width, yPosition, remainingWidth, 1, RLColor.Black);
            }
        }

        //Permet de dessiner les objets 
        private void DrawItem(IItem item, RLConsole inventoryConsole, int position)
        {
            int xPosition = 40;
            int yPosition = 3 + (position * 2);
            string place = (position + 1).ToString();
            inventoryConsole.Print(xPosition, yPosition, $"{place} - {item.Name}", item is NoItem ? Swatch.DbOldStone : Swatch.DbLight);
        }

        //Permet de gérer les temps de rechargement des capacités
        public void Tick()
        {
            QAbility?.Tick();
            WAbility?.Tick();
            EAbility?.Tick();
            RAbility?.Tick();
        }
    }
}