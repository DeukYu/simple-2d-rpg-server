#pragma once
#include "Allocator.h"

template<typename Type, typename... Args>	
Type* New(Args&&... args)
{
	Type* memory = static_cast<Type*>(BaseAllocator::Alloc(sizeof(Type)));
	new(memory)Type(std::forward<Args>(args)...);
	return memory;
}

template<typename Type>
void Delete(Type* obj)
{
	obj->~Type();
	BaseAllocator::Release(obj);
}