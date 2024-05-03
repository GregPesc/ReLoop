namespace EntityNS
{
    public class Entity
    {
        protected int maxHealth;
        public int MaxHealth
        {
            get { return maxHealth; }
        }

        protected int health;
        public int Health
        {
            get { return health; }
        }
        public bool DecreaseHealthBy(int amount)
        {
            // return: vivo = true, morto = false
            if (amount > 0)
            {
                health -= amount;
            }

            if (health <= 0)
            {
                return false;
            }
            return true;
        }

        public void IncreaseHealthBy(int amount)
        {
            if (amount > 0)
            {
                health += amount;

                if (health > maxHealth)
                {
                    health = maxHealth;
                }
            }
        }

        public int attack;
        public int defence;
        public int defenceMultiplier = 1;
        protected int exp;
        public int Exp
        {
            get { return exp; }
        }

        public Entity(int maxHealth, int health, int attack, int defence, int exp)
        {
            this.maxHealth = maxHealth;
            this.health = health;
            this.attack = attack;
            this.defence = defence;
            this.exp = exp;
        }
    }

    public class Player : Entity
    {
        public int level;
        public int keys;
        public int heals;

        public Player(int maxHealth = 100, int health = 100, int attack = 10, int defence = 10, int exp = 0) : base(maxHealth, health, attack, defence, exp)
        {
            this.maxHealth = maxHealth;
            this.health = health;
            this.attack = attack;
            this.defence = defence;
            this.exp = exp;

            level = 1;
            keys = 0;
            heals = 0;
        }

        public void IncreaseExpBy(int amount)
        {
            exp += amount;
            if (exp >= 100)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            maxHealth += 10;
            health = maxHealth;
            attack += 5;
            defence += 5;
            level += 1;
            exp -= 100;

            if (exp >= 100)
            {
                LevelUp();
            }
        }
    }

    public class Enemy : Entity
    {
        public Enemy(int maxHealth, int health, int attack, int defence, int exp) : base(maxHealth, health, attack, defence, exp)
        {
            this.maxHealth = maxHealth;
            this.health = health;
            this.attack = attack;
            this.defence = defence;
            this.exp = exp;
        }
    }
}