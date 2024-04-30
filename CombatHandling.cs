using EntityNS;

namespace CombatHandlingNS
{
    class CombatHandling
    {
        private int progress; // numero di porte aperte
        private Player player;
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

            // TODO: statistiche nemici esponenziali
            for (int i = 0; i < numberOfEnemies; i++)
            {
                int maxHealth = random.Next(10, 25) * this.progress;
                int health = maxHealth;
                int attack = random.Next(5, 15) * this.progress;
                int defence = random.Next(5, 15) * this.progress;
                int exp = random.Next(5, 20) * (this.progress + 10) / 10;

                enemies.Add(new Enemy(maxHealth, health, attack, defence, exp));
            }
        }

        private bool handleAttack(Entity attaccker, Entity defender)
        {
            // https://www.reddit.com/r/gamedesign/comments/pxhx8d/what_are_common_damage_formulas_for_games_that/
            double damageCalculation = attaccker.attack / ((defender.defence * defender.defenceMultiplier + 100) / 100);
            bool alive = defender.DecreaseHealthBy((int) damageCalculation);
            
            attaccker.defenceMultiplier = 1;
            defender.defenceMultiplier = 1;

            return alive;
        }

        // return = 0 -> continua; 1 -> fine stanza; 2 -> morto giocatore
        // actions = 0 -> niente; 1 -> attacco; 2 -> difesa
        public int Turn(int playerAction)
        {
            Enemy currentEnemy = this.enemies[0];
            Random random = new Random();
            int enemyAction = random.Next(1, 3);
            bool alive = true;

            switch (playerAction)
            {
                case 0:
                    break;
                case 1:
                    alive = handleAttack(player, currentEnemy);
                    break;
                case 2:
                    player.defenceMultiplier = 2;
                    break;
                default:
                    throw new Exception("Invalid action int");
            }

            // enemy dead
            if (!alive)
            {
                this.player.AddExp(currentEnemy.Exp);
                this.enemies.Remove(currentEnemy);
            }

            alive = true;

            // all enemies dead
            if (enemies.Count() == 0)
            {
                return 1;
            }

            else
            {
                switch (enemyAction)
                {
                    case 0:
                        break;
                    case 1:
                        alive = handleAttack(currentEnemy, player);
                        break;
                    case 2:
                        currentEnemy.defenceMultiplier = 2;
                        break;
                    default:
                        throw new Exception("Invalid action int");
                }
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