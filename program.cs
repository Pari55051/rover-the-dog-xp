using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

class RoverForm : Form
{
    private PictureBox roverBox;
    private Point mouseOffset;
    private bool isMouseDown = false;
    private Timer animationTimer;
    private Random rand = new Random();

    private string[] gifNames = new string[]
    {
        "paw-dabbing.gif",
        "sniffing.gif",
        "rover-football.gif",
        "reading.gif",
        "reading-red.gif",
        "picture-of-him.gif",
        "newspaper.gif",
        "moviestar.gif",
        "masterchef.gif",
        "licks-screen.gif",
        "jumps-away.gif",
        "idle-sleep.gif",
        "idle.gif",
        "waiting.gif",
        "digging-success.gif",
        "digging.gif",
        "drops-bag.gif",
        "drops-guest-bag.gif",
        "excited-play.gif"
    };

    public RoverForm()
    {
        // Form settings
        this.FormBorderStyle = FormBorderStyle.None;
        this.TopMost = true;
        this.BackColor = Color.Lime;
        this.TransparencyKey = Color.Lime;
        this.StartPosition = FormStartPosition.CenterScreen;

        // PictureBox for Rover
        roverBox = new PictureBox();
        roverBox.Image = LoadGifFromResources("idle.gif"); // start idle
        roverBox.BackColor = Color.Lime;
        roverBox.SizeMode = PictureBoxSizeMode.AutoSize;
        this.Controls.Add(roverBox);

        // Drag support
        roverBox.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                mouseOffset = new Point(-e.X, -e.Y);
            }
        };
        roverBox.MouseMove += (s, e) =>
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                this.Location = mousePos;
            }
        };
        roverBox.MouseUp += (s, e) => { isMouseDown = false; };

        // Animation timer (change every 5–10 seconds)
        animationTimer = new Timer();
        animationTimer.Interval = rand.Next(5000, 10000); // random interval
        animationTimer.Tick += (s, e) =>
        {
            string gif = gifNames[rand.Next(gifNames.Length)];
            roverBox.Image = LoadGifFromResources(gif);
            animationTimer.Interval = rand.Next(5000, 10000);
        };
        animationTimer.Start();
    }

    private Image LoadGifFromResources(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
                throw new Exception("Missing resource: " + resourceName);
            return Image.FromStream(stream);
        }
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new RoverForm());
    }
}
