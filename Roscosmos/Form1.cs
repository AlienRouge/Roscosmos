using System;
using System.Windows.Forms;

namespace Roscosmos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private double x;
        private double y;
        private double t;
        private const double dt = 0.01;

        private void launchButton_Click(object sender, EventArgs e)
        {
            pauseButton.BackgroundImage = Properties.Resources.circled_pause;
            pauseButton.Visible = true;

            Body.Y0 = (double) edHeight.Value;
            Body.V0 = (double) edSpeed.Value;
            Body.Angle = (double) edAngle.Value;

            x = 0;
            y = Body.Y0;
            t = 0;

            Body.FlightTime = (Body.V0 * Math.Sin(Body.Angle * Math.PI / 180) +
                          Math.Sqrt(Math.Pow(Body.V0 * Math.Sin(Body.Angle * Math.PI / 180), 2) +
                                    2 * Body.g * Body.Y0)) / Body.g;

            Body.MaxHeight = Body.Y0 + Math.Pow(Body.V0 * Math.Sin(Body.Angle * Math.PI / 180), 2) / (2 * Body.g);
            Body.MaxDistance = Body.V0 * Math.Cos(Body.Angle * Math.PI / 180) * Body.FlightTime;

            chart1.Series[0].Points.Clear();
            chart1.ChartAreas[0].AxisX.Maximum = Math.Round(Body.MaxDistance, 3) + Math.Round(Body.MaxDistance * 0.2, 3);
            chart1.ChartAreas[0].AxisY.Maximum = Math.Round(Body.MaxHeight, 3) + Math.Round(Body.MaxHeight * 0.2, 3);
            chart1.Series[0].Points.AddXY(x, y);
            label4.Text = Body.MaxDistance.ToString();

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Text = Math.Round(t, 2).ToString();
            label5.Text = Math.Round(Body.FlightTime, 2).ToString();
            t += dt;
            x = Math.Round(Body.V0 * Math.Cos(Body.Angle * Math.PI / 180) * t, 3);
            y = Body.Y0 + Body.V0 * Math.Sin(Body.Angle * Math.PI / 180) * t - Body.g * t * t / 2;
            chart1.Series[0].Points.AddXY(x, y);
            if (!(y <= 0)) return;
            timer1.Stop();
            pauseButton.Visible = false;
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            pauseButton.BackgroundImage =
                timer1.Enabled ? Properties.Resources.circled_pause : Properties.Resources.circled_play;
        }
    }

    public static class Body
    {
        public const double g = 9.81;

        public static double Y0;
        public static double V0;
        public static double Angle;

        public static double MaxHeight;
        public static double MaxDistance;
        public static double FlightTime;
    }
}