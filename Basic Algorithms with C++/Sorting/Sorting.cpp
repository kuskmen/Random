#include "stdafx.h"
#include "Sorting.h"
#include <algorithm> // std::swap

template <typename T>
void CSorting::InsertionSort(T* arr, size_t size)
{
	for (unsigned int i = 1; i < size; i++) {
		unsigned int j = i;
		
		while (*(arr + j) < *(arr + j - 1) && j > 0) {
			std::swap(*(arr+j), *(arr + j-1));
			j--;
		}
	}
}

template __declspec(dllexport) void CSorting::InsertionSort<int>(int* arr, size_t size);
template __declspec(dllexport) void CSorting::InsertionSort<float>(float* arr, size_t size);
template __declspec(dllexport) void CSorting::InsertionSort<double>(double* arr, size_t size);
template __declspec(dllexport) void CSorting::InsertionSort<char>(char* arr, size_t size);