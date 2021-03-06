package ws.winx.hid;


import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Semaphore;
import android.hardware.usb.UsbConstants;
import android.hardware.usb.UsbDevice;
import android.hardware.usb.UsbDeviceConnection;
import android.hardware.usb.UsbEndpoint;
import android.hardware.usb.UsbInterface;
import android.hardware.usb.UsbManager;
import android.util.Log;

public class HIDDeviceWrapper {

	protected static final String TAG = "Unity";//"HIDDeviceWrapper"
	private UsbDeviceConnection __connection;
	UsbDevice _device;
	private UsbEndpoint _readEndpoint;


	UsbManager _usbManager;


	
	boolean _lockEndpoint=false;
	public final int index;
	public final int VID;
	public final int PID;
	private UsbInterface __controlInterface;
	public final String path;
	
	private static ExecutorService __executor=Executors.newCachedThreadPool();
	//private static final ReentrantLock __lock = new ReentrantLock();
	private static final Semaphore __endPointLock=new Semaphore(1);
	private static ReadRunnable __readRunnable;

	

	public HIDDeviceWrapper(UsbDevice device, UsbManager usbManager) {

		this.index=device.getDeviceId();
		this.VID=device.getVendorId();
		this.PID=device.getProductId();
		this.path=device.getDeviceName();
		
		_device = device;
		_usbManager = usbManager;
	}

	public boolean open() {
		
		 Log.d(TAG, "Try to open device");
		 Log.d(TAG, "Number Interfaces=" + _device.getInterfaceCount());
         Log.d(TAG, "Number Endpoints=" + _device.getInterface(0).getEndpointCount());
        
		
		__connection=_usbManager.openDevice(_device);

		if (_device.getInterfaceCount() < 0) {
			Log.e(TAG, "No interface to be clamed.");
			return false;
		}

		__controlInterface = _device.getInterface(0);

		if (!get_connection().claimInterface(__controlInterface, true)) {
			Log.e(TAG, "Could not claim control interface.");
			return false;
		}

		   _readEndpoint = __controlInterface.getEndpoint(0);
	        if (_readEndpoint.getDirection()!=UsbConstants.USB_DIR_IN || _readEndpoint.getType() != UsbConstants.USB_ENDPOINT_XFER_INT) {
	            Log.e(TAG, "Endpoint is not interrupt type");
	            return false;
	        }
	        
	        Log.d(TAG, "End point Address:" + _readEndpoint.getAddress());
	         Log.d(TAG, "End point Type:" + _readEndpoint.getType());//type=3 USB_ENDPOINT_XFER_INT (Interupt)
	         Log.d(TAG, "End point Direction:" + _readEndpoint.getDirection());//type=128 USB_DIR_IN
			
		
		
		
		return true;

	}



	public void Dispose() {
		if (__connection != null){
					
					if(__controlInterface!=null)
						get_connection().releaseInterface(__controlInterface);
					
				}
		
		HIDDeviceWrapper.__executor.shutdownNow();
		
	}
	
	public void write(byte[] from, int timeout) {

		
		
		Log.d(TAG,"write "+HexDump.dumpHexString((byte[]) from)+" size"+((byte[]) from).length);
		HIDDeviceWrapper.__executor.execute(new WriteRunnable(this).read(from).timeout(timeout));
		//((Activity)AndroidHID.context).runOnUiThread(__writeRunnable.read(from).timeout(timeout));


	}

	public void write(byte[] from, IReadWriteListener listener, int timeout) {

		
		
		//!!! maybe build pool of WriteRunnable 
		Log.d(TAG,"write with listener:"+HexDump.dumpHexString((byte[]) from)+" size"+((byte[]) from).length);
		HIDDeviceWrapper.__executor.execute(new WriteRunnable(this).read(from).timeout(timeout).addEventListener(listener));
	    //((Activity)AndroidHID.context).runOnUiThread(__writeRunnable.read(from).timeout(timeout).addEventListener(listener));


	}

	public void read(byte[] into,IReadWriteListener listener, int timeout) {

		if(__readRunnable==null)
		    __readRunnable=new ReadRunnable(this);
		

			
		//as read is done with requestWait() block thread until Interupt, so another queue on same endpoint would
		//cancel previous so prevent multiply read calls	
		if(__readRunnable.is_isReady()) 
		//((Activity)AndroidHID.context).runOnUiThread(__readRunnable.read((byte[])into).timeout(timeout).addEventListener(listener));
			  HIDDeviceWrapper.__executor.execute(__readRunnable.read(into).timeout(timeout).addEventListener(listener));
		
		 

		
	}

	public static Semaphore getEndPointlock() {
		return __endPointLock;
	}

	public UsbDeviceConnection get_connection() {
		return __connection;
	}

	public static ExecutorService get__executor() {
		return __executor;
	}

	public UsbEndpoint get_readEndpoint() {
		return _readEndpoint;
	}





	
}
