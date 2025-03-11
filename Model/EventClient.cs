using System.Diagnostics;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;


namespace Oar_Audio.Model
{
    class EventClient : IAudioSessionEventsHandler
    {
        public readonly Source _source;
        public delegate void HandleOnStateChanged(Source source, AudioSessionState state);
        public readonly HandleOnStateChanged _callback;

        public EventClient(Source source, HandleOnStateChanged callback)
        {
            _source = source;
            _callback = callback;
        }

        public void OnStateChanged(AudioSessionState state)
        {
            _callback?.Invoke(_source, state);
        }


        public void OnChannelVolumeChanged(uint channelCount, nint newVolumes, uint channelIndex) { }

        public void OnDisplayNameChanged(string displayName) { }

        public void OnGroupingParamChanged(ref Guid groupingId) { }

        public void OnIconPathChanged(string iconPath) { }

        public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason) { }

        public void OnVolumeChanged(float volume, bool isMuted) { }
    }
}
