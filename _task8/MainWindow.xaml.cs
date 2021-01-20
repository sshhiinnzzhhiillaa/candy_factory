using _task8.Domain;
using _task8.Messages;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _task8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<SugarTakenMessage>(this, async message =>
            {
                var sugarIcon = new PackIconMaterial()
                {
                    Kind = PackIconMaterialKind.CubeOutline
                };

                var bigCubeX = Canvas.GetLeft(bigSugarCube) + 25;
                var bigCubeY = Canvas.GetTop(bigSugarCube) + 25;
                var miniCubeX = 310;
                var miniCubeY = 69 * message.ConveyorIndex + 35;

                await MoveIcon(message, sugarIcon, bigCubeX, bigCubeY, miniCubeX, miniCubeY, message.Conveyor.ProcessingTime);

                if (message.Conveyor.IsCrashed)
                {
                    return;
                }

                var candyIcon = new PackIconMaterial()
                {
                    Kind = PackIconMaterialKind.Candycane
                };

                var humanX = 310;
                var candyY = 69 * message.ConveyorIndex + 35;
                var endX = humanX + 200;

                await MoveIcon(message, candyIcon, humanX, candyY, endX, candyY, message.Conveyor.ProcessingTime);
            });
        }

        private async Task MoveIcon(SugarTakenMessage message, PackIconMaterial icon, double fromX, double fromY, double toX, double toY, int milliseconds)
        {
            myCanvas.Children.Add(icon);

            Canvas.SetLeft(icon, fromX);
            Canvas.SetTop(icon, fromY);

            TranslateTransform trans = new TranslateTransform();
            icon.RenderTransform = trans;
            DoubleAnimation animationX = new DoubleAnimation(fromX, toX, TimeSpan.FromMilliseconds(milliseconds));
            DoubleAnimation animationY = new DoubleAnimation(fromY, toY, TimeSpan.FromMilliseconds(milliseconds));

            animationX.Completed += (obj, args) =>
            {
                myCanvas.Children.Remove(icon);
            };

            Storyboard sb = new Storyboard();
            sb.Duration = TimeSpan.FromMilliseconds(milliseconds);

            sb.Children.Add(animationX);
            sb.Children.Add(animationY);

            Storyboard.SetTarget(animationX, icon);
            Storyboard.SetTarget(animationY, icon);

            Storyboard.SetTargetProperty(animationX, new PropertyPath("(Canvas.Left)"));
            Storyboard.SetTargetProperty(animationY, new PropertyPath("(Canvas.Top)"));


            myWindow.Resources.Add(Guid.NewGuid().ToString(), sb);

            sb.Begin();

            await Task.Delay(milliseconds);
        }


    }
}
