﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Examples.ICSE2011
{
    class PipedInputStream
    {
        public bool closedByWriter = false;
        public bool closedByReader = false;
        public bool connected = false;

        //Thread readSide;
        //Thread writeSide;

        protected const int PIPE_SIZE = 1024;
        protected byte[] buffer = new byte[PIPE_SIZE];

        public int _in;
        public int _out;
        public bool _connected;

        internal void recieve(int b)
        {
            Contract.Requires(_connected && !(closedByWriter || closedByReader));
    //        /*if (!connected) {
    //    throw new IOException("Pipe not connected");
    //} else if (closedByWriter || closedByReader) {
    //    throw new IOException("Pipe closed");
    //} else if (readSide != null && !readSide.isAlive()) {
    //    throw new IOException("Read end dead");
    //}*/
            while (_in == _out)
            {
                ND(); // simulate readings
            }
            if (_in < 0)
            {
                _in = 0;
                _out = 0;
            }
	        buffer[_in++] = (byte)(b & 0xFF);
            if (_in >= PIPE_SIZE) {
                _in = 0;
            }
        }

        private void ND()
        {
        }

        internal void receivedLast()
        {
            closedByWriter = true;
        }


//public
//class PipedInputStream extends InputStream {
//    boolean closedByWriter = false;
//    boolean closedByReader = false;
//    boolean connected = false;

//    Thread readSide;
//    Thread writeSide;

//    protected static final int PIPE_SIZE = 1024;

//    protected byte buffer[] = new byte[PIPE_SIZE];

//    protected int in = -1;

//    protected int out = 0;

//    /**
//     * Creates a <code>PipedInputStream</code> so
//     * that it is connected to the piped output
//     * stream <code>src</code>. Data bytes written
//     * to <code>src</code> will then be  available
//     * as input from this stream.
//     *
//     * @param      src   the stream to connect to.
//     * @exception  IOException  if an I/O error occurs.
//     */
//    public PipedInputStream(PipedOutputStream src) throws IOException {
//    connect(src);
//    }

//    /**
//     * Creates a <code>PipedInputStream</code> so
//     * that it is not  yet connected. It must be
//     * connected to a <code>PipedOutputStream</code>
//     * before being used.
//     *
//     * @see     java.io.PipedInputStream#connect(java.io.PipedOutputStream)
//     * @see     java.io.PipedOutputStream#connect(java.io.PipedInputStream)
//     */
//    public PipedInputStream() {
//    }

//    /**
//     * Causes this piped input stream to be connected
//     * to the piped  output stream <code>src</code>.
//     * If this object is already connected to some
//     * other piped output  stream, an <code>IOException</code>
//     * is thrown.
//     * <p>
//     * If <code>src</code> is an
//     * unconnected piped output stream and <code>snk</code>
//     * is an unconnected piped input stream, they
//     * may be connected by either the call:
//     * <p>
//     * <pre><code>snk.connect(src)</code> </pre>
//     * <p>
//     * or the call:
//     * <p>
//     * <pre><code>src.connect(snk)</code> </pre>
//     * <p>
//     * The two
//     * calls have the same effect.
//     *
//     * @param      src   The piped output stream to connect to.
//     * @exception  IOException  if an I/O error occurs.
//     */
//    public void connect(PipedOutputStream src) throws IOException {
//    src.connect(this);
//    }

//    /**
//     * Receives a byte of data.  This method will block if no input is
//     * available.
//     * @param b the byte being received
//     * @exception IOException If the pipe is broken.
//     * @since     JDK1.1
//     */
//    protected synchronized void receive(int b) throws IOException {
//        if (!connected) {
//            throw new IOException("Pipe not connected");
//        } else if (closedByWriter || closedByReader) {
//        throw new IOException("Pipe closed");
//    } else if (readSide != null && !readSide.isAlive()) {
//            throw new IOException("Read end dead");
//        }

//    writeSide = Thread.currentThread();
//    while (in == out) {
//        if ((readSide != null) && !readSide.isAlive()) {
//        throw new IOException("Pipe broken");
//        }
//        /* full: kick any waiting readers */
//        notifyAll();
//        try {
//            wait(1000);
//        } catch (InterruptedException ex) {
//        throw new java.io.InterruptedIOException();
//        }
//    }
//    if (in < 0) {
//        in = 0;
//        out = 0;
//    }
//    buffer[in++] = (byte)(b & 0xFF);
//    if (in >= buffer.length) {
//        in = 0;
//    }
//    }

//    /**
//     * Receives data into an array of bytes.  This method will
//     * block until some input is available.
//     * @param b the buffer into which the data is received
//     * @param off the start offset of the data
//     * @param len the maximum number of bytes received
//     * @return the actual number of bytes received, -1 is
//     *          returned when the end of the stream is reached.
//     * @exception IOException If an I/O error has occurred.
//     */
//    synchronized void receive(byte b[], int off, int len)  throws IOException {
//    while (--len >= 0) {
//        receive(b[off++]);
//    }
//    }

//    /**
//     * Notifies all waiting threads that the last byte of data has been
//     * received.
//     */
//    synchronized void receivedLast() {
//    closedByWriter = true;
//    notifyAll();
//    }

//    /**
//     * Reads the next byte of data from this piped input stream. The
//     * value byte is returned as an <code>int</code> in the range
//     * <code>0</code> to <code>255</code>. If no byte is available
//     * because the end of the stream has been reached, the value
//     * <code>-1</code> is returned. This method blocks until input data
//     * is available, the end of the stream is detected, or an exception
//     * is thrown.
//     * If a thread was providing data bytes
//     * to the connected piped output stream, but
//     * the  thread is no longer alive, then an
//     * <code>IOException</code> is thrown.
//     *
//     * @return     the next byte of data, or <code>-1</code> if the end of the
//     *             stream is reached.
//     * @exception  IOException  if the pipe is broken.
//     */
//    public synchronized int read()  throws IOException {
//        if (!connected) {
//            throw new IOException("Pipe not connected");
//        } else if (closedByReader) {
//        throw new IOException("Pipe closed");
//    } else if (writeSide != null && !writeSide.isAlive()
//                   && !closedByWriter && (in < 0)) {
//            throw new IOException("Write end dead");
//        }

//        readSide = Thread.currentThread();
//    int trials = 2;
//    while (in < 0) {
//        if (closedByWriter) {
//        /* closed by writer, return EOF */
//        return -1;
//        }
//        if ((writeSide != null) && (!writeSide.isAlive()) && (--trials < 0)) {
//        throw new IOException("Pipe broken");
//        }
//            /* might be a writer waiting */
//        notifyAll();
//        try {
//            wait(1000);
//        } catch (InterruptedException ex) {
//        throw new java.io.InterruptedIOException();
//        }
//    }
//    int ret = buffer[out++] & 0xFF;
//    if (out >= buffer.length) {
//        out = 0;
//    }
//    if (in == out) {
//            /* now empty */
//        in = -1;
//    }
//    return ret;
//    }

//    /**
//     * Reads up to <code>len</code> bytes of data from this piped input
//     * stream into an array of bytes. Less than <code>len</code> bytes
//     * will be read if the end of the data stream is reached. This method
//     * blocks until at least one byte of input is available.
//     * If a thread was providing data bytes
//     * to the connected piped output stream, but
//     * the  thread is no longer alive, then an
//     * <code>IOException</code> is thrown.
//     *
//     * @param      b     the buffer into which the data is read.
//     * @param      off   the start offset of the data.
//     * @param      len   the maximum number of bytes read.
//     * @return     the total number of bytes read into the buffer, or
//     *             <code>-1</code> if there is no more data because the end of
//     *             the stream has been reached.
//     * @exception  IOException  if an I/O error occurs.
//     */
//    public synchronized int read(byte b[], int off, int len)  throws IOException {
//    if (b == null) {
//        throw new NullPointerException();
//    } else if ((off < 0) || (off > b.length) || (len < 0) ||
//           ((off + len) > b.length) || ((off + len) < 0)) {
//        throw new IndexOutOfBoundsException();
//    } else if (len == 0) {
//        return 0;
//    }

//        /* possibly wait on the first character */
//    int c = read();
//    if (c < 0) {
//        return -1;
//    }
//    b[off] = (byte) c;
//    int rlen = 1;
//    while ((in >= 0) && (--len > 0)) {
//        b[off + rlen] = buffer[out++];
//        rlen++;
//        if (out >= buffer.length) {
//        out = 0;
//        }
//        if (in == out) {
//                /* now empty */
//        in = -1;
//        }
//    }
//    return rlen;
//    }

//    /**
//     * Returns the number of bytes that can be read from this input
//     * stream without blocking. This method overrides the <code>available</code>
//     * method of the parent class.
//     *
//     * @return     the number of bytes that can be read from this input stream
//     *             without blocking.
//     * @exception  IOException  if an I/O error occurs.
//     * @since   JDK1.0.2
//     */
//  public synchronized int available() throws IOException {
//    if(in < 0)
//      return 0;
//    else if(in == out)
//      return buffer.length;
//    else if (in > out)
//      return in - out;
//    else
//      return in + buffer.length - out;
//  }

//    /**
//     * Closes this piped input stream and releases any system resources
//     * associated with the stream.
//     *
//     * @exception  IOException  if an I/O error occurs.
//     */
//    public void close()  throws IOException {
//    in = -1;
//    closedByReader = true;
//    }
//}

    }
    
    public class PipedOutputStream
    {
        private PipedInputStream sink;

        public PipedOutputStream()
        { 
        }
        //    public PipedOutputStream(PipedInputStream snk)  throws IOException {
        //    connect(snk);
        //    }

        //    public PipedOutputStream() {
        //    }

        public void connect()
        {
            Contract.Requires(this.sink == null || !this.sink._connected);

            this.sink = new PipedInputStream();
            this.sink._in = -1;
            this.sink._out = 0;
            this.sink._connected = true;
        }

        //    public synchronized void connect(PipedInputStream snk) throws IOException {
        //        if (snk == null) {
        //            throw new NullPointerException();
        //        } else if (sink != null || snk.connected) {
        //        throw new IOException("Already connected");
        //       }
        //       sink = snk;
        //       snk.in = -1;
        //       snk.out = 0;
        //           snk.connected = true;
        //    }
        public void write(int b)
        {
            Contract.Requires(this.sink != null);

            this.sink.recieve(b);
        }

        //    public void write(int b)  throws IOException {
        //        if (sink == null) {
        //            throw new IOException("Pipe not connected");
        //        }
        //    sink.receive(b);
        //    }

        //    public void write(byte b[], int off, int len) throws IOException {
        //        if (sink == null) {
        //            throw new IOException("Pipe not connected");
        //        } else if (b == null) {
        //        throw new NullPointerException();
        //    } else if ((off < 0) || (off > b.length) || (len < 0) ||
        //           ((off + len) > b.length) || ((off + len) < 0)) {
        //        throw new IndexOutOfBoundsException();
        //    } else if (len == 0) {
        //        return;
        //    } 
        //    sink.receive(b, off, len);
        //    }

        public void flush()
        {
            if (this.sink != null)
            {

            }
        }
        //    public synchronized void flush() throws IOException {
        //          if (sink != null) {
        //                  synchronized (sink) {
        //                      sink.notifyAll();
        //                  }
        //          }
        //    }

        public void close()
        {
            if (this.sink != null)
            {
                this.sink.receivedLast();
            }
        }

        //    public void close()  throws IOException {
        //      if (sink != null) {
        //          sink.receivedLast();
        //      }
        //    }
        //}
    }
}