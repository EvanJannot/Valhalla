using System;
using System.Media;
using RLNET;
using RogueSharp.Random;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Items;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples
{
    public static class Game
    {
        private static int _atkX;
        private static int _defX;
        private static int _awaX;
        private static int _hpX;
        private static string _name = "Rogue";
        private static char _symbol;
        private static readonly int _screenWidth = 80;
        private static readonly int _screenHeight = 48;
        private static readonly int _mapWidth = 60;
        private static readonly int _mapHeight = 28;
        private static readonly int _messageWidth = 80;
        private static readonly int _messageHeight = 10;
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 48;
        private static readonly int _inventoryWidth = 60;
        private static readonly int _inventoryHeight = 10;

        private static RLRootConsole _rootConsole;
        private static RLConsole _mapConsole;
        private static RLConsole _messageConsole;
        private static RLConsole _statConsole;
        private static RLConsole _inventoryConsole;
        private static RLConsole _endgame;
        private static RLConsole _pauseConsole;

        private static int _mondeLevel = 0;
        private static int _mapLevel = 1;
        private static bool _alive = true;
        private static bool _renderRequired = true;

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
            Reset();
            Console.WriteLine("Comment vous applez-vous ?");
            _name = Console.ReadLine();
            Console.WriteLine("Veuillez sélectionner une des classes : \n" +
                "1 : Guerrier \n" +
                "2 : Berserk \n" +
                "3 : Brute \n" +
                "4 : Eclaireur");
            int classe = int.Parse(Console.ReadLine());
            while (classe != 1 & classe != 2 & classe != 3 & classe != 4)
            {
                Console.WriteLine("Veuillez entrer un chiffre correspondant à une classe.");
                classe = int.Parse(Console.ReadLine());
            }
            StatsClasse(classe);

            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = "Music/titleTheme.wav";
            player.PlayLooping();

            string fontFileName = "FontFile.png";
            string consoleTitle = $"'Valhalla - Level {_mondeLevel}'";
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            MessageLog = new MessageLog();

            End = new End();
            End.Add("Vous avez perdu");

            Pause = new Pause();
            Pause.Add("Vous avez mis le jeu en pause,");
            Pause.Add("Pour reprendre, il vous suffit de vous deplacer");

            Player = new Player();
            SchedulingSystem = new SchedulingSystem();

            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 1, 20, 20, _mapLevel, _mondeLevel);
            DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);

            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 16, 16, 1.2f, consoleTitle);
            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);
            _endgame = new RLConsole(_screenWidth, _screenHeight);
            _pauseConsole = new RLConsole(_mapWidth, _mapHeight);

            CommandSystem = new CommandSystem();
            TargetingSystem = new TargetingSystem();

            Player.Item1 = new RevealMapScroll();
            Player.Item2 = new RevealMapScroll();

            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;

            _rootConsole.Run();
        }

        public static void EndGame()
        {
            _alive = false;
            End.Add($"Vous etes arrive jusqu'au niveau '{_mapLevel}'");
            End.Add("Vous pouvez quitter le jeu en appuyant sur ESCP");
            End.Add("Vous pouvez relancer une partie en appuyant sur O");
        }

        public static void Reset()
        {
            _alive = true;
            _renderRequired = true;
            _atkX = 0;
            _defX = 0;
            _awaX = 0;
            _hpX = 0;
            _mondeLevel = 0;
            _mapLevel = 1;
        }

        public static void StatsClasse(int classe)
        {
            if (classe == 1)
            {
                _symbol = (char)61;
            }
            else if (classe == 2)
            {
                _symbol = (char)145;
                _atkX = 1;
                _defX = -1;
            }
            else if (classe == 3)
            {
                _symbol = (char)158;
                _hpX = 30;
                _defX = -1;
            }
            else if (classe == 4)
            {
                _symbol = (char)157;
                _hpX = -25;
                _awaX = 5;
            }
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            if (_alive)
            {
                bool didPlayerAct = false;
                RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

                if (TargetingSystem.IsPlayerTargeting)
                {
                    if (keyPress != null)
                    {
                        _renderRequired = true;
                        TargetingSystem.HandleKey(keyPress.Key);
                    }
                }
                else if (CommandSystem.IsPlayerTurn)
                {
                    if (keyPress != null)
                    {
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
                        else if (keyPress.Key == RLKey.Delete)
                        {
                            _rootConsole.Close();
                        }
                        else if (keyPress.Key == RLKey.Space)
                        {
                            if (DungeonMap.CanMoveDownToNextLevel())
                            {
                                MessageLog = new MessageLog();
                                CommandSystem = new CommandSystem();
                                if (_mondeLevel == 0 | _mondeLevel == 4)
                                {
                                    if (_mondeLevel == 0)
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Music/forest.wav";
                                        player.PlayLooping();
                                    }
                                    else if (_mondeLevel == 4)
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Music/theme+.wav";
                                        player.PlayLooping();
                                    }
                                    MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, _mapLevel, ++_mondeLevel);
                                    DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                    _rootConsole.Title = $"Valhalla - Niveau {_mondeLevel}.{_mapLevel}";
                                    didPlayerAct = true;
                                }
                                else if (_mapLevel == 4)
                                {
                                    if (_mondeLevel == 1)
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Music/boss1.wav";
                                        player.PlayLooping();
                                    }
                                    else if (_mondeLevel == 2)
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Music/boss2.wav";
                                        player.PlayLooping();
                                    }
                                    else if (_mondeLevel == 3)
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Music/jojo.wav";
                                        player.PlayLooping();
                                    }
                                    else
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Music/boss+.wav";
                                        player.PlayLooping();
                                    }
                                    MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 1, 15, 15, ++_mapLevel, _mondeLevel);
                                    DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                    _rootConsole.Title = $"Valhalla - Niveau {_mondeLevel}.{_mapLevel}";
                                    didPlayerAct = true;
                                }
                                else if (_mapLevel == 5)
                                {
                                    MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 2, 10, 10, ++_mapLevel, _mondeLevel);
                                    DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                    _rootConsole.Title = $"Valhalla - Niveau {_mondeLevel}.{_mapLevel}";
                                    didPlayerAct = true;
                                }
                                else if (_mapLevel == 6)
                                {
                                    if (_mondeLevel != 3)
                                    {
                                        if (_mondeLevel == 1)
                                        {
                                            SoundPlayer player = new SoundPlayer();
                                            player.SoundLocation = "Music/cave.wav";
                                            player.PlayLooping();
                                        }
                                        else if (_mondeLevel == 2)
                                        {
                                            SoundPlayer player = new SoundPlayer();
                                            player.SoundLocation = "Music/castle.wav";
                                            player.PlayLooping();
                                        }
                                        else
                                        {
                                            SoundPlayer player = new SoundPlayer();
                                            player.SoundLocation = "Music/theme+.wav";
                                            player.PlayLooping();
                                        }
                                        _mapLevel = 1;
                                        MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, _mapLevel, ++_mondeLevel);
                                        DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                        _rootConsole.Title = $"Valhalla - Niveau {_mondeLevel}.{_mapLevel}";
                                        didPlayerAct = true;
                                    }
                                    else
                                    {
                                        SoundPlayer player = new SoundPlayer();
                                        player.SoundLocation = "Music/titleTheme.wav";
                                        player.PlayLooping();
                                        _mapLevel = 1;
                                        MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 1, 25, 25, _mapLevel, ++_mondeLevel);
                                        DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                        _rootConsole.Title = $"Valhalla - Fin";
                                        didPlayerAct = true;
                                    }

                                }
                                else if (_mapLevel != 6 && _mondeLevel != 0)
                                {
                                    MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, ++_mapLevel, _mondeLevel);
                                    DungeonMap = mapGenerator.CreateMap(_mondeLevel, _mapLevel, _atkX, _defX, _awaX, _hpX, _name, _symbol);
                                    _rootConsole.Title = $"Valhalla - Niveau {_mondeLevel}.{_mapLevel}";
                                    didPlayerAct = true;
                                }

                            }
                        }
                        else
                        {
                            didPlayerAct = CommandSystem.HandleKey(keyPress.Key);
                        }

                        if (didPlayerAct)
                        {
                            _renderRequired = true;
                            CommandSystem.EndPlayerTurn();
                        }
                    }
                }
                else
                {
                    CommandSystem.ActivateMonsters();
                    _renderRequired = true;
                }
            }
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            if (_alive)
            {
                if (_renderRequired)
                {
                    _mapConsole.Clear();
                    _messageConsole.Clear();
                    _statConsole.Clear();
                    _inventoryConsole.Clear();
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
            else
            {
                _mapConsole.Clear();
                _messageConsole.Clear();
                _statConsole.Clear();
                _inventoryConsole.Clear();
                RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
                End.Draw(_endgame);
                RLConsole.Blit(_endgame, 0, 0, _screenWidth, _screenHeight, _rootConsole, 0, 0);
                _rootConsole.Draw();
                if (keyPress != null)
                {
                    if (keyPress.Key == RLKey.O)
                    {
                        ActorGenerator.ResetPlayer();
                        _rootConsole.Close();
                        Main();
                    }
                    else if (keyPress.Key == RLKey.Escape)
                    {
                        _rootConsole.Close();
                    }
                }

            }
        }
    }
}
