# OptiScan Xamarin Demo APP 

OptiScan SDK Integration Steps 

To use our Xamarin package in another app , proceed as follows: 

1.Please Download and install the nuget package and mentioned version 
Dependency Required
```
<PackageReference Include="OptiscanXamarin">
      <Version>1.0.0</Version>
</PackageReference>
<PackageReference Include="Xamarin.Google.Android.Material">
      <Version>1.3.0</Version>
</PackageReference>
    <PackageReference Include="Xamarin.Google.Dagger">
      <Version>2.37.0</Version>
</PackageReference>
```
## Runtime Permission 

Add the below camera permission to your AndroidManifest.xml file 
```gradle 
<uses-permission android:name="android.permission.CAMERA" /> 
```
 

## Add UI dependency in application 

Add the below UI view components in application layout file. 
```gradle
<androidx.camera.view.PreviewView 
    android:id="@+id/previewView" 
    android:layout_width="match_parent" 
    android:layout_height="match_parent" /> 
 
<com.obs.optiscan.scanner.ViewFinderView 
    android:id="@+id/viewfinderView" 
    android:layout_width="match_parent" 
    android:layout_height="match_parent" /> 
 
<com.obs.optiscan.scanner.tensorflow.view.OverlayView 
    android:id="@+id/tracking_overlay" 
    android:layout_width="match_parent" 
    android:layout_height="match_parent" /> 
```
 

These three view components are mandatory. Camera preview used to display the preview for camera and ViewFinderView and OverlayView are used to show bounding box once we got success result by decode process. 

 

## Simple Usage access from Activity 

In Activity we need to initialize the OptiscanFactory to create a session for scanning process. It will return CameraScan object. 
```gradle
 CameraScan mCameraScan = OptiScanFactory.Instance.CreateScanSession(this, (AndroidX.Camera.View.PreviewView)previewView);
 mCameraScan.SetOnScanResultCallback(this)
           ?.SetPlayBeep(true)
           ?.SetBeepSoundResource(Resource.Raw.beep)
           ?.SetVibrate(false)
           ?.SetCameraConfig(new CameraConfig())
           ?.SetAutoFlashlight(true)
           ?.SetAutoExposure(false)
           ?.SetDebugMode(false)
           ?.SetScanType("any")
           ?.SetDarkLightLux(4f)
           ?.SetBrightLightLux(100f)
           ?.SetQrBarcodeDetection(true)
           ?.BindFlashlightView(ivFlash)
           ?.BindOverlayView(trackingOverlay)
           ?.UpdateConfidenceValue(0.5f)
           ?.BindSliderView(sliderExposure)
           ?.SetIsContinuousScan(true); 
```
 We can get decode value result in below callback methods. It will be triggered once input frame processed. 
```gradle
  public void OnScanResultSuccess(ScanResult result)
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this.ApplicationContext, "Decode Value: " + result.Text, ToastLength.Long).Show();
            });
            Log.Debug(TAG, "Decode Value: " + result.Text);

        }
        
         public void OnScanResultFailure(string error)
        {
            Log.Error(TAG, error);
        }
```
