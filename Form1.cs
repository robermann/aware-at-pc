using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        const int MAX_TICK = 1000 * 60 * 5; // 5 minutes
        const int MIN_TICK = 1000 * 30;     //30 seconds
        const int SHOW_TIME = 2000;         //2 seconds

        const int TEST_TICK = 2000;

        DateTime lastTick = DateTime.Now;

        private int itemsIndex = 0;
        private Item[] items =
        {
            new Item("h..d", Color.Cyan),
            new Item("h..s", Color.Red),
            new Item("l..s", Color.Yellow),
            new Item("b..t", Color.Orange),
            new Item("f..t", Color.LightGreen)
        };

        public Form1()
        {
            InitializeComponent();

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
            trayIcon.MouseMove += new MouseEventHandler(this.NotifyIcon1_MouseMove);
            #endregion
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            BringToFront();

            itemsIndex = (int)randomColor.Next(0, items.Length);
            
            //". . ."
            tooltip.Show(items[itemsIndex].getText(), this, randomPosition.Next(0, Width), randomPosition.Next(0, Height), SHOW_TIME);

            timer1.Interval = randomTick.Next(MIN_TICK, MAX_TICK);
            lastTick = DateTime.Now;

        }

        private void NotifyIcon1_MouseMove(Object sender, MouseEventArgs e)
        {
            int remainingMilliseconds = (int)(lastTick.AddMilliseconds(timer1.Interval).Subtract(DateTime.Now).TotalMilliseconds);

            TimeSpan t = TimeSpan.FromMilliseconds(remainingMilliseconds);
            string text = string.Format("{0:D2}m:{1:D2}s", 
                t.Minutes,
                t.Seconds);

            trayIcon.Text = text + " left";
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
