using EntityNS;
using CombatHandlingNS;

namespace OverworldNS
{
    public class Overworld
    {
        private int progress;
        private readonly Player player;
        private readonly Form form;
        private CombatHandling? combatHandling;

        public Overworld(Form form)
        {
            this.progress = 0;
            this.player = new Player();
            this.form = form;
            this.combatHandling = null;
        }

        public void GameplayLoop()
        {
            GenerateRoom();
        }

        public void GenerateRoom()
        {
            this.progress += 1;
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
            this.combatHandling = new CombatHandling(this.player, this.progress);

            // setup interface

            // one time set
            Button attackBtn = new Button();
            attackBtn.Location = new Point(100, 100);
            attackBtn.Size = new Size(50, 50);
            attackBtn.Click += new EventHandler(HandleClick);
            attackBtn.Name = "attack_btn";
            attackBtn.Text = "Atk";
            this.form.Controls.Add(attackBtn);

            Button defenceBtn = new Button();
            defenceBtn.Location = new Point(200, 100);
            defenceBtn.Size = new Size (50, 50);
            defenceBtn.Click += new EventHandler(HandleClick);
            defenceBtn.Name = "defence_btn";
            defenceBtn.Text = "Def";
            this.form.Controls.Add(defenceBtn);

            // need to refresh
            Label playerHealth = new Label();
            playerHealth.Location = new Point(10, 10);
            playerHealth.AutoSize = true;
            playerHealth.Name = "player_health";
            this.form.Controls.Add(playerHealth);

            Label enemyHealth = new Label();
            enemyHealth.Location = new Point(200, 10);
            enemyHealth.AutoSize = true;
            enemyHealth.Name = "enemy_health";
            this.form.Controls.Add(enemyHealth);

            Label enemiesRemaining = new Label();
            enemiesRemaining.Location = new Point(200, 40);
            enemiesRemaining.AutoSize = true;
            enemiesRemaining.Name = "number_of_enemies";
            this.form.Controls.Add(enemiesRemaining);

            this.RefreshInterface();
        }

        private void DefenceBtn_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void RefreshInterface()
        {
            Label playerHealth = this.form.Controls.Find("player_health", true).FirstOrDefault() as Label;
            playerHealth.Text = $"Health: {player.Health}";

            Label enemyHealth = this.form.Controls.Find("enemy_health", true).FirstOrDefault() as Label;
            try
            {
                enemyHealth.Text = $"Health: {this.combatHandling.enemies[0].Health}";
            }
            catch (Exception)
            {
                enemyHealth.Text = "Health: 0";
            }

            Label enemiesRemaining = this.form.Controls.Find("number_of_enemies", true).FirstOrDefault() as Label;
            enemiesRemaining.Text = $"Enemies remaining: {combatHandling.enemies.Count()}";
        }

        public void RemoveAll()
        {
            // https://stackoverflow.com/questions/8466343/why-controls-do-not-want-to-get-removed
            while (this.form.Controls.Count > 0)
            {
                this.form.Controls[0].Dispose();
            }
        }

        private void EndGame()
        {
            Label label = new Label();
            label.Text = "Sei morto.";
            label.Location = new Point(400, 400);
            label.AutoSize = true;
            this.form.Controls.Add(label);
            label.Update();
            Thread.Sleep(3000);
            this.RemoveAll();
            this.StartMenu();
        }

        public void StartMenu()
        {
            Button start = new Button();
            start.Text = "Start";
            start.Size = new Size(100, 100);
            start.Location = new Point(300, 200);
            start.Click += new EventHandler(StartGame);
            this.form.Controls.Add(start);

            Button quit = new Button();
            quit.Text = "Quit";
            quit.Size = new Size(100, 100);
            quit.Location = new Point(500, 200);
            quit.Click += new EventHandler((object? sender, EventArgs e) => this.form.Close());
            this.form.Controls.Add(quit);
        }

        private void StartGame(object? sender, EventArgs e)
        {
            this.RemoveAll();
            this.GameplayLoop();
        }

        private void HandleClick(object? sender, EventArgs e)
        {
            Button attackBtn = this.form.Controls.Find("attack_btn", true).FirstOrDefault() as Button;
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

                int gameState = this.combatHandling.Turn(action);
                RefreshInterface();
                switch (gameState)
                {
                    case 0:
                        break;
                    case 1:
                        this.RemoveAll();
                        Label win = new Label();
                        win.Location = new Point(200, 300);
                        win.AutoSize = true;
                        win.Text = "Hai vinto il combattimento";
                        this.form.Controls.Add(win);
                        win.Update();
                        Thread.Sleep(1000);
                        this.RemoveAll();
                        this.GameplayLoop();
                        break;
                    case 2:
                        this.RemoveAll();
                        this.EndGame();
                        break;
                    default:
                        throw new Exception("Invalid gameState");
                }
            }
            attackBtn.Enabled = true;
        }
    }
}