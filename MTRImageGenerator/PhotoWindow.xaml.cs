using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace MTRImageGenerator
{
	/// <summary>
	/// Логика взаимодействия для PhotoWindow.xaml
	/// </summary>
	public partial class PhotoWindow : Elysium.Controls.Window
	{
		private FilterInfoCollection videodevices;
		private VideoCaptureDevice videoSource;
		private Bitmap photo;
		private bool shouldCapture = false;
		private bool shouldRefresh = false;
		private MainWindow mainWindow;

		public PhotoWindow ( MainWindow mainWindow )
		{
			InitializeComponent( );

			this.mainWindow = mainWindow;

			var videoSourcePlayer = new VideoSourcePlayer( );
			wfhVideoPlayer.Child = videoSourcePlayer;
			videodevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );
			var videoCaptureDevice = new VideoCaptureDevice( videodevices [ 0 ].MonikerString );
			videoSource = videoCaptureDevice;

			videoSourcePlayer.VideoSource = videoSource;
			videoSource.NewFrame += videoSource_NewFrame;
			videoSource.Start( );
		}

		void videoSource_NewFrame ( object sender, NewFrameEventArgs eventArgs )
		{
			if ( shouldCapture )
			{
				shouldCapture = false;

				int height = eventArgs.Frame.Height;
				int width = eventArgs.Frame.Width / (height / 240);

				photo = eventArgs.Frame.Clone( new Rectangle( 0, 0, width, height ), PixelFormat.DontCare );
				Dispatcher.Invoke( DispatcherPriority.Send, ( Action ) ( ( ) => { imgPhoto.Source = photo.ToBitmapImage( ); } ) );
			}
		}

		private void btnCapture_Click ( object sender, RoutedEventArgs e )
		{
			shouldCapture = true;
			btnSave.IsEnabled = true;
		}

		private void btnSave_Click ( object sender, RoutedEventArgs e )
		{
			mainWindow.Photo = photo;
			Close( );
		}

		private void Window_Closing ( object sender, System.ComponentModel.CancelEventArgs e )
		{
			videoSource.Stop();
		}
	}

	public static class MyExtensions
	{
		public static BitmapImage ToBitmapImage ( this Bitmap bitmap )
		{
			using ( MemoryStream memory = new MemoryStream( ) )
			{
				bitmap.Save( memory, ImageFormat.Png );
				memory.Position = 0;
				BitmapImage bitmapImage = new BitmapImage( );
				bitmapImage.BeginInit( );
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit( );

				return bitmapImage;
			}
		}
	}
}
