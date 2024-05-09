#pragma once
class AccountManager
{
	USE_LOCK;

public:
	void Lock();
};

extern AccountManager GAccountManager;