using EntityNS;
using CombatHandlingNS;
using ReLoop.Properties;

namespace OverworldNS
{
    public class Overworld
    {
        private readonly Form form;
        private int progress;
        private Player player;
        private CombatHandling? combatHandling;
        private TreasureRoom? treasureRoom;
        public TableLayoutPanel? actionsLayout = null;
        public TableLayoutPanel? playerStatsLayout = null;
        public TableLayoutPanel? enemyStatsLayout = null;
        public TableLayoutPanel? doorLayout = null;
        private const int doorsCount = 12;
        public int currentScreen = 0;
        private bool[] openedDoors = new bool[doorsCount];
        public bool onDoorSelectionScreen = false;
        private List<int> roomsWithKeys = new List<int>();
        private int lastRoomID = -1;
        private bool isBoss = false;

        public Overworld(Form form)
        {
            this.form = form;
        }

        private void StartGame(object? sender, EventArgs e)
        {
            progress = 0;
            player = new Player();
            currentScreen = 0;
            openedDoors = new bool[doorsCount];
            lastRoomID = -1;
            List<int> rooms = new();
            for (int i = 0; i < doorsCount; i++)
            {
                rooms.Add(i);
            }

            Random rnd = new Random();
            for (int i = 0; i < 3; i++)
            {
                int num = rnd.Next(rooms.Count);
                roomsWithKeys.Add(rooms.ElementAt(num));
                rooms.RemoveAt(num);
            }
            RemoveAll();
            GameplayLoop();
        }


        public void GameplayLoop()
        {
            RemoveAll();
            onDoorSelectionScreen = true;

            if (roomsWithKeys.Contains(lastRoomID))
            {
                Label key = new Label
                {
                    Name = "key_label",
                    Text = "Hai trovato una chiave nell'ultima stanza!",
                    AutoSize = true,
                    Location = new Point(100, 100)
                };
                form.Controls.Add(key);
                player.keys += 1;
                lastRoomID = -1;
            }

            Label keys = new Label
            {
                Name = "keys",
                Text = $"Chiavi: {player.keys}",
                AutoSize = true,
                Location = new Point(800, 50),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
            };
            form.Controls.Add(keys);

            Label instructions = new Label
            {
                Name = "instructions",
                Text = "Usa freccia destra e sinistra per muoverti tra le porte (< >)",
                AutoSize = true,
                Location = new Point(100, 50)
            };
            form.Controls.Add(instructions);

            doorLayout = new TableLayoutPanel
            {
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                ColumnCount = 3,
                RowCount = 1,
                Size = new Size(form.ClientSize.Width - 100, form.ClientSize.Height * 3 / 5),
                Location = new Point(50, form.ClientSize.Height * 1 / 4),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetDouble
            };

            if (currentScreen == 4)
            {
                // porta del boss
                doorLayout.ColumnCount = 1;
                doorLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
                doorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

                PictureBox bossDoor = new PictureBox
                {
                    Name = "door_boss",
                    Dock = DockStyle.Fill,
                    Image = new Bitmap(Resources.porta_boss),
                    SizeMode = PictureBoxSizeMode.Zoom,
                };

                bossDoor.Click += new EventHandler(HandleClick);

                doorLayout.Controls.Add(bossDoor);
            }
            else
            {
                doorLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
                doorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
                doorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
                doorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

                // porte da mostrare:
                // da currentScreen * 3 a currentScreen * 3 + 2
                for (int i = currentScreen * 3; i <= currentScreen * 3 + 2; i++)
                {
                    PictureBox door = new PictureBox
                    {
                        Name = $"door_{i}",
                        Dock = DockStyle.Fill,
                        Image = openedDoors[i] ? new Bitmap(Resources.porta_aperta) : new Bitmap(Resources.porta_chiusa),
                        SizeMode = PictureBoxSizeMode.Zoom,
                    };
                    if (openedDoors[i] == false)
                    {
                        door.Click += new EventHandler(HandleClick);
                    }

                    doorLayout.Controls.Add(door, i % 3, 0);
                }
            }
            form.Controls.Add(doorLayout);
        }

        private void GenerateRoom()
        {
            onDoorSelectionScreen = false;

            RemoveAll();

            progress += 1;
            Random random = new Random();
            int rnd = random.Next(1, 2);

            if (rnd == 0)
            {
                CombatRoom();
            }
            else if (rnd == 1)
            {
                TreasureRoom();
            }
        }

        private void TreasureRoom()
        {
            treasureRoom = new TreasureRoom();
            for (int i = 0; i < treasureRoom.Treasures.Count; i++)
            {
                PictureBox treasure_btn = new PictureBox
                {
                    Name = "treasure_" + treasureRoom.Treasures[i].id,
                    Text = "Tesoro",
                    Location = new Point(150 + 150 * i + 50 * (i - 1), 100),
                    Size = new Size(150, 150),
                    Image = Resources.tesoro,
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                treasure_btn.Click += new EventHandler(HandleClick);

                form.Controls.Add(treasure_btn);
            }
        }

        private void CombatRoom(bool isBoss = false)
        {
            combatHandling = new CombatHandling(player, progress, isBoss);

            // setup interface

            // one time set
            actionsLayout = new TableLayoutPanel();
            actionsLayout.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            actionsLayout.ColumnCount = 2;
            actionsLayout.RowCount = 2;

            actionsLayout.Size = new Size(form.ClientSize.Width - 100, form.ClientSize.Height / 2 - 50);
            actionsLayout.Location = new Point(50, form.ClientSize.Height / 2);

            actionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
            actionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));

            actionsLayout.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;

            form.Controls.Add(actionsLayout);

            Button specialAttackBtn = new Button
            {
                Name = "special_attack_btn",
                Text = "SpAtk",
                Dock = DockStyle.Fill,
            };
            specialAttackBtn.Click += new EventHandler(HandleClick);
            actionsLayout.Controls.Add(specialAttackBtn, 0, 0);

            Button attackBtn = new Button
            {
                Name = "attack_btn",
                Text = "Atk",
                Dock = DockStyle.Fill,
            };
            attackBtn.Click += new EventHandler(HandleClick);
            actionsLayout.Controls.Add(attackBtn, 1, 0);

            Button defenceBtn = new Button
            {
                Name = "defence_btn",
                Text = "Def",
                Dock = DockStyle.Fill,
            };
            defenceBtn.Click += new EventHandler(HandleClick);
            actionsLayout.Controls.Add(defenceBtn, 0, 1);

            // need to refresh
            Button healBtn = new Button
            {
                Name = "heal_btn",
                Text = "Cura",
                Dock = DockStyle.Fill,
            };
            healBtn.Click += new EventHandler(HandleClick);
            actionsLayout.Controls.Add(healBtn, 1, 1);


            playerStatsLayout = new TableLayoutPanel
            {
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                ColumnCount = 2,
                RowCount = 3,
                Size = new Size((int)(form.ClientSize.Width * 0.2), form.ClientSize.Height / 2 - 100),
                Location = new Point(50, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            playerStatsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
            playerStatsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
            playerStatsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));
            playerStatsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));
            playerStatsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));

            form.Controls.Add(playerStatsLayout);

            Label playerLevelText = new Label
            {
                Name = "player_level_text",
                Dock = DockStyle.Fill,
                Text = "Livello:"
            };
            playerStatsLayout.Controls.Add(playerLevelText, 0, 0);

            Label playerLevel = new Label
            {
                Name = "player_level",
                Dock = DockStyle.Fill,
            };
            playerStatsLayout.Controls.Add(playerLevel, 1, 0);

            Label playerHealthText = new Label
            {
                Name = "player_health_text",
                Dock = DockStyle.Fill,
                Text = "Vita:"
            };
            playerStatsLayout.Controls.Add(playerHealthText, 0, 1);

            Label playerHealth = new Label
            {
                Name = "player_health",
                Dock = DockStyle.Fill
            };
            playerStatsLayout.Controls.Add(playerHealth, 1, 1);

            Label playerHealsText = new Label
            {
                Name = "player_heals_text",
                Dock = DockStyle.Fill,
                Text = "Cure:"
            };
            playerStatsLayout.Controls.Add(playerHealsText, 0, 2);

            Label playerHeals = new Label
            {
                Name = "player_heals",
                Dock = DockStyle.Fill,
            };
            playerStatsLayout.Controls.Add(playerHeals, 1, 2);


            enemyStatsLayout = new TableLayoutPanel
            {
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                ColumnCount = 2,
                RowCount = 3,
                Size = new Size((int)(form.ClientSize.Width * 0.2), form.ClientSize.Height / 2 - 100),
                Location = new Point(form.ClientSize.Width - 50 - ((int)(form.ClientSize.Width * 0.2)), 50),
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };

            enemyStatsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
            enemyStatsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
            enemyStatsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));
            enemyStatsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f));

            form.Controls.Add(enemyStatsLayout);

            Label enemyHealthText = new Label
            {
                Name = "enemy_health_text",
                Dock = DockStyle.Fill,
                Text = "Vita:"
            };
            enemyStatsLayout.Controls.Add(enemyHealthText, 0, 0);

            Label enemyHealth = new Label
            {
                Name = "enemy_health",
                Dock = DockStyle.Fill,
            };
            enemyStatsLayout.Controls.Add(enemyHealth, 1, 0);

            Label enemiesRemainingText = new Label
            {
                Name = "number_of_enemies_text",
                Dock = DockStyle.Fill,
                Text = "Nemici rimanenti:"
            };
            enemyStatsLayout.Controls.Add(enemiesRemainingText, 0, 1);

            Label enemiesRemaining = new Label
            {
                Name = "number_of_enemies",
                Dock = DockStyle.Fill
            };
            enemyStatsLayout.Controls.Add(enemiesRemaining, 1, 1);

            RefreshInterface();
        }

        private void RefreshInterface()
        {
            Button healBtn = actionsLayout.Controls.Find("heal_btn", true).FirstOrDefault() as Button ?? throw new Exception("heal_btn button not found");
            if (player.heals == 0)
            {
                healBtn.Enabled = false;
            }
            else
            {
                healBtn.Enabled = true;
            }

            Label playerHealth = playerStatsLayout.Controls.Find("player_health", true).FirstOrDefault() as Label ?? throw new Exception("player_health label not found");
            playerHealth.Text = $"{player.Health}";

            Label playerLevel = playerStatsLayout.Controls.Find("player_level", true).FirstOrDefault() as Label ?? throw new Exception("player_level label not found");
            playerLevel.Text = $"{player.level}";

            Label playerHeals = playerStatsLayout.Controls.Find("player_heals", true).FirstOrDefault() as Label ?? throw new Exception("player_heals label not found");
            playerHeals.Text = $"{player.heals}";

            Label enemyHealth = enemyStatsLayout.Controls.Find("enemy_health", true).FirstOrDefault() as Label ?? throw new Exception("enemy_health label not found");
            try
            {
                enemyHealth.Text = $"{combatHandling.enemies[0].Health}";
            }
            catch (Exception)
            {
                enemyHealth.Text = "Health: 0";
            }

            Label enemiesRemaining = enemyStatsLayout.Controls.Find("number_of_enemies", true).FirstOrDefault() as Label ?? throw new Exception("number_of_enemies label not found");
            enemiesRemaining.Text = $"{combatHandling.enemies.Count()}";
        }

        private void RemoveAll()
        {
            // https://stackoverflow.com/questions/8466343/why-controls-do-not-want-to-get-removed
            while (form.Controls.Count > 0)
            {
                form.Controls[0].Dispose();
            }
        }

        private void WinGame()
        {
            RemoveAll();
            onDoorSelectionScreen = false;
            Label win = new Label
            {
                Name = "win",
                Text = $"Hai sconfitto il boss del dungeon!\nIl tuo punteggio: {doorsCount - progress}",
                AutoSize = true,
                Location = new Point(form.ClientSize.Width / 2 - 100, form.ClientSize.Height / 2 - 50),
            };
            form.Controls.Add(win);
        }

        private void LoseGame()
        {
            Label label = new Label
            {
                Text = "Sei morto.",
                Location = new Point(400, 400),
                AutoSize = true
            };
            form.Controls.Add(label);
            label.Update();
            Thread.Sleep(3000);
            RemoveAll();
            StartMenu();
        }

        public void StartMenu()
        {
            Button start = new Button
            {
                Text = "Start",
                Size = new Size(100, 100),
                Location = new Point(300, 200)
            };
            start.Click += new EventHandler(StartGame);
            form.Controls.Add(start);

            Button quit = new Button
            {
                Text = "Esci",
                Size = new Size(100, 100),
                Location = new Point(500, 200)
            };
            quit.Click += new EventHandler((object? sender, EventArgs e) => form.Close());
            form.Controls.Add(quit);
        }

        private void HandleClick(object? sender, EventArgs e)
        {
            if (sender is PictureBox)
            {
                PictureBox pic = (PictureBox)sender;

                if (pic.Name.Contains("door"))
                {
                    PictureBox door = pic;
                    if (door.Name.Contains("boss"))
                    {
                        if (player.keys >= 3)
                        {
                            // genera la stanza del boss
                            RemoveAll();
                            isBoss = true;
                            CombatRoom(isBoss);
                        }
                        else
                        {
                            // mostra messaggio che ti serve 3 chiavi
                            Label noKeys = new Label
                            {
                                Name = "nokeys",
                                Text = "Ti servono 3 chiavi per affrontare il Boss!",
                                AutoSize = true,
                                Location = new Point(100, 100),
                                Anchor = AnchorStyles.Top | AnchorStyles.Left
                            };
                            form.Controls.Add(noKeys);
                        }
                    }
                    else
                    {
                        int id = Convert.ToInt32(door.Name.Split("_")[1]);
                        openedDoors[id] = true;
                        lastRoomID = id;
                        GenerateRoom();
                    }
                }
                else if (pic.Name.Contains("treasure"))
                {
                    Treasure treasure = treasureRoom.Treasures.Find(x => x.id == Convert.ToInt32(pic.Name.Split("_")[1]));
                    form.Controls.Remove(pic);
                    Action<Player>? post_open_effect = treasure.Open();
                    treasureRoom.Treasures.Remove(treasure);
                    if (post_open_effect != null)
                    {
                        Label item = new Label
                        {
                            Name = "item_lbl",
                            Text = "Hai trovato un oggetto che ti aumenta una statistica!",
                            Location = new Point(200, 300),
                            AutoSize = true,
                        };
                        form.Controls.Add(item);
                        post_open_effect(player);

                        if (treasureRoom.Treasures.Count == 0)
                        {
                            RemoveAll();
                            GameplayLoop();
                        }
                    }
                    else
                    {
                        // null = nemico
                        RemoveAll();
                        CombatRoom();
                    }
                }
            }

            else if (sender is Button)
            {
                int action = 0;

                Button button = (Button)sender;
                if (button.Name == "attack_btn")
                {
                    action = 1;
                }
                else if (button.Name == "defence_btn")
                {
                    action = 2;
                }
                else if (button.Name == "special_attack_btn")
                {
                    action = 3;
                }
                else if (button.Name == "heal_btn")
                {
                    action = 4;
                }

                int gameState = combatHandling.Turn(action);
                RefreshInterface();
                switch (gameState)
                {
                    case 0:
                        break;
                    case 1:
                        if (isBoss)
                        {
                            WinGame();
                        }
                        else
                        {
                            player.IncreaseHealthBy(player.maxHealth);

                            RemoveAll();
                            Label win = new Label
                            {
                                Location = new Point(200, 300),
                                AutoSize = true,
                                Text = "Hai vinto il combattimento"
                            };
                            form.Controls.Add(win);
                            win.Update();
                            Thread.Sleep(1000);
                            RemoveAll();

                            GameplayLoop();
                        }
                        break;
                    case 2:
                        RemoveAll();
                        LoseGame();
                        break;
                    default:
                        throw new Exception("Invalid gameState");
                }
            }
        }
    }
}