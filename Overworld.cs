using EntityNS;
using CombatHandlingNS;
using System.Windows.Forms;

namespace OverworldNS
{
    public class Overworld
    {
        private readonly Form form;
        private int progress;
        private Player player;
        private CombatHandling? combatHandling;
        private TreasureRoom? treasureRoom;
        private TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();

        public Overworld(Form form)
        {
            this.form = form;
            progress = 0;
            player = new Player();
            combatHandling = null;
        }

        private void GameplayLoop()
        {
            GenerateRoom();
        }

        private void GenerateRoom()
        {
            progress += 1;
            Random random = new Random();
            int rnd = random.Next(0, 2);

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
                Button treasure_btn = new Button
                {
                    Name = "treasure_" + treasureRoom.Treasures[i].id,
                    Text = "Tesoro",
                    Location = new Point(50 + 100 * i, 100),
                    Size = new Size(50, 50),
                };
                treasure_btn.Click += new EventHandler(HandleClick);

                form.Controls.Add(treasure_btn);
            }
        }

        private void CombatRoom()
        {
            combatHandling = new CombatHandling(player, progress);

            // setup interface

            // one time set
            tableLayoutPanel.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.RowCount = 2;

            float collpcent = 100.0f / 2;
            float rowpcent = 100.0f / 2;

            tableLayoutPanel.Size = new Size(960 - 100, 544 / 2 - 50);
            tableLayoutPanel.Location = new Point(50, 544 / 2);

            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, collpcent));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, collpcent));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, rowpcent));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, rowpcent));

            tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetDouble;

            tableLayoutPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;

            form.Controls.Add(tableLayoutPanel);

            Button specialAttackBtn = new Button
            {
                Name = "special_attack_btn",
                Text = "SpAtk",
                Dock = DockStyle.Fill,
            };
            specialAttackBtn.Click += new EventHandler(HandleClick);
            tableLayoutPanel.Controls.Add(specialAttackBtn, 0, 0);

            Button attackBtn = new Button
            {
                Name = "attack_btn",
                Text = "Atk",
                Dock = DockStyle.Fill,
            };
            attackBtn.Click += new EventHandler(HandleClick);
            tableLayoutPanel.Controls.Add(attackBtn, 1, 0);

            Button defenceBtn = new Button
            {
                Name = "defence_btn",
                Text = "Def",
                Dock = DockStyle.Fill,
            };
            defenceBtn.Click += new EventHandler(HandleClick);
            tableLayoutPanel.Controls.Add(defenceBtn, 0, 1);

            // need to refresh
            Button healBtn = new Button
            {
                Name = "heal_btn",
                Text = "Heal",
                Dock = DockStyle.Fill,
            };
            healBtn.Click += new EventHandler(HandleClick);
            tableLayoutPanel.Controls.Add(healBtn, 1, 1);

            Label playerHealth = new Label
            {
                Name = "player_health",
                Location = new Point(10, 10),
                AutoSize = true,
            };
            form.Controls.Add(playerHealth);

            Label enemyHealth = new Label
            {
                Name = "enemy_health",
                Location = new Point(200, 10),
                AutoSize = true,
            };
            form.Controls.Add(enemyHealth);

            Label enemiesRemaining = new Label
            {
                Name = "number_of_enemies",
                Location = new Point(200, 40),
                AutoSize = true,
            };
            form.Controls.Add(enemiesRemaining);

            RefreshInterface();
        }

        private void RefreshInterface()
        {
            Button healBtn = tableLayoutPanel.Controls.Find("heal_btn", true).FirstOrDefault() as Button ?? throw new Exception("heal_btn button not found");
            if (player.heals == 0)
            {
                healBtn.Enabled = false;
            }
            else
            {
                healBtn.Enabled = true;
            }

            Label playerHealth = form.Controls.Find("player_health", true).FirstOrDefault() as Label ?? throw new Exception("player_health label not found");
            playerHealth.Text = $"Health: {player.Health}";

            Label enemyHealth = form.Controls.Find("enemy_health", true).FirstOrDefault() as Label ?? throw new Exception("enemy_health label not found");
            try
            {
                enemyHealth.Text = $"Health: {combatHandling.enemies[0].Health}";
            }
            catch (Exception)
            {
                enemyHealth.Text = "Health: 0";
            }

            Label enemiesRemaining = form.Controls.Find("number_of_enemies", true).FirstOrDefault() as Label ?? throw new Exception("number_of_enemies label not found");
            enemiesRemaining.Text = $"Enemies remaining: {combatHandling.enemies.Count()}";
        }

        private void RemoveAll()
        {
            // https://stackoverflow.com/questions/8466343/why-controls-do-not-want-to-get-removed
            while (form.Controls.Count > 0)
            {
                form.Controls[0].Dispose();
            }
        }

        private void EndGame()
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
                Text = "Quit",
                Size = new Size(100, 100),
                Location = new Point(500, 200)
            };
            quit.Click += new EventHandler((object? sender, EventArgs e) => form.Close());
            form.Controls.Add(quit);
        }

        private void StartGame(object? sender, EventArgs e)
        {
            player = new Player();
            progress = 0;
            RemoveAll();
            GameplayLoop();
        }

        private void HandleClick(object? sender, EventArgs e)
        {
            int action = 0;

            if (sender is Button)
            {
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
                else if (button.Name.Contains("treasure"))
                {
                    Treasure treasure = treasureRoom.Treasures.Find(x => x.id == Convert.ToInt32(button.Name.Split("_")[1]));
                    form.Controls.Remove(button);
                    Action<Player>? post_open_effect = treasure.Open();
                    if (post_open_effect != null)
                    {
                        Label item = new Label
                        {
                            Name = "item_lbl",
                            Text = $"Hai trovato un oggetto che ti aumenta una statistica!",
                            Location = new Point(200, 200),
                            AutoSize = true,
                        };
                        form.Controls.Add(item);
                        post_open_effect(player);
                    }
                    else
                    {
                        // null = nemico
                        RemoveAll();
                        CombatRoom();
                    }

                    goto SkipToEnd;
                }

                int gameState = combatHandling.Turn(action);
                RefreshInterface();
                switch (gameState)
                {
                    case 0:
                        break;
                    case 1:
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
                        break;
                    case 2:
                        RemoveAll();
                        EndGame();
                        break;
                    default:
                        throw new Exception("Invalid gameState");
                }
            }
            SkipToEnd:;
        }
    }
}