#pragma once
#include <stdlib.h>

typedef struct _WorkerContainer* WorkerContainer;

typedef void (*ContainerWorker)(WorkerContainer container);

/// <summary>
/// An execution environment, invented to to reuse buffer's memory in many executions.
/// Everything in container should use buffer for allocation, even output;
/// </summary>
struct _WorkerContainer
{
	ContainerWorker worker;
	// input data for worker 
	void* input;
	// readonly result of execution, which also could use buffer
	void* output;
	// Every execution buffer regards as empty, so output will be droped
	void* buffer;	
	size_t capacity;
};

void ExecuteWorkerContainer(WorkerContainer container);

/// <remarks>can reset buffer field</remarks>
void* EnsureContainerBufferCapacity(WorkerContainer container, size_t capacity);