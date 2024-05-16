#include "pch.h"
#include <thread>
#include <mutex>
#include <CoreMacro.h>
#include <ThreadManager.h>

class Wraight
{
public:
	int mHp = 150;
	int mPosX = 0;
	int mPosY = 0;
};

class Missile
{
public:
	void SetTarget(Wraight* target)
	{
		_target = target;
	}

	void Update() {
		int PosX = _target->mPosX;
		int PosY = _target->mPosY;
	}

	Wraight* _target = nullptr;
};

int main()
{   
	Wraight* wraight = new Wraight();
	Missile* missile = new Missile();
	missile->SetTarget(wraight);

	wraight->mHp = 0;
	delete wraight;

	while (true)
	{
		missile->Update();
	}
	delete missile;
}