#include <stdio.h>

// Booleans
typedef int bool;

// Uints
typedef unsigned int uint;

// Strings
typedef char* string;

// ContentType.cs
typedef enum {
	Null_ContentType, /* Added a Null value */
	ThreeOrFewerConsecutiveBlks,
	MoreThanThreeConsecutiveBlks
} ContentType;
bool isLegalContentType(ContentType c) {
	return c == Null_ContentType || c == ThreeOrFewerConsecutiveBlks || c == MoreThanThreeConsecutiveBlks;
}

// SutPlatform.cs
typedef enum {
	Null_SutPlatform, /* Added a Null value */
	Windows7,
	NonWindows
} SutPlatform;
bool isLegalSutPlatform(SutPlatform s) {
	return s == Null_SutPlatform || s == Windows7 || s == NonWindows;
}

// Line 18
typedef enum {
	Null_ModelState, /* Added a Null value */
	GetPlatform, 
	Init,
	Idle,
	Listening,
	SendNegoResp,
	SendBlkList,
	SendBlk,
	VerifyContent,
	End
} ModelState;
bool isLegalModelState(ModelState m) {
	return m == Null_ModelState || m == GetPlatform || m == Init || m == Idle || m == Listening || m == SendNegoResp || m == SendBlkList || m == SendBlk || m == VerifyContent || m == End;
}

/*
 * region Variable (Line 81)
 */

// Line 86
ModelState _state;

// Line 92
ContentType _contentType;

// Line 97
SutPlatform _platform;

// Line 103
bool _isTimerExpireInBlk;

// Line 107
bool _isSameSegmentInBlk;

// Line 111
bool _isWellFormedInBlk;

// Line 117
bool _isTimerExpireInList;

// Line 121
bool _isSameSegmentInList;

// Line 125
bool _isWellFormedInList;

// Line 129
bool _isOverlapInList;

// Line 135
bool _isDownloadComplete;

// Line 140
uint _blockIndex;

// Line 145
uint _maxBlocks;

// Line 152
bool _isNormalMode;

// Line 157
bool _isTestingNego;

// Line 161
bool _isSendNegoResp;

// Line 75
// [AcceptingStateCondition]
bool AcceptStateCond() {
	return _state == End;
}

/*
 * region Helper Function (Line 483)
 */

// Line 498
void SutClientUpdateState(ModelState srvState) {
	_state = srvState;
}

// Line 506
void ResetVariables() {
	_isTimerExpireInBlk = 0;
	_isSameSegmentInBlk = 1;
	_isWellFormedInBlk = 1;
	
	_isDownloadComplete = 0;
	_blockIndex = 0;
	
	_isTimerExpireInList = 0;
	_isSameSegmentInList = 1;
	_isWellFormedInList = 1;
	_isOverlapInList = 1;
}

// Line 528
void PCCRRCapture(int id, string description) {
	printf("MS-PCCRR req #%d: %s\n", id, description);
}

// Line 538
void SetBlkListMode(bool isNormal) {
	_isNormalMode = isNormal;
}

/*
 * region Action (Line 164)
 */

// Line 178
// [Action("return GetSutClientPlatform(out sutPlatform)")]
void GetSutClientPlatform(SutPlatform sutPlatform) {
	_platform = NonWindows; // XXX SIMPLIFICATION0 XXX before it said: sutPlatform;
	_state = Init;
}
bool statepre_GetSutClientPlatform() {
	return _state == GetPlatform;
}
bool parampre_GetSutClientPlatform(SutPlatform sutPlatform) {
	return sutPlatform == Windows7 || sutPlatform == NonWindows;
}

// Line 194
// [Action]
void InitSutClient(bool isTestingNegotiation) {
	_isTestingNego = 0;             // XXX ISSUE0 XXX before it said: XXX  isTestingNegotiation;
	SutClientUpdateState(Idle);
}
bool statepre_InitSutClient() {
	return _state == Init;
}
bool parampre_InitSutClient(bool isTestingNegotiation) {
	return 1;
}

// Line 208
// [Action]
void ReceiveMsgNegoReq() {
	SutClientUpdateState(SendNegoResp);
}
bool statepre_ReceiveMsgNegoReq() {
	return _state == Listening && _platform != Windows7;
}
bool parampre_ReceiveMsgNegoReq() {
	return 1;
}

// Line 221
// [Action]
void SendMsgNegoResp() {
	_isSendNegoResp = 1;
	SutClientUpdateState(Listening);
}
bool statepre_SendMsgNegoResp() {
	return _isTestingNego || _state == SendNegoResp;
}
bool parampre_SendMsgNegoResp() {
	return 1;
}

// Line 242
// [Action]
void TriggerSutClientDownload(ContentType cType) {
	_contentType = cType;
	_state = Listening;
}
bool statepre_TriggerSutClientDownload() {
	return _state == Idle;
}
bool parampre_TriggerSutClientDownload(ContentType cType) {
	return cType == ThreeOrFewerConsecutiveBlks || cType == MoreThanThreeConsecutiveBlks;
}

// Line 256
// [Action]
void ReceiveMsgGetBlkList() {
	PCCRRCapture(178, "[GetBlockList Initiation]The client instance of the Retrieval Protocol instantiation MUST construct and send a GetBlocks message to the server.");
	SutClientUpdateState(SendBlkList);
}
bool statepre_ReceiveMsgGetBlkList() {
	return _state == Listening;
}
bool parampre_ReceiveMsgGetBlkList() {
	return 1;
}

// Line 275
// [Action("SendMsgBlkList(isTimerExpire, isSameSegment, isWellFormed, isOverlap)")]
void SendMsgBlkListNormal(bool isTimerExpire, bool isSameSegment, bool isWellFormed, bool isOverlap) {
	_isTimerExpireInList = isTimerExpire;
	_isSameSegmentInList = isSameSegment;
	_isWellFormedInList = isWellFormed;
	_isOverlapInList = isOverlap;
	
    SutClientUpdateState(Listening);
}
bool statepre_SendMsgBlkListNormal() {
	return _state == SendBlkList && _isNormalMode;
}
bool parampre_SendMsgBlkListNormal(bool isTimerExpire, bool isSameSegment, bool isWellFormed, bool isOverlap) {
	return !isTimerExpire && isSameSegment && isWellFormed && isOverlap;
}

// Line 298
// [Action("SendMsgBlkList(isTimerExpire, isSameSegment, isWellFormed, isOverlap)")]
void SendMsgBlkListAbnormal(bool isTimerExpire, bool isSameSegment, bool isWellFormed, bool isOverlap) {
	_isTimerExpireInList = isTimerExpire;
	_isSameSegmentInList = isSameSegment;
	_isWellFormedInList = isWellFormed;
	_isOverlapInList = isOverlap;
	
    SutClientUpdateState(Listening);
}
bool statepre_SendMsgBlkListAbnormal() {
	return _state == SendBlkList && !_isNormalMode;
}
bool parampre_SendMsgBlkListAbnormal(bool isTimerExpire, bool isSameSegment, bool isWellFormed, bool isOverlap) {
	bool ret = !(!isTimerExpire && isSameSegment && isWellFormed && isOverlap);
	if (isTimerExpire)
		ret = ret && isSameSegment && isWellFormed && isOverlap;
	if (!isSameSegment)
		ret = ret && !isTimerExpire && isWellFormed && isOverlap;
	if (!isWellFormed)
		ret = ret && !isTimerExpire && isSameSegment && isOverlap;
	if (!isOverlap)
		ret = ret && !isTimerExpire && isSameSegment && isWellFormed;
	return ret;
}

// Line 326
// [Action]
void ReceiveMsgGetBlk(uint index) {
	_blockIndex = index;
	PCCRRCapture(186, "[GetBlocks Initiation]The client instance of the Retrieval Protocol MUST send a GetBlocks message to the server.");
	SutClientUpdateState(SendBlk);
}
bool statepre_ReceiveMsgGetBlk() {
	return _state == Listening;
}
bool parampre_ReceiveMsgGetBlk(uint index) {
	return index >= 0 && index < 4; // XXX ORIGINAL said "< _maxBlocks" instead of "< 4"
}

// Line 347
// [Action]
void SendMsgBlk(ContentType cType, bool isTimerExpire, bool isSameSegment, bool isWellFormed, uint index) {
	// XXX FORCING PARAMETERS SINCE COULDN'T SPLIT PRECONDITION
	cType = _contentType;
	index = _blockIndex;
	// XXX END FORCING

	_isTimerExpireInBlk = isTimerExpire;
	_isSameSegmentInBlk = isSameSegment;
	_isWellFormedInBlk = isWellFormed;

	if (isTimerExpire || !isSameSegment || !isWellFormed)
		_isDownloadComplete = 0;
	else
		_isDownloadComplete = 1;

	SutClientUpdateState(VerifyContent);
}
bool statepre_SendMsgBlk() {
	return _state == SendBlk;
}
bool parampre_SendMsgBlk(ContentType cType, bool isTimerExpire, bool isSameSegment, bool isWellFormed, uint index) {
	bool ret = 1;
	if (isTimerExpire)
		ret = ret && isSameSegment && isWellFormed;
	if (!isSameSegment)
		ret = ret && !isTimerExpire && isWellFormed;
	if (!isWellFormed)
		ret = ret && !isTimerExpire && isSameSegment;
	
	if (isTimerExpire || !isSameSegment || !isWellFormed)
		ret = ret && index == 0;

	// XXX ORIGINAL said "ret = ret && cType == _contentType && index == _blockIndex"
	ret = ret && isValidaContentType(cType);
	
	return ret;
}

// Line 382
// Action("ReceivingTimeOut()")
void SutClientReceivingTimeOut() {
	if (!_isNormalMode) {
		if (_isTimerExpireInList) {
			PCCRRCapture(180, "[GetBlockList Initiation]The client instance of the Retrieval Protocol instantiation MUST start the Request Timer.");
			PCCRRCapture(1216, "When the Request Timer expires before the exchange (GetBlockList or GetBlocks) is completed, the client MUST abort the current exchange.");
		}
		if (!_isSameSegmentInList) {
			PCCRRCapture(1200, "[MSG_BLKLIST Response Received]If the Segment ID does not match any request in the outstanding request list, then the peer will not send the MSG_GETBLKS.");
			PCCRRCapture(1205, "[MSG_BLKLIST Response Received]If the MSG_BLKLIST Response message is not corresponds to a GetBlockList request message in its outstanding request list, then the peer will not send the MSG_GETBLKS.");
		}
		if (!_isWellFormedInList) {
			PCCRRCapture(199, "On receiving a MSG_BLKLIST response message from a server, theF client MUST verify that it is well-formed and corresponds to a GetBlockList request message in its outstanding request list.");
		}
		if (!_isOverlapInList) {
			PCCRRCapture(201, "[MSG_BLKLIST Response Received]The MSG_BLKLIST MUST be silently discarded and the exchange MUST be aborted if:The block ranges do not overlap with the ranges specified in any request with a matching Segment ID in the outstanding request list.");
		}
	}
	
	if (_isTestingNego && _isSendNegoResp) {
		PCCRRCapture(198, "[MSG_NEGO_RESP Received]If there is no existing request message previously sent to the server stored in the outstanding request list, the client MUST silently discard the received message.");
	}
	
	ResetVariables();
	SutClientUpdateState(End);
}
bool statepre_SutClientReceivingTimeOut() {
	return _state == Listening;
}
bool parampre_SutClientReceivingTimeOut() {
	return 1;
}

// Line 426
// [Action]
bool SutClientVerifyBlockContent(ContentType cType, uint index) {
	// XXX FORCING PARAMETERS SINCE COULDN'T SPLIT PRECONDITION
	cType = _contentType;
	index = _blockIndex;
	// XXX END FORCING
	
	if (!_isDownloadComplete) {
		ResetVariables();
		SutClientUpdateState(Listening);
		return 0;
	} else {
		if (!_isTimerExpireInBlk) {
			PCCRRCapture(2156, "[Timers]Request Timer: When a peer sends a MSG_GETBLK request message and receive MSG_BLK response message before request timer timeout, then it will cache the block it received");
		}
		PCCRRCapture(159, "[Timers]Request Timer: <3> Section 3.1.2: Windows uses a 2 second timeout for each request message.");
		PCCRRCapture(210, "[MSG-BLK Response Received]Otherwise[if this verification[MSG_BLK response message is well-formed and corresponds to a GetBlocks request message in its outstanding request list (the Segment ID and block index would match that of an outstanding GetBlocks index would match that of an outstanding GetBlocks request)] is successful.], it[client] MUST: If an encryption algorithm (CryptoAlgoID <> 0) is specified in the MSG_BLK message, decrypt the block using the pre-provisioned key.");
		
		ResetVariables();
		if (_maxBlocks - 1 == index) {
			SutClientUpdateState(End);
		} else {
			SutClientUpdateState(Listening);
		}
		return 1;
	}
}
bool statepre_SutClientVerifyBlockContent() {
	return _state == VerifyContent;
}
bool parampre_SutClientVerifyBlockContent(ContentType cType, uint index) {
	// XXX ORIGINAL said "ret = ret && cType == _contentType && index == _blockIndex"
	return isValidContentType(cType);
}

/*
 * initializer (in original code it was done on variable definitions)
 */
void init() {
	_state = GetPlatform;
	_contentType = Null_ContentType;
	_platform = NonWindows; // XXX SIMPLIFICATION0 XXX original said XXX Null_SutPlatform;
	_isTimerExpireInBlk = 0;
	_isSameSegmentInBlk = 1;
	_isWellFormedInBlk = 1;
	_isTimerExpireInList = 0;
	_isSameSegmentInList = 1;
	_isWellFormedInList = 1;
	_isOverlapInList = 1;
	_isDownloadComplete = 0;
	_blockIndex = 0;
	_maxBlocks = 4;
	_isNormalMode = 1;
	_isTestingNego = 0;
	_isSendNegoResp = 0;
}

/* 
 * invariant (not in original code, but added to incorporate enums restrictions)
 */
bool invariant() {
	bool ret = 1;
	ret = ret && (_isTestingNego == 0); // XXX ISSUE1 XXX original invariant did not say anything about _isTestingNego, but as we corrected ISSUE0 we need this here
	ret = ret && (_platform == NonWindows); // XXX SIMPLIFICATION0 XXX original invariant did not say anything about _platform
	ret = ret && isLegalContentType(_contentType);
	ret = ret && isLegalSutPlatform(_platform);
	ret = ret && isLegalModelState(_state);
	return ret;
}

int main() {
	ModelState x;
	x = End;
	x = Init;

	printf("%d",x);
	
	return 0;
}


