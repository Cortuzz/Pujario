#include "WorkerContainer.h"

void ExecuteWorkerContainer(WorkerContainer container) { container->worker(container); }

void* EnsureContainerBufferCapacity(WorkerContainer container, size_t capacity) {
	if (container->capacity < capacity)
		container->buffer = realloc(container->buffer, capacity);
	return container->buffer;
}