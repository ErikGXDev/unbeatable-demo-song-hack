using AOT;
using FMOD.Studio;
using FMOD;
using FMODUnity;
using Rhythm;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityMainThreadDispatcher;
using System.Collections;
using UnityEngine;

namespace UnbeatableSongHack
{
    // This is unused, it's only a reference now.
    // FMOD is weird.
    internal class FMODInstances
    {
        [MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
        public static RESULT MusicEventCallback(EVENT_CALLBACK_TYPE type, IntPtr @event, IntPtr parameterPtr)
        {
            return type switch
            {
                EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND => HandleCreateProgrammerSound(new EventInstance(@event), parameterPtr),
                EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND => HandleDestroyProgrammerSound(parameterPtr),
                EVENT_CALLBACK_TYPE.DESTROYED => HandleDestroyed(new EventInstance(@event)),
                _ => RESULT.OK,
            };
        }


        public static RESULT HandleCreateProgrammerSound(EventInstance instance, IntPtr parameterPtr)
        {
            instance.getUserData(out var userdata);
            PlayInfo playInfo = GCHandle.FromIntPtr(userdata).Target as PlayInfo;
            MODE mODE = MODE.LOOP_NORMAL | MODE.CREATECOMPRESSEDSAMPLE | MODE.NONBLOCKING;
            PROGRAMMER_SOUND_PROPERTIES properties = GetProgrammerSoundProperties(parameterPtr);

            switch (playInfo.source)
            {
                case PlaySource.FromTable:
                    {
                        string key = Marshal.PtrToStringUni(playInfo.key);

                        SOUND_INFO info;
                        RESULT soundInfo = RuntimeManager.StudioSystem.getSoundInfo(key, out info);
                        if (soundInfo != 0)
                        {
                            return soundInfo;
                        }
                        Sound sound2;
                        RESULT rESULT2 = RuntimeManager.CoreSystem.createSound(info.name_or_data, mODE | info.mode, ref info.exinfo, out sound2);
                        if (rESULT2 == RESULT.OK)
                        {
                            properties.sound = sound2.handle;
                            properties.subsoundIndex = info.subsoundindex;
                            Marshal.StructureToPtr(properties, parameterPtr, fDeleteOld: false);
                            Core.GetLogger().Msg("OK Sound");
                            break;
                        }
                        return rESULT2;
                    }
                case PlaySource.FromFile:
                    {
                        string name = Marshal.PtrToStringUni(playInfo.key);
                        Sound sound;
                        RESULT rESULT = RuntimeManager.CoreSystem.createSound(name, mODE, out sound);
                        if (rESULT == RESULT.OK)
                        {
                            properties.sound = sound.handle;
                            properties.subsoundIndex = -1;
                            Marshal.StructureToPtr(properties, parameterPtr, fDeleteOld: false);
                            break;
                        }
                        return rESULT;
                    }
            }
            
            return RESULT.OK;
        }



        public static RESULT HandleDestroyProgrammerSound(IntPtr parameterPtr)
        {
            new Sound(GetProgrammerSoundProperties(parameterPtr).sound).release();
            return RESULT.OK;
        }

        public static RESULT HandleDestroyed(EventInstance instance)
        {
            instance.getUserData(out var userdata);
            Marshal.FreeHGlobal((GCHandle.FromIntPtr(userdata).Target as PlayInfo).key);
            GCHandle.FromIntPtr(userdata).Free();
            return RESULT.OK;
        }

        public static PROGRAMMER_SOUND_PROPERTIES GetProgrammerSoundProperties(IntPtr ptr)
        {
            return Marshal.PtrToStructure<PROGRAMMER_SOUND_PROPERTIES>(ptr);
        }


    }
}
