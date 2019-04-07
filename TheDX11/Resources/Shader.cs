using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDX11.Resources
{
    public class Shader : IDisposable
    {
        private readonly Device device;

        internal InputLayout ShaderInputLayout { get; }
        internal VertexShader VertexShader { get; }
        internal PixelShader PixelShader { get; }

        public bool Instancing { get; }

        public bool ZWrite { get; }

        public class ShaderInclude : Include
        {
            private readonly string[] incPaths;

            public IDisposable Shadow { get; set; }

            public ShaderInclude(string[] incPaths)
            {
                this.incPaths = incPaths;
            }

            public void Close(Stream stream)
            {
                stream.Close();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public Stream Open(IncludeType type, string fileName, Stream parentStream)
            {
                foreach (var dir in incPaths)
                {
                    if (File.Exists(dir + "/" + fileName))
                        return new FileStream(dir + "/" + fileName, FileMode.Open);
                }
                throw new Exception();
            }
        }

        internal Shader(Device device, string shaderFile, string[] includePaths)
        {
            var shaderContent = File.ReadAllText(shaderFile);
            var shaderData = JsonConvert.DeserializeObject<ShaderData>(shaderContent);

            var shaderDir = Path.GetDirectoryName(shaderFile);

            var shaderInclude = new ShaderInclude(includePaths);

            var vertexMacros = new List<ShaderMacro> { new ShaderMacro("VERTEX_SHADER", 1) };
            var pixelMacros = new List<ShaderMacro> { new ShaderMacro("PIXEL_SHADER", 1) };

            ZWrite = shaderData.ZWrite;
            Instancing = shaderData.Instancing;
            if (Instancing)
            {
                vertexMacros.Add(new ShaderMacro("INSTANCING", 1));
                pixelMacros.Add(new ShaderMacro("INSTANCING", 1));
            }

            ShaderBytecode vertexShaderByteCode = ShaderBytecode.CompileFromFile(shaderDir + "/" + shaderData.Vertex.Path, shaderData.Vertex.Entry, "vs_5_0", ShaderFlags.None, EffectFlags.None, vertexMacros.ToArray(), shaderInclude);
            ShaderBytecode pixelShaderByteCode = ShaderBytecode.CompileFromFile(shaderDir + "/" + shaderData.Pixel.Path, shaderData.Pixel.Entry, "ps_5_0", ShaderFlags.None, EffectFlags.None, pixelMacros.ToArray(), shaderInclude);

            InputElement[] inputElements = LoadInputs(shaderData.Vertex.Input, shaderData.Instancing);
            ShaderInputLayout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);

            VertexShader = new VertexShader(device, vertexShaderByteCode);
            PixelShader = new PixelShader(device, pixelShaderByteCode);


            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();
            this.device = device;
        }

        // call from render thread!
        public void Activate()
        {
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            device.ImmediateContext.InputAssembler.InputLayout = ShaderInputLayout;
            device.ImmediateContext.VertexShader.Set(VertexShader);
            device.ImmediateContext.PixelShader.Set(PixelShader);
        }

        public void Dispose()
        {
            PixelShader.Dispose();
            VertexShader.Dispose();
            ShaderInputLayout.Dispose();
        }

        private InputElement[] LoadInputs(IEnumerable<ShaderData.ShaderInput> inputs, bool instancing)
        {
            List<InputElement> inputElements = new List<InputElement>();

            int i = 0;
            foreach (var input in inputs)
            {
                var semanticName = input.Semantic;
                int semanticIndex = 0;
                if (char.IsDigit(semanticName[semanticName.Length - 1]))
                {
                    semanticIndex = semanticName[semanticName.Length - 1] - '0';
                    semanticName = semanticName.Substring(0, semanticName.Length - 1);
                }

                var format = SharpDX.DXGI.Format.R32G32B32_Float;

                switch (input.Type)
                {
                    case ShaderData.ShaderInputType.Float:
                        format = SharpDX.DXGI.Format.R32_Float;
                        break;
                    case ShaderData.ShaderInputType.Float2:
                        format = SharpDX.DXGI.Format.R32G32_Float;
                        break;
                    case ShaderData.ShaderInputType.Float3:
                        format = SharpDX.DXGI.Format.R32G32B32_Float;
                        break;
                    case ShaderData.ShaderInputType.Float4:
                        format = SharpDX.DXGI.Format.R32G32B32A32_Float;
                        break;
                    default:
                        throw new Exception("Unsuported format");
                }

                inputElements.Add(new InputElement()
                {
                    SemanticName = semanticName,
                    SemanticIndex = semanticIndex,
                    Format = format,
                    Slot = 0,
                    AlignedByteOffset = i == 0 ? 0 : InputElement.AppendAligned,
                    Classification = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                });

                i++;
            }

            if (instancing)
            {
                for (int j = 1; j < 5; ++j)
                {
                    inputElements.Add(new InputElement()
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = j + 1,
                        Format = SharpDX.DXGI.Format.R32G32B32A32_Float,
                        Slot = 1,
                        AlignedByteOffset = j == 1 ? 0 : InputElement.AppendAligned,
                        Classification = InputClassification.PerInstanceData,
                        InstanceDataStepRate = 1
                    });
                }
            }

            return inputElements.ToArray();
        }
    }
    
    internal class ShaderData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ShaderInputType
        {
            Float,
            Float2,
            Float3,
            Float4
        }

        public class ShaderInput
        {
            public ShaderInputType Type { get; set; }
            public string Semantic { get; set; }
        }

        public class PixelVertexData
        {
            public string Path { get; set; }
            public string Entry { get; set; }
            public List<ShaderInput> Input { get; set; }
        }

        public PixelVertexData Pixel { get; set; }
        public PixelVertexData Vertex { get; set; }
        public int Textures { get; set; }
        public bool Instancing { get; set; }

        public bool ZWrite { get; set; }
    }
}
