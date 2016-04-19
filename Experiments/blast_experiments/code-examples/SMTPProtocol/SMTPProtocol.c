/******************************************/
/***********    TYPES         *************/
/******************************************/

typedef int boolean;
typedef int exception;
exception EXC_PARSER = 6;
exception EXC_NOSUCHAUTH = 5;
exception EXC_AUTH = 4;
exception EXC_SOCKET = 3;
exception EXC_IO = 2;
exception EXC_SMTP = 1;
exception EXC_NO = 0;

/******************************************/
/***********    OTRAS DEFS    *************/
/******************************************/

int __BLAST_NONDET;

void sendCommand(char* command, exception* EXCEPTION);
void readSingleLineResponse(exception* EXCEPTION);

/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is Ristretto Mail API.
 *
 * The Initial Developers of the Original Code are
 * Timo Stich and Frederik Dietz.
 * Portions created by the Initial Developers are Copyright (C) 2004
 * All Rights Reserved.
 *
 * Contributor(s):
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

/******************************************/
/***********      Constants   *************/
/******************************************/

	/** JDK 1.4+ logging framework logger, used for logging. */
	// private static final Logger LOG = Logger.getLogger("org.columba.ristretto.smtp");

	/*private static final*/ 
	//char[] STOPWORD = { '\r', '\n', '.', '\r', '\n' };

	/*private static final*/ 
	//int DEFAULTPORT = 25;

	/**
	 * @deprecated Use NOT_CONNECTED instead
	 */
	/*public static final*/
	//int NO_CONNECTION = 0;
	
	/**
	 * Protocol states.
	 */
	/*public static final*/
	int NOT_CONNECTED = 0;
	
	/*public static final*/
	int PLAIN = 1;
	
	/*public static final*/
	int AUTHORIZED = 2;

	/**
	 * Address types.
	 */
	/*public static final*/
	//int TO = 0;
	
	/*public static final*/
	//int CC = 1;

/******************************************/
/***********      Fields      *************/
/******************************************/

	/*private*/
	//char* host;

	/*private*/
	int port;

	/*private*/
	//Socket socket;

	/*protected*/
	//SMTPInputStream in;

	/*protected*/
	//OutputStream out;

	/*private*/
	int state;

/******************************************/
/***********   Constructors   *************/
/******************************************/

	/**
	 * Constructs the SMTPProtocol.
	 * 
	 * @param host
	 *            the sever name to connect to
	 * @param port
	 *            the port to connect to
	 */
	/*public*/ void SMTPProtocol(char* host_p, int port_p) {
		//host = host_p;
		port = port_p;
		state = NOT_CONNECTED;
	}

	/**
	 * Constructs the SMTPProtocol. Uses the default port 25 to connect to the
	 * server.
	 * 
	 * @param host
	 *            the sever name to connect to
	 */
	/*public*/ //void SMTPProtocol2(char* host_p) {
	//	SMTPProtocol1(host_p, DEFAULTPORT);
	//}

/******************************************/
/***********  Public methods  *************/
/******************************************/

	/**
	 * Opens the connection to the SMTP server.
	 * 
	 * @return the domain name of the server
	 * @throws IOException
	 * @throws SMTPException
	 */
	/*public String*/
	void openPort(exception* EXCEPTION) /*throws IOException, SMTPException*/ {
		// try {
			//socket = new Socket(host, port);
			//socket.setSoTimeout(RistrettoConfig.getInstance().getTimeout());
			if (__BLAST_NONDET) {
				*EXCEPTION = EXC_IO; 
				goto CATCH_openPort;
			}

			//createStreams();
			if (__BLAST_NONDET) { 
				if (__BLAST_NONDET)
					*EXCEPTION = EXC_IO; 
				else
					*EXCEPTION = EXC_SOCKET; 
				goto CATCH_openPort;
			}

			//SMTPResponse response = 
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_openPort;
			
			
			if (__BLAST_NONDET) { // response.isERR())
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response.getMessage());
				goto CATCH_openPort;
			}
			//String domain = response.getDomain();

			// don't care what the server has to say here.
			if (__BLAST_NONDET) { //response.isHasSuccessor()) {
				//response =
				*EXCEPTION = EXC_NO; 
				readSingleLineResponse(EXCEPTION);
				if (*EXCEPTION != EXC_NO) 
					goto CATCH_openPort;
						
				while (__BLAST_NONDET) { //response.isHasSuccessor() && response.isOK()) {
					//response = 
					*EXCEPTION = EXC_NO;
					readSingleLineResponse(EXCEPTION);
					if (*EXCEPTION != EXC_NO) 
						goto CATCH_openPort;
				}
			}

			state = PLAIN;

			return; // domain;

//		} catch (SocketException e) {
CATCH_openPort:
		if (*EXCEPTION == EXC_SOCKET) {
			//e.printStackTrace();
			
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED) {
				// throw e;
				return;
			} else {
				*EXCEPTION = EXC_NO;
				return; // return "";
			}
		}
		//}
	}

	/**
	 * Switches to a SSL connection using the TLS extension.
	 * 
	 * @throws IOException
	 * @throws SSLException
	 * @throws SMTPException
	 */
	//public
	void startTLS(exception* EXCEPTION) { //throws IOException, SSLException, SMTPException {
//		try {
			*EXCEPTION = EXC_NO;
			sendCommand("STARTTLS", EXCEPTION); //, null);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_startTLS;

			//SMTPResponse response = 
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_startTLS;
			
			if (__BLAST_NONDET) {//response.isERR()) {
				*EXCEPTION = EXC_SMTP;  //throw new SMTPException(response.getMessage());
				goto CATCH_startTLS;
			}

			// socket = RistrettoSSLSocketFactory.getInstance().createSocket(socket, host, port, true);
			if (__BLAST_NONDET) {
				*EXCEPTION = EXC_IO; 
				goto CATCH_startTLS;
			}

			// handshake (which cyper algorithms are used?)
			// ((SSLSocket) socket).startHandshake();
			if (__BLAST_NONDET) {
				*EXCEPTION = EXC_IO; 
				goto CATCH_startTLS;
			}

			//createStreams();
			if (__BLAST_NONDET) { 
				if (__BLAST_NONDET)
					*EXCEPTION = EXC_IO; 
				else
					*EXCEPTION = EXC_SOCKET; 
				goto CATCH_startTLS;
			}
			
			return;
			
//		} catch (SocketException e) {
CATCH_startTLS:
		if (*EXCEPTION == EXC_SOCKET) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED) {
				// throw e;
				return;
			} else {
				*EXCEPTION = EXC_NO;
				return;
			}
		}
			
	//	}

	}


	boolean spre_ehlo() {
		return state >= PLAIN;
	}
	/**
	 * Sends the EHLO command to the server. This command can be used to fetch
	 * the capabilities of the server. <br>
	 * Note: Only ESMTP servers understand this comand.
	 * 
	 * @see #helo(InetAddress)
	 * 
	 * @param domain
	 *            the domain name of the client
	 * @return the capabilities of the server
	 * @throws IOException
	 * @throws SMTPException
	 */
	//public String[] 
	void ehlo(exception* EXCEPTION) { // InetAddress domain) throws IOException, SMTPException {
		//ensureState(PLAIN);
		//try {

			//LinkedList capas = new LinkedList();
			//String ipaddress = domain.getHostAddress();

			*EXCEPTION = EXC_NO;
			sendCommand("EHLO", EXCEPTION); //, new String[] { "[" + ipaddress + "]" });
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_ehlo;

			// First response should be the greeting or a EHLO not supported
			//SMTPResponse response = 
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_ehlo;

			if (__BLAST_NONDET) { //response.isERR()) {
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response.getMessage());
				goto CATCH_ehlo;
			}

			if (__BLAST_NONDET) { //response.isHasSuccessor()) {
				//response = 
				*EXCEPTION = EXC_NO;
				readSingleLineResponse(EXCEPTION);
				if (*EXCEPTION != EXC_NO) 
					goto CATCH_ehlo;

				while (__BLAST_NONDET) { //response.isHasSuccessor() && response.isOK()) {
					//capas.add(response.getMessage());
					// response = 
					*EXCEPTION = EXC_NO;
					readSingleLineResponse(EXCEPTION);
					if (*EXCEPTION != EXC_NO) 
						goto CATCH_ehlo;
				}
				//capas.add(response.getMessage());
			}

			return; // (String[]) capas.toArray(new String[] {});

		//} catch (SocketException e) {
CATCH_ehlo:
		if (*EXCEPTION == EXC_SOCKET) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED)
				return; //throw e;

			else {
				*EXCEPTION = EXC_NO;
				return; // new String[0];
				
			}
				
		}
		//}

	}

	boolean spre_helo() {
		return state >= PLAIN;
	}
	/**
	 * Sends the HELO command to the SMTP server. Needed only for non ESMTP
	 * servers. Use #ehlo(InetAddress) instead.
	 * 
	 * @see #ehlo(InetAddress)
	 * 
	 * @param domain
	 * @throws IOException
	 * @throws SMTPException
	 */
	//public 
	void helo(exception* EXCEPTION) { //InetAddress domain) throws IOException, SMTPException {
		//ensureState(PLAIN);
		//try {
			//String ipaddress = domain.getHostAddress();
			*EXCEPTION = EXC_NO;
			sendCommand("HELO", EXCEPTION); //new String[] { "[" + ipaddress + "]" });
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_helo;

			//SMTPResponse response = 
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_helo;
			
			if (__BLAST_NONDET) { //response.isERR())
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response);
				goto CATCH_helo;
			}
			
			return;
					
		//} catch (SocketException e) {
CATCH_helo:			
		if (*EXCEPTION == EXC_SOCKET) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED)
				return; //throw e;
			else {
				*EXCEPTION = EXC_NO;
				return;
			}
				
		}
				
		//}

	}

	boolean spre_auth() {
		return state >= PLAIN;
	}
	/**
	 * Authenticates a user. This is done with the Authentication mechanisms
	 * provided by the
	 * @param algorithm 
	 * 
	 * @link{org.columba.ristretto.auth.AuthenticationFactory}. @param
	 *                                                          algorithm the
	 *                                                          algorithm used
	 *                                                          to authenticate
	 *                                                          the user (e.g.
	 *                                                          PLAIN,
	 *                                                          DIGEST-MD5)
	 * @param user
	 *            the user name
	 * @param password
	 *            the password
	 * @throws IOException
	 * @throws SMTPException
	 * @throws AuthenticationException
	 */
	//public 
	void auth(exception* EXCEPTION) { // String algorithm, String user, char[] password) throws IOException, SMTPException, AuthenticationException {
		//ensureState(PLAIN);
		//try {
			//try {
				//AuthenticationMechanism auth = AuthenticationFactory.getInstance().getAuthentication(algorithm);
				if (__BLAST_NONDET) {
					*EXCEPTION = EXC_NOSUCHAUTH;
					goto CATCH1_auth;
				}
				*EXCEPTION = EXC_NO;
				sendCommand("AUTH", EXCEPTION); //, new String[] { algorithm });
				if (*EXCEPTION != EXC_NO)
					goto CATCH1_auth;

				//auth.authenticate(this, user, password);
				if (__BLAST_NONDET) { 
					if (__BLAST_NONDET)
						*EXCEPTION = EXC_IO; 
					else
						*EXCEPTION = EXC_AUTH; 
					goto CATCH1_auth;
				}
	
			goto NOCATCH1_auth;
				
//			} catch (NoSuchAuthenticationException e) {
CATCH1_auth:
			if (*EXCEPTION == EXC_NOSUCHAUTH) {
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(e);
			}
			goto CATCH2_auth;
	//		}

NOCATCH1_auth:

			//SMTPResponse response = 
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO)
				goto CATCH2_auth;
			
			if (__BLAST_NONDET) { // response.isERR())
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response);
				goto CATCH2_auth;
			}

			state = AUTHORIZED;
			
			return;
			
//		} catch (SocketException e) {
CATCH2_auth:
		if (*EXCEPTION == EXC_SOCKET) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED)
				return; // throw e;
			else {
				*EXCEPTION = EXC_NO;
				return;
			}
				
		}
	//	}

	}

	boolean spre_mail() {
		return state >= PLAIN;
	}
	/**
	 * Sends a MAIL command which specifies the sender's email address and
	 * starts a new mail.
	 * 
	 * @see #rcpt(Address)
	 * @see #data(InputStream)
	 * 
	 * @param from
	 *            the email address of the sender
	 * @throws IOException
	 * @throws SMTPException
	 */
	//public 
	void mail(exception* EXCEPTION) { // Address from) throws IOException, SMTPException {
		//ensureState(PLAIN);
		//try {
			*EXCEPTION = EXC_NO;
			sendCommand("MAIL", EXCEPTION); // , new String[] { "FROM:" + from.getCanonicalMailAddress() });
			if (*EXCEPTION != EXC_NO)
				goto CATCH_mail;

			//SMTPResponse response =
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_mail; 
				
			if (__BLAST_NONDET) { // response.isERR())
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response);
				goto CATCH_mail;	
			}
			
			return;
				
		//} catch (SocketException e) {
CATCH_mail:		
		if (*EXCEPTION == EXC_SOCKET) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED)
				return; // throw e;
			else {
				*EXCEPTION = EXC_NO;
				return;
			}
		//}
		}
	}

	/**
	 * Sends a RCPT TO: command which specifies a recipient of the mail started
	 * by the MAIL command. This command can be called repeatedly to add more
	 * recipients.
	 * 
	 * @see #mail(Address)
	 * @see #data(InputStream)
	 * 
	 * @param address
	 *            the email address of a recipient.
	 * @throws IOException
	 * @throws SMTPException
	 */
	//public 
	void rcpt(exception* EXCEPTION) { //Address address) throws IOException, SMTPException {
		//try {
			*EXCEPTION = EXC_NO;
			sendCommand("RCPT", EXCEPTION); //, new String[] { "TO:" + address.getCanonicalMailAddress() });
			if (*EXCEPTION != EXC_NO)
				goto CATCH_rcpt;
			
			//SMTPResponse response = 
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_rcpt; 
				
			if (__BLAST_NONDET) { // response.isERR())
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response);
				goto CATCH_rcpt;	
			}
			
			return;
				
		//} catch (SocketException e) {
CATCH_rcpt:			
		if (*EXCEPTION == EXC_SOCKET) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED)
				return; //throw e;
			else {
				*EXCEPTION = EXC_NO;
				return;
			}
		}
		//}

	}

	/**
	 * Sends a RCPT command which specifies a recipient of the mail started by
	 * the MAIL command. This command can be called repeatedly to add more
	 * recipients. You can pass the type parameter to either send a RCPT TO or
	 * CC.
	 * @param type 
	 * 
	 * @see #mail(Address)
	 * @see #data(InputStream)
	 * @see #TO
	 * @see #CC
	 * 
	 * @param address
	 *            the email address of a recipient.
	 * @throws IOException
	 * @throws SMTPException
	 */
/*	public void rcpt(int type, Address address) throws IOException,
			SMTPException {
		try {
			switch (type) {
			case TO: {
				sendCommand("RCPT", new String[] { "TO:"
						+ address.getCanonicalMailAddress() });
				break;
			}

			case CC: {
				sendCommand("RCPT", new String[] { "CC:"
						+ address.getCanonicalMailAddress() });
				break;
			}
			}

			SMTPResponse response = readSingleLineResponse();
			if (response.isERR())
				throw new SMTPException(response);
		} catch (SocketException e) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED)
				throw e;
		}

	}*/

	boolean spre_data() {
		return state >= PLAIN;
	}
	/**
	 * Sends a DATA command which sends the mail to the recipients specified by
	 * the RCPT command. Can be cancelled with #dropConnection().
	 * 
	 * @see #mail(Address)
	 * @see #rcpt(Address)
	 * 
	 * @param data
	 *            the mail
	 * @throws IOException
	 * @throws SMTPException
	 */
	//public 
	void data(exception* EXCEPTION) { //InputStream data) throws IOException, SMTPException {
		//ensureState(PLAIN);

		//try {
			*EXCEPTION = EXC_NO;
			sendCommand("DATA", EXCEPTION ); //, null);
			if (*EXCEPTION != EXC_NO)
				goto CATCH1_data;

			//SMTPResponse response = 
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH1_data; 
			
			if (__BLAST_NONDET) { //response.getCode() == 354) {
				//try {
					//copyStream(new StopWordSafeInputStream(data), out);
					//out.write(STOPWORD);
					//out.flush();
					if (__BLAST_NONDET) {
						*EXCEPTION = EXC_IO;
						goto CATCH2_data;
					}
					
					goto NOCATCH2_data;
					
				//} catch (IOException e) {
CATCH2_data:
				if (*EXCEPTION == EXC_IO) {
					state = NOT_CONNECTED;
					//throw e;
				}
				goto CATCH1_data;
				
				//}
			} else {
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response);
				goto CATCH1_data;
			}

NOCATCH2_data:
			//response = 
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH1_data; 
				
			if (__BLAST_NONDET) { //response.isERR())
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response);
				goto CATCH1_data;
			}

			return;

		//} catch (SocketException e) {
CATCH1_data:
		if (*EXCEPTION == EXC_SOCKET) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED)
				return; //throw e;
			else {
				*EXCEPTION = EXC_NO;
				return;
			}
		//}
		}
	}

	/**
	 * Sends the QUIT command and closes the socket.
	 * 
	 * @throws IOException
	 * @throws SMTPException
	 */
	//public 
	void quit(exception* EXCEPTION) { //throws IOException, SMTPException {
		//try {
			*EXCEPTION = EXC_NO;
			sendCommand("QUIT", EXCEPTION); // null);
			if (*EXCEPTION != EXC_NO)
				goto CATCH_quit;

			/*socket.close();
			in = null;
			out = null;
			socket = null;*/

			state = NOT_CONNECTED;
			
			return;
			
		//} catch (SocketException e) {
CATCH_quit:
		if (*EXCEPTION == EXC_SOCKET) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED)
				return; //throw e;
			else {
				*EXCEPTION = EXC_NO;
				return;
			}
		}
		//}
	}

	/**
	 * Sends a RSET command which resets the current session.
	 * 
	 * @throws IOException
	 * @throws SMTPException
	 */
	//public 
	void reset(exception* EXCEPTION) { //throws IOException, SMTPException {
		//try {
			*EXCEPTION = EXC_NO;
			sendCommand("RSET", EXCEPTION); //, null);
			if (*EXCEPTION != EXC_NO)
				goto CATCH_reset;

			//SMTPResponse response = 
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_reset; 
				
			if (__BLAST_NONDET) { //response.isERR())
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response);
				goto CATCH_reset; 
			}
			
			return;
				
		//} catch (SocketException e) {
CATCH_reset:		
		if (*EXCEPTION == EXC_SOCKET) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED)
				return; //throw e;
			else {
				*EXCEPTION = EXC_NO;
				return;
			}
		}
		//}

	}

	/**
	 * Sends a VRFY command which verifies the given email address.
	 * 
	 * @param address
	 *            email address to verify
	 * @throws IOException
	 * @throws SMTPException
	 */
	//public 
	void verify(exception* EXCEPTION) { //String address) throws IOException, SMTPException {
		//try {
			*EXCEPTION = EXC_NO;
			sendCommand("VRFY", EXCEPTION); //, new String[] { address });
			if (*EXCEPTION != EXC_NO)
				goto CATCH_verify;

			//SMTPResponse response = 
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_verify; 
			
			if (__BLAST_NONDET) { //response.isERR())
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response);
				goto CATCH_verify; 
			}
				
			return;
				
		//} catch (SocketException e) {
CATCH_verify:		
		if (*EXCEPTION == EXC_SOCKET) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED)
				return; // throw e;
			else {
				*EXCEPTION = EXC_NO;
				return;
			}
		}
		//}

	}

	boolean spre_expand() {
		return state >= PLAIN;
	}
	/**
	 * Expands a given mailinglist address to all members of that list.
	 * 
	 * @param mailinglist
	 *            the mailinglist address
	 * @return the members of the mailinglist
	 * @throws IOException
	 * @throws SMTPException
	 */
	//public Address[] 
	void expand(exception* EXCEPTION) { // Address mailinglist) throws IOException, SMTPException {
		// ensureState(PLAIN);
		//try {
			//LinkedList addresses = new LinkedList();
			*EXCEPTION = EXC_NO;
			sendCommand("VRFY", EXCEPTION); //, new String[] { mailinglist.getCanonicalMailAddress() });
			if (*EXCEPTION != EXC_NO)
				goto CATCH1_expand;

			// First response should be the greeting or a EHLO not supported
			//SMTPResponse response = 
			*EXCEPTION = EXC_NO;
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH1_expand; 
				
			if (__BLAST_NONDET) { // response.isERR())
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response);
				goto CATCH1_expand;	
			}

			if (__BLAST_NONDET) { //response.isHasSuccessor()) {
				//response = 
				*EXCEPTION = EXC_NO;
				readSingleLineResponse(EXCEPTION);
				if (*EXCEPTION != EXC_NO) 
					goto CATCH1_expand; 

				while (__BLAST_NONDET) { //response.isHasSuccessor() && response.isOK()) {
					//try {
						//addresses.add(AddressParser.parseAddress(response.getMessage()));
						if (__BLAST_NONDET) { 
							*EXCEPTION = EXC_PARSER;
							goto CATCH2_expand;
						}
						goto NOCATCH2_expand;
					//} catch (ParserException e) {
CATCH2_expand:
					if (*EXCEPTION == EXC_PARSER) {
						//LOG.severe(e.getLocalizedMessage());
						goto NOCATCH2_expand;
					} else {
						goto CATCH1_expand;
					}
NOCATCH2_expand:
					//}
					//response = 
					*EXCEPTION = EXC_NO;
					readSingleLineResponse(EXCEPTION);
					if (*EXCEPTION != EXC_NO) 
						goto CATCH1_expand; 
				}

				//try {
					//addresses.add(AddressParser.parseAddress(response.getMessage()));
					if (__BLAST_NONDET) { 
						*EXCEPTION = EXC_PARSER;
						goto CATCH3_expand;
					}
					goto NOCATCH3_expand;
							
				//} catch (ParserException e) {
CATCH3_expand:				
			if (*EXCEPTION == EXC_PARSER) {
				//	LOG.severe(e.getMessage());
				goto NOCATCH3_expand;
			} else {
				goto CATCH1_expand;
			}
				//}
NOCATCH3_expand:
	;
			}

			return; // (Address[]) addresses.toArray(new Address[] {});
			
CATCH1_expand:
		if (*EXCEPTION == EXC_SOCKET) {
		//} catch (SocketException e) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED) {
				return; //throw e;

			} else {
				*EXCEPTION = EXC_NO;
				return; //new Address[0];
			}
		//}
		}
	}

	boolean spre_noop() {
		return state >= PLAIN;
	}
	/**
	 * Sends a NOOP command to the server.
	 * 
	 * @throws IOException
	 * @throws SMTPException
	 */
	//public 
	void noop(exception* EXCEPTION) { // throws IOException, SMTPException {
		//ensureState(PLAIN);
		//try {
			*EXCEPTION = EXC_NO;
			sendCommand("NOOP", EXCEPTION); //, null);
			if (*EXCEPTION != EXC_NO)
				goto CATCH_noop;
			
			//SMTPResponse response = 
			readSingleLineResponse(EXCEPTION);
			if (*EXCEPTION != EXC_NO) 
				goto CATCH_noop; 
				
			if (__BLAST_NONDET) { //response.isERR())
				*EXCEPTION = EXC_SMTP; //throw new SMTPException(response.getMessage());
				goto CATCH_noop; 
			}
				
			return;
				
		//} catch (SocketException e) {
CATCH_noop:		
		if (*EXCEPTION == EXC_SOCKET) {
			// Catch the exception if it was caused by
			// dropping the connection
			if (state != NOT_CONNECTED)
				return; //throw e;
			else {
				*EXCEPTION = EXC_NO;
				return;
			}
				
		}
		//}

	}

	/**
	 * @see org.columba.ristretto.auth.AuthenticationServer#authReceive()
	 */
	//public byte[] 
	void authReceive(exception* EXCEPTION) { //throws AuthenticationException, IOException {

		//try {
			//SMTPResponse response = in.readSingleLineResponse();
			if (__BLAST_NONDET) {
				if (__BLAST_NONDET)
					*EXCEPTION = EXC_SMTP;
				else
					*EXCEPTION = EXC_IO;
				goto CATCH_authReceive;
			}

			if (__BLAST_NONDET) { //response.isOK()) {
				/*if (response.getMessage() != null) {
					return Base64.decodeToArray(response.getMessage());
				} else {
					return new byte[0];
				}*/
				return;

			} else {
				*EXCEPTION = EXC_AUTH; // throw new AuthenticationException(new SMTPException(response));
				goto CATCH_authReceive;
			}

			return;

		// } catch (SMTPException e) {
CATCH_authReceive:		
		if (*EXCEPTION == EXC_SMTP) {
			*EXCEPTION = EXC_AUTH; // throw new AuthenticationException(e);
			return;
		}
		// }
	}

	/**
	 * @see org.columba.ristretto.auth.AuthenticationServer#authSend(byte[])
	 */
	//public 
	void authSend(exception* EXCEPTION) { //byte[] call) throws IOException {
		sendCommand("", EXCEPTION); // Base64.encode(ByteBuffer.wrap(call), false).toString(), null);
	}

	/**
	 * @return Returns the state.
	 */
	/*public int getState() {
		return state;
	}*/

	/**
	 * @see org.columba.ristretto.auth.AuthenticationServer#getHostName()
	 */
	/*public String getHostName() {
		return host;
	}*/

	/**
	 * @see org.columba.ristretto.auth.AuthenticationServer#getService()
	 */
	/*public String getService() {
		return "smtp";
	}*/

	/**
	 * Drops the connection.
	 * 
	 * @throws IOException
	 *  
	 */
	//public 
	void dropConnection(exception* EXCEPTION) { //throws IOException {
		if (state != NOT_CONNECTED) {
			state = NOT_CONNECTED;

			/*socket.close();
			in = null;
			out = null;
			socket = null;*/
			if (__BLAST_NONDET) {
				*EXCEPTION = EXC_SMTP;
				goto CATCH_dropConnection;
			}
		}
		
		return;
		
CATCH_dropConnection:
		return;
	}

/******************************************/
/***********  Private methods  ************/
/******************************************/

/*	private void createStreams() throws IOException {
		if (RistrettoLogger.logStream != null) {
			in = new SMTPInputStream(new LogInputStream(
					socket.getInputStream(), RistrettoLogger.logStream));
			out = new LogOutputStream(socket.getOutputStream(),
					RistrettoLogger.logStream);
		} else {
			in = new SMTPInputStream(socket.getInputStream());
			out = socket.getOutputStream();

		}
	}
	*/

	//protected 
	void sendCommand(char* command, exception* EXCEPTION) { //, String[] parameters)
			//throws IOException {
//		try {
			// write the command
			//out.write(command.getBytes());

			// write optional parameters
			/*if (parameters != null) {
				for (int i = 0; i < parameters.length; i++) {
					out.write(' ');
					out.write(parameters[i].getBytes());
				}
			}*/

			// write CRLF
			//out.write('\r');
			//out.write('\n');

			// flush the stream
			//out.flush();
			
			if (__BLAST_NONDET) {
				*EXCEPTION = EXC_IO;
				goto CATCH_sendCommand;
			}
			
			return;
			
	//	} catch (IOException e) {
CATCH_sendCommand:	
		if (*EXCEPTION == EXC_IO) {
			state = NOT_CONNECTED;
			return; //throw e;
		}
			
//		}
	}

/*	private void copyStream(InputStream in, OutputStream out)
			throws IOException {
		byte[] buffer = new byte[10240];
		int copied = 0;
		int read;

		read = in.read(buffer);
		while (read != -1) {
			out.write(buffer, 0, read);
			copied += read;
			read = in.read(buffer);
		}
	}*/

	/*private void ensureState(int s) throws SMTPException {
		if (state < s)
			throw new SMTPException("Wrong state!");
	}*/
	
	//protected SMTPResponse
	void readSingleLineResponse(exception* EXCEPTION) { // throws IOException, SMTPException{
		//try {
			//return in.readSingleLineResponse();
			if (__BLAST_NONDET) {
				if (__BLAST_NONDET)
					*EXCEPTION = EXC_SMTP;
				else
					*EXCEPTION = EXC_IO;
				goto CATCH_readSingleLineResponse;
			}
		
		return;			
		
		//} catch (IOException e) {
CATCH_readSingleLineResponse:
		if (*EXCEPTION == EXC_IO) {
			state = NOT_CONNECTED;
			//throw e;
		}
		//}
		
	}

