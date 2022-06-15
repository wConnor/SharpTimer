using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Regex numeric_regex = new Regex("[^0-9]+");
        private ManualResetEvent mre = new ManualResetEvent(true);
        private Thread timerThread;
        private Timer timer;
        private DateTime timeStarted;
        private bool quitFlag = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void runTimer()
        {
            for (; this.timer._totalSeconds >= 0; this.timer._totalSeconds--)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.tbxHours.Text = (this.timer._totalSeconds / 3600).ToString("00");
                    this.tbxMinutes.Text = (this.timer._totalSeconds / 60).ToString("00");
                    this.tbxSeconds.Text = (this.timer._totalSeconds % 60).ToString("00");
                });
                Thread.Sleep(1000);

                this.mre.WaitOne();
            }

            this.Dispatcher.Invoke(() =>
            {
                this.bStartStop.Content = "Start";
                this.bPauseResume.IsEnabled = false;
                this.bPauseResume.Content = "Pause";
                this.tbxHours.IsReadOnly = false;
                this.tbxMinutes.IsReadOnly = false;
                this.tbxSeconds.IsReadOnly = false;
            });

            if (!this.quitFlag)
            {
                new ToastContentBuilder()
                    .AddText("Sharp Timer")
                    .AddText(this.timer._hours.ToString("00") + ":" +
                            this.timer._minutes.ToString("00") + ":" +
                            this.timer._seconds.ToString("00") + " timer started at " + this.timeStarted + " has elapsed.")
                    .Show();
            }
        }

        private void tbxHours_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = this.numeric_regex.IsMatch(e.Text);
        }

        private void tbxMinutes_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = this.numeric_regex.IsMatch(e.Text);
        }

        private void tbxSeconds_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = this.numeric_regex.IsMatch(e.Text);
        }

        private void bStartStop_Click(object sender, RoutedEventArgs e)
        {
            // start button clicked; prepare and start the timer.
            if ((string) this.bStartStop.Content == "Start")
            {
                this.bStartStop.Content = "Stop";
                this.bPauseResume.IsEnabled = true;
                this.tbxHours.IsReadOnly = true;
                this.tbxMinutes.IsReadOnly = true;
                this.tbxSeconds.IsReadOnly = true;

                this.timer = new Timer(
                    Convert.ToInt32(this.tbxHours.Text),
                    Convert.ToInt32(this.tbxMinutes.Text),
                    Convert.ToInt32(this.tbxSeconds.Text));
                this.timeStarted = DateTime.Now;
                this.quitFlag = false;
                this.timerThread = new Thread(new ThreadStart(runTimer));
                this.timerThread.Start();
            }
            // stop button clicked; end the timer.
            else
            {
                this.bStartStop.Content = "Start";
                this.bPauseResume.IsEnabled = false;
                this.bPauseResume.Content = "Pause";
                this.tbxHours.IsReadOnly = false;
                this.tbxMinutes.IsReadOnly = false;
                this.tbxSeconds.IsReadOnly = false;

                this.tbxHours.Text = "00";
                this.tbxMinutes.Text = "00";
                this.tbxSeconds.Text = "00";
                this.quitFlag = true;
                this.mre.Set();
                this.timer._totalSeconds = 0;
            }
        }

        private void bPauseResume_Click(object sender, RoutedEventArgs e)
        {
            // pause button clicked; suspend the timer.
            if ((string) this.bPauseResume.Content == "Pause")
            {
                this.mre.Reset();
                this.bPauseResume.Content = "Resume";
            }
            // resume button clicked; continue the timer.
            else
            {
                this.mre.Set();
                this.bPauseResume.Content = "Pause";
            }       
        }
    }
}
