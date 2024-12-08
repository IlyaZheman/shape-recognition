using Unity.Sentis;
using UnityEngine;

namespace Test
{
    public class ClassifyHandwrittenDigit : MonoBehaviour
    {
        // https://github.com/Unity-Technologies/sentis-samples/tree/main

        public ModelAsset modelAsset;

        private Model runtimeModel;
        private Worker worker;
        public float[] results;

        private void Awake()
        {
            var sourceModel = ModelLoader.Load(modelAsset);

            // Create a functional graph that runs the input model and then applies softmax to the output.
            var graph = new FunctionalGraph();
            var inputs = graph.AddInputs(sourceModel);
            var outputs = Functional.Forward(sourceModel, inputs);
            var softmax = Functional.Softmax(outputs[0]);

            // Create a model with softmax by compiling the functional graph.
            runtimeModel = graph.Compile(softmax);

            // Create an engine
            worker = new Worker(runtimeModel, BackendType.GPUCompute);
        }

        public void UpdateResults(Texture2D inputTexture)
        {
            // Create input data as a tensor
            using Tensor inputTensor = TextureConverter.ToTensor(inputTexture, width: 28, height: 28, channels: 1);

            // Run the model with the input data
            worker.Schedule(inputTensor);

            // Get the result
            var outputTensor = worker.PeekOutput() as Tensor<float>;

            // outputTensor is still pending
            // Either read back the results asynchronously or do a blocking download call
            results = outputTensor.DownloadToArray();

            LogMax();
        }

        private void LogMax()
        {
            var index = -1;
            var value = -1f;
            for (var i = 0; i < results.Length; i++)
            {
                if (results[i] <= value)
                    continue;

                value = results[i];
                index = i;
            }

            Debug.Log(index);
        }

        private void OnDisable()
        {
            // Tell the GPU we're finished with the memory the engine used
            worker.Dispose();
        }
    }
}