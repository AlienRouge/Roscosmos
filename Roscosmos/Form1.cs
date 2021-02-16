using System;
using System.Windows.Forms;
using Roscosmos.Properties;

namespace Roscosmos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private (double, double)pos;

        private double t;
        private const double dt = 0.01;

        private void launchButton_Click(object sender, EventArgs e)
        {
            pauseButton.BackgroundImage = Resources.circled_pause;
            pauseButton.Visible = true;

            Body.Y0 = (double) edHeight.Value;
            Body.V0 = (double) edSpeed.Value;
            Body.Angle = (double) edAngle.Value;

            pos.Item1 = 0;
            pos.Item2 = Body.Y0;
            t = 0;

            Body.CalculateFlightParams();
            
            chart1.Series[0].Points.Clear();
            chart1.ChartAreas[0].AxisX.Maximum = Math.Round(Body.MaxDistance + Body.MaxDistance * 0.15, 3);
            chart1.ChartAreas[0].AxisY.Maximum = Math.Round(Body.MaxHeight + Body.MaxHeight * 0.15, 3);
            chart1.Series[0].Points.AddXY(pos.Item1, pos.Item2);

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Text = Math.Round(t, 2).ToString();
            label5.Text = Math.Round(Body.FlightTime, 2).ToString();

            t += dt;
            (pos.Item1, pos.Item2) =Body.CalculatePosition(t);
            chart1.Series[0].Points.AddXY(pos.Item1, pos.Item2);
            if (!(pos.Item2 <= 0)) return;
            timer1.Stop();
            pauseButton.Visible = false;
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            pauseButton.BackgroundImage =
                timer1.Enabled ? Resources.circled_pause : Resources.circled_play;
        }
    }

    public static class Body
    {
        private const double g = 9.81;

        public static double Y0;
        public static double V0;
        public static double Angle;

        public static double MaxHeight;
        public static double MaxDistance;
        public static double FlightTime;

        public static void CalculateFlightParams()
        {
            FlightTime = (V0 * Math.Sin(Angle * Math.PI / 180) +
                               Math.Sqrt(Math.Pow(V0 * Math.Sin(Angle * Math.PI / 180), 2) +
                                         2 * g * Y0)) / g;
            MaxHeight = Y0 + Math.Pow(V0 * Math.Sin(Angle * Math.PI / 180), 2) / (2 * g);
            MaxDistance = V0 * Math.Cos(Angle * Math.PI / 180) * FlightTime;
        }

        public static (double x, double y) CalculatePosition(double t)
        {
            var x = Math.Round(V0 * Math.Cos(Angle * Math.PI / 180) * t, 3);
            var y = Y0 + V0 * Math.Sin(Angle * Math.PI / 180) * t - g * t * t / 2;
            return (x, y);
        }
    }
}