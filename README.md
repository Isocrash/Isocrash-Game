# Raymarcher
C# Engine based on CUDA. Very WIP

# How to build project

- Install the CUDA Toolkit 10.1
- Install the Altimesh extention
- Make sure Visual Studio is able to compile C++
- First build the C# then the C++ project. A solution build should be working then.

If the project is not compiling, go into the C++ solution and make sure both HybridizerDllFullPath and both RootNamespace/ProjectName are as well then edit GPUCamera.cs' HybRunner dll's name.

The built program should run with no problems. However if a black screen appears try to lower the HybRunner's SetDistrib() blockDimX parameter to 256 or 128.

# The program cannot run on machines that have not the CUDA Toolkit yet for obscure reasons.
