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
            this.form.Controls.Add(attackBtn);

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

        private void RemoveAll()
        {
            // https://stackoverflow.com/questions/8466343/why-controls-do-not-want-to-get-removed
            foreach (Control item in form.Controls)
            {
                item.Dispose();
            }
            this.form.Update();
        }

        private void EndGame()
        {
            Label label = new Label();
            label.Text = "Sei morto.";
            label.Location = new Point(400, 400);
            label.AutoSize = true;
            this.form.Controls.Add(label);
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