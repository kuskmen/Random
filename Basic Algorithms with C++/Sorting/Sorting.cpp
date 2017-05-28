#include "stdafx.h"
#include "Sorting.h"
#include <algorithm> // std::swap

template <typename T>
void CSorting::InsertionSort(T* arr, size_t size) {
	for (unsigned int i = 1; i < size; i++) {
		unsigned int j = i;
		
		while (*(arr + j) < *(arr + j - 1) && j > 0) {
			std::swap(*(arr+j), *(arr + j-1));
			j--;
		}
	}
}

template <typename T>
void CSorting::SelectionSort(T* arr, size_t size) {
	for (unsigned int i = 0; i < size; i++) {
		T min = *(arr + i);
		for (unsigned int j = i; j < size; j++) {
			if (*(arr + j) < min) {
				min = *(arr + j);
			}
		}
		std::swap(min, *(arr + i));
	}
}



#if SORTING_EXPORTS

	// Insertion Sort
	template __declspec(dllexport) void CSorting::InsertionSort<int>(int* arr, size_t size);
	template __declspec(dllexport) void CSorting::InsertionSort<float>(float* arr, size_t size);
	template __declspec(dllexport) void CSorting::InsertionSort<double>(double* arr, size_t size);
	template __declspec(dllexport) void CSorting::InsertionSort<char>(char* arr, size_t size);

	// Selection Sort
	template __declspec(dllexport) void CSorting::SelectionSort<int>(int* arr, size_t size);
	template __declspec(dllexport) void CSorting::SelectionSort<float>(float* arr, size_t size);
	template __declspec(dllexport) void CSorting::SelectionSort<double>(double* arr, size_t size);
	template __declspec(dllexport) void CSorting::SelectionSort<char>(char* arr, size_t size);

#endif