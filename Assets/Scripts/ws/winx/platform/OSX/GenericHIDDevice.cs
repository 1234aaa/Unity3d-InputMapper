// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX 
using System;
using ws.winx.platform;
using ws.winx.devices;
using System.Runtime.InteropServices;

namespace ws.winx.platform.osx
{

	
	using CFString = System.IntPtr;

	
	using CFTypeRef = System.IntPtr;
	using IOHIDDeviceRef = System.IntPtr;
	using IOHIDElementRef = System.IntPtr;
	
	using IOHIDValueRef = System.IntPtr;
	using IOOptionBits = System.IntPtr;
	using IOReturn =Native.IOReturn;// System.IntPtr;
	
	using IOHIDElementType=Native.IOHIDElementType;
	using CFIndex =System.Int64;

	public class HidAsyncState
	{
		private readonly object _callerDelegate;
		private readonly object _callbackDelegate;
		
		public HidAsyncState(object callerDelegate, object callbackDelegate)
		{
			_callerDelegate = callerDelegate;
			_callbackDelegate = callbackDelegate;
		}
		
		public object CallerDelegate { get { return _callerDelegate; } }
		public object CallbackDelegate { get { return _callbackDelegate; } }
	}
	
	
	
	public class GenericHIDDevice:HIDDevice
	{
		
		protected delegate HIDReport ReadDelegate(int timeout);
		private delegate bool WriteDelegate(byte[] data,int timeout);
		
		readonly IntPtr RunLoop = Native.CFRunLoopGetCurrent();

		volatile bool IsReadInProgress = false;

		private HIDReport __lastHIDReport;

		public bool IsOpen { get; private set; }
			
		private int _InputReportByteLength=16;
			
			override public int InputReportByteLength
			{
				get { return _InputReportByteLength; }
				set {
					if (value < 2) throw new Exception("InputReportByteLength should be >1 ");     
                    _InputReportByteLength = value;
                    __lastHIDReport.Data = CreateInputBuffer();
                }
			}
		private int _OutputReportByteLength=8;
			
			override public int OutputReportByteLength
			{
				get { return _OutputReportByteLength; }
				set { if (value < 2) throw new Exception("InputReportByteLength should be >1 ");  _OutputReportByteLength = value; }
			}

		GCHandle DeviceGCHandle;
		bool isDeviceGCHandleIntialized=false;

		private IntPtr __deviceHandle;

				public GenericHIDDevice (int index, int VID, int PID, IntPtr deviceHandle, IHIDInterface hidInterface, string devicePath, 
		        string name = ""):base(index,VID,PID,deviceHandle,hidInterface,devicePath,name)
				{
					__deviceHandle = deviceHandle;

			        try{

				OpenDevice();
				CloseDevice();


				__lastHIDReport = new HIDReport(this.index, CreateInputBuffer(),HIDReport.ReadStatus.Success);	



			}catch(Exception e){

				UnityEngine.Debug.LogException(e);
						}
					
				}

		public void Test ()
		{
						byte[] buffer = new byte[8];

			IOReturn result=Native.IOHIDDeviceOpen (__deviceHandle, 0);
			if (result != Native.IOReturn.kIOReturnSuccess)
								return;
								
			
						IntPtr bufferIntPtr = Marshal.AllocHGlobal (buffer.Length);
						Marshal.Copy (buffer, 0, bufferIntPtr, buffer.Length);

		
						if (!isDeviceGCHandleIntialized) {
							DeviceGCHandle = GCHandle.Alloc (this);	
						}
			
			
						//				IOHIDDeviceScheduleWithRunLoop(dev, CFRunLoopGetCurrent(), kCFRunLoopDefaultMode);
						//				IOHIDDeviceRegisterInputReportCallback(dev, h->buffer, sizeof(h->buffer),
						//				                                       input_callback, h);
			
						// Schedule the device on the current run loop in case it isn't already scheduled
			Native.IOHIDDeviceScheduleWithRunLoop (__deviceHandle, Native.CFRunLoopGetCurrent(), Native.RunLoopModeDefault);
			
			
			
						// Register a callback		
						Native.IOHIDDeviceRegisterInputReportCallback (__deviceHandle, bufferIntPtr, 8, InputReportCallback,
			                                              
			                                              GCHandle.ToIntPtr (DeviceGCHandle));


				}


		private byte[] CreateInputBuffer()
		{
			return CreateBuffer((int)InputReportByteLength - 1);
		}
		
		private byte[] CreateOutputBuffer()
		{
			return CreateBuffer((int)OutputReportByteLength - 1);
		}

		private static byte[] CreateBuffer(int length)
		{
			byte[] buffer = null;
			Array.Resize(ref buffer, length + 1);
			return buffer;
		}

		public override HIDReport ReadDefault ()
		{
			if (IsOpen == false) OpenDevice();

			if (!isDeviceGCHandleIntialized) {
								try {
				
				
				
										DeviceGCHandle = GCHandle.Alloc (this);			
				
										// The device is not normally available in the InputValueCallback (HandleDeviceValueReceived), so we include
										// the device identifier as the context variable, so we can identify it and figure out the device later.
				
				
										Native.IOHIDDeviceRegisterInputValueCallback (__deviceHandle, DeviceValueReceived, GCHandle.ToIntPtr (DeviceGCHandle));
				
										Native.IOHIDDeviceScheduleWithRunLoop (__deviceHandle, RunLoop, Native.RunLoopModeDefault);

										isDeviceGCHandleIntialized=true;
						
				
								} catch (Exception e) {
										UnityEngine.Debug.LogException (e);
								}
						}
			
			return __lastHIDReport;
		}
		
		
		internal void OutputReportCallback(
			IntPtr          inContext,          // context from IOHIDDeviceRegisterInputReportCallback
			IOReturn        inResult,           // completion result for the input report operation
			IOHIDDeviceRef   inSender,           // IOHIDDeviceRef of the device this report is from
			Native.IOHIDReportType inType,             // the report type
			uint        inReportID,         // the report ID
			IntPtr       inReport,           // pointer to the report data
			int         inReportLength ){
			

			
		}


		//static void input_callback(void *context, IOReturn ret, void *sender,
		//IOHIDReportType type, uint32_t id, uint8_t *data, CFIndex len)
			internal void InputReportCallback(
			IntPtr          inContext,          // context from IOHIDDeviceRegisterInputReportCallback
			IOReturn        inResult,           // completion result for the input report operation
			IOHIDDeviceRef   inSender,           // IOHIDDeviceRef of the device this report is from
			Native.IOHIDReportType inType,             // the report type
			uint        inReportID,         // the report ID
			IntPtr       inReport,           // pointer to the report data
			int         inReportLength ){


		
			GenericHIDDevice hidDevice = (GenericHIDDevice)GCHandle.FromIntPtr(inContext).Target;

			if (hidDevice.__deviceHandle != inSender)
								return;

			   byte[] buffer = new byte[hidDevice.InputReportByteLength];
			Marshal.Copy(inReport, buffer, 0,hidDevice.InputReportByteLength);




			hidDevice.__lastHIDReport.Data = buffer;
			hidDevice.__lastHIDReport.Status = HIDReport.ReadStatus.Success;

			UnityEngine.Debug.Log (BitConverter.ToString (buffer));

			hidDevice.IsReadInProgress = false;

			//Marshal.FreeHGlobal (inReport);	
			//Native.IOHIDDeviceRegisterInputReportCallback (hidDevice.__deviceHandle, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero);
				
				
	    }
		
		/// <summary>
		/// Devices the value received.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="res">Res.</param>
		/// <param name="sender">Sender.</param>
		/// <param name="valRef">Value reference.</param>
		internal void DeviceValueReceived(IntPtr context, IOReturn res, IntPtr sender, IOHIDValueRef valRef)
		{

			
			IOHIDElementRef element = Native.IOHIDValueGetElement(valRef);
			uint uid=Native.IOHIDElementGetCookie(element);
			long value;
			Native.IOHIDElementType  type = Native.IOHIDElementGetType(element);
			GenericHIDDevice device;

			value = Native.IOHIDValueGetIntegerValue (valRef);

			UnityEngine.Debug.Log ("DeviceValueReceived:"+value);

			
			try{
				GCHandle gch = GCHandle.FromIntPtr(context);
				device=(GenericHIDDevice) gch.Target;
				
			}
			catch(Exception e){
				UnityEngine.Debug.LogException(e);
				return;
			}


		

			byte[] typeBuff = BitConverter.GetBytes ((uint)type);
			byte[] uidBuff = BitConverter.GetBytes (uid);
			byte[] valueBuff = BitConverter.GetBytes (value);

			
		    byte[] result = new byte[ typeBuff.Length + uidBuff.Length + valueBuff.Length ];
			System.Buffer.BlockCopy( typeBuff, 0, result, 0, typeBuff.Length );
			System.Buffer.BlockCopy( uidBuff, 0, result, typeBuff.Length, uidBuff.Length );
			System.Buffer.BlockCopy( valueBuff, 0, result, typeBuff.Length + uidBuff.Length, valueBuff.Length );
			



			device.__lastHIDReport.Data=result;
			
		}


        override public HIDReport ReadBuffered()
        {
            if (IsReadInProgress)
            {
                __lastHIDReport.Status = HIDReport.ReadStatus.Buffered;
                return __lastHIDReport;
            }

            IsReadInProgress = true;

			if(!IsOpen) OpenDevice();

			if (IsOpen) {
								byte[] buffer =CreateInputBuffer();
		
			
			
								IntPtr bufferIntPtr = Marshal.AllocHGlobal (buffer.Length);
								Marshal.Copy (buffer, 0, bufferIntPtr, buffer.Length);
			
			
								if (!isDeviceGCHandleIntialized) {
										DeviceGCHandle = GCHandle.Alloc (this);	
								
								isDeviceGCHandleIntialized=true;
			
							
								// Schedule the device on the current run loop in case it isn't already scheduled
								Native.IOHIDDeviceScheduleWithRunLoop (__deviceHandle, Native.CFRunLoopGetCurrent (), Native.RunLoopModeDefault);
			
								
			
								// Register a callback		
								Native.IOHIDDeviceRegisterInputReportCallback (__deviceHandle, bufferIntPtr, buffer.Length, InputReportCallback,
			                                               
			                                               GCHandle.ToIntPtr (DeviceGCHandle));

				}
								

						}

            return __lastHIDReport;

        }



        protected void EndReadBuffered(IAsyncResult ar)
        {

            var callerDelegate = (ReadDelegate)ar.AsyncState;

            callerDelegate.EndInvoke(ar);


            IsReadInProgress = false;
        }

		override public void Read(ReadCallback callback)
		{
			Read(callback, 0);
		}
		
		override public void Read(ReadCallback callback,int timeout)
		{
			if (IsReadInProgress)
			{
				//UnityEngine.Debug.Log("Clone paket");
				__lastHIDReport.Status = HIDReport.ReadStatus.Buffered;
				callback.BeginInvoke(__lastHIDReport, EndReadCallback, callback);
				// callback.Invoke(__lastHIDReport);
				return;
			}
			
			IsReadInProgress = true;
			
			//TODO make this fields or use pool
			var readDelegate = new ReadDelegate(Read);
			var asyncState = new HidAsyncState(readDelegate, callback);
			readDelegate.BeginInvoke(timeout,EndRead, asyncState);
		}





		protected HIDReport Read(int timeout)
		{
			
			if (IsOpen == false) OpenDevice();
			try
			{
				return ReadData(timeout);
			}
			catch
			{
				return new HIDReport(-1,null,HIDReport.ReadStatus.ReadError);
			}
			
			
			
		}

		protected HIDReport ReadData(int timeout)
		{
			var buffer = new byte[] { };
			var status = HIDReport.ReadStatus.NoDataRead;
		
			if (InputReportByteLength > 0)
			{

				
				buffer = CreateInputBuffer();


				IntPtr bufferIntPtr = Marshal.AllocHGlobal(buffer.Length);
				Marshal.Copy(buffer, 0, bufferIntPtr, buffer.Length);

				if(isDeviceGCHandleIntialized){//might use GCHanled.isAllocated
					DeviceGCHandle = GCHandle.Alloc (this);	
				}


//				IOHIDDeviceScheduleWithRunLoop(dev, CFRunLoopGetCurrent(), kCFRunLoopDefaultMode);
//				IOHIDDeviceRegisterInputReportCallback(dev, h->buffer, sizeof(h->buffer),
//				                                       input_callback, h);

//				// Schedule the device on the current run loop in case it isn't already scheduled
//				Native.IOHIDDeviceScheduleWithRunLoop(__deviceHandle, RunLoop, Native.RunLoopModeDefault);
//				
//
//				
//				// Register a callback		
//				Native.IOHIDDeviceRegisterInputReportCallback(__deviceHandle, bufferIntPtr, InputReportByteLength,InputReportCallback, 
//				                                              GCHandle.ToIntPtr(DeviceGCHandle) );
//				
//
//				
//				Marshal.FreeHGlobal(bufferIntPtr);

				
				if (timeout>0)
				{
					// Trap in the run loop until a report is received
					Native.CFRunLoopRunInMode(Native.RunLoopModeDefault,timeout,false);
					
					// The run loop has returned, so unschedule the device
					Native.IOHIDDeviceUnscheduleFromRunLoop(__deviceHandle, RunLoop, Native.RunLoopModeDefault);
				}
				else
				{
					try
					{



//						// Trap in the run loop until a report is received
//						Native.CFRunLoopRun();
//						
//						// The run loop has returned, so unschedule the device
//						Native.IOHIDDeviceUnscheduleFromRunLoop(__deviceHandle, RunLoop, Native.RunLoopModeDefault);
//
//						Native.IOHIDDeviceRegisterInputValueCallback (__deviceHandle, IntPtr.Zero, IntPtr.Zero);
//						
					}
					catch { status = HIDReport.ReadStatus.ReadError; }
				}
			}
			
			
			__lastHIDReport.Data = buffer;
			
			__lastHIDReport.index = this.index;
			
			__lastHIDReport.Status = status;
			
			
			return __lastHIDReport;// new HIDReport(this.index, buffer, status);
		}
		

		protected void EndReadCallback(IAsyncResult ar)
		{
			// Because you passed your original delegate in the asyncState parameter
			// of the Begin call, you can get it back here to complete the call.
			ReadCallback dlgt = (ReadCallback)ar.AsyncState;
			
			// Complete the call.
			dlgt.EndInvoke(ar);
		}
		
		protected void EndRead(IAsyncResult ar)
		{
					
			var hidAsyncState = (HidAsyncState)ar.AsyncState;
			var callerDelegate = (ReadDelegate)hidAsyncState.CallerDelegate;
			var callbackDelegate = (ReadCallback)hidAsyncState.CallbackDelegate;
			var data = callerDelegate.EndInvoke(ar);
			
			
			if ((callbackDelegate != null)) callbackDelegate.BeginInvoke(data, EndReadCallback, callbackDelegate);
			
			//if ((callbackDelegate != null)) callbackDelegate.Invoke(data);
			
			IsReadInProgress = false;
		}



		public override void Write(object data, HIDDevice.WriteCallback callback,int timeout)
		{
			
			var writeDelegate = new WriteDelegate(Write);
			var asyncState = new HidAsyncState(writeDelegate, callback);
			writeDelegate.BeginInvoke((byte[])data,timeout, EndWrite, asyncState);
			
		}
		
		public override void Write(object data, HIDDevice.WriteCallback callback)
		{
			this.Write((byte[])data,callback, 0);
		}
		
		
		/// <summary>
		/// Syncro write
		/// </summary>
		/// <param name="data"></param>
		public override void Write(object data)
		{
			this.WriteData((byte[])data,0);
		}



		protected bool Write(byte[] data, int timeout)
		{
			
			if (IsOpen == false) OpenDevice();
			try
			{
				return WriteData(data, timeout);
			}
			catch
			{
				return false;
			}
			
		}
		
		
		private bool WriteData(byte[] data, int timeout)
		{
			
			
			var buffer = CreateOutputBuffer();

			
			Array.Copy(data, 1, buffer, 0, data.Length-1);

			IntPtr bufferIntPtr = Marshal.AllocHGlobal(buffer.Length);
			Marshal.Copy(buffer, 0, bufferIntPtr, buffer.Length);

			

			if (timeout>0)
			{
				return IOReturn.kIOReturnSuccess== Native.IOHIDDeviceSetReportWithCallback(
					__deviceHandle,
					Native.IOHIDReportType.kIOHIDReportTypeOutput,
					data[0],
					bufferIntPtr,
					buffer.Length,
					timeout,
					OutputReportCallback,
					IntPtr.Zero);

				

			}
			else
			{
				try
				{
					return IOReturn.kIOReturnSuccess == Native.IOHIDDeviceSetReport(__deviceHandle,
					                                 Native.IOHIDReportType.kIOHIDReportTypeOutput,
					                                 data[0],
					                                 bufferIntPtr,
					                                 buffer.Length);
					
				}
				catch(Exception ex) {
					UnityEngine.Debug.LogException(ex);
					return false; }
			}
		}


		
		private void EndWrite(IAsyncResult ar)
		{
			var hidAsyncState = (HidAsyncState)ar.AsyncState;
			var callerDelegate = (WriteDelegate)hidAsyncState.CallerDelegate;
			var callbackDelegate = (WriteCallback)hidAsyncState.CallbackDelegate;
			var result = callerDelegate.EndInvoke(ar);
			
			if ((callbackDelegate != null)) callbackDelegate.BeginInvoke(result, EndWriteCallback, callbackDelegate);
			//if ((callbackDelegate != null)) callbackDelegate.Invoke(result);
		}
		
		
		protected void EndWriteCallback(IAsyncResult ar)
		{
			// Because you passed your original delegate in the asyncState parameter
			// of the Begin call, you can get it back here to complete the call.
			WriteCallback dlgt = (WriteCallback)ar.AsyncState;
			
			// Complete the call.
			dlgt.EndInvoke(ar);
		}
		
		internal void OpenDevice(){
			IOReturn result=Native.IOHIDDeviceOpen(__deviceHandle, (int)Native.IOHIDOptionsType.kIOHIDOptionsTypeNone);
			IsOpen = (result == IOReturn.kIOReturnSuccess);
			
		}
		
		internal void CloseDevice()
		{
			
			if (__deviceHandle != IntPtr.Zero) {


								if (IsOpen)
										IsOpen = !(IOReturn.kIOReturnSuccess == Native.IOHIDDeviceClose (__deviceHandle, (int)Native.IOHIDOptionsType.kIOHIDOptionsTypeNone));
			}

		}
		
		public override void Dispose ()
		{

			
			if (isDeviceGCHandleIntialized) {
				DeviceGCHandle.Free ();
			}




		}
		
		
	}
	
}
#endif