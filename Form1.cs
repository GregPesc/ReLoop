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
            this.ClientSize = new Size(960, 544);
            overworld = new Overworld(this);
            overworld.StartMenu();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (overworld != null && overworld.actionsLayout != null)
            {

                overworld.actionsLayout.Size = new Size(this.ClientSize.Width - 100, this.ClientSize.Height / 2 - 50);
                overworld.actionsLayout.Location = new Point(50, this.ClientSize.Height / 2);
            }
        }
    }
}