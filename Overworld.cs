using EntityNS;
using CombatHandlingNS;

namespace OverworldNS
{
    public class Overworld
    {
        private readonly Form form;
        private int progress;
        private Player player;
        private CombatHandling? combatHandling;

        public Overworld(Form form)
        {
            this.form = form;
            progress = 0;
            player = new Player();
            combatHandling = null;
        }

        public void GameplayLoop()
        {
            GenerateRoom();
        }

        public void GenerateRoom()
        {
            progress += 1;
            Random random = new Random();
            int rnd = random.Next(0, 1);

            if (rnd == 0)
            {
                CombatRoom();
            }

            // TODO: altri tipi di stanze
        }

        public void CombatRoom()
        {
            combatHandling = new CombatHandling(player, progress);

            // setup interface

            // one time set
            Button specialAttackBtn = new Button
            {
                Name = "special_attack_btn",
                Text = "SpAtk",
                Location = new Point(0, 100),
                Size = new Size(50, 50),
            };
            specialAttackBtn.Click += new EventHandler(HandleClick);
            form.Controls.Add(specialAttackBtn);

            Button attackBtn = new Button
            {
                Name = "attack_btn",
                Text = "Atk",
                Location = new Point(100, 100),
                Size = new Size(50, 50),
            };
            attackBtn.Click += new EventHandler(HandleClick);
            form.Controls.Add(attackBtn);

            Button defenceBtn = new Button
            {
                Name = "defence_btn",
                Text = "Def",
                Location = new Point(200, 100),
                Size = new Size(50, 50),
            };
            defenceBtn.Click += new EventHandler(HandleClick);
            form.Controls.Add(defenceBtn);

            // need to refresh
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

        public void RefreshInterface()
        {
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

        public void RemoveAll()
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
            Button attackBtn = form.Controls.Find("attack_btn", true).FirstOrDefault() as Button ?? throw new Exception("attack_btn button not found");
            attackBtn.Enabled = false;

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

                int gameState = combatHandling.Turn(action);
                RefreshInterface();
                switch (gameState)
                {
                    case 0:
                        break;
                    case 1:
                        player.IncreaseHealthBy(player.MaxHealth);

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
            attackBtn.Enabled = true;
        }
    }
}