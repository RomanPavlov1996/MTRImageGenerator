using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MTRImageGenerator
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		private int imageCounter = 0;
		public Bitmap Photo;

		public MainWindow ( )
		{
			InitializeComponent( );
		}

		private void btnReset_Click ( object sender, RoutedEventArgs e )
		{
			txtName.Text = null;
			txtPhone.Text = null;
			txtDescription.Text = null;
		}

		private void btnGenerate_Click ( object sender, RoutedEventArgs e )
		{
			Bitmap myBitmap = new Bitmap( "Background.png" );
			Graphics myGraphics = Graphics.FromImage( myBitmap );
			Font myFont = new Font( "Arial", 13 );
			Font DescFont = new Font( "Arial", 10, System.Drawing.FontStyle.Italic );
			Brush myBrush = Brushes.Black;
			float offsetX = 45;
			float offsetY = 10;
			int NameLength = 20;
			int spacesCount = 1;

			if ( txtName.Text.Length > NameLength )
			{
				string [ ] parts = txtName.Text.Split( ' ' );
				spacesCount = parts.Length;

				for ( int i = 0; i < parts.Length; i++ )
				{
					myGraphics.DrawString( parts [ i ], myFont, myBrush, new PointF( offsetX, offsetY + ( 15 * i ) ) );
				}
			}
			else
			{
				myGraphics.DrawString( txtName.Text, myFont, myBrush, new PointF( offsetX, offsetY ) );
			}

			myGraphics.DrawString( "Отзывы по телефону:", DescFont, myBrush, new PointF( offsetX, offsetY + ( spacesCount * 14 ) + 5 ) );
			myGraphics.DrawString( txtPhone.Text, DescFont, myBrush, new PointF( offsetX, offsetY + ( spacesCount * 14 + 21 ) ) );
			myGraphics.DrawString( txtDescription.Text, DescFont, myBrush, new PointF( offsetX, offsetY + 43 + spacesCount * 12 ) );


			myGraphics.DrawImage(Photo, 246, 0, 90, 120);

			Directory.CreateDirectory( Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory ) + "\\Экспорт из генератора изображений" );
			string savePath = Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory ) + "\\Экспорт из генератора изображений\\" + imageCounter + ".png";
			myBitmap.Save( savePath, ImageFormat.Png );

			imageCounter++;
			ckbReady.Visibility = Visibility.Visible;
		}

		private void textBox_TextChanged ( object sender, System.Windows.Controls.TextChangedEventArgs e )
		{
			ckbReady.Visibility = System.Windows.Visibility.Hidden;
		}

		private void btnSave_Click ( object sender, RoutedEventArgs e )
		{
			using ( StreamWriter TempFile = File.CreateText( "Template.txt" ) )
			{
				TempFile.Write( txtDescription.Text );
			}
		}

		private void btnOpen_Click ( object sender, RoutedEventArgs e )
		{
			using ( StreamReader TempFile = File.OpenText( "Template.txt" ) )
			{
				txtDescription.Text = TempFile.ReadToEnd( );
			}
		}

		private void btnTakePhoto_Click ( object sender, RoutedEventArgs e )
		{
			var photoWindow = new PhotoWindow( this );
			photoWindow.ShowDialog();
		}
	}
}
