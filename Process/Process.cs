using OpenCvSharp;
using FFMpegCore;
using System;
using ICSharpCode.SharpZipLib.Zip;
using ASCII_Art.ViewModel;
using System.IO;

namespace ASCII_Art.Process
{
    internal class VideoIn
    {
        public const string BASE = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\\\"^`'. ";
        public bool Availability;
        public int Size;
        public string[] Data;
        private MainViewModel DataContext;
        public int Duration;
        public double FPS;
        public int FrameCount;
        public int Row=0;
        public int Column=0;
        public readonly VideoCapture vc;
        private readonly string FileName;

        public struct Resolution_ { public int Width, Height; }
        public Resolution_ Resolution;

        public VideoIn(string fileName)
        {
            vc = new VideoCapture(fileName);
            FileName = fileName;
            if (vc.IsOpened())
            {
                Availability = true;
                Resolution.Width = vc.FrameWidth;
                Resolution.Height = vc.FrameHeight;
                Size = 40;
                Duration = (int)(vc.FrameCount / vc.Fps);
                FPS = vc.Fps;
                FrameCount = vc.FrameCount;
            }
            else
                Availability = false;
        }

        public void Get()
        {
            //string output = FileName.Substring(0, FileName.LastIndexOf('.') + 1);
            DataContext.InfoText = "Start processing ...\nNow is processing the audio...";
            FFMpeg.ExtractAudio(FileName, ".cache\\2.mp3");
            int w = Resolution.Width;
            int h = Resolution.Height;
            Mat mat = new Mat();
            int RowSpan = h / Size;
            int ColumnSpan = h / Size / 2;
            string[] data = new string[FrameCount];
            for (int i = 0; i < FrameCount; i++)
            {
                vc.Read(mat);
                DataContext.InfoText = "Start processing ...\nNow is processing frame " + (i + 1) + "/" + FrameCount;
                for (int Row = 0; Row < h; Row += RowSpan)
                {
                    if (i == 0)
                        this.Row++;
                    for (int Column = 0; Column < w; Column += ColumnSpan)
                    {
                        if (i == 0 && Row == 0)
                            this.Column++;
                        Vec3b m = mat.Get<Vec3b>(Row, Column);
                        int b = m.Item0, g = m.Item1, r = m.Item2;
                        float grey = (0.302f * r + 0.581f * g + 0.117f * b);
                        int index = (int)Math.Round(grey * (BASE.Length + 1) / 255);
                        data[i] += (index >= BASE.Length) ? " " : BASE[index].ToString();
                    }
                    if (Row + RowSpan >= h) break;
                    data[i] += "\n";
                }
            }
            Data = data;
            DataContext.InfoText = "Start processing ...\nNow is packaging...";
        }

        public  void ZipFile()
        {
            string outputName = DataContext.OutputDir+"\\" + DataContext.FileName.Substring(0, DataContext.FileName.LastIndexOf("."))+".asc";
            ZipOutputStream zpo = new ZipOutputStream(new FileStream(outputName, FileMode.Create));
            DirectoryInfo di=new DirectoryInfo(".cache");
            FileInfo[] fs=di.GetFiles("*.*");
            byte[] vs = new byte[10 * 1024];
            foreach (FileInfo fi in fs)
            {
                ZipEntry entry = new ZipEntry(fi.Name)
                {
                    Size = fi.Length
                };
                zpo.PutNextEntry(entry);
                int len;
                Stream input=fi.Open(FileMode.Open);
                while ((len = input.Read(vs, 0, 10 * 1024)) > 0)
                    zpo.Write(vs, 0, len);
                input.Close();
            }
            zpo.Finish();
            zpo.Close();
        }

        public void SetDataContext(MainViewModel model)
        {
            DataContext = model;
        }
    }
}
