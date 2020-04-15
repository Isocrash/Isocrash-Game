using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OpenCL.Net;

namespace Raymarcher.Rendering
{
    internal class Renderer
    {
        private static Device UsedDevice { get; set; }

        private static CommandQueue Queue { get; set; }

        private static Mem memInput { get; set; }
        private static Mem memOutput;
        private static Mem memTime;
        private static Mem memVolume { get; set; }

        private static IntPtr memory;

        private static Stopwatch sw = new Stopwatch();

        //private static byte[] entryPixels = new byte[2560 * 1080 * 4];

        private static int inputSize = 0;
        private static int outputSize = 0;
        private static int modelSize = 0;
        private static int timeSize = sizeof(float);
        [EngineInitializer(322)]
        public static void Initialize()
        {
            Platform[] platforms = Cl.GetPlatformIDs(out ErrorCode error);
            if (error != ErrorCode.Success)
            {
                Log.Print("Impossible to run OpenCL, no any graphic platform available, abording launch.");

                Application.Exit();
            }

            Vector2I res = Graphics.RenderResolution;
            int pixelXAmount = res.x;
            int pixelYAmount = res.y;

            int amountOfObjects = 1;

            unsafe
            {
                inputSize = sizeof(C_CAMERA);
                outputSize = sizeof(byte) * pixelXAmount * pixelYAmount * 4;
            }

            UsedDevice = Cl.GetDeviceIDs(platforms[0], DeviceType.All, out error)[0];
            gpu_context = Cl.CreateContext(null, 1, new Device[] { UsedDevice }, null, IntPtr.Zero, out error);
            InfoBuffer namebuffer = Cl.GetDeviceInfo(UsedDevice, DeviceInfo.Name, out error);
            Log.Print("OpenCL Running on " + namebuffer);

            Queue = Cl.CreateCommandQueue(gpu_context, UsedDevice, CommandQueueProperties.OutOfOrderExecModeEnable, out error);
            if (error != ErrorCode.Success)
            {
                Console.WriteLine("Impossible to create gpu queue, abording launch.");

                Application.Exit();
            }

            CLoader.LoadProjectPaths(@".\libs", new[] { "c" }, out string[] cfiles, out string[] hfiles);
            Program program = CLoader.LoadProgram(cfiles, hfiles, UsedDevice, gpu_context);

            //Program prog = CLoader.LoadProgram(CLoader.GetCFilesDir(@".\", new[] { "cl" }).ToArray(), new[] { "headers" }, UsedDevice, gpu_context); 
            kernel = Cl.CreateKernel(program, "rm_render_entry", out error);

            if(error != ErrorCode.Success)
            {
                Log.Print("Error when creating kernel: " + error.ToString());
            }

            memory = new IntPtr(outputSize);
            memInput = (Mem)Cl.CreateBuffer(gpu_context, MemFlags.ReadOnly, inputSize, out error);
            memTime = (Mem)(Cl.CreateBuffer(gpu_context, MemFlags.ReadOnly, timeSize, out error));
            memOutput = (Mem)Cl.CreateBuffer(gpu_context, MemFlags.WriteOnly, outputSize, out error);

            

            //GPU_PARAM param = new GPU_PARAM() { X_RESOLUTION = res.x, Y_RESOLUTION = res.y };

            ////Vector3D pos = camera.Malleable.Position;
            //Quaternion q = camera.Malleable.Rotation;


            

            IntPtr notused;
            InfoBuffer local = new InfoBuffer(new IntPtr(4));
            error = Cl.GetKernelWorkGroupInfo(kernel, UsedDevice, KernelWorkGroupInfo.WorkGroupSize, new IntPtr(sizeof(int)), local, out notused);
            if (error != ErrorCode.Success)
            {
                Log.Print("Error getting kernel workgroup info: " + error.ToString());
            }

            //int intPtrSize = 0;
            intPtrSize = Marshal.SizeOf(typeof(IntPtr));
            
            Cl.SetKernelArg(kernel, 0, (IntPtr)intPtrSize, memInput);
            Cl.SetKernelArg(kernel, 1, (IntPtr)intPtrSize, memTime);
            
            Cl.SetKernelArg(kernel, 4, (IntPtr)intPtrSize, memOutput);

            //Cl.SetKernelArg(kernel, 2, new IntPtr(4), pixelAmount * 4);
            workGroupSizePtr = new IntPtr[] { new IntPtr(pixelXAmount * pixelYAmount) };

            
        }
        private static Context gpu_context;
        private static int intPtrSize = 0;
        private static Kernel kernel;
        private static IntPtr[] workGroupSizePtr;
        private static Stopwatch bakeSW = new Stopwatch();
        internal static double RenderTime = 0.0D;

        static Quaternion rot = EQuaternion.FromEuler(0D, 0D, 0D);
        static Vector3D axis = Vector3D.Up;
        static double rotationSpeed = 20.0D;
        static double angle = 0D;

        internal static List<C_VOLUME> volumes = new List<C_VOLUME>();
        public static Bitmap Bake(Camera camera)
        {
            bakeSW.Start();
            Vector2I res = Graphics.GetRenderResolution();
            int totPixels = res.x * res.y;
            C_CAMERA cam = new C_CAMERA(camera);

            angle += rotationSpeed * Time.DeltaTime;
            rot = Quaternion.CreateFromAxisAngle((Vector3)axis, (float)(angle * 0.0174533D));

            float time = (float)Time.TimeSinceStart;

            /*Bitmap image = new Bitmap(
                "assets/textures/2d/nord-vpn.png");*/

            Voxel sword = Voxel.GenerateDebug(new int3(32, 32, 32));///Voxel.CreateFromImage(image);
            C_VOXEL model = new C_VOXEL(sword);


            int modelSizeSize;
            int modelColorsSize;
            unsafe
            {
                modelSizeSize = sizeof(int3);
                modelColorsSize = sizeof(Colour32) * model.colors.Length;
            }

            ErrorCode error = ErrorCode.Success;

            Mem memModelSize = (Mem)Cl.CreateBuffer(gpu_context, MemFlags.ReadOnly, modelSizeSize, out error);
            Mem memModelColors = (Mem)Cl.CreateBuffer(gpu_context, MemFlags.ReadOnly, modelColorsSize, out error);

            Cl.SetKernelArg(kernel, 2, (IntPtr)intPtrSize, memModelSize);
            Cl.SetKernelArg(kernel, 3, (IntPtr)intPtrSize, memModelColors);

            Event event0;
            try
            {
                error = Cl.EnqueueWriteBuffer(Queue, (IMem)memInput, Bool.True, IntPtr.Zero, new IntPtr(inputSize), cam, 0, null, out event0);
                error = Cl.EnqueueWriteBuffer(Queue, (IMem)memModelSize, Bool.True, IntPtr.Zero, new IntPtr(modelSizeSize), model.size, 0, null, out event0);
                error = Cl.EnqueueWriteBuffer(Queue, (IMem)memModelColors, Bool.True, IntPtr.Zero, new IntPtr(modelColorsSize), model.colors, 0, null, out event0);

                error = Cl.EnqueueWriteBuffer(Queue, (IMem)memTime, Bool.True, IntPtr.Zero, new IntPtr(sizeof(float)), time, 0, null, out event0);
                //error = Cl.EnqueueWriteBuffer(Queue, (IMem)memNVolume, Bool.True, IntPtr.Zero, new IntPtr(nVolumeSize), vols.Length, 0, null, out event0);
            }
            catch (Exception e)
            {
                Log.Print("Error when enqueuing buffer:\n\t-OpenCL Error:" + error.ToString() + "\n\t-DotNet Error: " + e);
            }
            

            byte[] bp = new byte[totPixels * 4];

            sw.Start();
            error = Cl.EnqueueNDRangeKernel(Queue, kernel, 1, null, workGroupSizePtr, null, 0, null, out event0);


            if (error != ErrorCode.Success)
            {
                Log.Print("Error when enqueuing the NDRange of kernel: " + error.ToString());
            }

            Cl.Finish(Queue);

            ErrorCode execError = Cl.EnqueueReadBuffer(Queue, (IMem)memOutput, Bool.True, IntPtr.Zero, memory, bp, 0, null, out event0);
            if (execError != ErrorCode.Success)
            {
                Log.Print("Error while rendering: " + execError.ToString());
                //return new Bitmap(1, 1);
            }
            sw.Stop();
            RenderTime = sw.Elapsed.TotalMilliseconds;
            sw.Reset();

            //Stopwatch swbm = new Stopwatch();
            //swbm.Start();
            Bitmap bm = Imaging.RawToImage(bp, res.x, res.y, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //swbm.Stop();
            //Log.Print("Byte* to Bitmap => " + Math.Round(swbm.Elapsed.TotalMilliseconds,2) + "ms");

            //bakeSW.Stop();
            //Log.Print("Camera image bake took " + Math.Round(bakeSW.Elapsed.TotalMilliseconds, 2) + "ms");
            //bakeSW.Reset();
            return bm;
        }
    }

    internal struct C_BOX
    {
        public float3 center;
        public C_QUATERNION rotation;
        public float3 extents;
    }

    internal struct C_VOXEL
    {
        public int3 size;
        public Colour32[] colors;
        public C_BOX box;

        public C_VOXEL(Voxel voxel)
        {
            this.size = voxel.Size;
            this.colors = voxel.Colours;

            this.box = new C_BOX()
            {
                center = new float3(0.0F, 0.0F, 0.0F),
                extents = new float3(this.size.x / 2.0F, this.size.y / 2.0F, this.size.z / 2.0F),
                rotation = new C_QUATERNION(new Quaternion(0.0F, 0.0F, 0.0F, 1.0F))
            };
        }
    }
}
