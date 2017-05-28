#include "stdafx.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SortingTests
{		
	TEST_CLASS(InsertionSortTests)
	{
		public:

			TEST_METHOD(Insertion_Sort_With_Empty_Array_Should_Be_Sorted_By_Default) {
				// Arrange
				int arr[1];

				// Act
				CSorting::InsertionSort(arr, 1);

				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}
			TEST_METHOD(Insertion_Sort_With_Array_With_One_Element_Should_Be_Sorted_By_Default) {
				// Arrange
				int arr[1] = { 1 };

				// Act
				CSorting::InsertionSort(arr, 1);

				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}
			TEST_METHOD(Insertion_Sort_With_Integers)
			{
				// Arrange
				int arr[7] = { 8, 9, 1, 2, 4, 5, 8 };
			
				// Act
				CSorting::InsertionSort(arr, 7);
			
				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}
			TEST_METHOD(Insertion_Sort_With_Floats) 
			{
				// Arrange
				float arr[7] = { 1.1f, 1.2f, 1.0f, 0.2f, 0.3f, 0.4f, 0.7f };

				// Act
				CSorting::InsertionSort(arr, 7);

				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}
			TEST_METHOD(Insertion_Sort_With_Doubles) 
			{
				// Arrange
				double arr[7] = { 1.2 , 1.1, 1.001, 1.001, 1.01, 1.07, 2.0 };

				// Act
				CSorting::InsertionSort(arr, 7);

				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}
			TEST_METHOD(Insertion_Sort_With_Chars) {
				// Arrange
				char arr[7] = { "asdgbf" };

				// Act
				CSorting::InsertionSort(arr, 7);

				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}

	};
}