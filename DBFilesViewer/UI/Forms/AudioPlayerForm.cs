using System;
using System.Windows.Forms;
using DBFilesViewer.Data.IO.CASC;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace DBFilesViewer.UI.Forms
{
    /// <summary>
    /// Provides the user with the ability to read MP3 and WAV files from CASC.
    /// </summary>
    public partial class AudioPlayerForm : Form
    {
        private uint _fileDataID;
        private Action<float> setVolumeHandler;

        public AudioPlayerForm()
        {
            InitializeComponent();
        }

        public AudioPlayerForm(uint fileDataID)
        {
            InitializeComponent();
            _fileDataID = fileDataID;
        }

        private void OnTrackBarPositionScroll(object sender, EventArgs e)
        {
            if (_waveOut != null)
                _audioFileReader.CurrentTime = TimeSpan.FromSeconds(trackBarPosition.Value);
        }

        private void OnButtonPauseClick(object sender, EventArgs e)
        {
            if (_waveOut?.PlaybackState == PlaybackState.Playing)
            {
                _waveOut.Pause();
                timer1.Stop();
            }
        }

        private void OnButtonStopClick(object sender, EventArgs e)
        {
            _waveOut?.Stop();
        }

        private void OnButtonPlayClick(object sender, EventArgs e)
        {
            if (_waveOut?.PlaybackState == PlaybackState.Playing)
                return;

            timer1.Interval = 20;

            if (_waveOut?.PlaybackState == PlaybackState.Paused)
            {
                _waveOut.Play();
                timer1.Start();
            }
            else
            {
                try
                {
                    CreateWaveOut();
                }
                catch (Exception driverCreateException)
                {
                    MessageBox.Show(driverCreateException.Message);
                    return;
                }

                ISampleProvider sampleProvider;
                try
                {
                    sampleProvider = CreateInputStream();
                }
                catch (Exception createException)
                {
                    MessageBox.Show(createException.Message, "Error while loading stream");
                    return;
                }

                labelTotalTime.Text =
                    $"{(int) _audioFileReader.TotalTime.TotalMinutes:00}:{_audioFileReader.TotalTime.Seconds:00}.{_audioFileReader.TotalTime.Milliseconds:000}";

                trackBarPosition.Maximum = (int)_audioFileReader.TotalTime.TotalSeconds;

                try
                {
                    _waveOut.Init(sampleProvider);
                    if (_waveOut != null)
                        _waveOut.PlaybackStopped += (o, args) => {
                            if (args.Exception == null)
                                timer1.Stop();
                        };
                }
                catch (Exception initException)
                {
                    MessageBox.Show(initException.Message, "Error Initializing Output");
                    return;
                }

                _waveOut?.Play();
                timer1.Start();
            }
        }

        private ISampleProvider CreateInputStream()
        {
            try
            {
                _audioFileReader = new VorbisWaveReader(Manager.OpenFile(_fileDataID));
            }
            catch
            {
                _audioFileReader = new Mp3FileReader(Manager.OpenFile(_fileDataID),
                    waveFormat => new NLayer.NAudioSupport.Mp3FrameDecompressor(waveFormat));
            }

            var sampleChannel = new SampleChannel(_audioFileReader, true);
            sampleChannel.PreVolumeMeter += OnPreVolumeMeter;
            setVolumeHandler = vol => sampleChannel.Volume = vol;

            return new MeteringSampleProvider(sampleChannel);
        }

        private void OnPreVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            waveformPainter1.AddMax(e.MaxSampleValues[0]);
        }

        private void CreateWaveOut()
        {
            CloseWaveOut();
            _waveOut = new WaveOut();
        }

        private void CloseWaveOut()
        {
            _waveOut?.Stop();
            if (_audioFileReader != null)
            {
                // this one really closes the file and ACM conversion
                var disposableFileReader = _audioFileReader as IDisposable;
                disposableFileReader?.Dispose();
                _audioFileReader = null;
            }
            if (_waveOut != null)
            {
                _waveOut.Dispose();
                _waveOut = null;
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_waveOut != null && _audioFileReader != null)
            {
                var currentTime = (_waveOut.PlaybackState == PlaybackState.Stopped) ? TimeSpan.Zero : _audioFileReader.CurrentTime;
                trackBarPosition.Value = (int)currentTime.TotalSeconds;
                labelCurrentTime.Text = $"{(int)currentTime.TotalMinutes:00}:{currentTime.Seconds:00}.{currentTime.Milliseconds:000}";
            }
            else
            {
                trackBarPosition.Value = 0;
            }
        }

        private IWavePlayer _waveOut;
        private WaveStream _audioFileReader;

        private void VolumeChanged(object sender, EventArgs e)
        {
            setVolumeHandler?.Invoke(volumeSlider1.Volume);
        }

        private void OnClose(object sender, FormClosingEventArgs e)
        {
            CloseWaveOut();
        }
    }
}
