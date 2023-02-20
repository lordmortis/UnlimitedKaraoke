using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

namespace UnlimitedKaraoke.Runtime.Audio
{
    public class FModAudioSystem : IAudioSystem, IInitializable, ITickable, IDisposable
    {
        public IReadOnlyList<IAudioOutput> Outputs { get; private set; }

        public event Action<IReadOnlyList<IAudioOutput>> OnOutputsUpdated;

        private readonly List<FModAudioOutput> outputs = new ();
        private readonly Queue<FModAudioOutput> unusedOutputs = new();
        
        private FMOD.System currentSystem;
        private bool systemInitialized;

        public FModAudioSystem()
        {
            Outputs = outputs.AsReadOnly();
        }

        public void Initialize()
        {
            var result = Factory.System_Create(out currentSystem);
            if (result != RESULT.OK)
            {
                Debug.LogError("Could not create FMOD System!: " + result);
                return;
            }

            result = currentSystem.setSoftwareChannels(128);
            if (result != RESULT.OK)
            {
                Debug.LogError($"Could not set software channels: {result}");
                return;
            }

            #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            
            result = currentSystem.setOutput(OUTPUTTYPE.ASIO);
            if (result != RESULT.OK)
            {
                Debug.LogError($"Could not set output to core audio: {result}");
                return;
            }

            #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    
            result = currentSystem.setOutput(OUTPUTTYPE.COREAUDIO);
            if (result != RESULT.OK)
            {
                Debug.LogError($"Could not set output to core audio: {result}");
                return;
            }
            #endif

            result = currentSystem.setSoftwareFormat(48000, SPEAKERMODE.RAW, 4);
            if (result != RESULT.OK)
            {
                Debug.LogError($"Could not set format: {result}");
                return;
            }

            result = currentSystem.setDSPBufferSize(1024, 20);
            if (result != RESULT.OK)
            {
                Debug.LogError($"Could not buffer size format: {result}");
                return;
            }
            
            result = currentSystem.setStreamBufferSize(1000, TIMEUNIT.MS);
            if (result != RESULT.OK)
            {
                Debug.LogError($"Could not set buffer: {result}");
                return;
            }
            
            result = currentSystem.getNumDrivers(out int numDrivers);
            if (result != RESULT.OK)
            {
                Debug.LogError($"Could not get drivers from FMOD System: {result}");
                return;
            }
            
            
            foreach (var output in outputs) unusedOutputs.Enqueue(output);
            outputs.Clear();

            for (int i = 0; i < numDrivers; i++)
            {
                result = currentSystem.getDriverInfo(i, out string name, 100, out Guid guid,
                    out int systemRate, out SPEAKERMODE speakerMode, out int speakerModeChannels);
                if (result != RESULT.OK)
                {
                    Debug.LogError($"Could not get drivers info for driver {i}: {result}");
                    continue;
                }

                if (!unusedOutputs.TryDequeue(out FModAudioOutput output))
                {
                    output = new FModAudioOutput();
                }

                output.Index = i;
                output.Name = name;
                output.SamplingRate = systemRate;
                output.SpeakerMode = speakerMode;
                output.SpeakerModeChannels = speakerModeChannels;
                output.DeviceGuid = guid;
                outputs.Add(output);
            }
            
            OnOutputsUpdated?.Invoke(Outputs);
        }

        public void SetOutput(IAudioOutput output)
        {
            if (output is not FModAudioOutput fModAudioOutput)
            {
                Debug.LogError("Cannot set output! not the right type!");
                return;
            }

            var result = currentSystem.setDriver(fModAudioOutput.Index);
            if (result != RESULT.OK)
            {
                Debug.LogError($"Could not set output to {fModAudioOutput.Name}: {result}");
                return;
            }

            if (!systemInitialized)
            {
                result = currentSystem.init(10, INITFLAGS.NORMAL, IntPtr.Zero);
                if (result != RESULT.OK)
                {
                    Debug.LogError($"Could not init fmod: {result}");
                    return;
                }
                
                systemInitialized = true;
            }
        }

        public void StartLoadingFile(string filename, int channelIndex)
        {
            var result = currentSystem.createSound(filename, MODE.CREATESTREAM, out var sound);
            if (result != RESULT.OK)
            {
                Debug.LogError($"File loadin failed! {result}");
                return;
            }

            result = currentSystem.getMasterChannelGroup(out var masterChannelGroup);
            if (result != RESULT.OK)
            {
                Debug.LogError($"getting master channel failed! {result}");
                return;
            }

            currentSystem.playSound(sound, masterChannelGroup, false, out var channel);
            if (result != RESULT.OK)
            {
                Debug.LogError($"playing sound failed! {result}");
                return;
            }
            
            result = channel.getDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, out var dspHead);
            if (result != RESULT.OK)
            {
                Debug.LogError($"getting DSP failed! {result}");
                return;
            }            

            result = dspHead.getOutput(0, out _, out var channelDspHeadOutputConnection);
            if (result != RESULT.OK)
            {
                Debug.LogError($"getting channel dsp output channel failed! {result}");
                return;
            }   
            
            result = sound.getFormat(out _, out _, out var soundChannels, out _);
            if (result != RESULT.OK)
            {
                Debug.LogError($"getting format failed! {result}");
                return;
            }   
            
            var matrix = new float[soundChannels * 4];
            for (int i = 0; i < soundChannels; i++)
            {
                int pos = soundChannels * (channelIndex + i) + i;
                if (pos >= matrix.Length)
                {
                    break;
                }
                matrix[pos] = 1.0f;
            }
            
            result = channelDspHeadOutputConnection.setMixMatrix(matrix, 4, soundChannels);
            if (result != RESULT.OK)
            {
                Debug.LogError($"could not set mix matrix! {result}");
                return;
            }   
            
        }

        public void Play()
        {
            throw new System.NotImplementedException();
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }

        public void Tick()
        {
            if (systemInitialized)
            {
                var result = currentSystem.update();
                if (result != RESULT.OK)
                {
                    Debug.LogError($"System update failed! {result}");
                }
            }
        }

        public void Dispose()
        {
            if (systemInitialized) currentSystem.close();
            currentSystem.release();
        }
    }
}