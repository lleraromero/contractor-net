//#include <stdlib.h>
//#include <stdio.h>
//#include <string.h>

// Booleans
typedef int boolean;

// Objects (are ints)
typedef int Object;

// Necessary signatures
void AL_add(int index, Object element);
Object AL_set(int index, Object element);
Object AL_remove(int index);
Object AL_get(int index);


// Necessary memory management
void* my_calloc(int amount, int size) {
	return (void*) 1;
}

void my_memcpy(void* dest, void* orig, int size) {

}

/*
 * ListItr fields
 */
int LI_cursor;
int LI_lastRet;
int LI_expectedModCount;

/*
 * ArrayList fields
 */
int AL_modCount;
Object* AL_elementData;
int AL_elementData_length;
int AL_size;

/*
 * Invariant
 */
boolean invariant() {
	return // AL_elementData == (void*) 0 ||
		(0 <= LI_cursor && LI_cursor <= AL_size
		&& 0 <= AL_size && AL_size <= AL_elementData_length
		&& (-1 == LI_lastRet || (LI_cursor < AL_size && LI_lastRet == LI_cursor) || (LI_cursor > 0 && LI_lastRet == LI_cursor - 1))
		&& 0 <= LI_expectedModCount
		&& 0 <= AL_modCount
		&& 10 <= AL_elementData_length 
		);
}

/*
 * ListItr methods
 */
boolean par_pre_ListItr(int index, int modCount, int elementData_length, int size) {
	return 0 <= index && index <= size // IndexOutOfBoundsException
		&& 0 <= size && size <= elementData_length
		&& 0 <= modCount
		&& 10 <= elementData_length;
}
boolean system_pre_ListItr() {
	return 1;
}
void ListItr(int index, int modCount, int elementData_length, int size) {
	// get a reference to ArrayList attributes
	AL_modCount = modCount;	
	AL_elementData = my_calloc(elementData_length, sizeof(Object)); // new Object[initialCapacity];
	AL_elementData_length = elementData_length;
	AL_size = size;
	
	// create Iterator structure
	LI_cursor = index;
	LI_lastRet = -1;
	LI_expectedModCount = AL_modCount;
}

/*
void checkForComodification() {
	if (AL_modCount != LI_expectedModCount)
		throw new ConcurrentModificationException();
}
*/

boolean LI_hasNext() {
	return LI_cursor != AL_size;
}

boolean par_pre_LI_next() {
	return 1;
}
boolean system_pre_LI_next() {
	return AL_elementData != (void*) 0
		&& AL_modCount == LI_expectedModCount // checkForComodification()
		&& 0 <= LI_cursor && LI_cursor < AL_size; // IndexOutOfBoundsException
}
Object LI_next() {
//	checkForComodification();
//	try {
		Object next = AL_get(LI_cursor);
		LI_lastRet = LI_cursor++;
		return next;
//	} catch(IndexOutOfBoundsException e) {
//		checkForComodification();
//		throw new NoSuchElementException();
//	}
}

boolean par_pre_LI_remove() {
	return 1;
}
boolean system_pre_LI_remove() {
	return AL_elementData != (void*) 0
		&& LI_lastRet != -1 // IllegalStateException
		&& AL_modCount == LI_expectedModCount // checkForComodification()
		&& 0 <= LI_lastRet && LI_lastRet < AL_size; // IndexOutOfBoundsException
}
void LI_remove() {
//	if (LI_lastRet == -1)
//		throw new IllegalStateException();
//	checkForComodification();

//	try {
		AL_remove(LI_lastRet);
		if (LI_lastRet < LI_cursor)
			LI_cursor--;
		LI_lastRet = -1;
		LI_expectedModCount = AL_modCount;
//	} catch(IndexOutOfBoundsException e) {
//		throw new ConcurrentModificationException();
//	}
}

boolean LI_hasPrevious() {
	return LI_cursor != 0;
}

boolean par_pre_LI_previous() {
	return 1;
}
boolean system_pre_LI_previous() {
	return AL_elementData != (void*) 0
		&& AL_modCount == LI_expectedModCount // checkForComodification()
		&& 0 <= LI_cursor - 1 && LI_cursor - 1 < AL_size; // IndexOutOfBoundsException
}
Object LI_previous() {
//	checkForComodification();
//	try {
		int i = LI_cursor - 1;
		Object previous = AL_get(i);
		LI_lastRet = LI_cursor = i;
		return previous;
//	} catch(IndexOutOfBoundsException e) {
//		checkForComodification();
//		throw new NoSuchElementException();
//	}
}

int LI_nextIndex() {
	return LI_cursor;
}

int LI_previousIndex() {
	return LI_cursor-1;
}

boolean par_pre_LI_set(Object o) {
	return 1;
}
boolean system_pre_LI_set() {
	return AL_elementData != (void*) 0
		&& LI_lastRet != -1 // IllegalStateException
		&& AL_modCount == LI_expectedModCount // checkForComodification()
		&& 0 <= LI_lastRet && LI_lastRet < AL_size; // IndexOutOfBoundsException
}
void LI_set(Object o) {
//	if (LI_lastRet == -1)
//		throw new IllegalStateException();
//	checkForComodification();

//	try {
		AL_set(LI_lastRet, o);
		LI_expectedModCount = AL_modCount;
//	} catch(IndexOutOfBoundsException e) {
//		throw new ConcurrentModificationException();
//	}
}

boolean par_pre_LI_add(Object o) {
	return 1;
}
boolean system_pre_LI_add() {
	return AL_elementData != (void*) 0
		&& AL_modCount == LI_expectedModCount // checkForComodification()
		&& 0 <= LI_cursor && LI_cursor <= AL_size; // IndexOutOfBoundsException
}
void LI_add(Object o) {
//	checkForComodification();

//	try {
		AL_add(LI_cursor++, o);
		LI_lastRet = -1;
		LI_expectedModCount = AL_modCount;
//	} catch(IndexOutOfBoundsException e) {
//		throw new ConcurrentModificationException();
//	}
}

/*
 * ArrayList methods
 */
 
/*
void ArrayList(int initialCapacity) {
	if (initialCapacity < 0)
		throw new IllegalArgumentException("Illegal Capacity: "+ initialCapacity);
	AL_elementData = my_calloc(initialCapacity, sizeof(Object)); // new Object[initialCapacity];
	AL_elementData_length = initialCapacity;
}
*/

/*
void RangeCheck(int index) {
	if (index >= AL_size)
		throw new IndexOutOfBoundsException("Index: "+index+", Size: "+AL_size);
}
*/

Object AL_get(int index) {
//	RangeCheck(index);

	return 1; // XXX abstracted data away. AL_elementData[index];
}

Object AL_remove(int index) {
//	RangeCheck(index);

	AL_modCount++;
	Object oldValue = 1; // XXX abstracted data away. AL_elementData[index];

	int numMoved = AL_size - index - 1;
	if (numMoved > 0)
		my_memcpy(AL_elementData + index, AL_elementData + index + 1, numMoved); // System.arraycopy(elementData, index+1, elementData, index, numMoved);
	--AL_size; // XXX abstracted data away, kept index update. AL_elementData[--AL_size] = 0;

	return oldValue;
}


Object AL_set(int index, Object element) {
//	RangeCheck(index);

	Object oldValue = 1; // XXX abstracted data away. AL_elementData[index];
	// XXX abstracted data away. AL_elementData[index] = element;
	return oldValue;
}

void AL_ensureCapacity(int minCapacity) {
	AL_modCount++;
	int oldCapacity = AL_elementData_length;
	if (minCapacity > oldCapacity) {
		Object* oldData = AL_elementData;
		int newCapacity = (oldCapacity * 3)/2 + 1;
		if (newCapacity < minCapacity)
			newCapacity = minCapacity;
		AL_elementData = my_calloc(newCapacity, sizeof(Object)); //new Object[newCapacity];
		AL_elementData_length = newCapacity;
		my_memcpy(AL_elementData, oldData, AL_size); // System.arraycopy(oldData, 0, elementData, 0, size);
	}
}

void AL_add(int index, Object element) {
//	if (index > AL_size || index < 0)
//		throw new IndexOutOfBoundsException("Index: "+index+", Size: "+AL_size);

	AL_ensureCapacity(AL_size+1);  // Increments modCount!!
	my_memcpy(AL_elementData + index + 1, AL_elementData + index, AL_size - index); // System.arraycopy(elementData, index, elementData, index + 1, size - index);
	//AL_elementData[index] = element;
	AL_size++;
}




