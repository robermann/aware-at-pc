using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using aware_at_pc.Properties;

namespace WindowsFormsApp2
{
    //da: https://stackoverflow.com/a/2215918/1012022
    //da: https://stackoverflow.com/a/12991068/1012022
    //da: https://stackoverflow.com/a/10250051/1012022

    public partial class Form1 : Form
    {
        private NotifyIcon trayIcon;

        private ToolTip tooltip = new ToolTip();

        private Random randomTick = new Random();
        private Random randomColor = new Random();
        private Random randomPosition = new Random();

        const int MAX_TICK = 1000 * 60 * 2; // 3 minutes
        const int MIN_TICK = 1000 * 30;     //30 seconds
        const int POPUP_TIME = 5000;        // 3 seconds
        const int ICON_ON_TIME = 8000;      // 8 seconds

        readonly int MAX_SCREEN_WIDTH;
        readonly int MAX_SCREEN_HEIGHT;

        const int TEST_TICK = 2000;

        DateTime lastTick = DateTime.Now;

        private int itemsIndex = 0;
        private Item[] items =
        {
            new Item("h..d", Color.Cyan),
            new Item("h..s", Color.Red),
            new Item("l..s", Color.Yellow),
            new Item("b..t", Color.Orange),
            new Item("f..t", Color.LightGreen),
            new Item("b..y", Color.BlueViolet)
        };

        public Form1()
        {
            InitializeComponent();

            MAX_SCREEN_WIDTH = GetScreen().Width - 100;
            MAX_SCREEN_HEIGHT = GetScreen().Height - 50;

            Console.WriteLine("Max screen size = " + MAX_SCREEN_WIDTH + ":" + MAX_SCREEN_HEIGHT);

            #region Personalizzo il tooltip
            tooltip.OwnerDraw = true;
            tooltip.Draw += new DrawToolTipEventHandler(toolTip1_Draw);
            #endregion

            #region Inserisco trayicon per uscire dall'app
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.icon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
            trayIcon.Icon = Resources.leaf_green;
            #endregion
        }

        public Rectangle GetScreen()
        {
            return Screen.FromControl(this).Bounds;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            BringToFront();

            SystemSounds.Hand.Play();

            itemsIndex = (int)randomColor.Next(0, items.Length);

            trayIcon.Icon = Resources.leaf_green;
            timer2.Interval = ICON_ON_TIME;
            timer2.Start();

            int randomX = randomPosition.Next(10, MAX_SCREEN_WIDTH);
            int randomY = randomPosition.Next(10, MAX_SCREEN_HEIGHT);
            String text = items[itemsIndex].getText();

            //randomX = 242; 
            //randomY = 612;

            Console.WriteLine(DateTime.Now + " - \"" + text + "\" - X:Y = " +randomX+":"+randomY);

            tooltip.Show(text, this, randomX, randomY, POPUP_TIME);

            timer1.Interval = randomTick.Next(MIN_TICK, MAX_TICK);

            lastTick = DateTime.Now;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            trayIcon.Icon = Resources.leaf_grey;
            timer2.Stop();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            
            int remainingMilliseconds = (int)(lastTick.AddMilliseconds(timer1.Interval).Subtract(DateTime.Now).TotalMilliseconds);

            TimeSpan t = TimeSpan.FromMilliseconds(remainingMilliseconds);
            string text = string.Format("{0:D2}m:{1:D2}s",
                t.Minutes,
                t.Seconds);

            trayIcon.Visible = false;
            trayIcon.Text = text + " left";
            trayIcon.Visible = true;
            
        }

        void toolTip1_Draw(object sender, DrawToolTipEventArgs e)
        {
            Font f = new Font("Arial", 10.0f);
            
            tooltip.BackColor = items[itemsIndex].getColor();
            e.DrawBackground();
            e.DrawBorder();
            e.Graphics.DrawString(e.ToolTipText, f, Brushes.Black, new PointF(2, 2));
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Application.Exit();
        }
    }

    class Item
    {
        private String txt;
        private Color color;

        public Item(String txt, Color c)
        {
            this.txt = txt;
            this.color = c;
        }

        public String getText()
        {
            return txt;
        }

        public Color getColor()
        {
            return color;
        }
    }
}
