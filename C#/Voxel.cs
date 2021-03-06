﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raymarcher.Rendering;
using System.Drawing;
using System.Drawing.Imaging;
using OpenCL.Net;
using System.IO;

namespace Raymarcher
{
    public struct Voxel
    {
        public int3 Size { get; private set; }
        public Colour32[] Colours { get; private set; }

        public Colour32 this[int x, int y, int z]
        {
            get
            {
                return this.Colours[RayMath.FlatTo3D(this.Size, new int3(x, y, z))];
            }

            set
            {
                //Log.Print(new int3(x, y, z) + " => " + RayMath.FlatTo3D(this.Size, new int3(x, y, z)));
                this.Colours[RayMath.FlatTo3D(this.Size, new int3(x, y, z))] = value;
            }
        }

        public Voxel(int3 size)
        {
            this.Size = size;
            this.Colours = new Colour32[Size.x * Size.y * Size.z];
        }

        public Voxel(int3 size, Colour32[] colours)
        {
            this.Size = size;
            this.Colours = colours;
        }

        public static Voxel CreateFromImage(Bitmap image)
        {
            Voxel voxel = new Voxel(new int3(image.Width, image.Height, 1));

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    
                    Color pix = image.GetPixel(x, y);
                    Colour32 col = new Colour32(pix.R, pix.G, pix.B, pix.A);

                    voxel[x, y, 0] = col;//image.GetPixel(x, y);
                }
            }

            return voxel;
        }

        public static Image ConvertToImage(Voxel v)
        {
            Bitmap bm = new Bitmap(v.Size.x, v.Size.y);

            for (int y = 0; y < v.Size.y; y++)
            {
                for (int x = 0; x < v.Size.x; x++)
                {
                    Colour32 col = v[x, y, 0];

                    bm.SetPixel(x, y, Color.FromArgb(col.A, col.R, col.G, col.B));
                }
            }

            return bm;
        }

        public static Voxel GenerateDebug(int3 size)
        {
            Voxel voxel = new Voxel(size);

            for (int z = 0; z < size.z; z++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        Colour256 col256 = new Colour256(
                            r: x / ((double)size.x-1),
                            g: 1.0D - (y / ((double)size.y-1)),
                            b: z / ((double)size.z - 1),
                            a: 1.0D//(z % 2.0F == 0 ? 1.0D : 0.0D)
                            
                            );

                        if (x % 2 == 0) col256.A = 0.0D;
                        if (y % 2 == 0) col256.A = 0.0D;
                        if (z % 2 == 0) col256.A = 0.0D;

                        Colour32 col32 = (Colour32)col256;

                        voxel[x, y, z] = col256;
                    }
                }
            }

            return voxel;
        }
    }
}
