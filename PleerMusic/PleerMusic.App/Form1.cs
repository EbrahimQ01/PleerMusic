﻿using PleerMusic.App.Controls.PlayMusic;
using PleerMusic.App.FormSongs;
using PleerMusic.App.UserControls;
using PleerMusic.DataLayer.Context;
using PleerMusic.DataLayer.Models;
using PleerMusic.Utility.MusicControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PleerMusic.App
{
    public partial class frmPleerMusic : Form
    {
        public frmPleerMusic()
        {
            InitializeComponent();
            CheckIsExist(_numberForm);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // CheckIsExist(_numberForm);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckIsExist(Convert.ToInt32(((Button)sender).Tag));
        }

        private int _numberForm = 1;



        /// <summary>
        /// Check For Is Form In plList Existr Or Repetitious 
        /// </summary>
        /// <param name="tagForm"></param>
        private void CheckIsExist(int tagForm)
        {

            if (plListMusic.Controls.Count == 0)
            {
                SelectForms(tagForm);
            }

            else if (tagForm != Convert.ToInt32(plListMusic.Controls[0].Tag))
            {
                plListMusic.Controls.Clear();
                SelectForms(tagForm);

            }


        }


        /// <summary>
        /// Choose between forms For Show in Panel List Music
        /// </summary>
        /// <param name="tagForm">Number Form</param>
        private void SelectForms(int tagForm)
        {

            _numberForm = tagForm;
            switch (tagForm)
            {

                case 1:
                    {
                        frmSongs songs = new frmSongs();
                        songs.TopLevel = false;
                        songs.Dock = DockStyle.Fill;
                        this.plListMusic.Controls.Add(songs);
                        songs.Show();
                        break;
                    }


            }
        }

        private async void addMusicToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            List<string> musicList = SelectMusic();
            if (musicList != null)
            {
                List<(Music music, AlbumMusic album, Singer singer)> listClass = await ConvertAddresToClass.ToClass(musicList);

                await InsertMusicsToDb(listClass);
                musicList = null;
                listClass = null;
            }
        }

        private List<string> SelectMusic()
        {

            using (OpenFileDialog openFile = new OpenFileDialog())
            {

                openFile.Multiselect = true;

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    return openFile.FileNames.ToList();
                }
            }
            return null;
        }

        private async Task InsertMusicsToDb(List<(Music music, AlbumMusic album, Singer singer)> listClass)
        {

            using (uowPleerMusic db = new uowPleerMusic())
            {
                await Task.Run(() =>
                {
                    foreach ((Music music, AlbumMusic album, Singer singer) item in listClass)
                    {
                        db.pAlbumMusic.Insert(item.album);
                        db.pMusic.Insert(item.music);
                        db.pSinger.Insert(item.singer);

                        db.Save();
                    }

                });
            }
            plListMusic.Controls.Clear();
            CheckIsExist(_numberForm);

        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayOrStop();
        }

        private void PlayOrStop()
        {

            if (PlayMusicContorl.IsPlay)
            {
                PlayMusicContorl.StopMusic();
                btnPlay.Text = "Stop";
            }
            else
            {
                PlayMusicContorl.PlayMusic();
                btnPlay.Text = "Play";
            }
        }

        private void moreToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void trbVolume_Scroll(object sender, EventArgs e)
        {
            PlayMusicContorl.VolumeMusic((float)trbVolume.Value);
        }

        private void timShowPositionNowMusic_Tick(object sender, EventArgs e)
        {
            lblPositionTime.Text = PlayMusicContorl.PositonTime.ToString();
            trbTimeMusic.Value = PlayMusicContorl.TimeShift;

        }

        private void trbTimeMusic_Scroll(object sender, EventArgs e)
        {
            PlayMusicContorl.TimeShift = trbTimeMusic.Value;
        }


        public int SelectedMusicNumber = 1;

        private void SelectMusicInList()
        {
            frmSongs frm = (frmSongs)plListMusic.Controls[0];


            if (SelectedMusicNumber == frm.Controls.Count )
                SelectedMusicNumber = 1;

            else if (SelectedMusicNumber == -1)
                SelectedMusicNumber = frm.Controls.Count-2;

            if (plListMusic.Controls[0] is frmSongs)
            {

                int selectMusicReal = ((frm.Controls.Count - 1) - SelectedMusicNumber);
                PlayMusicContorl.Address = frm.Controls[selectMusicReal].Tag.ToString();
                PlayMusicContorl.StartUse();
                SetDataMusic();
            }

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SelectedMusicNumber += 2;
            SelectMusicInList();
        }




        public void SetDataMusic()
        {
            PlayMusicContorl.VolumeMusic(trbVolume.Value);
            pcMusicImage.Image = PlayMusicContorl.GetImageMusic();
            lblMaxTime.Text = PlayMusicContorl.TotalTime;
            timShowPositionNowMusic.Enabled = true;
            trbTimeMusic.Maximum = PlayMusicContorl.MaxLength;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            SelectedMusicNumber -= 2;
            SelectMusicInList();
        }
    }
}