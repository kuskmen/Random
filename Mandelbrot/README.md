# Mandelbrot

This is my implementation of rendering Mandelbrot set.
Project has fairly simple interface and features.

![Mandelbrot](https://image.ibb.co/e1A9he/Mandelbrot.png)

- Features
	- Zoom in (left click on the picture box) and zoom out (right click on the picture box)
	- Able to choose what degree of parallelism will be used for the render as well as iteratons before concluding which point is in or out of Mandelbrot set
	- Timer to show how much the actual rendering took
	- TODO: Make snapshots of the set and save it as a picture.
- Performance considerations
	- Cardioid bulb checking was implemented with the formula taken from [wikipedia](https://en.wikipedia.org/wiki/Mandelbrot_set#Optimizations).
	- Due to mathematical properties of Mandelbrot set, calculating magnitude of complex number is done once each 8 iterations.
