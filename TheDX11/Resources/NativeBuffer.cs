using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace TheDX11.Resources
{
    public enum BufferTypeEnum
    {
        Vertex,
        Index,
        ConstPixel,
        ConstVertex,
        StructuredBuffer,
        StructuredBufferVertexOnly,
        StructuredBufferPixelOnly
    }

    public interface INativeBuffer
    {
        void Activate(int slot);
    }

    public class NativeBuffer<T> : IDisposable, INativeBuffer where T : struct
    {
        private readonly Device device;

        public BufferTypeEnum BufferType { get; }

        public int Length { get; private set; }

        internal Buffer Buffer { get; private set; }
        
        internal VertexBufferBinding VertexBufferBinding { get; private set; }

        internal ShaderResourceView shaderView { get; private set; }

        internal NativeBuffer(Device device, BufferTypeEnum bufferType, int length)
        {
            this.device = device;
            this.BufferType = bufferType;
                        
            Length = length;
            CreateBuffer();
        }

        internal NativeBuffer(Device device, BufferTypeEnum bufferType, T[] data)
        {
            this.device = device;
            this.BufferType = bufferType;

            Length = data.Length;
            CreateBufferWithData(data);
        }

        private bool IsStructuredBuffer => BufferType == BufferTypeEnum.StructuredBuffer || BufferType == BufferTypeEnum.StructuredBufferPixelOnly || BufferType == BufferTypeEnum.StructuredBufferVertexOnly;

        private BufferDescription GetBufferDescription()
        {
            return new BufferDescription()
            {
                Usage = ResourceUsage.Default,
                SizeInBytes = Utilities.SizeOf<T>() * Length, // Must be divisable by 16 bytes, so this is equated to 32 (?)
                BindFlags = BufferTypeToBindFlags(BufferType),
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = IsStructuredBuffer ? ResourceOptionFlags.BufferStructured : ResourceOptionFlags.None,
                StructureByteStride = IsStructuredBuffer ? Utilities.SizeOf<T>() : 0,
            };
        }

        private void CreateShaderView()
        {
            if (!IsStructuredBuffer)
                return;

            if (shaderView != null)
                shaderView.Dispose();

            var desc = new ShaderResourceViewDescription()
            {
                Format = SharpDX.DXGI.Format.Unknown,
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.ExtendedBuffer
            };
            desc.BufferEx.FirstElement = 0;
            desc.BufferEx.ElementCount = Length;
            desc.BufferEx.Flags = 0;

            shaderView = new ShaderResourceView(device, Buffer, desc);
        }

        private void CreateBuffer()
        {
            BufferDescription bufferDesc = GetBufferDescription();
            Buffer = new Buffer(device, bufferDesc);
            VertexBufferBinding = new VertexBufferBinding(Buffer, Utilities.SizeOf<T>(), 0);
            CreateShaderView();
        }

        private void CreateBufferWithData(T[] data)
        {
            BufferDescription bufferDesc = GetBufferDescription();
            Buffer = Buffer.Create(device, data, bufferDesc);
            VertexBufferBinding = new VertexBufferBinding(Buffer, Utilities.SizeOf<T>(), 0);
            CreateShaderView();
        }

        // this method can be called only from render thread
        public void UpdateBuffer(T[] newData)
        {
            if (Length != newData.Length)
            {
                Dispose();
                Length = newData.Length;
                CreateBuffer();
            }
            device.ImmediateContext.UpdateSubresource(newData, Buffer);
        }
        
        // this method can be called only from render thread
        public void UpdateBuffer(ref T newData)
        {
            if (Length != 1)
            {
                Dispose();
                Length = 1;
                CreateBuffer();
            }
            device.ImmediateContext.UpdateSubresource(ref newData, Buffer);
        }

        private static BindFlags BufferTypeToBindFlags(BufferTypeEnum bufferType)
        {
            switch (bufferType)
            {
                case BufferTypeEnum.StructuredBuffer:
                case BufferTypeEnum.StructuredBufferPixelOnly:
                case BufferTypeEnum.StructuredBufferVertexOnly:
                    return BindFlags.ShaderResource | BindFlags.UnorderedAccess;
                case BufferTypeEnum.Vertex:
                    return BindFlags.VertexBuffer;
                case BufferTypeEnum.Index:
                    return BindFlags.IndexBuffer;
                case BufferTypeEnum.ConstPixel:
                case BufferTypeEnum.ConstVertex:
                    return BindFlags.ConstantBuffer;
                default:
                    throw new Exception("Unsupported buffer type");
            }
        }

        public void Activate(int slot)
        {
            if (BufferType == BufferTypeEnum.Vertex)
            {
                device.ImmediateContext.InputAssembler.SetVertexBuffers(slot, VertexBufferBinding);
            }
            else if (BufferType == BufferTypeEnum.Index)
            {
                device.ImmediateContext.InputAssembler.SetIndexBuffer(Buffer, SharpDX.DXGI.Format.R32_UInt, slot);
            }
            else if (BufferType == BufferTypeEnum.ConstPixel)
            {
                device.ImmediateContext.PixelShader.SetConstantBuffer(slot, Buffer);
            }
            else if (BufferType == BufferTypeEnum.ConstVertex)
            {
                device.ImmediateContext.VertexShader.SetConstantBuffer(slot, Buffer);
            }
            else if (BufferType == BufferTypeEnum.StructuredBuffer)
            {
                device.ImmediateContext.PixelShader.SetShaderResource(slot, shaderView);
                device.ImmediateContext.VertexShader.SetShaderResource(slot, shaderView);
            }
            else if (BufferType == BufferTypeEnum.StructuredBufferPixelOnly)
            {
                device.ImmediateContext.PixelShader.SetShaderResource(slot, shaderView);
            }
            else if (BufferType == BufferTypeEnum.StructuredBufferVertexOnly)
            {
                device.ImmediateContext.VertexShader.SetShaderResource(slot, shaderView);
            }
            else
            {
                throw new Exception("Unsupported buffer type");
            }
        }

        public void Dispose()
        {
            shaderView?.Dispose();
            shaderView = null;

            Buffer.Dispose();
            Buffer = null;
        }
    }
}
