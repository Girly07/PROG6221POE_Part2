using System;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace PROG6221POE
{
    /*
     * AUDIO PLAYER - Handles voice greeting playback.
     */

    public static class AudioPlayer
    {
        public static void PlayGreeting(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show(
                    "Audio file missing:\n" + filePath,
                    "Audio Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                SoundPlayer player = new SoundPlayer(filePath);

                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Audio playback failed:\n" + ex.Message,
                    "Playback Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}