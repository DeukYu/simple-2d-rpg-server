#include "pch.h"
#include "PlayerManager.h"
#include "AccountManager.h"

void PlayerManager::PlayerThenAccount()
{
	WRITE_LOCK;
	GAccountManager->Lock();
}

void PlayerManager::Lock()
{
}
