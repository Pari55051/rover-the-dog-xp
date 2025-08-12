using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;


class RoverApp : Form
{
    PictureBox roverBox;
    Random rand = new Random();
    Timer idleTimer;
    Timer surpriseTimer;
    bool isMouseDown;
    Point mouseOffset;

    int idleMin = 5000, idleMax = 8000;
    int surpriseMin = 20000, surpriseMax = 400000;

    string[] idleGifs = {
        "idle.gif",
        "idle-sleep.gif",
        "sniffing.gif",
        "waiting.gif",
        "paw-dabbing.gif"
    };

    string[] surpriseGifs = {
        "digging.gif",
        "digging-success.gif",
        "drops-bag.gif",
        "drops-guest-bag.gif",
        "jumps-away.gif"
    };

    public RoverApp()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.TopMost = true;
        this.BackColor = Color.Lime;
        this.TransparencyKey = Color.Lime;
        this.StartPosition = FormStartPosition.CenterScreen;

        roverBox = new PictureBox();
        roverBox.BackColor = Color.Lime;
        roverBox.SizeMode = PictureBoxSizeMode.AutoSize;
        roverBox.Image = LoadGifFromResources("idle.gif");
        roverBox.MouseDown += RoverBox_MouseDown;
        roverBox.MouseMove += RoverBox_MouseMove;
        roverBox.MouseUp += RoverBox_MouseUp;
        roverBox.ContextMenuStrip = BuildContextMenu();
        this.Controls.Add(roverBox);

        idleTimer = new Timer();
        idleTimer.Interval = rand.Next(idleMin, idleMax);
        idleTimer.Tick += OnIdleTimerTick;
        idleTimer.Start();

        surpriseTimer = new Timer();
        surpriseTimer.Interval = rand.Next(surpriseMin, surpriseMax);
        surpriseTimer.Tick += OnSurpriseTimerTick;
        surpriseTimer.Start();
    }

    private void OnIdleTimerTick(object sender, EventArgs e)
    {
        PlayRandomIdle();
        idleTimer.Interval = rand.Next(5000, 8000);
    }

    private void OnSurpriseTimerTick(object sender, EventArgs e)
    {
        PlayRandomSurprise();
        surpriseTimer.Interval = rand.Next(20000, 40000);
    }



    private void RoverBox_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isMouseDown = true;
            mouseOffset = new Point(-e.X, -e.Y);
        }
    }

    private void RoverBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (isMouseDown)
        {
            Point mousePos = Control.MousePosition;
            mousePos.Offset(mouseOffset.X, mouseOffset.Y);
            this.Location = mousePos;
        }
    }

    private void RoverBox_MouseUp(object sender, MouseEventArgs e)
    {
        isMouseDown = false;
    }



    private ContextMenuStrip BuildContextMenu()
    {
        var menu = new ContextMenuStrip();

        // tricks
        menu.Items.Add("Play Football", null, delegate { PlayAction("rover-football.gif"); });
        menu.Items.Add("Read Newspaper", null, delegate { PlayAction("newspaper.gif"); });
        menu.Items.Add("Dig a Hole", null, delegate { PlayDigAction(); });
        menu.Items.Add("Cook as Masterchef", null, delegate { PlayAction("masterchef.gif"); });
        menu.Items.Add("Pose as a Movie Star", null, delegate { PlayAction("moviestar.gif"); });
        menu.Items.Add("Lick the Screen", null, delegate { PlayAction("licks-screen.gif"); });
        menu.Items.Add("Scratch", null, delegate { PlayAction("scratching-sad.gif"); });


        // settings
        var settingsMenu = new ToolStripMenuItem("Settings");

        var speedMenu = new ToolStripMenuItem("Animation Speed");
        speedMenu.DropDownItems.Add("Slow", null, delegate { SetSpeed("slow"); });
        speedMenu.DropDownItems.Add("Nomarl", null, delegate { SetSpeed("normal"); });
        speedMenu.DropDownItems.Add("Fast", null, delegate { SetSpeed("fast"); });
        settingsMenu.DropDownItems.Add(speedMenu);

        var stayOnTopItem = new ToolStripMenuItem("Stay On Top");
        stayOnTopItem.Checked = this.TopMost;
        stayOnTopItem.Click += delegate
        {
            this.TopMost = !this.TopMost;
            stayOnTopItem.Checked = this.TopMost;
        };
        settingsMenu.DropDownItems.Add(stayOnTopItem);

        settingsMenu.DropDownItems.Add("Exit Rover", null, delegate { Application.Exit(); });

        menu.Items.Add(settingsMenu);

        return menu;
    }


    private void SetSpeed(string mode)
    {
        switch (mode)
        {
            case "slow":
                idleMin = 7000; idleMax = 10000;
                surpriseMin = 30000; surpriseMax = 45000;
                break;
            case "normal":
                idleMin = 5000; idleMax = 8000;
                surpriseMin = 20000; surpriseMax = 30000;
                break;
            case "fast":
                idleMin = 3000; idleMax = 5000;
                surpriseMin = 10000; surpriseMax = 20000;
                break;
        }

        idleTimer.Interval = rand.Next(idleMin, idleMax);
        surpriseTimer.Interval = rand.Next(surpriseMin, surpriseMax);
    }

    // digging with two gifs
    private void PlayDigAction()
    {
        roverBox.Image = LoadGifFromResources("digging.gif");
        var t = new Timer();
        t.Interval = 2000;
        t.Tick += delegate
        {
            t.Stop();
            roverBox.Image = LoadGifFromResources("digging-success.gif");

            var t2 = new Timer();
            t2.Interval = 2000;
            t2.Tick += delegate
            {
                t2.Stop();
                PlayRandomIdle();
            };
            t2.Start();
        };
        t.Start();
    }



    private Image LoadGifFromResources(string resourceName)
    {
        var asm = Assembly.GetExecutingAssembly();
        using (Stream stream = asm.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
                throw new Exception("Resource not found: " + resourceName);
            return Image.FromStream(stream);
        }
    }



    private void PlayRandomIdle()
    {
        string gif = idleGifs[rand.Next(idleGifs.Length)];
        roverBox.Image = LoadGifFromResources(gif);
    }

    private void PlayRandomSurprise()
    {
        string gif = surpriseGifs[rand.Next(surpriseGifs.Length)];
        roverBox.Image = LoadGifFromResources(gif);

        var t = new Timer();
        t.Interval = 3000;
        t.Tick += delegate
        {
            t.Stop();
            PlayRandomIdle();
        };
        t.Start();
    }

    private void PlayAction(string gif)
    {
        roverBox.Image = LoadGifFromResources(gif);
        var t = new Timer();
        t.Interval = 3000;
        t.Tick += delegate
        {
            t.Stop();
            PlayRandomIdle();
        };
        t.Start();
    }




    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new RoverApp());
    }
}
