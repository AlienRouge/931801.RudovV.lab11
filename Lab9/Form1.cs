using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab9
{
    public partial class Form1 : Form
    {
        readonly double[] Einstein = new double[5];

        public Form1()
        {
            InitializeComponent();
            chart1.Legends.Clear();
            numericUpDown5.Enabled = false;
            AverageValuelbl.Text =
                VarianceValuelbl.Text = ChiSquaredBoolValuelbl.Text =
                    ChiSquaredBoolValuelbl.Text = ChiSquaredValuelbl.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Einstein[0] = (double) numericUpDown1.Value;
            Einstein[1] = (double) numericUpDown2.Value;
            Einstein[2] = (double) numericUpDown3.Value;
            Einstein[3] = (double) numericUpDown4.Value;
            Einstein[4] = 1 - (Einstein[0] + Einstein[1] + Einstein[2] + Einstein[3]);

            if (Einstein[4] <= 0 || Einstein[4] > 1)
            {
                return;
            }

            numericUpDown5.Value = (decimal) Einstein[4];
            chart1.Series[0].Points.Clear();
            Random rand = new Random();

            int[] stats = new int[5];
            var n = (int) numericTimes.Value;

            for (var i = 0; i < n; i++)
            {
                var q = (double) rand.NextDouble();
                for (var j = 0; j < 5; j++)
                {
                    q -= Einstein[j];
                    if (!(q <= 0)) continue;
                    stats[j]++;
                    break;
                }
            }

            for (var i = 0; i < 5; i++)
            {
                chart1.Series[0].Points.AddXY(i + 1, (float) stats[i] / n);
            }

            (double[] AveVar, double[] errors) = calculateStatistics(stats, Einstein, n);
            (double chiResult, bool isTrue) = chi_square_test(stats, Einstein, n);

            AverageValuelbl.Text = $@"Average: {Math.Round(AveVar[0], 3),-10}(error = {Math.Round(errors[0] * 100)}%)";
            VarianceValuelbl.Text =
                $@"Variance: {Math.Round(AveVar[1], 3),-10}(error = {Math.Round(errors[1] * 100)}%)";

            ChiSquaredValuelbl.Text = $@"Chi-squared: {Math.Round(chiResult, 3)} > 9.488 is";
            ChiSquaredBoolValuelbl.Text = isTrue.ToString();
            ChiSquaredBoolValuelbl.ForeColor = isTrue ? Color.Red : Color.Green;
        }

        static (double result, bool isTrue) chi_square_test(int[] n, double[] p, int N)
        {
            double result = 0;
            for (int i = 0; i < 5; i++)
                result += (n[i] * n[i]) / (N * p[i]);
            result -= N;
            return (result, result > 9.488);
        }

        static (double[] results, double[] errors) calculateStatistics(int[] n, double[] p, int N)
        {
            var sho = new double[] {0, 0};
            var kavo = new double[] {0, 0};
            for (int i = 0; i < 5; i++)
            {
                sho[0] += p[i] * i;
                sho[1] += (float) n[i] / N * i;
                kavo[0] += p[i] * i * i;
                kavo[1] += (float) n[i] / N * i * i;
            }

            kavo[0] -= sho[0] * sho[0]; // Var
            kavo[1] -= sho[1] * sho[1]; // EVar

            double eError = Math.Abs(sho[1] - sho[0]) / Math.Abs(sho[0]);
            double dError = Math.Abs(kavo[1] - kavo[0]) / Math.Abs(kavo[0]);
            double[] results = new[] {sho[1], kavo[1]};

            return (new[] {sho[1], kavo[1]}, new[] {eError, dError});
        }
    }
}