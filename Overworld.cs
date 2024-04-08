using EntityNS;
using CombatHandlingNS;

namespace OverworldNS
{
    public class Overworld
    {
        private int progress;
        private Player player;

        public Overworld()
        {
            this.progress = 0;
            this.player = new Player();
        }

        public void GenerateRoom()
        {
            this.progress += 1;
            Random random = new Random();
            int rnd = random.Next(0, 2);

            if (rnd == 0)
            {
                CombatRoom();
            }

            // TODO: altri tipi di stanze
        }

        public void CombatRoom()
        {
            // imposta interfaccia
            // bottoni collegati a Turn ecc.
            CombatHandling combatHandling = new CombatHandling(this.player, this.progress);
        }
    }
}