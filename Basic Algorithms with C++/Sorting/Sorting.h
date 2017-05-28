#pragma once
#ifdef SORTING_EXPORTS
	#define SORTING_API __declspec(dllexport)
#else
	#define SORTING_API __declspec(dllimport)
#endif

class SORTING_API CSorting {
public:
	template <typename T>
	static void InsertionSort(T* arr, size_t size);
};