using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LokiSoundExplorer
{

    public partial class Form1 : Form
    {

        public int indexSelected = 0;

        LokiSound ls = new LokiSound();
        List<WavFileContainer> wavFiles = new List<WavFileContainer>();

        MediaPlayer media = new MediaPlayer();

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog fileD = new OpenFileDialog();
            fileD.Filter = "FEAR2 Sound File |*.snd";
            fileD.ShowDialog();

            if (ls.ReadSoundFile(fileD.FileName))
            {
                int i = 0;
                foreach(var item in ls.unknownTable)
                {
                    ListViewItem lItem = new ListViewItem();

                    lItem.Text = "Sound# " + (i+1).ToString();

                    if(item.chan.Any())
                        lItem.SubItems.Add(item.chan[0].sample_rate.ToString());
                    else
                        lItem.SubItems.Add("Unknown");
                    
                    lItem.SubItems.Add(item.bit_depth.ToString());
                    listView1.Items.Add(lItem);
                    i++;
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            IWaveProvider provider = new RawSourceWaveStream(
                         new MemoryStream(ls.waveFiles[listView1.SelectedIndices[0]].wavChannels[0].buf), new WaveFormat(ls.unknownTable[indexSelected].chan[0].sample_rate, 16, 1));

            WaveOut _waveOut = new WaveOut();

            _waveOut.Init(provider);
            _waveOut.Play();

            //MediaPlayer m = new MediaPlayer(ls.waveFiles[indexSelected].wavChannels[0].buf);
            //m.Play();

        }
    }
}
