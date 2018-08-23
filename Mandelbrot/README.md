# Mandelbrot

This is my implementation of rendering Mandelbrot set.
Project has fairly simple interface and features.

![Mandelbrot](https://image.ibb.co/e1A9he/Mandelbrot.png)

- Features
	- Zoom in (left click on the picture box) and zoom out (right click on the picture box)
	- Able to choose what degree of parallelism will be used for the render as well as iteratons before concluding which point is in or out of Mandelbrot set
	- Timer to show how much the actual rendering took
	- _TODO:_ Make snapshots of the set and save it as a picture.
- Performance considerations
	- Cardioid bulb checking was implemented with the formula taken from [wikipedia](https://en.wikipedia.org/wiki/Mandelbrot_set#Optimizations).
	- Due to mathematical properties of Mandelbrot set, calculating magnitude of complex number is done once each 8 iterations.
	- For the drawing unsafe code and fixed pointers are used in order to improve performance and garbage collector shinenigans. (maybe this unsafe code can be replaced by Memory<T>, new feature in C#7.2)
	- Object allocations and garbage collection should be minimized almost to zero, almost everything is allocated on the stack so in theory there should be no need of garbage collector at all. (this can be done with the latest CLR changes allowing to plug new custom garbage collector implementation)
	- The idea was to render Mandelbrot using the CPU(hence there is no code trying to do that with the GPU), currently project is in a state of using standard libraries for doing the parallelism and complex number arithmetics.
	- _TODO:_ Performance can be greatly improved if SIMD instructions are used for data parallelism, this will be next step however.

## Whats next?
I can see how some benchmarking and disassembly inspection can give much insights on the further microoptimizations that can be made. Also like I said SIMD instructions can greatly help with the performance.
