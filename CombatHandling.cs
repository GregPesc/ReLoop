using EntityNS;

namespace CombatHandlingNS
{
    class CombatHandling
    {
        public List<Enemy> enemies = new List<Enemy>();
        private Player player;
        private int progress; // numero di porte aperte
        private int playerDefenceMultiplier = 1;
        private int enemyDefenceMultiplier = 1;

        public CombatHandling(Player player, int progress)
        {
            this.player = player;
            this.progress = progress;
        }

        private void GenerateEnemies()
        {
            Random random = new Random();
            int numberOfEnemies = random.Next(1, 4);

            // TODO: statistiche nemici esponenziali
            for (int i = 0; i < numberOfEnemies; i++)
            {
                int maxHealth = random.Next(10, 25) * this.progress;
                int health = maxHealth;
                int attack = random.Next(5, 15) * this.progress;
                int defence = random.Next(5, 15) * this.progress;
                int exp = random.Next(5, 20);

                enemies.Add(new Enemy(maxHealth, health, attack, defence, exp));
            }
        }

        private void handleAttack(Entity attaccker, Entity defender)
        {
            bool alive = defender.DecreaseHealthBy(attaccker.attack - defender.defence * defender.defenceMultiplier);
            
            attaccker.defenceMultiplier = 1;
            defender.defenceMultiplier = 1;

            return alive;
        }

        // return = 0 -> continua; 1 -> fine stanza; 2 -> morto giocatore
        // actions = 1 -> attacco; 2 -> difesa
        public int Turn(int playerAction)
        {
            Enemy currentEnemy = this.enemies[0];
            Random random = new Random();
            int enemyAction = random.Next(1, 3);
            bool alive = true;

            if (playerAction == 1)
            {
                alive = handleAttack(player, currentEnemy);
            }
            else if (playerAction == 2)
            {
                player.defenceMultiplier = 2;
            }

            // player dead
            if (!alive)
            {
                return 2;
            }

            if (enemyAction == 1)
            {
                alive = handleAttack(currentEnemy, player);
            }
            else if (enemyAction == 2)
            {
                enemyDefenceMultiplier = 2;
            }

            // enemy dead
            if (!alive)
            {
                this.player.AddExp(currentEnemy.Exp);
                this.enemies.Remove(currentEnemy);
            }

            // all enemies dead
            if (enemies.Count == 0)
            {
                return 1;
            }

            // player alive and enemies remaining
            return 0;
        }
    }
}