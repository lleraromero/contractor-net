#include <stdlib.h>
#include <stdio.h>
#include <string.h>

// Booleans
typedef int boolean;
boolean true = 1;
boolean false = 0;

// Other types
typedef char byte;
typedef int Thread;
int null = 0;

// Non-deterministic activity
void ND();






/*
 * PipedInputStream
 */

boolean closedByWriter;
boolean closedByReader;
boolean connected;

Thread readSide;
boolean readSide_alive;
Thread writeSide;
boolean writeSide_alive;

int PIPE_SIZE;
byte* buffer;

int in;
int out;

void sink_receive(int b) { // throws IOException {
	/*if (!connected) {
		throw new IOException("Pipe not connected");
	} else if (closedByWriter || closedByReader) {
		throw new IOException("Pipe closed");
	} else if (readSide != null && !readSide.isAlive()) {
		throw new IOException("Read end dead");
	}*/
	
	writeSide = 25; //Thread.currentThread();
	while (in == out) {
		/*if ((readSide != null) && !readSide.isAlive()) {
			throw new IOException("Pipe broken");
		}*/
		/* full: kick any waiting readers */
		//notifyAll();
		//try {
		//	wait(1000);
		//} catch (InterruptedException ex) {
		//	throw new java.io.InterruptedIOException();
		//}
		ND(); // simulate readings
	}
	if (in < 0) {
		in = 0;
		out = 0;
	}
	buffer[in++] = (byte)(b & 0xFF);
	if (in >= PIPE_SIZE) {
		in = 0;
	}
}

/*void sink_receive2(byte* b, int off, int len) { // throws IOException {
	while (--len >= 0) {
		sink_receive1(b[off++]);
	}
}*/

void sink_receivedLast() {
	closedByWriter = true;
	//notifyAll();
}




/*
 * PipedOutputStram
 */

boolean sink_null;

boolean sys_pre_PipedOutputStream() {
	return 1;
}
boolean par_pre_PipedOutputStream() {
	return 1;
}
void PipedOutputStream() {
	sink_null = true;
}

boolean sys_pre_connect() {
	return sink_null;
}
boolean par_pre_connect() {
	return 1;
}
void connect(/* PipedInputStream snk */) { // throws IOException {
	/*if (snk == null) {
		throw new NullPointerException();
	} else if (sink != null || snk.connected) {
		throw new IOException("Already connected");
	} */
	// sink = snk;
	
	closedByWriter = false;
	closedByReader = false;

	readSide = null;
	readSide_alive = false;
	writeSide = null;
	writeSide_alive = false;

	PIPE_SIZE = 1024;
	buffer = calloc(PIPE_SIZE, sizeof(byte));
	
	in = -1;
	out = 0;
	connected = true;
	
	sink_null = false;
}

	



boolean sys_pre_write() {
	return !sink_null 
		&& connected 
		&& !closedByWriter
		&& !closedByReader
		&& (readSide == null || readSide_alive);
}
boolean par_pre_write(int b) {
	return 1;
}
void write(int b) { // throws IOException {
//	if (sink_null) {
//		throw new IOException("Pipe not connected");
//	}
	sink_receive(b);
}

/*boolean sys_pre_write2() {
	return !sink_null 
		&& connected 
		&& !closedByWriter
		&& !closedByReader
		&& (readSide == null || readSide_alive);		
}
boolean par_pre_write2(byte* b, int b_length, int off, int len) {
	return b != null
		&& (off >= 0) && (off <= b_length) && (len >= 0) 
		&& ((off + len) <= b_length) && ((off + len) >= 0);
}
void write2(byte* b, int b_length, int off, int len) { // throws IOException {
//	if (sink_null) {
//		throw new IOException("Pipe not connected");
//	} else if (b == null) {
//		throw new NullPointerException();
//	} else if ((off < 0) || (off > b.length) || (len < 0) || ((off + len) > b.length) || ((off + len) < 0)) {
//		throw new IndexOutOfBoundsException();
//	} else 
	if (len == 0) {
		return;
	} 
	sink_receive2(b, off, len);
}*/

boolean sys_pre_flush() {
	return 1;
}
boolean par_pre_flush() {
	return 1;
}
void flush() { // throws IOException {
	if (!sink_null) {
//		synchronized (sink) {
//			sink.notifyAll();
//		}
	}
}

boolean sys_pre_close() {
	return 1;
}
boolean par_pre_close() {
	return 1;
}
void close() { // throws IOException {
	if (!sink_null) {
		sink_receivedLast();
	}
}









/*
 * Invariant
 */
boolean invariant() {
	return 1;
	// TODO
}

