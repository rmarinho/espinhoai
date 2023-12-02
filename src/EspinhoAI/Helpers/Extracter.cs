using System;
// using Microsoft.ML.OnnxRuntime;
// using Microsoft.ML.OnnxRuntime.Tensors;
using System.Text;
using SkiaSharp;

namespace EspinhoAI
{
    public class Extracter
	{
    }
//         IVisionSample _mobilenet;
//         IVisionSample _ultraface;

//         IVisionSample Mobilenet => _mobilenet ??= new MobilenetSample();
//         //   IVisionSample Ultraface => _ultraface ??= new UltrafaceSample();

//         List<string> ExecutionProviderOptions = new List<string>();
//         public Extracter()
// 		{
//             // See:
//             // ONNX Runtime Execution Providers: https://onnxruntime.ai/docs/execution-providers/
//             // Core ML: https://developer.apple.com/documentation/coreml
//             // NNAPI: https://developer.android.com/ndk/guides/neuralnetworks
//             ExecutionProviderOptions.Add(nameof(ExecutionProviders.CPU));

//             if (DeviceInfo.Platform == DevicePlatform.Android)
//             {
//                 ExecutionProviderOptions.Add(nameof(ExecutionProviders.NNAPI));
//             }

//             if (DeviceInfo.Platform == DevicePlatform.iOS)
//             {
//                 ExecutionProviderOptions.Add(nameof(ExecutionProviders.CoreML));
//             }

//             //ExecutionProviderOptions.SelectedIndex = 0;

           
//         }

//      //   private readonly InferenceSampleApi inferenceSampleApi;

//         public async Task ExecuteTests(Action<string> addOutput)
//         {
      
//             //addOutput("Testing execution\nComplete output is written to Console in this trivial example.\n\n");

//             //// run the testing in a background thread so updates to the UI aren't blocked
//             //await Task.Run(() =>
//             //{
//             //    //addOutput(OutputLabel, "Testing using default platform-specific session options... ");
//             //    inferenceSampleApi.Execute();
//             //    //addOutput(OutputLabel, "done.\n");
//             //    Thread.Sleep(1000); // artificial delay so the UI updates gradually

//             //    // demonstrate a range of usages by recreating the inference session with different session options.
//             //    //addOutput(OutputLabel, "Testing using default platform-specific session options... ");
//             //    inferenceSampleApi.CreateInferenceSession(SessionOptionsContainer.Create());
//             //    inferenceSampleApi.Execute();
//             //    //addOutput(OutputLabel, "done.\n");
//             //    Thread.Sleep(1000);

//             //    //addOutput(OutputLabel, "Testing using named platform-specific session options... ");
//             //    inferenceSampleApi.CreateInferenceSession(SessionOptionsContainer.Create("ort_with_npu"));
//             //    inferenceSampleApi.Execute();
//             //    //addOutput(OutputLabel, "done.\n");
//             //    Thread.Sleep(1000);

//             //   // addOutput(OutputLabel, "Testing using default platform-specific session options via ApplyConfiguration extension... ");
//             //    inferenceSampleApi.CreateInferenceSession(new SessionOptions().ApplyConfiguration());
//             //    inferenceSampleApi.Execute();
//             // //   addOutput(OutputLabel, "done.\n");
//             //    Thread.Sleep(1000);

//             //  //  addOutput(OutputLabel, "Testing using named platform-specific session options via ApplyConfiguration extension... ");
//             //    inferenceSampleApi.CreateInferenceSession(new SessionOptions().ApplyConfiguration("ort_with_npu"));
//             //    inferenceSampleApi.Execute();
//             //    //addOutput(OutputLabel, "done.\n\n");
//             //    Thread.Sleep(1000);
//             //});

//            // addOutput(OutputLabel, "Testing successfully completed! See the Console log for more info.");
//         }
//     }

//     //public class InferenceSampleApi : IDisposable
//     //{
//     //     public const string Identifier = "Mobilenet V2";
//     //    public const string ModelFilename = "mobilenetv2-12.onnx";


//     //    public InferenceSampleApi()
//     //    {
//     //        _model =  Utils.LoadResource(ModelFilename).Result;

//     //        // this is the data for only one input tensor for this model
//     //        //var inputData = LoadTensorFromEmbeddedResource("TestData.bench.in");

//     //        // create default session with default session options
//     //        // Creating an InferenceSession and loading the model is an expensive operation, so generally you would
//     //        // do this once. InferenceSession.Run can be called multiple times, and concurrently.
//     //        CreateInferenceSession();

//     //        // setup sample input data
//     //        var inputMeta = _inferenceSession.InputMetadata;
//     //        _inputData = new List<OrtValue>(inputMeta.Count);
//     //        _orderedInputNames = new List<string>(inputMeta.Count);

//     //        foreach (var name in inputMeta.Keys)
//     //        {
//     //            // We create an OrtValue in this case over the buffer of potentially different shapes.
//     //            // It is Okay as long as the specified shape does not exceed the actual length of the buffer
//     //            var shape = Array.ConvertAll<int, long>(inputMeta[name].Dimensions, Convert.ToInt64);
//     //            Debug.Assert(ShapeUtils.GetSizeForShape(shape) <= inputData.LongLength);

//     //            var ortValue = OrtValue.CreateTensorValueFromMemory(inputData, shape);
//     //            _inputData.Add(ortValue);

//     //            _orderedInputNames.Add(name);
//     //        }
//     //    }

//     //    public void CreateInferenceSession(SessionOptions options = null)
//     //    {
//     //        // Optional : Create session options and set any relevant values.
//     //        // If an additional execution provider is needed it should be added to the SessionOptions prior to
//     //        // creating the InferenceSession. The CPU Execution Provider is always added by default.
//     //        if (options == null)
//     //        {
//     //            options = new SessionOptions { LogId = "Sample" };
//     //        }

//     //        _inferenceSession = new InferenceSession(_model, options);
//     //    }

//     //    public void Execute()
//     //    {
//     //        // Run the inference
//     //        // 'results' is an IDisposableReadOnlyCollection<OrtValue> container
//     //        using (var results = _inferenceSession.Run(null, _orderedInputNames, _inputData, _inferenceSession.OutputNames))
//     //        {
//     //            // dump the results
//     //            for (int i = 0; i < results.Count; ++i)
//     //            {
//     //                var name = _inferenceSession.OutputNames[i];
//     //                Console.WriteLine("Output for {0}", name);
//     //                // We can now access the native buffer directly from the OrtValue, no copy is involved.
//     //                // Spans are structs and are stack allocated. They do not add any GC pressure.
//     //                ReadOnlySpan<float> span = results[i].GetTensorDataAsSpan<float>();
//     //                Console.Write($"Input {i} results:");
//     //                for (int k = 0; k < span.Length; ++k)
//     //                {
//     //                    Console.Write($" {span[k]}");
//     //                }
//     //                Console.WriteLine();
//     //            }
//     //        }
//     //    }

//     //    protected virtual void Dispose(bool disposing)
//     //    {
//     //        if (disposing && !_disposed)
//     //        {
//     //            _inferenceSession?.Dispose();
//     //            _inferenceSession = null;

//     //            if (_inputData != null)
//     //                foreach (var v in _inputData)
//     //                {
//     //                    v?.Dispose();
//     //                }

//     //            _disposed = true;
//     //        }
//     //    }

//     //    public void Dispose()
//     //    {
//     //        Dispose(true);
//     //        GC.SuppressFinalize(this);
//     //    }

//     //    static float[] LoadTensorFromEmbeddedResource(string path)
//     //    {
//     //        var tensorData = new List<float>();
//     //        var assembly = typeof(InferenceSampleApi).Assembly;

//     //        using (var inputFile =
//     //            new StreamReader(assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{path}")))
//     //        {
//     //            inputFile.ReadLine(); // skip the input name
//     //            string[] dataStr = inputFile.ReadLine().Split(new char[] { ',', '[', ']' },
//     //                                                          StringSplitOptions.RemoveEmptyEntries);
//     //            for (int i = 0; i < dataStr.Length; i++)
//     //            {
//     //                tensorData.Add(Single.Parse(dataStr[i]));
//     //            }
//     //        }

//     //        return tensorData.ToArray();
//     //    }


//     //    private bool _disposed = false;
//     //    private readonly byte[] _model;
//     //    private readonly List<string> _orderedInputNames;
//     //    private readonly List<OrtValue> _inputData;
//     //    private InferenceSession _inferenceSession;
//     //}

    public interface IVisionSample
    {
        string Name { get; }
        string ModelName { get; }
        Task InitializeAsync();
      //  Task UpdateExecutionProviderAsync(ExecutionProviders executionProvider);
        Task<ImageProcessingResult> ProcessImageAsync(byte[] image);
    }

    public class ImageProcessingResult
    {
        public byte[] Image { get; private set; }
        public string Caption { get; private set; }

        internal ImageProcessingResult(byte[] image, string caption = null)
        {
            Image = image;
            Caption = caption;
        }
    }

//     public class VisionSampleBase<TImageProcessor> : IVisionSample where TImageProcessor : new()
//     {
//         byte[] _model;
//         string _name;
//         string _modelName;
//         Task _prevAsyncTask;
//         TImageProcessor _imageProcessor;
//         InferenceSession _session;
//         ExecutionProviders _curExecutionProvider;

//         public VisionSampleBase(string name, string modelName)
//         {
//             _name = name;
//             _modelName = modelName;
//             _ = InitializeAsync();
//         }

//         public string Name => _name;
//         public string ModelName => _modelName;
//         public byte[] Model => _model;
//         public InferenceSession Session => _session;
//         public TImageProcessor ImageProcessor => _imageProcessor ??= new TImageProcessor();

//         public async Task UpdateExecutionProviderAsync(ExecutionProviders executionProvider)
//         {
//             // make sure any existing async task completes before we change the session
//             await AwaitLastTaskAsync();

//             // creating the inference session can be expensive and should be done as a one-off.
//             // additionally each session uses memory for the model and the infrastructure required to execute it,
//             // and has its own threadpools.
//             _prevAsyncTask =
//                 Task.Run(() =>
//                 {
//                     if (executionProvider == _curExecutionProvider)
//                     {
//                         return;
//                     }

//                     var options = new SessionOptions();

//                     if (executionProvider == ExecutionProviders.CPU)
//                     {
//                         // CPU Execution Provider is always enabled
//                     }
//                     else if (executionProvider == ExecutionProviders.NNAPI)
//                     {
//                         options.AppendExecutionProvider_Nnapi();
//                     }
//                     else if (executionProvider == ExecutionProviders.CoreML)
//                     {
//                         // add CoreML if the device has an Apple Neural Engine. if it doesn't performance
//                         // will most likely be worse than with the CPU Execution Provider.
//                         options.AppendExecutionProvider_CoreML(CoreMLFlags.COREML_FLAG_ONLY_ENABLE_DEVICE_WITH_ANE);
//                     }

//                     _session = new InferenceSession(_model, options);
//                 });
//         }

//         protected virtual Task<ImageProcessingResult> OnProcessImageAsync(byte[] image) =>
//             throw new NotImplementedException();

//         public Task InitializeAsync()
//         {
//             _prevAsyncTask = Initialize();
//             return _prevAsyncTask;
//         }

//         public async Task<ImageProcessingResult> ProcessImageAsync(byte[] image)
//         {
//             await AwaitLastTaskAsync().ConfigureAwait(false);

//             return await OnProcessImageAsync(image);
//         }

//         async Task AwaitLastTaskAsync()
//         {
//             if (_prevAsyncTask != null)
//             {
//                 await _prevAsyncTask.ConfigureAwait(false);
//                 _prevAsyncTask = null;
//             }
//         }

//         async Task Initialize()
//         {
//             _model = await Utils.LoadResource(_modelName);
//             try
//             {
//                 _session = new InferenceSession(_model);  // CPU execution provider is always enabled
//                 _curExecutionProvider = ExecutionProviders.CoreML;
//             }
//             catch (Exception ex)
//             {

//             }
        
//         }
//     }

//     // See: https://github.com/onnx/models/tree/main/vision/classification/mobilenet
//     // Model download: https://github.com/onnx/models/blob/main/vision/classification/mobilenet/model/mobilenetv2-12.onnx
//     // NOTE: We use the fp32 version of the model in this example as the int8 version uses internal ONNX Runtime 
//     // operators. Due to that, it will not work well with NNAPI or CoreML.
//     public class MobilenetSample : VisionSampleBase<MobilenetImageProcessor>
//     {
//         public const string Identifier = "Mobilenet V2";
//         public const string ModelFilename = "mobilenetv2-12.onnx";

//         public MobilenetSample()
//             : base(Identifier, ModelFilename) { }

//         protected override async Task<ImageProcessingResult> OnProcessImageAsync(byte[] image)
//         {
//             // Resize and center crop
//             using var preprocessedImage = await Task.Run(() => ImageProcessor.PreprocessSourceImage(image)).ConfigureAwait(false);

//             // Convert to Tensor of normalized float RGB data with NCHW ordering
//             var tensor = await Task.Run(() => ImageProcessor.GetTensorForImage(preprocessedImage)).ConfigureAwait(false);

//             // Run the model
//             var predictions = await Task.Run(() => GetPredictions(tensor)).ConfigureAwait(false);

//             // Get the pre-processed image for display to the user so they can see the actual input to the model
//             var preprocessedImageData = await Task.Run(() => ImageProcessor.GetBytesForBitmap(preprocessedImage)).ConfigureAwait(false);

//             var caption = string.Empty;

//             if (predictions.Any())
//             {
//                 var builder = new StringBuilder();

//                 if (predictions.Any())
//                 {
//                     builder.Append($"Top {predictions.Count} predictions: {Environment.NewLine}{Environment.NewLine}");
//                 }

//                 foreach (var prediction in predictions)
//                 {
//                     builder.Append($"{prediction.Label} ({prediction.Confidence * 100:0.00}%){Environment.NewLine}");
//                 }

//                 caption = builder.ToString();
//             }

//             return new ImageProcessingResult(preprocessedImageData, caption);
//         }

//         List<MobilenetPrediction> GetPredictions(Tensor<float> input)
//         {
//             // Setup inputs and outputs
//             var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input", input) };

//             // Run inference
//             using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = Session.Run(inputs);

//             // Postprocess to get softmax vector
//             IEnumerable<float> output = results.First().AsEnumerable<float>();
//             float sum = output.Sum(x => (float)Math.Exp(x));
//             IEnumerable<float> softmax = output.Select(x => (float)Math.Exp(x) / sum);

//             // Extract top 3 predicted classes
//             var top3 = softmax.Select((x, i) => new MobilenetPrediction
//             {
//                 Label = MobilenetLabelMap.Labels[i],
//                 Confidence = x
//             })
//                               .OrderByDescending(x => x.Confidence)
//                               .Take(3)
//                               .ToList();

//             return top3;
//         }
//     }

//     public interface IImageProcessor<TImage, TPrediction, TTensor>
//     {
//         // pre-process the image.
//         TImage PreprocessSourceImage(byte[] sourceImage);

//         // convert the PreprocessSourceImage output to a Tensor of type TTensor 
//         Tensor<TTensor> GetTensorForImage(TImage image);

//         // apply the predictions to the image if applicable
//         // e.g. draw the bounding box around an area selected by the model
//         byte[] ApplyPredictionsToImage(IList<TPrediction> predictions, TImage image);
//     }

//     public class SkiaSharpImageProcessor<TPrediction, TTensor> : IImageProcessor<SKBitmap, TPrediction, TTensor>
//     {
//         protected virtual SKBitmap OnPreprocessSourceImage(SKBitmap sourceImage) => sourceImage;
//         protected virtual Tensor<TTensor> OnGetTensorForImage(SKBitmap image) => throw new NotImplementedException();
//         protected virtual void OnPrepareToApplyPredictions(SKBitmap image, SKCanvas canvas) { }
//         protected virtual void OnApplyPrediction(TPrediction prediction, SKPaint textPaint, SKPaint rectPaint, SKCanvas canvas) { }

//         public byte[] ApplyPredictionsToImage(IList<TPrediction> predictions, SKBitmap image)
//         {
//             // Annotate image to reflect predictions and save for viewing
//             using SKSurface surface = SKSurface.Create(new SKImageInfo(image.Width, image.Height));
//             using SKCanvas canvas = surface.Canvas;

//             // Normalize paint size based on 800f shortest edge
//             float ratio = 800f / Math.Min(image.Width, image.Height);
//             var textSize = 32 * ratio;
//             var strokeWidth = 2f * ratio;

//             using SKPaint textPaint = new SKPaint { TextSize = textSize, Color = SKColors.White };
//             using SKPaint rectPaint = new SKPaint { StrokeWidth = strokeWidth, IsStroke = true, Color = SKColors.Red };

//             canvas.DrawBitmap(image, 0, 0);

//             OnPrepareToApplyPredictions(image, canvas);

//             foreach (var prediction in predictions)
//                 OnApplyPrediction(prediction, textPaint, rectPaint, canvas);

//             canvas.Flush();

//             using var snapshot = surface.Snapshot();
//             using var imageData = snapshot.Encode(SKEncodedImageFormat.Jpeg, 100);
//             byte[] bytes = imageData.ToArray();

//             return bytes;
//         }

//         public byte[] GetBytesForBitmap(SKBitmap bitmap)
//         {
//             using var image = SKImage.FromBitmap(bitmap);
//             using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
//             var bytes = data.ToArray();

//             return bytes;
//         }

//         public Tensor<TTensor> GetTensorForImage(SKBitmap image)
//             => OnGetTensorForImage(image);

//         public Size GetSizeForSourceImage(byte[] sourceImage)
//         {
//             using var image = SKBitmap.Decode(sourceImage);
//             return new Size(image.Width, image.Height);
//         }

//         public SKBitmap GetImageFromBytes(byte[] sourceImage, float shortestEdge = -1.0f)
//         {
//             var image = SKBitmap.Decode(sourceImage);

//             if (shortestEdge > 0.0)
//             {
//                 float ratio = shortestEdge / Math.Min(image.Width, image.Height);
//                 image = image.Resize(new SKImageInfo((int)(ratio * image.Width),
//                                                      (int)(ratio * image.Height)),
//                                                      SKFilterQuality.Medium);
//             }

//             return image;
//         }

//         public SKBitmap PreprocessSourceImage(byte[] sourceImage)
//         {
//             // Read image
//             using var image = SKBitmap.Decode(sourceImage);
//             return OnPreprocessSourceImage(image);
//         }
//     }

//     public class MobilenetImageProcessor : SkiaSharpImageProcessor<MobilenetPrediction, float>
//     {
//         // pre-processing as per https://github.com/onnx/models/blob/main/vision/classification/imagenet_inference.ipynb
//         //
//         // The steps are:
//         //  - Resize so the smallest side is 256
//         //  - Center crop to 244 x 244
//         //  - normalize
//         //  - convert to NCHW format with RGB ordering
//         //    - N == batch size (1 in this case)
//         //    - C == channels - 'r', 'g', 'b' channels
//         //    - H == height
//         //    - W == width
//         const int RequiredHeight = 224;
//         const int RequiredWidth = 224;

//         protected override SKBitmap OnPreprocessSourceImage(SKBitmap sourceImage)
//         {
//             // calculate ratio to reduce the smallest side to 256
//             const int ResizeTo = 256;
//             float ratio = (float)ResizeTo / Math.Min(sourceImage.Width, sourceImage.Height);
//             using SKBitmap scaledBitmap = sourceImage.Resize(
//                 new SKImageInfo((int)(Math.Ceiling(ratio * sourceImage.Width)),
//                                 (int)(Math.Ceiling(ratio * sourceImage.Height))),
//                                 SKFilterQuality.Medium);

//             // center crop
//             var horizontalCrop = Math.Max(scaledBitmap.Width - RequiredWidth, 0);
//             var verticalCrop = Math.Max(scaledBitmap.Height - RequiredHeight, 0);
//             var leftOffset = horizontalCrop == 0 ? 0 : horizontalCrop / 2;
//             var topOffset = verticalCrop == 0 ? 0 : verticalCrop / 2;

//             var cropRect = SKRectI.Create(new SKPointI(leftOffset, topOffset),
//                                           new SKSizeI(RequiredWidth, RequiredHeight));

//             using SKImage currentImage = SKImage.FromBitmap(scaledBitmap);
//             using SKImage croppedImage = currentImage.Subset(cropRect);

//             SKBitmap croppedBitmap = SKBitmap.FromImage(croppedImage);
//             return croppedBitmap;
//         }

//         protected override Tensor<float> OnGetTensorForImage(SKBitmap image)
//         {
//             Tensor<float> input = new DenseTensor<float>(new[] { 1, 3, RequiredHeight, RequiredWidth });

//             // per-channel normalization values that are applied when converting from a byte to a float
//             var mean = new[] { 0.485f, 0.456f, 0.406f };
//             var stddev = new[] { 0.229f, 0.224f, 0.225f };

//             for (int y = 0; y < image.Height; y++)
//             {
//                 for (int x = 0; x < image.Width; x++)
//                 {
//                     // write normalized values to input[N, C, H, W]
//                     var pixel = image.GetPixel(x, y);
//                     input[0, 0, y, x] = ((pixel.Red / 255f) - mean[0]) / stddev[0];
//                     input[0, 1, y, x] = ((pixel.Green / 255f) - mean[1]) / stddev[1];
//                     input[0, 2, y, x] = ((pixel.Blue / 255f) - mean[2]) / stddev[2];
//                 }
//             }

//             return input;
//         }
//     }

//     public class MobilenetPrediction
//     {
//         public string Label { get; set; }
//         public float Confidence { get; set; }
//     }

//     public enum ExecutionProviders
//     {
//         CPU,   // CPU execution provider is always available by default
//         NNAPI, // NNAPI is available on Android
//         CoreML // CoreML is available on iOS/macOS
//     }

    public static class Utils
    {

        internal static async Task<byte[]> LoadResource(string name)
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(name);
            using MemoryStream memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        public static byte[] HandleOrientation(byte[] image)
        {
            using var memoryStream = new MemoryStream(image);
            using var imageData = SKData.Create(memoryStream);
            using var codec = SKCodec.Create(imageData);
            var orientation = codec.EncodedOrigin;

            using var bitmap = SKBitmap.Decode(image);
            using var adjustedBitmap = AdjustBitmapByOrientation(bitmap, orientation);

            // encode the raw bytes in a known format that SKBitmap.Decode can handle.
            // doing this makes our APIs a little more flexible as they can take multiple image formats as byte[].
            // alternatively we could use SKBitmap instead of byte[] to pass the data around and avoid some
            // SKBitmap.Encode/Decode calls, at the cost of being tightly coupled to the SKBitmap type.
            using var stream = new MemoryStream();
            using var wstream = new SKManagedWStream(stream);

            adjustedBitmap.Encode(wstream, SKEncodedImageFormat.Jpeg, 100);
            var bytes = stream.ToArray();

            return bytes;
        }

        static SKBitmap AdjustBitmapByOrientation(SKBitmap bitmap, SKEncodedOrigin orientation)
        {
            switch (orientation)
            {
                case SKEncodedOrigin.BottomRight:

                    using (var canvas = new SKCanvas(bitmap))
                    {
                        canvas.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                        canvas.DrawBitmap(bitmap.Copy(), 0, 0);
                    }

                    return bitmap;

                case SKEncodedOrigin.RightTop:

                    using (var rotatedBitmap = new SKBitmap(bitmap.Height, bitmap.Width))
                    {
                        using (var canvas = new SKCanvas(rotatedBitmap))
                        {
                            canvas.Translate(rotatedBitmap.Width, 0);
                            canvas.RotateDegrees(90);
                            canvas.DrawBitmap(bitmap, 0, 0);
                        }

                        rotatedBitmap.CopyTo(bitmap);
                        return bitmap;
                    }

                case SKEncodedOrigin.LeftBottom:

                    using (var rotatedBitmap = new SKBitmap(bitmap.Height, bitmap.Width))
                    {
                        using (var canvas = new SKCanvas(rotatedBitmap))
                        {
                            canvas.Translate(0, rotatedBitmap.Height);
                            canvas.RotateDegrees(270);
                            canvas.DrawBitmap(bitmap, 0, 0);
                        }

                        rotatedBitmap.CopyTo(bitmap);
                        return bitmap;
                    }

                default:
                    return bitmap;
            }
        }
    }
}

