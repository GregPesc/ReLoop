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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }

            if (overworld.onDoorSelectionScreen)
            {
                if (e.KeyCode == Keys.Right)
                {
                    overworld.currentScreen += 1;
                    if (overworld.currentScreen >= 5)
                    {
                        overworld.currentScreen = 0;
                    }
                }
                if (e.KeyCode == Keys.Left)
                {
                    overworld.currentScreen -= 1;
                    if (overworld.currentScreen < 0)
                    {
                        overworld.currentScreen = 4;
                    }
                }
                overworld.GameplayLoop();
            }
            else if (e.KeyCode == Keys.Space && overworld.onStory)
            {
                overworld.GameplayLoop();
            }
        }
    }
}