using OverworldNS;

namespace ReLoop
{
    public partial class Form1 : Form
    {
        Overworld overworld;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            overworld = new Overworld(this);
            overworld.StartMenu();
        }
    }
}