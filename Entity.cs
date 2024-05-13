namespace EntityNS
{
    public class TreasureRoom
    {
        public List<Treasure> Treasures = new List<Treasure>();

        public TreasureRoom()
        {
            GenerateTreasures();
        }

        private void GenerateTreasures()
        {
            Random random = new Random();
            int numberOfTreasures = random.Next(1, 4);

            for (int i = 0; i < numberOfTreasures; i++)
            {
                Treasures.Add(new Treasure(random.Next(0, 3), i));
            }
        }
    }

    public class Treasure
    {
        public int id;
        // 0 = niente; 1 = buff/oggetto; 2 = nemico
        public int type;
        public string? name = null;
        public string? stat = null;
        public int amount = 0;
        public Action<Player>? action = null;

        public Treasure(int type, int id)
        {
            this.type = type;
            action = GenerateContents();
            this.id = id;
        }

        private Action<Player>? GenerateContents()
        {
            if (type == 0)
            {
                name = "Niente";
                return (player) => { };
            }
            else if (type == 1)
            {
                name = "Un buff";
                Random random = new Random();
                int stat = random.Next(0, 5);
                amount = random.Next(2, 5);
                return stat switch
                {
                    0 => (player) => { player.maxHealth += amount * 5; }

                    ,
                    1 => (player) => { player.attack += amount * 2; }

                    ,
                    2 => (player) => { player.defence += amount * 2; }

                    ,
                    3 => (player) => { player.IncreaseExpBy(amount * 8); }

                    ,
                    4 => (player) => { player.heals += 1; }

                    ,
                    _ => throw new NotImplementedException("treasure stat not valid"),
                };
            }
            else if (type == 2)
            {
                return null;
            }
            throw new NotImplementedException("treasure type not valid");
        }

        public Action<Player>? Open()
        {
            return action;
        }
    }

    public class Entity
    {
        public int maxHealth;
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