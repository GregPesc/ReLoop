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
        // vivo = true, morto = false
        public bool DecreaseHealthBy(int amount)
        {
            if (amount > 0)
            {
                this.health -= amount;
            }

            if (this.health <= 0)
            {
                return false;
            }
            return true;
        }
           
        // TODO: fix this
        public void IncreaseHealthBy(int amount)
        {
            if (amount > 0)
            {
                if (this.health + amount > this.maxHealth)
                {
                    this.health += this.maxHealth;
                }
                else
                {
                    this.health += amount;
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

        public Player(int maxHealth = 100, int health = 100, int attack = 10, int defence = 10, int exp = 0) : base(maxHealth, health, attack, defence, exp)
        {
            this.maxHealth = maxHealth;
            this.health = health;
            this.attack = attack;
            this.defence = defence;
            this.exp = exp;

            this.level = 1;
            this.keys = 0;
        }

        public void AddExp(int expGained)
        {
            this.exp += expGained;
            if (this.exp >= 100)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            this.maxHealth += 10;
            this.health = maxHealth;
            this.attack += 5;
            this.defence += 5;
            this.level += 1;
            this.exp -= 100;
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