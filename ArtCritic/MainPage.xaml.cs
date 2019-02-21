using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.AI.MachineLearning;
using Windows.Media;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace MNIST_Demo
{
    public sealed partial class MainPage : Page
    {
        private Helper                  helper = new Helper();
        RenderTargetBitmap              renderBitmap = new RenderTargetBitmap();

        private ArtCriticModel ModelGen;
        private ArtCriticInput ModelInput = new ArtCriticInput();
        private ArtCriticOutput ModelOutput;


        public MainPage()
        {
            this.InitializeComponent();
            
            // Set supported inking device types.
            inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(
                new Windows.UI.Input.Inking.InkDrawingAttributes()
                {
                    Color = Windows.UI.Colors.Black,
                    Size = new Size(10, 10),
                    IgnorePressure = true,
                    IgnoreTilt = true,
                }
            );
            LoadModelAsync();
        }

        private async Task LoadModelAsync()
        {
            //Load a machine learning model
            StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/ArtCritic.onnx"));
            ModelGen = await ArtCriticModel.CreateFromStreamAsync(modelFile as IRandomAccessStreamReference);
        }

private async void recognizeButton_Click(object sender, RoutedEventArgs e)
{
    //Bind model input with contents from InkCanvas
    VideoFrame vf = await helper.GetHandWrittenImage(inkGrid);

    ModelInput.data = ImageFeatureValue.CreateFromVideoFrame(vf);
    // Evaluate the model
    ModelOutput = await ModelGen.EvaluateAsync(ModelInput);

    //Display the results
    numberLabel.Text = ModelOutput.classLabel.GetAsVectorView()[0];
}

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.StrokeContainer.Clear();
            numberLabel.Text = "";
        }
    }
}
