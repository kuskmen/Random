#include "stdafx.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SelectionSortTests {

	TEST_CLASS(SelectionSortTests) {

		public:

			TEST_METHOD(SelectionSort_With_Empty_Array_Should_Be_Sorted_By_Default) {
				// Arrange
				int arr[1];

				// Act
				CSorting::SelectionSort(arr, 1);
				
				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}
			TEST_METHOD(SelectionSort_With_Array_With_One_Element_Should_Be_Sorted_By_Default) {
				// Arrange
				int arr[1] = { 1 };

				// Act
				CSorting::SelectionSort(arr, 1);

				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}
			TEST_METHOD(SelectionSort_With_Integers) {
				// Arrange
				int arr[5] = { 5, 3, 4, 1, 2 };

				// Act
				CSorting::SelectionSort(arr, 5);

				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}
			TEST_METHOD(SelectionSort_With_Floats) {
				// Arrange
				float arr[5] = { 5.0f, 3.1f, 4.2f, 1.01f, 2.02f };

				// Act
				CSorting::SelectionSort(arr, 5);

				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}
			TEST_METHOD(SelectionSort_With_Doubles) {
				// Arrange
				int arr[5] = { 5.2, 3.3, 4.4, 1.1, 2.1 };

				// Act
				CSorting::SelectionSort(arr, 5);

				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}
			TEST_METHOD(SelectionSort_With_Chars) {
				// Arrange
				char arr[5] = { "asfd" };

				// Act
				CSorting::SelectionSort(arr, 5);

				// Assert
				Assert::IsTrue(std::is_sorted(std::begin(arr), std::end(arr)));
			}
	};
}