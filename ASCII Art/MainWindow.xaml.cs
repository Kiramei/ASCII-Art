using ASCII_Art.ViewModel;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ASCII_Art.Process;
using ICSharpCode.SharpZipLib.Zip;

namespace ASCII_Art
{
    public partial class MainWindow : Window
    {
        readonly MainViewModel Data;
        private VideoIn Video;
        readonly string LOGO_STRING = "\n                ┌────────────────────┐\n" +
                                      "                │  Welcome to use ASCII Art!   │\n" +
                                      "                └────────────────────┘\n\n";
        readonly string COMPLETE_STRING = "\n                ┌────────────────────┐\n" +
                                          "                │               Completed!              │\n" +
                                          "                └────────────────────┘\n\n";
        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists(".cache")) Directory.CreateDirectory(".cache");
            Data = new MainViewModel();
            DataContext = Data;
        }


        private void Create(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".mp4",
                Filter = @"Video File(*.mp4,*avi,...)|*.mp4;*.avi;*.mpeg;*.wmv;*.mov;*.rmvb;*.flv"
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                Init(new FileInfo(dialog.FileName));
                
                RunAnimation(new StackPanel[] { MainOption }, true, new Thickness[] { new Thickness(0, 60, 0, 0) });
                RunAnimation(new StackPanel[] { InfoPanel, BoxOption }, false, new Thickness[] { new Thickness(20, 20, 20, 20), new Thickness(20, 20, 20, 20) }, 0.7);
            }
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".mp4",
                Filter = @"Project File(*.asc)|*.asc"
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                File.Delete(".cache\\1.dat");
                File.Delete(".cache\\2.mp3");
                new FastZip().ExtractZip(dialog.FileName, ".cache", "");
                System.Diagnostics.Process.Start("Runner.exe").WaitForExit();
            }
        }

        private void Init(FileInfo file)
        {
            Video = new VideoIn(file.FullName);
            Data.FileName = file.Name;
            Data.FileSize = file.Length / 1048576 + "MB";
            Data.OutputDir = file.DirectoryName;
            Data.StartButtonName = "Start";
            Data.InfoText = LOGO_STRING +
                            "\t    You can double click the \n\t    Directory text to change it.\n";
            if (Video.Availability)
            {
                int d = Video.Duration;
                Data.InfoText += $"\n\t    Your file is up to the demand!\n" +
                                 $"\t    Here is the information of it:\n" +
                                 $"\t    Duration:\t\t{d / 3600}:{d % 3600 / 60}:{d % 60}\n" +
                                 $"\t    Resolution:\t\t{Video.Resolution.Width}×{Video.Resolution.Height}\n" +
                                 $"\t    FramePerSecond:\t{Video.FPS}\n" +
                                 $"\t    Please press Start to build the ASCII.";
                Data.CanStart = true;
            }
            else
            {
                Data.InfoText += "\n\n      We're sorry that your video file seems broken,\n " +
                                 "\tor the videotype is not supported.\n" +
                                 "\tPlease go back and select a new one.";
                Data.CanStart = false;
            }
            return;
        }

        private void SetOutput(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Data.OutputDir = folderBrowserDialog.SelectedPath;
            }
        }

        void RunAnimation(StackPanel[] invoke, bool fadeMode, Thickness[] arrival, double delay = 0)
        {
            for (int i = 0; i < invoke.Length; i++)
            {
                var sb = new Storyboard();
                int to = fadeMode ? 0 : 1;
                var ani1 = new ThicknessAnimation
                {
                    To = arrival[i],
                    Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                    DecelerationRatio = 1,
                    BeginTime = TimeSpan.FromSeconds(delay)
                };
                var ani2 = new DoubleAnimation
                {
                    To = to,
                    Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                    DecelerationRatio = 1,
                    BeginTime = TimeSpan.FromSeconds(delay)
                };
                Storyboard.SetTargetProperty(ani1, new PropertyPath("Margin"));
                Storyboard.SetTargetProperty(ani2, new PropertyPath("Opacity"));
                sb.Children.Add(ani1);
                sb.Children.Add(ani2);
                Storyboard.SetTarget(sb, invoke[i]);
                int finalI = i;
                if (fadeMode)
                {
                    sb.Completed += (a, e) =>
                    {
                        invoke[finalI].Visibility = Visibility.Collapsed;
                        invoke[finalI].IsEnabled = false;
                    };
                }
                else
                {
                    invoke[i].Visibility = Visibility.Visible;
                    invoke[finalI].IsEnabled = true;
                }
                sb.Begin();
            }
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            RunAnimation(new StackPanel[] { MainOption }, false, new Thickness[] { new Thickness(0, 60, 0, 0) }, 0.7);
            RunAnimation(new StackPanel[] { BoxOption, InfoPanel }, true, new Thickness[] { new Thickness(-80, 20, 120, 20), new Thickness(120, 20, -80, 20) });
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            if ("Start".Equals(Data.StartButtonName))
            {
                Video.SetDataContext(Data);
                Data.CanStart = false;
                new Thread(() =>
                {
                    Video.Get();
                    string[] s = Video.Data;
                    StreamWriter sw = new StreamWriter(new FileStream( ".cache\\1.dat", FileMode.Create));
                    sw.WriteLine(Video.FrameCount.ToString()+","+Video.Column+","+Video.Row+","+Video.FPS);
                    foreach (var x in s) sw.WriteLine(x);
                    sw.Close();
                    Video.ZipFile();
                    Data.InfoText = COMPLETE_STRING;
                    Data.CanStart = true;
                    Data.StartButtonName = "View";
                }).Start();
            }
            else
                System.Diagnostics.Process.Start("Runner.exe");
        }
    }
}
