using System;
using System.Collections.Generic;
using System.Media;
using RLNET;
using RogueSharp.Random;
using Valhalla.Core;
using Valhalla.Items;
using Valhalla.Systems;

namespace Valhalla
{
    public static class Game
    {
        private static List<string> _listeNoms = new List<string> { "puduku", "grolar", "padecervo", "MoNsTeRsLaYeR" };

        //On définit les variables de bonus de stat liées aux classes
        private static int _atkX;
        private static int _defX;
        private static int _awaX;
        private static int _hpX;
        private static string _name = "Rogue";
        private static char _symbol;


        //On définit les tailles des différents écrans (map, stats, inventaire, boite de dialogues)
        private static readonly int _screenWidth = 80;  //Taille de la console générale
        private static readonly int _screenHeight = 52;
        private static readonly int _mapWidth = 60; //Taille de la map
        private static readonly int _mapHeight = 28;
        private static readonly int _messageWidth = 80; //Taille de la boite de dialogue
        private static readonly int _messageHeight = 14;
        private static readonly int _statWidth = 20; //Taille de l'écran des stats
        private static readonly int _statHeight = 52;
        private static readonly int _inventoryWidth = 60; //Taille de l'inventaire
        private static readonly int _inventoryHeight = 10;

        //On définit les différentes consoles du jeu apparaissant à l'écran
        private static RLRootConsole _rootConsole;
        private static RLConsole _mapConsole;
        private static RLConsole _messageConsole;
        private static RLConsole _statConsole;
        private static RLConsole _inventoryConsole;

        //Ces consoles sont celles de fin du jeu en cas de mort et de pause
        private static RLConsole _endgame;
        private static RLConsole _pauseConsole;

        private static int _mondeLevel = 0; //Niveau du monde, dans chaque monde il y a un boss et des ennemis spécifiques
        private static int _mapLevel = 1; //Définition du niveau au sein du monde 
        public static int MONDE
        {
            get
            { return _mondeLevel; }
        }
        public static int LEVEL
        {
            get
            { return _mapLevel; }
        }

        private static bool _alive = true; //Variable vérifiant si le joueur est vivant ou mort
        private static bool _winner = false; //Variable vérifiant si le joueur a fini les 3 mondes 
        private static bool _renderRequired = true; //Variable vérifiant si il faut mettre à jour la console

        //On définit les différents getter et setter pour les éléments principaux de notre jeu (joueur, map, messages, le système de combat, la pause et la fin)
        public static Player Player { get; set; }
        public static DungeonMap DungeonMap { get; private set; }
        public static MessageLog MessageLog { get; private set; }
        public static CommandSystem CommandSystem { get; private set; }
        public static SchedulingSystem SchedulingSystem { get; private set; }
        public static TargetingSystem TargetingSystem { get; private set; }
        public static End End { get; private set; }
        public static Pause Pause { get; private set; }
        public static IRandom Random { get; private set; }

        public static void Main()
        {
            Console.Clear();
            //On remet tout à 0 (stats du perso, infos du perso, etc)
            Reset();
            //Explication du lore, et des commandes si nécessaire
            Console.WriteLine("Bonjour, \n" +
                "Tu te réveilles enfin ... \n" +
                "Tu es mort. \n" +
                "Mais ne t'en fais pas ! Ta vie ne s'arrête pas là. \n" +
                "Enfin d'une certaine manière si mais il y a bien des choses après... \n" +
                "En tant que valeureux guerrier viking, tu as une chance de parvenir au Valhalla!! Le paradis des guerriers vikings! \n" +
                "Malheureusement il y a un petit soucis... \n" +
                "Il se trouve qu'un ancien Roi viking bloque l'accès et le seul moyen d'y entrer est de le vaincre :) \n" +
                "Mais pas de panique si tu es là c'est que tu devrai pouvoir le vaincre !\n" +
                "Mais sais-tu comment te servir de ton corps dans le monde d'après ? \n" +
                "-O : oui\n" +
                "-N : non\n");
            string _answer = Console.ReadLine().ToUpper();
            while (_answer != "N" & _answer != "O") //Boucle de vérification de la valeur entrée 
            {
                Console.WriteLine("Veuillez sélectionner une des deux options");
                _answer = Console.ReadLine().ToUpper();
            }
            if (_answer == "N") //Instructions 
            {
                Console.Clear();
                Console.WriteLine("\n" +
                    "Déplacements : \n" +
                    "Il suffit d'utiliser les flèches directionnelles du clavier!\n \n" +
                    "Changement d'étage : \n" +
                    "Il suffit d'appuyer sur espace en se trouvant sur un escalier descendant.\n \n" +
                    "Attaque : \n" +
                    "Pour attaquer il suffit d'avancer vers un ennemi de manière répétée pour le frapper. \n \n" +
                    "Inventaire : \n" +
                    "Pour utiliser un objet, il vous suffit d'appuyer sur la touche liée à l'emplacement. \n \n" +
                    "Capacités : \n" +
                    "Même principe que pour les objets ! \n \n" +
                    "Equipement : \n" +
                    "Il est automatiquement ramassé lorsque le joueur passe dessus !\n \n" +
                    "Vous pouvez mettre en pause à l'aide de ECHAP et quitter le jeu avec SUPPR \n \n" +
                    "As-tu compris?\n" +
                    "-O: Oui \n" +
                    "-N: Non");
                string _controlOk = Console.ReadLine().ToUpper();
                while (_controlOk != "N" & _controlOk != "O") //Boucle de vérification de la valeur entrée 
                {
                    Console.WriteLine("Veuillez sélectionner une des deux options");
                    _controlOk = Console.ReadLine().ToUpper();
                }

            }
            Console.Clear();
            //On demande au jouur d'entrer son nom et de choisir une classe
            Random _choixNom = new Random();
            int _emplacementNom = _choixNom.Next(0, 4);
            _name = _listeNoms[_emplacementNom];
            Console.WriteLine("Mais dis moi, comment t'appelles-tu ?");
            Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine($"Tu t'appelles bien '{_name}' ? \n" +
                "Oui : O \n" +
                "Non : N");
            string _nom = Console.ReadLine().ToUpper();
            while (_nom != "N" & _nom != "O") //Boucle de vérification de la valeur entrée 
            {
                Console.WriteLine("Veuillez sélectionner une des deux options");
                _nom = Console.ReadLine().ToUpper();
                Console.WriteLine();
            }
            Console.WriteLine($"\nCa marche, je t'appelerai donc '{_name}' !\n");
            Console.WriteLine("Quel été ta classe sur Terre ? \n" +
                "1 : Guerrier (classe équilibrée)\n" +
                "2 : Berserk (attaque + 1, défense -1)\n" +
                "3 : Brute (vie +30, défense -1)\n" +
                "4 : Eclaireur (vision +5 cases, vie -25)");
            string classe = Console.ReadLine();
            while (classe != "1" & classe != "2" & classe != "3" & classe != "4") //On vérifie que le choix correspond bien à une classe
            {
                Console.WriteLine("Veuillez entrer un chiffre correspondant à une classe.");
                classe = Console.ReadLine();
            }
            int _classe = int.Parse(classe);
            //On définit les variables de bonus en fonction de la classe sélectionnée
            StatsClasse(_classe);

            //On joue la musique de début de jeu en boucle 
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = "Musiques/titleTheme.wav";
            player.PlayLooping();

            string fontFileName = "FontFile.png"; //On renseigne le nom du fichier contenant la police d'écriture à utiliser
            string consoleTitle = $"'Valhalla - Niveau {_mondeLevel}'"; //On définit le titre de la console avec le nom du jeu suivi du niveau actuel
            int seed = (int)DateTime.UtcNow.Ticks; //On créé la seed du jeu, celle ci correspond au nombre d'intervales de 100 nanosecondes écoulés depuis le 01/01/0001
            Random = new DotNetRandom(seed); //On construit un nombre aléatoire unique à partir de cette seed. Ainsi si à chaque démarrage du jeu on garde la même seed, les maps seront les mêmes

            //On créé la boite de dialogue qui contiendra les messages du jeu (rammasage d'équipement, d'objet, focus d'un monstre, mort d'un monstre)
            MessageLog = new MessageLog();

            //On créé la boite de dialogue à afficher en cas de mort
            End = new End();

            //On créé la boite de dialogue à afficher en cas de pause
            Pause = new Pause();
            Pause.Add("Vous avez mis le jeu en pause,");
            Pause.Add("Pour reprendre, il vous suffit de vous deplacer");

            //On créé le joueur et le système de temps du jeu
            Player = new Player();
            SchedulingSystem = new SchedulingSystem();

            //On génère la map en mettant une salle de 20x20 blocs 
            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 1, 20, 20, _mapLevel, _mondeLevel);
            DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
            MessageLog.Add("Oh");
            MessageLog.Add("Te voila enfin");
            MessageLog.Add("Comme tu as du l'entendre tu dois surmonter");
            MessageLog.Add("certaines epreuves avant de parvenir au Valhalla");
            MessageLog.Add("Tu dois d'abord vaincre l'ours residant dans la foret");
            MessageLog.Add("ensuite tu pourra acceder a la caverne");
            MessageLog.Add("Au fond de cette caverne tu devra vaincre le golem");
            MessageLog.Add("Une fois ce dernier vaincu tu aura accees au chateau");
            MessageLog.Add("Tu devra alors stopper le Roi maudit");
            MessageLog.Add("C'est ce dernier qui empeche l'acces au Valhalla");
            MessageLog.Add("Une fois vaincu tu pourra enfin parvenir au paradis des vikings");

            //On génère les différentes consoles sur l'écran 
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 16, 16, 1.2f, consoleTitle); //On indique le fichier de police à utiliser à la fenêtre principale ainsi que le titre
            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);
            _endgame = new RLConsole(_screenWidth, _screenHeight);
            _pauseConsole = new RLConsole(_mapWidth, _mapHeight);

            //On créé le système de commande qui permet de se battre, de se déplacer ou encore de ramasser quelque chose
            CommandSystem = new CommandSystem();

            //On créé le système qui permet à un ennemi de nous cibler et de nous poursuivre
            TargetingSystem = new TargetingSystem();

            //On donne au joueur deux objets au début, deux cartes 
            Player.Item1 = new RevealMapScroll();
            Player.Item2 = new RevealMapScroll();
            Player.Item3 = new DestructionWand();

            //On met en place un gestionnaire pour l'update et le render de la console
            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;

            //On lance la console
            _rootConsole.Run();
        }

        //Passe l'état du joueur à mort et affiche le message de fin
        public static void EndGame()
        {
            _alive = false;
            if (_winner == true)
            {
                End.Add("Vous avez reussi a gagner votre place au valhalla");
                End.Add($"En continuant vous etes parvenu jusqu'au niveau '{_mondeLevel}','{_mapLevel}' ");
                End.Add("Vous avez definitivement prouve votre valeur");
                End.Add($"Felecitations '{_name}'");
            }
            else
            {
                End.Add("Vous avez perdu");
                End.Add($"Vous etes arrive jusqu'au niveau '{_mondeLevel}','{_mapLevel}'");
                End.Add("Malheureusement le Roi vous empeche d'atteindre le Valhalla");
                End.Add("Vous etes contraint d'errer sans but dans les limbes");
            }
            End.Add($"Vous etes monte au niveau '{Player.Lvl}'");
            End.Add("Vous pouvez quitter le jeu en appuyant sur ESCP");
            End.Add("Vous pouvez relancer une partie en appuyant sur O");

        }

        //Réinitialise les informations de jeu, remet le joueur en vie si il était mort. Permet de démarrer une nouvelle partie
        public static void Reset()
        {
            _alive = true;
            _winner = false;
            _renderRequired = true;
            _atkX = 0;
            _defX = 0;
            _awaX = 0;
            _hpX = 0;
            _mondeLevel = 0;
            _mapLevel = 1;
        }

        //On affecte aux bonus de statistiques les valeurs correspondantes à la classe sélectionnées
        public static void StatsClasse(int classe)
        {
            if (classe == 1) //Guerrier
            {
                _symbol = (char)61;
            }
            else if (classe == 2) //Berserk
            {
                _symbol = (char)145;
                _atkX = 1;
                _defX = -1;
            }
            else if (classe == 3) //Brute
            {
                _symbol = (char)158;
                _hpX = 30;
                _defX = -1;
            }
            else if (classe == 4) //Eclaireur
            {
                _symbol = (char)157;
                _hpX = -25;
                _awaX = 5;
            }
        }

        //On met à jour les éléments de jeu
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            if (_alive) //Si le joueur est vivant
            {
                //Le joueur est d'abord considéré comme n'ayant pas joué et on stock la touche sur laquelle il appuie
                bool didPlayerAct = false;
                RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

                if (TargetingSystem.IsPlayerTargeting) //Si le joueur est en train de viser (avec un sort)
                {
                    if (keyPress != null) //Et qu'il appuie sur une touche
                    {
                        _renderRequired = true; //On doit effectuer le rendu de la console
                        TargetingSystem.HandleKey(keyPress.Key); //En fonction de la touche entrée l'action sera différente (on gère ici le déplacement de la zone de visée et sa validation)
                    }
                }
                else if (CommandSystem.IsPlayerTurn) //Si c'est au tour du joueur
                {
                    if (keyPress != null) //Et qu'il a appuyé sur une touche
                    {
                        //On traite le cas des touches directionnelles
                        if (keyPress.Key == RLKey.Up)
                        {
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                        }
                        else if (keyPress.Key == RLKey.Down)
                        {
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                        }
                        else if (keyPress.Key == RLKey.Left)
                        {
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                        }
                        else if (keyPress.Key == RLKey.Right)
                        {
                            didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                        }
                        //On traite le cas du bouton echap correspondant à l'écran de pause
                        else if (keyPress.Key == RLKey.Escape)
                        {
                            _mapConsole.Clear();
                            _messageConsole.Clear();
                            _statConsole.Clear();
                            _inventoryConsole.Clear();
                            Pause.Draw(_pauseConsole);
                            RLConsole.Blit(_pauseConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, _inventoryHeight);
                            _rootConsole.Draw();
                        }
                        //On traite le cas du bouton suppr qui correspond au fait de quitter le jeu
                        else if (keyPress.Key == RLKey.Delete)
                        {
                            _rootConsole.Close();
                        }
                        //On traite le cas du bouton espace qui permet de changer d'étage
                        else if (keyPress.Key == RLKey.Space)
                        {
                            if (DungeonMap.CanMoveDownToNextLevel()) //Si on peut changer d'étage (si on est sur un escalier descendant)
                            {
                                MessageLog = new MessageLog();
                                CommandSystem = new CommandSystem();
                                if (_mondeLevel == 0 | _mondeLevel == 4) //Si on est au monde 0 ou 4
                                {
                                    if (_mondeLevel == 0) //Si on est actuellement au monde 0, on va jouer la musique du monde 1 en passant l'escalier
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Musiques/forest.wav";
                                        player.PlayLooping();
                                    }
                                    else if (_mondeLevel == 4) //Si on est au monde 4, on va jouer la musique du jeu post fin en passant l'escalier 
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Musiques/theme+.wav";
                                        player.PlayLooping();
                                    }
                                    //Dans les deux cas on génère une map identique avec au maximum 20 salles et des salles pouvant faire une taille comprise entre 7 et 13.
                                    MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, _mapLevel, ++_mondeLevel);
                                    DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                    _rootConsole.Title = $"Valhalla - Niveau {_mondeLevel}.{_mapLevel}"; //On change le titre de la fenêtre
                                    didPlayerAct = true;
                                }
                                else if (_mapLevel == 4) //Le lvl 4 correspond à l'étage juste avant les salles de marchand et boss donc en fonction du monde on définie une musique de boss différente
                                {
                                    MessageLog.Add("Bienvenue mon brave");
                                    MessageLog.Add("Tu as accompli un bon nombre d'epreuves pour arriver jusqu'a moi");
                                    MessageLog.Add($"Ici tu peux m'acheter des objets utiles pour ta quete a '{100 * _mondeLevel}' pieces d'or");
                                    MessageLog.Add("Bon courage pour atteindre le Valhalla");
                                    if (_mondeLevel == 1)
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Musiques/boss1.wav";
                                        player.PlayLooping();
                                    }
                                    else if (_mondeLevel == 2)
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Musiques/boss2.wav";
                                        player.PlayLooping();
                                    }
                                    else if (_mondeLevel == 3)
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Musiques/jojo.wav";
                                        player.PlayLooping();
                                    }
                                    else
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Musiques/boss+.wav";
                                        player.PlayLooping();
                                    }
                                    //Dans tous les cas on génère une map avec une salle pour le marchand de taille 15x15
                                    MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 1, 15, 15, ++_mapLevel, _mondeLevel);
                                    DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                    _rootConsole.Title = $"Valhalla - Niveau {_mondeLevel}.{_mapLevel}";
                                    didPlayerAct = true;
                                }
                                //Pour le boss on génère deux salles de taille 10x10
                                else if (_mapLevel == 5)
                                {
                                    MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 2, 10, 10, ++_mapLevel, _mondeLevel);
                                    DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                    _rootConsole.Title = $"Valhalla - Niveau {_mondeLevel}.{_mapLevel}";
                                    didPlayerAct = true;
                                }
                                else if (_mapLevel == 6) //Si on est à l'étage du boss, en passant l'escalier on change de monde
                                {
                                    if (_mondeLevel != 3) //Le monde 3 étant le dernier monde du jeu, la fin est différente, traitons d'abord les cas normaux
                                    {
                                        //En fonction du monde dans lequel on se trouve, la musique est différente
                                        if (_mondeLevel == 1)
                                        {
                                            SoundPlayer player = new SoundPlayer();
                                            player.SoundLocation = "Musiques/cave.wav";
                                            player.PlayLooping();
                                        }
                                        else if (_mondeLevel == 2)
                                        {
                                            SoundPlayer player = new SoundPlayer();
                                            player.SoundLocation = "Musiques/castle.wav";
                                            player.PlayLooping();
                                        }
                                        else //On entre dans ce else si on est dans un monde supérieur à 4 (post fin)
                                        {
                                            SoundPlayer player = new SoundPlayer();
                                            player.SoundLocation = "Musiques/theme+.wav";
                                            player.PlayLooping();
                                        }
                                        _mapLevel = 1;
                                        MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, _mapLevel, ++_mondeLevel);
                                        DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                        _rootConsole.Title = $"Valhalla - Niveau {_mondeLevel}.{_mapLevel}";
                                        didPlayerAct = true;
                                    }
                                    else //Lorsque l'on passe au monde 4, on génère une map assez semblable à celle de début
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Musiques/titleTheme.wav";
                                        player.PlayLooping();
                                        _mapLevel = 1;
                                        MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 1, 20, 20, _mapLevel, ++_mondeLevel);
                                        DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                        _rootConsole.Title = $"Valhalla - Fin";
                                        MessageLog.Add("Felicitation tu as termine ta quete");
                                        MessageLog.Add("Merci d'avoir mis fin a la colere du roi maudit");
                                        MessageLog.Add("Tu as bien merite ta place au valhalla");
                                        MessageLog.Add("Mais avant cela prends donc cet escalier afin de repousser tes limites");
                                        MessageLog.Add("En effet dans les salles suivantes se trouveront tous les monstres");
                                        MessageLog.Add("que tu as affonte");
                                        MessageLog.Add("Le but est de repousser tes limites pour devenir");
                                        MessageLog.Add("le plus grand hero bon courage");
                                        _winner = true;
                                        didPlayerAct = true;
                                    }

                                }
                                else if (_mapLevel != 6 && _mondeLevel != 0) //Si on ne change pas de monde on recréé juste un étage en passant l'escalier
                                {
                                    MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, ++_mapLevel, _mondeLevel);
                                    DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                    _rootConsole.Title = $"Valhalla - Niveau {_mondeLevel}.{_mapLevel}";
                                    didPlayerAct = true;
                                }

                            }
                        }
                        else //Si le joueur appuie sur une autre touche
                        {
                            didPlayerAct = CommandSystem.HandleKey(keyPress.Key); //Si la touche sur laquelle il a appuyé a entrainée une action (sort, objet) on considère que le joueur a joué
                        }

                        if (didPlayerAct) //Si le joueur a agi on doit mettre à jour la console et terminer le tour
                        {
                            _renderRequired = true;
                            CommandSystem.EndPlayerTurn();
                        }
                    }
                }
                else //Si ce n'est pas le tour du joueur, c'est le tour des monstres
                {
                    CommandSystem.ActivateMonsters();
                    _renderRequired = true;
                }
            }
        }

        //On effectue le rendu de la console
        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            if (_alive) //Si le joueur est en vie 
            {
                if (_renderRequired) //Et qu'il faut mettre à jour la console
                {
                    //On efface ce qui est sur les consoles
                    _mapConsole.Clear();
                    _messageConsole.Clear();
                    _statConsole.Clear();
                    _inventoryConsole.Clear();

                    //Puis on les re dessine
                    DungeonMap.Draw(_mapConsole, _statConsole, _inventoryConsole, _mondeLevel);
                    MessageLog.Draw(_messageConsole);
                    TargetingSystem.Draw(_mapConsole);
                    RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, _inventoryHeight);
                    RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight, _rootConsole, _mapWidth, 0);
                    RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight, _rootConsole, 0, _screenHeight - _messageHeight);
                    RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight, _rootConsole, 0, 0);
                    _rootConsole.Draw();

                    _renderRequired = false;
                }
            }
            else //Si le joueur est mort
            {
                //On efface les consoles
                _mapConsole.Clear();
                _messageConsole.Clear();
                _statConsole.Clear();
                _inventoryConsole.Clear();
                RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress(); //On récupère la touche sur laquelle le joueur appuie
                End.Draw(_endgame); //On dessine l'écran de fin
                RLConsole.Blit(_endgame, 0, 0, _screenWidth, _screenHeight, _rootConsole, 0, 0);
                _rootConsole.Draw();
                if (keyPress != null) //Si il a appuyé sur une touche
                {
                    if (keyPress.Key == RLKey.O) //Et qu'il veut rejoueur
                    {
                        ActorGenerator.ResetPlayer(); //On remet le joueur à zero on ferme cette console et on relance la partie
                        _rootConsole.Close();
                        Main();
                    }
                    else if (keyPress.Key == RLKey.Escape) //Si il ne veut pas rejouer on quitte 
                    {
                        _rootConsole.Close();
                    }
                }

            }
        }
    }
}
