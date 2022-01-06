using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace LokiSoundExplorer
{
    using ExtensionMethods;
    using NAudio.Wave;
    public partial class Form1 : Form
    {
        WaveOut _waveOut = new WaveOut();
        LokiSound ls = new LokiSound();

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog fileD = new OpenFileDialog();
            fileD.Filter = "FEAR2 Sound File |*.snd";
            fileD.ShowDialog();
            if (ls == null)
                ls = new LokiSound();
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

                    //calc size

                    string fileSize = ls.waveFiles[i].wavChannels[0].data_length.ToString();
                    fileSize = fileSize.ToSize(MyExtensions.SizeUnits.KB);

                    lItem.SubItems.Add(fileSize + " KB");
                    listView1.Items.Add(lItem);
                    i++;
                }

                detailGroupBox.Enabled = true;
                actionGroupBox.Enabled = true;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null && listView1.SelectedItems.Count > 0)
            {
                var item = sender as ListView;
                int selectedIndex = listView1.SelectedIndices[0];

                float time = ls.waveFiles[selectedIndex].wavChannels[0].data_length / (ls.waveFiles[selectedIndex].wavChannels[0].sample_rate * ls.waveFiles[selectedIndex].wavChannels[0].channel_count * ls.waveFiles[selectedIndex].wavChannels[0].bit_depth / 8);

                TimeSpan t = TimeSpan.FromSeconds(time);

                lengthLabel.Text = "Length: " + t.ToString(@"mm\:ss");
                bitDepthLabel.Text = "Bit Depth: " + ls.waveFiles[selectedIndex].wavChannels[0].bit_depth.ToString();
                bitRateLabel.Text = "Bitrate: " + ls.waveFiles[selectedIndex].wavChannels[0].sample_rate.ToString();
                fileSizeLabel.Text = "Size: " + listView1.Items[selectedIndex].SubItems[3].Text;

                foreach (Control gb in actionGroupBox.Controls)
                    gb.Enabled = true;

            }
            else
                foreach (Control gb in actionGroupBox.Controls)
                    gb.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            IWaveProvider provider = new RawSourceWaveStream(
                         new MemoryStream(ls.waveFiles[listView1.SelectedIndices[0]].wavChannels[0].buf), 
                         new WaveFormat((int)ls.waveFiles[listView1.SelectedIndices[0]].wavChannels[0].sample_rate,
                         ls.waveFiles[listView1.SelectedIndices[0]].wavChannels[0].bit_depth,
                         ls.waveFiles[listView1.SelectedIndices[0]].wavChannels[0].channel_count));

            VolumeWaveProvider16 volumeWaveProvider = new VolumeWaveProvider16(provider);
            volumeWaveProvider.Volume = volumeSlider1.Volume;

            _waveOut.Init(volumeWaveProvider);
            _waveOut.Play();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            _waveOut.Stop();
        }

        private void closeStripMenuItem_Click(object sender, EventArgs e)
        {
            ls = null;
            listView1.Items.Clear();
            GC.Collect();

            lengthLabel.Text = "Length: ";
            bitDepthLabel.Text = "Bit Depth:";
            bitRateLabel.Text = "Bitrate: ";
            fileSizeLabel.Text = "Size: ";

            actionGroupBox.Enabled = false;
            foreach (Control item in actionGroupBox.Controls)
                item.Enabled = false;
        }
    }
}
