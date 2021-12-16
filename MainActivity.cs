using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Views.InputMethods;
using Com.Optiscan.Scanner;
using Android.Widget;
using Android.Content;
using Com.Optiscan;
using Android.Util;
using Android.Content.PM;
using Android;
using System;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Com.Optiscan.Scanner.Model;
using Com.Optiscan.Scanner.Model;
using Com.Optiscan.Scanner;
using Com.Optiscan.Scanner.Tensorflow.View;
using Com.Optiscan;
using Google.Android.Material.Slider;

namespace barcodescanner_xamarin_samples
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ActivityCompat.IOnRequestPermissionsResultCallback, CameraScan.IOnScanResultCallback
    {
        InputMethodManager imm;
        string TAG = "OPTISCAN";
        CameraScan mCameraScan;
        private int REQUEST_CAMERA = 0;
        private int REQUEST_STORAGE = 1;
        private ImageView ivFlash;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            imm = (InputMethodManager)GetSystemService(Context.InputMethodService);

            var previewView = FindViewById<AndroidX.Camera.View.PreviewView>(Resource.Id.previewView);
            ivFlash = (ImageView)FindViewById<ImageView>(Resource.Id.ivFlash);
            OverlayView trackingOverlay = (OverlayView)FindViewById<OverlayView>(Resource.Id.tracking_overlay);
            Slider sliderExposure = (Slider)FindViewById<Slider>(Resource.Id.sliderExposure);



            Java.Lang.JavaSystem.LoadLibrary("opencv_java4");

            mCameraScan = OptiScanFactory.Instance.CreateScanSession(this, (AndroidX.Camera.View.PreviewView)previewView);

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

            ivFlash.Click += (sender, e) =>
            {
                toggleTorchState();
            };

            ShowCamera();
        }



        /* public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
{
    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}*/

        private void toggleTorchState()
        {
            if (mCameraScan == null) return;

            bool isTorch = mCameraScan.IsTorchEnabled;
            mCameraScan.EnableTorch(!isTorch);
            ivFlash.Selected = !isTorch;

        }

        public void ShowCamera()
        {
            // Check if the Camera permission is already available.
            if (ActivityCompat.CheckSelfPermission(this, Android.Manifest.Permission.Camera) != (int)Permission.Granted ||
                ActivityCompat.CheckSelfPermission(this, Android.Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            {

                // Camera permission has not been granted
                RequestCameraPermission();
            }
            else
            {
                // Camera permissions is already available, show the camera preview.
                Log.Info(TAG, "CAMERA permission has already been granted. Displaying camera preview.");
                mCameraScan.StartCamera();
            }
        }

        void RequestCameraPermission()
        {
            Log.Info(TAG, "CAMERA permission has NOT been granted. Requesting permission.");

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                // Provide an additional rationale to the user if the permission was not granted
                // and the user would benefit from additional context for the use of the permission.
                // For example if the user has previously denied the permission.
                Log.Info(TAG, "Displaying camera permission rationale to provide additional context.");

                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, REQUEST_CAMERA);

            }
            else
            {
                // Camera permission has not been granted yet. Request it directly.
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, REQUEST_CAMERA);
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == REQUEST_CAMERA)
            {
                // Received permission result for camera permission.
                Log.Info(TAG, "Received response for Camera permission request.");

                // Check if the only required permission has been granted
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    // Camera permission has been granted, preview can be displayed
                    Log.Info(TAG, "CAMERA permission has now been granted. Showing preview.");
                    mCameraScan.StartCamera();
                }
                else
                {
                    Log.Info(TAG, "CAMERA permission was NOT granted.");
                }
            }
            else if (requestCode == REQUEST_STORAGE)
            {
                Log.Info(TAG, "Received response for contact permissions request.");

                // We have requested multiple permissions for contacts, so all of them need to be
                // checked.
                //if (PermissionUtil.VerifyPermissions(grantResults))
                //{
                //    // All required permissions have been granted, display contacts fragment.
                //}
                //else
                //{
                //    Log.Info(TAG, "Contacts permissions were NOT granted.");
                //}

            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        public void OnScanResultFailure(string error)
        {
            Log.Error(TAG, error);
        }

        public void OnScanResultSuccess(ScanResult result)
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this.ApplicationContext, "Decode Value: " + result.Text, ToastLength.Long).Show();
            });
            Log.Debug(TAG, "Decode Value: " + result.Text);

        }


    }
}