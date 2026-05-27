using System;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace PROG6221POE
{
    public static class AudioPlayer
    {
        public static void PlayGreeting(string filePath)
        {
            // Makes sure the audio file exists first
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Audio file missing: " + filePath);

                return;
            }

            try
            {
                // Loads and plays the greeting
                SoundPlayer player = new SoundPlayer(filePath);

                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Audio playback failed: " + ex.Message);
            }
        }
    }
}