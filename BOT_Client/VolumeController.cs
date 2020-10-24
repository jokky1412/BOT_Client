using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;


public static class VolumeController {
	static VolumeController() {
	}

	public static void VolumeUp(int parUpPercent) {
		NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
		//Get all the devices, no matter what condition or status
		NAudio.CoreAudioApi.MMDevice dev = MMDE.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Communications);

		dev.AudioEndpointVolume.VolumeStepUp();
	}

	public static void VolumeDown(int parDownPercent) {
		NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
		//Get all the devices, no matter what condition or status
		NAudio.CoreAudioApi.MMDevice dev = MMDE.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Communications);

		dev.AudioEndpointVolume.VolumeStepDown();
	}

	public static void SwitchMute() {
		NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
		//Get all the devices, no matter what condition or status
		NAudio.CoreAudioApi.MMDevice dev = MMDE.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Communications);

		dev.AudioEndpointVolume.Mute = !dev.AudioEndpointVolume.Mute;
	}
}
