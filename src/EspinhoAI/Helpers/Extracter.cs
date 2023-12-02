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
}

