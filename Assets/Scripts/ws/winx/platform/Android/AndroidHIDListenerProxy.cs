﻿#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ws.winx.platform.android
{
    public class AndroidHIDListenerProxy : AndroidJavaProxy/* count this as Interface so you need to implement */
	{
		public event EventHandler<AndroidMessageArgs<AndroidJavaObject>> DeviceConnectedEvent;
        public event EventHandler<AndroidMessageArgs<int>> DeviceDisconnectedEvent;
        public event EventHandler Error;

        public AndroidHIDListenerProxy() : base("ws.winx.hid.IHIDListener") { }
        void onAttached(AndroidJavaObject device) {
            UnityEngine.Debug.Log("AndroidHIDListenerProxy >>>>> AndroidHID" + device);  
            
			if(DeviceConnectedEvent!=null) DeviceConnectedEvent(this,new AndroidMessageArgs<AndroidJavaObject>(device));}
        void onDetached(int pid) { if (DeviceDisconnectedEvent != null) DeviceDisconnectedEvent(this, new AndroidMessageArgs<int>(pid)); }
        void onError(String error) { if (Error != null) Error(this, new AndroidMessageArgs<String>(error)); }
	}
}
#endif
