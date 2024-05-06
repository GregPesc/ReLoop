using EntityNS;

namespace CombatHandlingNS
{
    class CombatHandling
    {
        private readonly int progress;
        private readonly Player player;
        public List<Enemy> enemies = new List<Enemy>();

        public CombatHandling(Player player, int progress)
        {
            this.progress = progress;
            this.player = player;
            GenerateEnemies();
        }

        private void GenerateEnemies()
        {
            Random random = new Random();
            int numberOfEnemies = random.Next(1, 4);

            double SCALING = Math.Log2(progress) + 1;

            for (int i = 0; i < numberOfEnemies; i++)
            {
                int maxHealth = (int)(random.Next(10, 25) * SCALING);
                int health = maxHealth;
                int attack = (int)(random.Next(5, 15) * SCALING);
                int defence = (int)(random.Next(5, 15) * SCALING);
                int exp = (int)(random.Next(5, 20) * SCALING);

                enemies.Add(new Enemy(maxHealth, health, attack, defence, exp));
            }
        }

        private static bool HandleAttack(Entity attaccker, Entity defender, int bonus = 0)
        {
            // formula taken from: https://redd.it/pxhx8d
            double damageCalculation = (attaccker.attack + bonus * 1.5) / ((defender.defence * defender.defenceMultiplier + 100) / 100);
            bool alive = defender.DecreaseHealthBy((int)damageCalculation);

            attaccker.defenceMultiplier = 1;
            defender.defenceMultiplier = 1;

            return alive;
        }

        public int Turn(int playerAction)
        {
            // actions: 0 = niente;     1 = attacco;        2 = difesa;             3 = special attack      4 = heal
            // return:  0 = continua;   1 = fine stanza;    2 = morto giocatore

            Enemy currentEnemy = enemies[0];
            Random random = new Random();
            int enemyAction = random.Next(1, 3);
            bool alive = true;

            switch (playerAction)
            {
                case 0:
                    break;
                case 1:
                    alive = HandleAttack(player, currentEnemy);
                    break;
                case 2:
                    player.defenceMultiplier = 2;
                    break;
                case 3:
                    // 10% of health removed, converted into bonus damage
                    int bonus = (int)Math.Floor((double)player.Health / 10);
                    player.DecreaseHealthBy(bonus);
                    alive = HandleAttack(player, currentEnemy, bonus);
                    break;
                case 4:
                    player.heals -= 1;
                    int heal = player.maxHealth * 42 / 100;
                    player.IncreaseHealthBy(heal);
                    break;
                default:
                    throw new Exception("Invalid action int");
            }

            // enemy dead
            if (!alive)
            {
                player.IncreaseExpBy(currentEnemy.Exp);
                enemies.Remove(currentEnemy);
            }

            alive = true;

            // all enemies dead
            if (enemies.Count == 0)
            {
                return 1;
            }

            switch (enemyAction)
            {
                case 0:
                    break;
                case 1:
                    alive = HandleAttack(currentEnemy, player);
                    break;
                case 2:
                    currentEnemy.defenceMultiplier = 2;
                    break;
                default:
                    throw new Exception("Invalid action int");
            }


            // player dead
            if (!alive)
            {
                return 2;
            }

            // player alive and enemies remaining
            return 0;
        }
    }
}